using AuthServer.Domain;
using AuthServer.Interfaces;
using AuthServer.Shared.Results;
using System.Net;
using DnsClient;
using System.Net.Mail;

namespace AuthServer.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public partial class UserManager<TId>(
        IRepository<TId, ApplicationUser<TId>, AuthContext<TId>> applicationUserRepository,
        IRepository<TId, ApplicationUserTokens<TId>, AuthContext<TId>> applicationUserTokenRepository,
        CacheService cacheService)
        : IUserManager<TId> where TId : IEquatable<TId>
    {
        private const string CacheKeyPrefix = "ApplicationUser_";

        /// <inheritdoc />
        public async Task<OperationResult> RegisterUserAsync(ApplicationUser<TId> user)
        {
            // Validate the email address
            if (!await IsEmailValidAsync(user.Email))
            {
                return OperationResult.Failed(ErrorCodes.InvalidEmailAddress, "Given email address is invalid.");
            }

            // Check if the user already exists in cache
            var cacheUser = cacheService.GetAsync<ApplicationUser<TId>>(CacheKeyPrefix + user.Email);

            if (cacheUser is not null && ServiceExtension.Configuration.User.RequireUniqueEmail)
                return OperationResult.Failed(ErrorCodes.EmailTaken, "User with specified email already exists");

            if (cacheUser is null && ServiceExtension.Configuration.User.RequireUniqueEmail &&
                applicationUserRepository.Entities.Any(x => x.Email == user.Email))
            {
                return OperationResult.Failed(ErrorCodes.EmailTaken, "User with specified email already exists");
            }

            // Create a new user
            //var password = GetPasswordHash(user.PasswordHash);
            if (applicationUserRepository.Entities.Any(x => x.UserName == user.UserName))
            {
                return OperationResult.Failed(ErrorCodes.UsernameTaken, "The requested Username is already in use");
            }

            // Should inherit the TFA setting from the configuration
            user.IsTwoFactorEnabled = ServiceExtension.Configuration.SignIn.RequireTwoFactorEnabled;

            // Insert record
            await applicationUserRepository.AddAsync(user);

            // Should inherit the TFA setting from the configuration and add token
            if (ServiceExtension.Configuration.SignIn.RequireTwoFactorEnabled)
            {
                await EnableTwoFactorAuthentication(user);
            }

            return OperationResult.Success();
        }

        /// <inheritdoc />
        public Task<OperationResult> RegisterUserAsync(ApplicationUser<TId> user, string password)
        {
            user.PasswordHash = password.GetHash();
            return RegisterUserAsync(user);
        }

        /// <inheritdoc />
        public Task<LoginResult> LoginAsync(string username, string password)
        {
            var user = applicationUserRepository.Entities.FirstOrDefault(x => x.UserName == username);
            if (user is null)
            {
                return Task.FromResult(new LoginResult
                {
                    Succeeded = false,
                    ErrorCode = ErrorCodes.UserNotFound,
                    ErrorDescription = "User with specified username was not found"
                });
            }

            if (ServiceExtension.Configuration.SignIn.RequireConfirmedEmail && !user.IsEmailConfirmed)
            {
                return Task.FromResult(new LoginResult
                {
                    Succeeded = false,
                    EmailNotConfirmed = true,
                    ErrorCode = ErrorCodes.EmailNotVerified,
                    ErrorDescription = "Email is not confirmed yet"
                });
            }

            if (ServiceExtension.Configuration.SignIn.RequireConfirmedAccount && !user.IsActive)
            {
                return Task.FromResult(new LoginResult
                {
                    Succeeded = false,
                    IsNotActive = true,
                    ErrorCode = ErrorCodes.UserIsNotActive,
                    ErrorDescription = "User is not active"
                });
            }

            if (ServiceExtension.Configuration.SignIn.RequireConfirmedPhoneNumber && !user.IsPhoneNumberConfirmed)
            {
                return Task.FromResult(new LoginResult
                {
                    Succeeded = false,
                    PhoneNotConfirmed = true,
                    ErrorCode = ErrorCodes.MobileNotVerified,
                    ErrorDescription = "User's Phone number is not confirmed yet"
                });
            }


            if (!string.Equals(user.PasswordHash, password.GetHash()))
            {
                return Task.FromResult(new LoginResult
                {
                    Succeeded = false,
                    IncorrectCredentials = true,
                    ErrorCode = ErrorCodes.IncorrectCredentials,
                    ErrorDescription = "Credentials supplied are incorrect"
                });
            }

            if (ServiceExtension.Configuration.SignIn.RequireTwoFactorEnabled && user.IsTwoFactorEnabled)
            {
                return Task.FromResult(new LoginResult
                {
                    Succeeded = false,
                    IsTwoFactorRequired = true,
                    ErrorCode = ErrorCodes.TwoFactorRequired,
                    ErrorDescription = "Two factor authentication is required for this user"
                });
            }

            return Task.FromResult(LoginResult.Success());
        }

        /// <inheritdoc />
        public async Task<LoginResult> LoginTwoFactorAsync(string username, string password, string otp)
        {
            var loginResult = await LoginAsync(username, password);

            if (!loginResult.IsTwoFactorRequired)
            {
                return loginResult;
            }

            var user = applicationUserRepository.Entities.FirstOrDefault(x => x.UserName == username);

            if (user is null)
            {
                return new LoginResult
                {
                    Succeeded = false,
                    ErrorCode = ErrorCodes.UserNotFound,
                    ErrorDescription = "User with specified username was not found"
                };
            }

            // check TFA code
            if (!user.IsTwoFactorEnabled)
            {
                return new LoginResult
                {
                    Succeeded = false,
                    ErrorCode = ErrorCodes.TwoFactorNotEnabled,
                    ErrorDescription = "Two factor authentication is not enabled for this user"
                };
            }

            var userToken = applicationUserTokenRepository.Entities
                .FirstOrDefault(x => x.UserId.Equals(user.Id));

            if (userToken is null || string.IsNullOrEmpty(userToken.Value))
            {
                return new LoginResult
                {
                    Succeeded = false,
                    ErrorCode = ErrorCodes.TwoFactorNotConfigured,
                    ErrorDescription = "Two factor authentication is not configured for this user"
                };
            }

            var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();

            if (!authenticator.CheckCode(userToken.Value, otp, user))
            {
                return new LoginResult
                {
                    Succeeded = false,
                    ErrorCode = ErrorCodes.InvalidTwoFactorCode,
                    ErrorDescription = "Two factor authentication code is invalid"
                };
            }

            return LoginResult.Success();
        }

        /// <inheritdoc />
        public Task<OperationResult<string>> GetTwoFactorCodeAsync(ApplicationUser<TId> user)
        {
            var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();
            var userToken = applicationUserTokenRepository.Entities
                .FirstOrDefault(x => x.UserId.Equals(user.Id));

            if (userToken is null || string.IsNullOrEmpty(userToken.Value))
            {
                return Task.FromResult(new OperationResult<string>
                {
                    Succeeded = false,
                    ErrorCode = ErrorCodes.TwoFactorNotConfigured,
                    ErrorDescription = "Two factor authentication is not configured for this user"
                });
            }

            return Task.FromResult(new OperationResult<string>
            {
                Data = authenticator.GetCode(userToken.Value),
                Succeeded = true
            });
        }


        /// <inheritdoc />

        public Task<OperationResult> GetConfirmEmailCode(ApplicationUser<TId> user)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<OperationResult> GetConfirmPhoneCode(ApplicationUser<TId> user)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />

        public Task<OperationResult> ConfirmPhoneCode(ApplicationUser<TId> user, string code)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />

        public Task<OperationResult> GetConfirmPhoneCode(ApplicationUser<TId> user, string code)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<OperationResult<string>> EnableTwoFactorAuthentication(ApplicationUser<TId> user)
        {
            user.IsTwoFactorEnabled = true;
            await applicationUserRepository.UpdateAsync(user);

            var userToken = applicationUserTokenRepository.Entities
                .FirstOrDefault(x => x.UserId.Equals(user.Id));

            if (userToken is not null)
                await applicationUserTokenRepository.DeleteAsync(userToken);


            userToken = new ApplicationUserTokens<TId>
            {
                LoginProvider = "[AuthService]",
                Name = "TwoFactorCode",
                UserId = user.Id,
                Value = TwoStepsAuthenticator.Authenticator.GenerateKey()
            };

            await applicationUserTokenRepository.AddAsync(userToken);


            return OperationResult<string>.Success(userToken.Value);
        }

    }

    public partial class UserManager<TId> where TId : IEquatable<TId>
    {
        private static async Task<bool> IsEmailValidAsync(string email)
        {
            var options = new LookupClientOptions(IPAddress.Parse("8.8.8.8"), IPAddress.Parse("8.8.4.4"))
            {
                Timeout = TimeSpan.FromSeconds(5.0)
            };

            var dnsQueryResponse = await new LookupClient(options)
                .QueryAsync(new MailAddress(email).Host.ToLower(), QueryType.MX)
                .ConfigureAwait(false);

            return dnsQueryResponse is { Answers.Count: > 0 };
        }
    }
}
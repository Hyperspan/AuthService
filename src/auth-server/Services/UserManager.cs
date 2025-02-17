using AuthServer.Domain;
using AuthServer.Interfaces;
using AuthServer.Shared.Results;
using System.Net;
using DnsClient;
using System.Net.Mail;
using System.Security.Claims;

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

            return Task.FromResult(LoginResult.Success(GetUserClaims(user)));
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
                .FirstOrDefault(x => x.UserId.Equals(user.Id) && x.Name == "TwoFactorCode");

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

            return LoginResult.Success(GetUserClaims(user));
        }

        /// <inheritdoc />
        public Task<OperationResult<string>> GetTwoFactorCodeAsync(ApplicationUser<TId> user)
        {
            var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();
            var userToken = applicationUserTokenRepository.Entities
                .FirstOrDefault(x => x.UserId.Equals(user.Id) && x.Name == "TwoFactorCode");

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
        public async Task<OperationResult<string>> GetConfirmEmailCode(ApplicationUser<TId> user)
        {
            var otpSecret = TwoStepsAuthenticator.Authenticator.GenerateKey();
            var userToken = applicationUserTokenRepository.Entities
                .FirstOrDefault(x => x.UserId.Equals(user.Id) && x.Name == "EmailVerify");

            if (userToken is not null)
            {
                await applicationUserTokenRepository.DeleteAsync(userToken);
            }

            await applicationUserTokenRepository.AddAsync(new ApplicationUserTokens<TId>
            {
                UserId = user.Id,
                Value = otpSecret,
                Name = "EmailVerify",
                LoginProvider = "[AuthService]",
            });

            var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();

            return OperationResult<string>.Success(authenticator.GetCode(otpSecret, DateTime.Now));
        }

        /// <inheritdoc />
        public async Task<OperationResult<string>> GetConfirmPhoneCode(ApplicationUser<TId> user)
        {

            var otpSecret = TwoStepsAuthenticator.Authenticator.GenerateKey();
            var userToken = applicationUserTokenRepository.Entities
                .FirstOrDefault(x => x.UserId.Equals(user.Id) && x.Name == "PhoneVerify");

            if (userToken is not null)
            {
                await applicationUserTokenRepository.DeleteAsync(userToken);
            }

            await applicationUserTokenRepository.AddAsync(new ApplicationUserTokens<TId>
            {
                UserId = user.Id,
                Value = otpSecret,
                Name = "PhoneVerify",
                LoginProvider = "[AuthService]",
            });

            var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();

            return OperationResult<string>.Success(authenticator.GetCode(otpSecret, DateTime.Now));
        }

        /// <inheritdoc />
        public async Task<OperationResult> ConfirmPhoneCode(ApplicationUser<TId> user, string code)
        {
            var otpSecret = applicationUserTokenRepository.Entities
                .FirstOrDefault(x => x.UserId.Equals(user.Id) && x.Name == "PhoneVerify");

            if (otpSecret is null || string.IsNullOrEmpty(otpSecret.Value))
            {
                return OperationResult.Failed(ErrorCodes.MobileNotVerified,
                    "Failed to verify the phone number");
            }

            var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();
            var checkCode = authenticator.CheckCode(otpSecret.Value, code, user);

            if (!checkCode)
                return OperationResult.Failed(ErrorCodes.InvalidTwoFactorCode,
                    "Failed to verify the phone number");

            user.IsPhoneNumberConfirmed = true;
            await applicationUserRepository.UpdateAsync(user);
            await applicationUserTokenRepository.DeleteAsync(otpSecret);
            return OperationResult.Success();

        }

        /// <inheritdoc />
        public async Task<OperationResult> ConfirmEmailCode(ApplicationUser<TId> user, string code)
        {

            var otpSecret = applicationUserTokenRepository.Entities
                .FirstOrDefault(x => x.UserId.Equals(user.Id) && x.Name == "EmailVerify");
            if (otpSecret is null || string.IsNullOrEmpty(otpSecret.Value))
            {
                return OperationResult.Failed(ErrorCodes.EmailNotVerified,
                    "Failed to verify the email address");
            }

            var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();
            var checkCode = authenticator.CheckCode(otpSecret.Value, code, user);

            if (!checkCode)
                return OperationResult.Failed(ErrorCodes.InvalidTwoFactorCode,
                    "Failed to verify the email address");

            user.IsEmailConfirmed = true;
            await applicationUserRepository.UpdateAsync(user);
            await applicationUserTokenRepository.DeleteAsync(otpSecret);
            return OperationResult.Success();

        }

        /// <inheritdoc />
        public async Task<OperationResult<string>> EnableTwoFactorAuthentication(ApplicationUser<TId> user)
        {
            user.IsTwoFactorEnabled = true;
            await applicationUserRepository.UpdateAsync(user);

            var userToken = applicationUserTokenRepository.Entities
                .FirstOrDefault(x => x.UserId.Equals(user.Id) && x.Name == "TwoFactorCode");

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

        /// <inheritdoc />
        public List<Claim> GetUserClaims(ApplicationUser<TId> user)
        {
            return
            [
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? "")
            ];
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
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
        Repository<int, ApplicationUser<int>, AuthContext<int>> repository,
        CacheService cacheService
        )
        : IUserManager<TId> where TId : IEquatable<TId>
    {
        private const string CacheKeyPrefix = "ApplicationUser_";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<OperationResult> RegisterUserAsync(ApplicationUser<TId> user)
        {
            // Validate the email address
            if (!await IsEmailValidAsync(user.Email))
            {
                throw new ApiErrorException(ErrorCodes.InvalidEmailAddress);
            }

            // Check if the user already exists in cache
            var cacheUser = cacheService.GetAsync<ApplicationUser<TId>>(CacheKeyPrefix + user.Email);
            if (cacheUser is not null && ServiceExtension.Configuration.User.RequireUniqueEmail)
                return new OperationResult
                {
                    ErrorCode = ErrorCodes.EmailTaken,
                    ErrorDescription = "User with specified email already exists",
                    Succeeded = false
                };
            if (cacheUser is null && ServiceExtension.Configuration.User.RequireUniqueEmail && repository.Entities.Any(x => x.Email == user.Email))
            {
                return new OperationResult
                {
                    ErrorCode = ErrorCodes.EmailTaken,
                    ErrorDescription = "User with specified email already exists",
                    Succeeded = false
                };
            }

            // Create a new user


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<OperationResult> RegisterUserAsync(ApplicationUser<TId> user, string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<LoginResult> LoginAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="otp"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<LoginResult> LoginTwoFactorAsync(string username, string password, string otp)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<OperationResult> GetConfirmEmailCode(ApplicationUser<TId> user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<OperationResult> GetConfirmPhoneCode(ApplicationUser<TId> user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<OperationResult> ConfirmPhoneCode(ApplicationUser<TId> user, string code)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<OperationResult> GetConfirmPhoneCode(ApplicationUser<TId> user, string code)
        {
            throw new NotImplementedException();
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
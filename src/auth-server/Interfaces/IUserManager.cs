using AuthServer.Domain;
using AuthServer.Shared.Results;

namespace AuthServer.Interfaces
{
    public interface IUserManager<TId> where TId : IEquatable<TId>
    {
        Task<OperationResult> RegisterUserAsync(ApplicationUser<TId> user);
        Task<OperationResult> RegisterUserAsync(ApplicationUser<TId> user, string password);
        Task<LoginResult> LoginAsync(string username, string password);
        Task<LoginResult> LoginTwoFactorAsync(string username, string password, string otp);
        Task<OperationResult<string>> GetConfirmEmailCode(ApplicationUser<TId> user);
        Task<OperationResult<string>> GetConfirmPhoneCode(ApplicationUser<TId> user);
        Task<OperationResult> ConfirmPhoneCode(ApplicationUser<TId> user, string code);
        Task<OperationResult> ConfirmEmailCode(ApplicationUser<TId> user, string code);
        Task<OperationResult<string>> EnableTwoFactorAuthentication(ApplicationUser<TId> user);
        Task<OperationResult<string>> GetTwoFactorCodeAsync(ApplicationUser<TId> user);
    }
}
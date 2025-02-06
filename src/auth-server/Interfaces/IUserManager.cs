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
        Task<OperationResult> GetConfirmEmailCode(ApplicationUser<TId> user);
        Task<OperationResult> GetConfirmPhoneCode(ApplicationUser<TId> user);
        Task<OperationResult> ConfirmPhoneCode(ApplicationUser<TId> user, string code);
        Task<OperationResult> GetConfirmPhoneCode(ApplicationUser<TId> user, string code);
    }
}
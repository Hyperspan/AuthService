using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [Keyless]
    public class ApplicationUserTokens<TId> : IdentityUserToken<TId> where TId : IEquatable<TId>;
}

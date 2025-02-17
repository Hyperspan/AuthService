#pragma warning disable
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Domain
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    [Keyless]
    public class ApplicationUserRoles<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser<TId> User { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ApplicationRole<TId> Role { get; set; }
    }
}

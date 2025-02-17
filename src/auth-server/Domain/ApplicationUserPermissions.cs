namespace AuthServer.Domain
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public class ApplicationUserPermissions<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual ApplicationUser<TId> User { get; set; } = default!;
        /// <summary>
        /// 
        /// </summary>
        public virtual ApplicationPermission<TId> Permission { get; set; } = default!;

    }
}

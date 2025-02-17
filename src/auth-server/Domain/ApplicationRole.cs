namespace AuthServer.Domain
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public class ApplicationRole<TId> : AuthEntity<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<ApplicationUser<TId>> Users { get; set; } = new List<ApplicationUser<TId>>();
        
        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<ApplicationUserRoles<TId>> UsersRoles { get; set; } =
            new List<ApplicationUserRoles<TId>>();
    }
}
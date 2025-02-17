namespace AuthServer.Domain
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public class ApplicationUser<TId> : AuthEntity<TId> where TId : IEquatable<TId>
    {

        /// <summary>
        /// 
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public bool IsTwoFactorEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsLockoutEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int LockoutDuration { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public bool IsPhoneNumberConfirmed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? AccessFailedCount { get; set; }

    }
}

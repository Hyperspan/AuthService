namespace AuthServer.Domain
{
    public class ApplicationUser<TId> : AuthEntity where TId : IEquatable<TId>
    {

        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public bool IsLockoutEnabled { get; set; }
        // In seconds
        public int LockoutDuration { get; set; }
        public string PasswordHash { get; set; }
        public bool IsPhoneNumberConfirmed { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public int? AccessFailedCount { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;

namespace AuthServer.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Configurations
    {

        /// <summary>
        /// Gets or sets the <see cref="ClaimsIdentityOptions"/> for the identity system.
        /// </summary>
        /// <value>
        /// The <see cref="ClaimsIdentityOptions"/> for the identity system.
        /// </value>
        public ClaimsIdentityOptions ClaimsIdentity { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="UserOptions"/> for the identity system.
        /// </summary>
        /// <value>
        /// The <see cref="UserOptions"/> for the identity system.
        /// </value>
        public UserOptions User { get; set; } = new()
        {
            RequireUniqueEmail = true
        };

        /// <summary>
        /// Gets or sets the <see cref="PasswordOptions"/> for the identity system.
        /// </summary>
        /// <value>
        /// The <see cref="PasswordOptions"/> for the identity system.
        /// </value>
        public PasswordOptions Password { get; set; } = new()
        {
            RequireDigit = true,
            RequireLowercase = true,
            RequireNonAlphanumeric = true,
            RequireUppercase = true,
            RequiredLength = 6,
        };

        /// <summary>
        /// Gets or sets the <see cref="LockoutOptions"/> for the identity system.
        /// </summary>
        /// <value>
        /// The <see cref="LockoutOptions"/> for the identity system.
        /// </value>
        public LockoutOptions Lockout { get; set; } = new()
        {
            AllowedForNewUsers = false,
            DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10),
            MaxFailedAccessAttempts = 5
        };

        /// <summary>
        /// Gets or sets the <see cref="SignInOptions"/> for the identity system.
        /// </summary>
        /// <value>
        /// The <see cref="SignInOptions"/> for the identity system.
        /// </value>
        public SigninConfiguration SignIn { get; set; } = new()
        {
            RequireConfirmedAccount = true,
            RequireConfirmedEmail = true,
            RequireConfirmedPhoneNumber = true,
            RequireTwoFactorEnabled = true,
            RestrictMultiLogin = false
        };

        /// <summary>
        /// Gets or sets the <see cref="TokenOptions"/> for the identity system.
        /// </summary>
        /// <value>
        /// The <see cref="TokenOptions"/> for the identity system.
        /// </value>
        public TokenOptions Tokens { get; set; } = new();
    }
}
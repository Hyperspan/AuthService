using Microsoft.AspNetCore.Identity;

namespace AuthServer.Shared;

/// <inheritdoc cref="SignInOptions"/>>
public class SigninConfiguration : SignInOptions
{
    /// <summary>
    /// Is TFA essential for login
    /// </summary>
    public bool RequireTwoFactorEnabled { get; set; }

    /// <summary>
    /// Flag to restrict multi login. If true, only one session is allowed per user other will be logged out. (using signalr)
    /// </summary>
    public bool RestrictMultiLogin { get; set; }


}
using Microsoft.AspNetCore.Identity;

namespace AuthServer.Shared;

/// <inheritdoc cref="SignInOptions"/>>
public class SigninConfiguration : SignInOptions
{
    /// <summary>
    /// Is TFA essential for login
    /// </summary>
    public bool RequireTwoFactorEnabled { get; set; }
}
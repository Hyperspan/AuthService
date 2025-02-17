using AuthServer.Shared.Enums;

namespace AuthServer.Domain
{
    public class UserLoginSessions<TId> where TId : IEquatable<TId>
    {
        public TId Id { get; set; }
        public TId UserId { get; set; }
        public string SessionId { get; set; }
        public LoginStatus Status { get; set; } = LoginStatus.Idle;
        public string IpAddress { get; set; }
        public string RefreshToken { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LoginDisconnectedAt { get; set; }
    }
}

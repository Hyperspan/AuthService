using System.Collections.Concurrent;
using System.ComponentModel;
using AuthServer.Domain;
using AuthServer.Interfaces;
using AuthServer.Shared.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace AuthServer.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class SessionHubService<TId>(
        IRepository<TId, UserLoginSessions<TId>, AuthContext<TId>> loginSessionRepository,
        IHttpContextAccessor httpContextAccessor) : Hub where TId : IEquatable<TId>
    {

        public override async Task OnConnectedAsync()
        {
            if (!string.IsNullOrEmpty(Context.UserIdentifier))
            {
                var userId = (TId?)TypeDescriptor.GetConverter(typeof(TId)).ConvertFromInvariantString(Context.UserIdentifier);

                // If the user already has an active connection, disconnect it
                var existingConnectionId = loginSessionRepository.Entities.FirstOrDefault(x =>
                    x.UserId == userId && x.Status == LoginStatus.Connected);

                if (existingConnectionId is not null)
                {
                    await Clients.Client(existingConnectionId.SessionId).SendAsync("ForceLogout");
                    existingConnectionId.Status = LoginStatus.Disconnected;
                    existingConnectionId.LoginDisconnectedAt = DateTime.UtcNow;
                    await loginSessionRepository.UpdateAsync(existingConnectionId);
                }

                // Store new connection
                var newConnection = new UserLoginSessions<TId>
                {
                    UserId = ,
                    SessionId = Context.ConnectionId,
                    IpAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString(),
                    LoginTime = DateTime.UtcNow,
                    Status = LoginStatus.Connected
                };

            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections.TryRemove(userId, out _);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}

using AuthServer.Domain;
using AuthServer.Services;
using AuthServer;
using Microsoft.EntityFrameworkCore;

namespace auth_server.tests
{
    internal static class Helper
    {
        internal static AuthContext<TId> GetContext<TId>() where TId : IEquatable<TId>
        {
            var options = new DbContextOptionsBuilder<AuthContext<TId>>()
                .UseSqlite("Filename=:memory:") // Use SQLite instead of InMemoryDatabase
                .Options;

            var context = new AuthContext<TId>(options);
            context.Database.OpenConnection(); // Required for SQLite in-memory
            context.Database.EnsureCreated();
            return context;
        }

        internal static Repository<TId, ApplicationUser<TId>, AuthContext<TId>> GetRepoInstance<TId>(
            this AuthContext<TId> authContext) where TId : IEquatable<TId>
        {
            return new Repository<TId, ApplicationUser<TId>, AuthContext<TId>>(authContext);
        }
    }
}
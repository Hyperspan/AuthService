using AuthServer.Domain;
using Microsoft.EntityFrameworkCore;

namespace AuthServer
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthContext<TId> : DbContext where TId : IEquatable<TId>
    {
        /// <summary>
        /// Auth context for entity framework
        /// </summary>
        /// <param name="options"></param>
        public AuthContext(DbContextOptions<AuthContext<TId>> options) : base(options)
        {

        }

        /// <summary>
        /// Auth context for entity framework
        /// </summary>
        public AuthContext()
        {

        }

        /// <summary>
        /// Override the model creating method to configure and modify tables
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
                         .SelectMany(t => t.GetProperties())
                         .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser<TId>>(entity => { entity.ToTable("Users", schema: "Account"); });
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<ApplicationUser<TId>>? ApplicationUsers { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<ApplicationUserTokens<TId>>? ApplicationUserTokens { get; set; }
    }
}
using AuthServer;
using AuthServer.Domain;
using AuthServer.Interfaces;
using AuthServer.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace auth_server.tests.Services
{
    public class UserManagerTests
    {
        private readonly IUserManager<int> _userManager;
        private readonly Repository<int, ApplicationUser<int>, AuthContext<int>> _repository;
        private readonly AuthContext<int> _authContext;
        public UserManagerTests()
        {
            _userManager = new UserManager<int>();


        }

        [Fact]
        public async Task RegisterUserAsync_ThrowsException()
        {
            await Assert.ThrowsAsync<NotImplementedException>(() => _userManager.RegisterUserAsync(It.IsAny<ApplicationUser<int>>()));
        }
    }
}

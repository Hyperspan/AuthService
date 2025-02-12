using AuthServer;
using AuthServer.Domain;
using AuthServer.Interfaces;
using AuthServer.Services;
using AuthServer.Shared;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace auth_server.tests.Services
{
    public class UserManagerTests
    {
        private const string CacheKeyPrefix = "ApplicationUser_";
        private readonly IUserManager<int> _userManager;
        private readonly Repository<int, ApplicationUser<int>, AuthContext<int>> _repository;
        private readonly AuthContext<int> _authContext = Helper.GetContext<int>();
        private readonly Mock<CacheService> _mockCacheService;

        public UserManagerTests()
        {
            _repository = _authContext.GetRepoInstance();
            _mockCacheService = new Mock<CacheService>(new Mock<IMemoryCache>().Object);
            _userManager = new UserManager<int>(_repository, _mockCacheService.Object);
        }

        #region Register User


        [Fact]
        public async Task RegisterUserAsync_ThrowsException_InvalidEmailAddress()
        {
            var user = new ApplicationUser<int>
            {
                Email = "test@assdad.com",
                UserName = "user"
            };
            var result = await _userManager.RegisterUserAsync(user);

            Assert.False(result.Succeeded);
            Assert.Equal(ErrorCodes.InvalidEmailAddress, result.ErrorCode);
        }

        [Fact]
        public async Task RegisterUserAsync_ThrowsException_HasUserInCache()
        {
            var user = new ApplicationUser<int>
            {
                Email = "test@google.com",
                UserName = "user"
            };

            _mockCacheService.Setup(x => x.GetAsync<ApplicationUser<int>>(CacheKeyPrefix + user.Email))
                .Returns(user);

            var result = await _userManager.RegisterUserAsync(user);

            Assert.False(result.Succeeded);
            Assert.Equal(ErrorCodes.EmailTaken, result.ErrorCode);
        }

        [Fact]
        public async Task RegisterUserAsync_ThrowsException_NotUserInCache()
        {
            var user = new ApplicationUser<int>
            {
                Email = "test@google.com",
                UserName = "user"
            };

            _mockCacheService.Setup(x => x.GetAsync<object?>(It.IsAny<string>()))
                .Returns(null);

            await _repository.AddAsync(user);

            var result = await _userManager.RegisterUserAsync(user);

            Assert.False(result.Succeeded);
            Assert.Equal(ErrorCodes.EmailTaken, result.ErrorCode);
        }

        [Fact]
        public async Task RegisterUserAsync_ThrowsException_UserNameTaken()
        {
            var userExisting = new ApplicationUser<int>
            {
                Email = "test@google.com",
                UserName = "user"
            };

            _mockCacheService.Setup(x => x.GetAsync<object?>(CacheKeyPrefix + userExisting.Email))
                .Returns(userExisting);

            await _repository.AddAsync(userExisting);

            var user = new ApplicationUser<int>
            {
                Email = "Testing@google.com",
                UserName = "user"
            };

            var result = await _userManager.RegisterUserAsync(user);

            Assert.False(result.Succeeded);
            Assert.Equal(ErrorCodes.UsernameTaken, result.ErrorCode);
        }

        [Fact]
        public async Task RegisterUserAsync_ReturnsSuccess()
        {
            var userExisting = new ApplicationUser<int>
            {
                Email = "test@google.com",
                UserName = "user"
            };

            _mockCacheService.Setup(x => x.GetAsync<object?>(CacheKeyPrefix + userExisting.Email))
                .Returns(userExisting);

            await _repository.AddAsync(userExisting);

            var user = new ApplicationUser<int>
            {
                Email = "Testing@google.com",
                UserName = "User"
            };

            var result = await _userManager.RegisterUserAsync(user);

            Assert.True(result.Succeeded);
            // Assert.Equal(ErrorCodes.UsernameTaken, result.ErrorCode);
        }

        [Fact]
        public async Task RegisterUserWithPasswordAsync_ReturnsSuccess()
        {
            var userExisting = new ApplicationUser<int>
            {
                Email = "test@google.com",
                UserName = "user"
            };

            _mockCacheService.Setup(x => x.GetAsync<object?>(CacheKeyPrefix + userExisting.Email))
                .Returns(userExisting);

            await _repository.AddAsync(userExisting);

            var user = new ApplicationUser<int>
            {
                Email = "Testing@google.com",
                UserName = "User"
            };

            var result = await _userManager.RegisterUserAsync(user, "test");
            Assert.True(result.Succeeded);

            var addedUser = _repository.Entities.FirstOrDefault(x => x.Email == user.Email);

            Assert.NotNull(addedUser);
            Assert.Equal("test".GetHash(), addedUser.PasswordHash);
        }

        #endregion


        #region Login

        

        #endregion

    }
}
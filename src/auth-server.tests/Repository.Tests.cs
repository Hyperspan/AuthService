using AuthServer;
using AuthServer.Domain;
using AuthServer.Services;
using AuthServer.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;

namespace auth_server.tests
{
    public class RepositoryTests
    {
        private readonly Mock<AuthContext<int>> _mockDbContext;
        private readonly Mock<DbSet<ApplicationUser<int>>> _mockDbSet;
        private readonly Repository<int, ApplicationUser<int>, DbContext> _repository;
        private readonly AuthContext<int> _authContext;

        public RepositoryTests()
        {
            _mockDbContext = new Mock<AuthContext<int>>();
            _mockDbSet = new Mock<DbSet<ApplicationUser<int>>>();
            _mockDbContext.Setup(db => db.Set<ApplicationUser<int>>()).Returns(_mockDbSet.Object);
            _repository = new Repository<int, ApplicationUser<int>, DbContext>(_mockDbContext.Object);
            _authContext = Helper.GetContext<int>();
        }

        [Fact]
        public async Task GetCount_ShouldReturnCount()
        {
            // Arrange
            var repository = await SaveUserDb(new ApplicationUser<int> { Id = 1, FullName = "Test" });

            var count = await repository.GetCount();

            Assert.Equal(1, count);
        }

        [Fact]
        public void Context_ShouldBeContext()
        {
            var repository = new Repository<int, ApplicationUser<int>, AuthContext<int>>(_authContext);
            Assert.Equal(_authContext, repository.Context);
        }

        [Fact]
        public async Task Entities_ShouldNotReturnDeletedEntities()
        {
            var repository = new Repository<int, ApplicationUser<int>, AuthContext<int>>(_authContext);
            await SaveUserDb(new ApplicationUser<int> { Id = 1, IsDeleted = true, FullName = "Ayush" });

            Assert.Equal(0, repository.Entities.Count());
        }

        [Fact]
        public async Task GetCountQuery_ShouldReturnCount()
        {
            // Arrange
            var repository = await SaveUserDb(new ApplicationUser<int> { Id = 1, FullName = "Test" });

            var count = await repository.GetCount("SELECT * FROM Users");

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task AddAsync_ShouldAddEntity()
        {
            var entity = new ApplicationUser<int> { Id = 1, IsDeleted = false };

            _mockDbSet.Setup(db => db.AddAsync(entity, default))
                .ReturnsAsync(It.IsAny<EntityEntry<ApplicationUser<int>>>);
            _mockDbContext.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1);

            var result = await _repository.AddAsync(entity);

            Assert.Equal(entity, result);
            _mockDbSet.Verify(db => db.AddAsync(entity, default), Times.Once);
            _mockDbContext.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldThrowError()
        {
            var entity = new ApplicationUser<int> { Id = 1, IsDeleted = false };

            _mockDbSet.Setup(db => db.AddAsync(entity, default))
                .ReturnsAsync(It.IsAny<EntityEntry<ApplicationUser<int>>>);
            _mockDbContext.Setup(db => db.SaveChangesAsync(default)).Throws<Exception>();


            var ex = await Assert.ThrowsAsync<ApiErrorException>(() => _repository.AddAsync(entity));
            Assert.Equal(ErrorCodes.InsertFailed, ex.ErrorCode);
        }

        [Fact]
        public async Task AddRangeAsync_ShouldAddEntity()
        {
            var entity = new List<ApplicationUser<int>>
            {
                new() { Id = 1, IsDeleted = false },
                new() { Id = 2, IsDeleted = false },
                new() { Id = 3, IsDeleted = false }
            };

            _mockDbSet.Setup(db => db.AddRangeAsync(entity, default));
            _mockDbContext.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1);

            var result = await _repository.AddRangeAsync(entity);

            Assert.Equal(true, result);
            _mockDbSet.Verify(db => db.AddRangeAsync(entity, default), Times.Once);
            _mockDbContext.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task AddRangeAsync_ShouldThrowError()
        {
            var entity = new List<ApplicationUser<int>>
            {
                new() { Id = 1, IsDeleted = false },
                new() { Id = 2, IsDeleted = false },
                new() { Id = 3, IsDeleted = false }
            };

            _mockDbSet.Setup(db => db.AddRangeAsync(entity, default));
            _mockDbContext.Setup(db => db.SaveChangesAsync(default)).Throws<Exception>();

            var ex = await Assert.ThrowsAsync<ApiErrorException>(() => _repository.AddRangeAsync(entity));
            Assert.Equal(ErrorCodes.InsertFailed, ex.ErrorCode);
        }

        [Fact]
        public async Task GetById_ShouldReturnEntity()
        {
            var entity = new ApplicationUser<int> { Id = 1, IsDeleted = false };
            _mockDbSet.Setup(db => db.FindAsync(entity.Id)).ReturnsAsync(entity);

            var result = await _repository.GetById(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity, result);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull()
        {
            var entity = new ApplicationUser<int> { Id = 1, IsDeleted = true };
            _mockDbSet.Setup(db => db.FindAsync(entity.Id)).ReturnsAsync(entity);

            var result = await _repository.GetById(entity.Id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenDeleted()
        {
            var entity = new ApplicationUser<int> { Id = 1, IsDeleted = true };
            _mockDbSet.Setup(db => db.FindAsync(entity.Id)).ReturnsAsync(entity);

            var result = await _repository.GetById(entity.Id);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenInvalidId()
        {
            var entity = new ApplicationUser<int> { Id = 0, IsDeleted = false };
            var ex = await Assert.ThrowsAsync<ApiErrorException>(() => _repository.DeleteAsync(entity));
            Assert.Equal(ErrorCodes.InvalidId, ex.ErrorCode);

        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenEntityNotFound()
        {
            var entity = new ApplicationUser<int> { Id = 1, IsDeleted = false };
            _mockDbSet.Setup(db => db.FindAsync(entity.Id)).ReturnsAsync((ApplicationUser<int>?)null);

            var ex = await Assert.ThrowsAsync<ApiErrorException>(() => _repository.DeleteAsync(entity));
            Assert.Equal(ErrorCodes.RecordNotFound, ex.ErrorCode);
        }


        [Fact]
        public async Task DeleteAsync_ShouldThrowException()
        {
            var entity = new ApplicationUser<int> { Id = 1, IsDeleted = false };
            var repository = await SaveUserDb(entity);
            await repository.Context.DisposeAsync();
            var ex = await Assert.ThrowsAsync<ApiErrorException>(() => repository.DeleteAsync(entity));
            Assert.Equal(ErrorCodes.DeleteFailed, ex.ErrorCode);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue()
        {
            var entity = new ApplicationUser<int> { Id = 1, IsDeleted = false };
            var repository = await SaveUserDb(entity);
            var ex = await repository.DeleteAsync(entity);
            Assert.True(ex);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEntity()
        {
            var user = new ApplicationUser<int> { Id = 1, UserName = "TestUser", IsDeleted = false };
            var repository = await SaveUserDb(user);
            user.UserName = "UpdatedUser";
            var result = await repository.UpdateAsync(user);

            Assert.True(result);
            Assert.Equal("UpdatedUser", (_authContext.ApplicationUsers?.FindAsync(user.Id))?.Result?.UserName);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException()
        {
            var user = new ApplicationUser<int> { Id = 1, UserName = "TestUser", IsDeleted = false };
            await SaveUserDb(user);
            user = new ApplicationUser<int> { Id = 2, UserName = "TestUser", IsDeleted = false };
            var repository = await SaveUserDb(user);
            user.Id = 1;
            var result = await Assert.ThrowsAsync<ApiErrorException>(async () => await repository.UpdateAsync(user));
            Assert.Equal(ErrorCodes.UpdateFailed, result.ErrorCode);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowExceptionInvalidId()
        {
            var user = new ApplicationUser<int> { Id = 1, UserName = "TestUser", IsDeleted = false };
            await SaveUserDb(user);
            user = new ApplicationUser<int> { Id = 2, UserName = "TestUser", IsDeleted = false };
            var repository = await SaveUserDb(user);
            user.Id = 0;
            user.UserName = "Updated";
            var result = await Assert.ThrowsAsync<ApiErrorException>(async () => await repository.UpdateAsync(user));
            Assert.Equal(ErrorCodes.InvalidId, result.ErrorCode);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowExceptionRecordNotFound()
        {
            var user = new ApplicationUser<int> { Id = 1, UserName = "TestUser", IsDeleted = false };
            await SaveUserDb(user);
            user = new ApplicationUser<int> { Id = 2, UserName = "TestUser", IsDeleted = true };
            var repository = await SaveUserDb(user);
            user.UserName = "Updated";
            var result = await Assert.ThrowsAsync<ApiErrorException>(async () => await repository.UpdateAsync(user));
            Assert.Equal(ErrorCodes.RecordNotFound, result.ErrorCode);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowExceptionRecordNotFoundWhenNull()
        {
            var user = new ApplicationUser<int> { Id = 1, UserName = "TestUser", IsDeleted = false };
            var repository = await SaveUserDb(user);
            user = new ApplicationUser<int> { Id = 2, UserName = "TestUser", IsDeleted = true };
            user.UserName = "Updated";
            var result = await Assert.ThrowsAsync<ApiErrorException>(async () => await repository.UpdateAsync(user));
            Assert.Equal(ErrorCodes.RecordNotFound, result.ErrorCode);
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList()
        {
            var user = new ApplicationUser<int> { Id = 1, UserName = "TestUser", IsDeleted = true };
            var repository = await SaveUserDb(user);
            var result = await repository.GetAllAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnList()
        {
            var user = new ApplicationUser<int> { Id = 1, UserName = "TestUser", IsDeleted = false };
            var repository = await SaveUserDb(user);
            var result = await repository.GetAllAsync();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(user, result.FirstOrDefault());
        }

        [Fact]
        public async Task GetAllQuery_ShouldReturnList()
        {
            var user = new ApplicationUser<int> { Id = 1, UserName = "TestUser", IsDeleted = false };
            var repository = await SaveUserDb(user);
            var result = await repository.GetAllAsync("Select * from Users");

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetAllQuery_ShouldThrowException()
        {
            var user = new ApplicationUser<int> { Id = 1, UserName = "TestUser", IsDeleted = false };
            var repository = await SaveUserDb(user);
            var result =
                await Assert.ThrowsAsync<ApiErrorException>(async () =>
                    await repository.GetAllAsync("Select * from Userss"));

            Assert.Equal(ErrorCodes.QueryFailed, result.ErrorCode);
        }

        [Fact]
        public async Task GetAll_ShouldThrowError()
        {
            _mockDbContext.Setup(db => db.ApplicationUsers).Throws<Exception>();
            var ex = await Assert.ThrowsAsync<ApiErrorException>(_repository.GetAllAsync);
            Assert.Equal(ErrorCodes.QueryFailed, ex.ErrorCode);
        }


        private async Task<Repository<int, ApplicationUser<int>, AuthContext<int>>> SaveUserDb(
            ApplicationUser<int> user)
        {
            var repository = _authContext.GetRepoInstance();
            _authContext.ApplicationUsers?.Add(user);
            await _authContext.SaveChangesAsync();
            return repository;
        }
    }
}
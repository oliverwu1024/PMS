using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PaintStore.API.Dtos;
using PaintStore.API.Repos;
using PaintStore.API.Services;
using PaintStore.Models.Methods;

namespace PaintStore.Tests.Services;

// Same pattern as OrderServiceTests: real UserService, fake (Moq) repository.
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _service = new UserService(_userRepo.Object, NullLogger<UserService>.Instance);
    }

    private static User MakeUser(int id, string name = "alice", string email = "alice@test.com")
    {
        return new User(name, email, "0400000000", 30, id);
    }

    // ---------- GetPagedAsync ----------

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -5)]
    public async Task GetPagedAsync_WhenPagingInvalid_ShouldThrowArgumentException(int pageNumber, int pageSize)
    {
        var act = () => _service.GetPagedAsync(pageNumber, pageSize);

        await act.Should().ThrowAsync<ArgumentException>();
        _userRepo.Verify(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetPagedAsync_WhenPagingValid_ShouldReturnRepositoryResult()
    {
        var users = new List<User> { MakeUser(1) };
        _userRepo.Setup(r => r.GetPagedAsync(1, 1)).ReturnsAsync(users);

        var result = await _service.GetPagedAsync(1, 1);

        result.Should().BeSameAs(users);
    }

    // ---------- GetByIdAsync ----------

    [Fact]
    public async Task GetByIdAsync_WhenUserMissing_ShouldThrowKeyNotFound()
    {
        _userRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((User?)null);

        var act = () => _service.GetByIdAsync(42);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ShouldReturnUser()
    {
        var user = MakeUser(42);
        _userRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(user);

        var result = await _service.GetByIdAsync(42);

        result.Should().BeSameAs(user);
    }

    // ---------- GetByNameAsync / GetByEmailAsync ----------

    [Fact]
    public async Task GetByNameAsync_WhenNoMatch_ShouldThrowKeyNotFound()
    {
        _userRepo.Setup(r => r.GetByNameAsync("nobody")).ReturnsAsync(new List<User>());

        var act = () => _service.GetByNameAsync("nobody");

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetByNameAsync_WhenMatch_ShouldReturnUsers()
    {
        var users = new List<User> { MakeUser(1) };
        _userRepo.Setup(r => r.GetByNameAsync("alice")).ReturnsAsync(users);

        var result = await _service.GetByNameAsync("alice");

        result.Should().BeSameAs(users);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenNoMatch_ShouldThrowKeyNotFound()
    {
        _userRepo.Setup(r => r.GetByEmailAsync("x@test.com")).ReturnsAsync(new List<User>());

        var act = () => _service.GetByEmailAsync("x@test.com");

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetByEmailAsync_WhenMatch_ShouldReturnUsers()
    {
        var users = new List<User> { MakeUser(1) };
        _userRepo.Setup(r => r.GetByEmailAsync("alice@test.com")).ReturnsAsync(users);

        var result = await _service.GetByEmailAsync("alice@test.com");

        result.Should().BeSameAs(users);
    }

    // ---------- CreateAsync ----------

    [Fact]
    public async Task CreateAsync_WhenCalled_ShouldCopyRequestFieldsAndSaveOnce()
    {
        // the fake gives back whatever user it was given
        _userRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var request = new UserCreateRequest
        {
            UserName = "alice",
            UserEmail = "alice@test.com",
            UserPhone = "0400000000",
            Age = 30
        };

        var result = await _service.CreateAsync(request);

        result.UserName.Should().Be("alice");
        result.Email.Should().Be("alice@test.com");
        result.PhoneNumber.Should().Be("0400000000");
        result.Age.Should().Be(30);
        // It.Is<User>(...) = "with a user that matches this condition"
        _userRepo.Verify(r => r.AddAsync(It.Is<User>(u => u.UserName == "alice")), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenRepositoryThrows_ShouldPassExceptionThrough()
    {
        // .ThrowsAsync = script the fake to fail, like the database rejecting the insert
        _userRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
                 .ThrowsAsync(new DbUpdateException("Add user failed"));

        var request = new UserCreateRequest
        {
            UserName = "alice",
            UserEmail = "alice@test.com",
            UserPhone = "0400000000",
            Age = 30
        };

        var act = () => _service.CreateAsync(request);

        await act.Should().ThrowAsync<DbUpdateException>();
    }

    // ---------- UpdateAsync ----------

    [Fact]
    public async Task UpdateAsync_WhenUserMissing_ShouldThrowAndNotSave()
    {
        _userRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((User?)null);

        var request = new UserUpdateRequest
        {
            UserName = "bob",
            UserEmail = "bob@test.com",
            UserPhone = "0411111111",
            Age = 40
        };

        var act = () => _service.UpdateAsync(42, request);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        _userRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserExists_ShouldChangeFieldsAndSave()
    {
        var user = MakeUser(42);
        _userRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(user);

        var request = new UserUpdateRequest
        {
            UserName = "bob",
            UserEmail = "bob@test.com",
            UserPhone = "0411111111",
            Age = 40
        };

        var result = await _service.UpdateAsync(42, request);

        result.UserName.Should().Be("bob");
        result.Email.Should().Be("bob@test.com");
        result.PhoneNumber.Should().Be("0411111111");
        result.Age.Should().Be(40);
        _userRepo.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    // ---------- DeleteAsync ----------

    [Fact]
    public async Task DeleteAsync_WhenUserMissing_ShouldThrowAndNotDelete()
    {
        _userRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((User?)null);

        var act = () => _service.DeleteAsync(42);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        _userRepo.Verify(r => r.DeleteAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserExists_ShouldDeleteThatUser()
    {
        var user = MakeUser(42);
        _userRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(user);

        await _service.DeleteAsync(42);

        _userRepo.Verify(r => r.DeleteAsync(user), Times.Once);
    }
}

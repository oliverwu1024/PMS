using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PaintStore.API.Data;
using PaintStore.API.Repos;
using PaintStore.Models.Methods;

namespace PaintStore.Tests.Repos;

// Same pattern as OrderRepositoryTests: real UserRepository, real AppDbContext, in-memory database.
public class UserRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _userRepository = new UserRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    // ---------- helpers ----------

    private static User MakeUser(int id, string name, string email)
    {
        return new User(name, email, "0400000000", 30, id);
    }

    private async Task SeedAsync(params User[] users)
    {
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();
    }

    // ---------- AddAsync ----------

    [Fact]
    public async Task AddAsync_WhenValidUser_ShouldSaveToDatabase()
    {
        var user = MakeUser(0, "alice", "alice@test.com");

        await _userRepository.AddAsync(user);

        var saved = await _context.Users.FirstOrDefaultAsync(u => u.Email == "alice@test.com");
        saved.Should().NotBeNull();
        saved!.UserName.Should().Be("alice");
    }

    [Fact]
    public async Task AddAsync_WhenIdIsZero_ShouldGetIdFromDatabase()
    {
        // UserService.CreateAsync always passes userId: 0 and relies on the database to assign one
        var user = MakeUser(0, "alice", "alice@test.com");

        var result = await _userRepository.AddAsync(user);

        result.Id.Should().BeGreaterThan(0);
    }

    // ---------- GetByIdAsync ----------

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ShouldReturnUser()
    {
        await SeedAsync(MakeUser(1, "alice", "alice@test.com"));

        var result = await _userRepository.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.UserName.Should().Be("alice");
    }

    [Theory]
    [InlineData(999)]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetByIdAsync_WhenUserMissing_ShouldReturnNull(int id)
    {
        await SeedAsync(MakeUser(1, "alice", "alice@test.com"));

        var result = await _userRepository.GetByIdAsync(id);

        result.Should().BeNull();
    }

    // ---------- GetByNameAsync / GetByEmailAsync ----------

    [Fact]
    public async Task GetByNameAsync_ShouldReturnOnlyExactMatches()
    {
        await SeedAsync(
            MakeUser(1, "alice", "a1@test.com"),
            MakeUser(2, "bob", "b@test.com"),
            MakeUser(3, "alice", "a2@test.com"),
            MakeUser(4, "alice2", "a3@test.com"));   // similar name, must NOT match

        var result = await _userRepository.GetByNameAsync("alice");

        result.Select(u => u.Id).Should().BeEquivalentTo(new[] { 1, 3 });
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnMatchingUser()
    {
        await SeedAsync(MakeUser(1, "alice", "alice@test.com"), MakeUser(2, "bob", "bob@test.com"));

        var result = await _userRepository.GetByEmailAsync("bob@test.com");

        result.Should().ContainSingle().Which.Id.Should().Be(2);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenNoMatch_ShouldReturnEmpty()
    {
        await SeedAsync(MakeUser(1, "alice", "alice@test.com"));

        var result = await _userRepository.GetByEmailAsync("nobody@test.com");

        result.Should().BeEmpty();
    }

    // ---------- GetPagedAsync ----------

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPageOrderedById()
    {
        await SeedAsync(
            MakeUser(3, "c", "c@test.com"),
            MakeUser(1, "a", "a@test.com"),
            MakeUser(2, "b", "b@test.com"));

        var page1 = await _userRepository.GetPagedAsync(1, 2);
        var page2 = await _userRepository.GetPagedAsync(2, 2);

        page1.Select(u => u.Id).Should().Equal(1, 2);
        page2.Select(u => u.Id).Should().Equal(3);
    }

    // ---------- UpdateAsync / DeleteAsync ----------

    [Fact]
    public async Task UpdateAsync_ShouldSaveChangesToDatabase()
    {
        var user = MakeUser(1, "alice", "alice@test.com");
        await SeedAsync(user);

        user.Age = 55;
        await _userRepository.UpdateAsync(user);

        var saved = await _context.Users.FirstAsync(u => u.Id == 1);
        saved.Age.Should().Be(55);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveUserFromDatabase()
    {
        var alice = MakeUser(1, "alice", "alice@test.com");
        await SeedAsync(alice, MakeUser(2, "bob", "bob@test.com"));

        await _userRepository.DeleteAsync(alice);

        _context.Users.Select(u => u.Id).Should().Equal(2);
    }
}

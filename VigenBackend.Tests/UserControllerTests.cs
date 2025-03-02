using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vigen_Repository.Models;
using Vigen_Repository.Controllers;
using VigenBackend.Tests.Mocks; // Importa las clases de mock

public class UserControllerTests
{
    private readonly Mock<vigendbContext> _mockContext;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly UserController _controller;
    private readonly List<User> _userList;

    public UserControllerTests()
    {
        _mockContext = new Mock<vigendbContext>();
        _mockConfig = new Mock<IConfiguration>();

        _userList = new List<User>
        {
            new User { Identification = "12345", Name = "Test User", Password = "securePass", Email = "test@example.com" }
        };

        var mockSet = new Mock<DbSet<User>>();
        var asyncUserList = new AsyncEnumerableMock<User>(_userList);

        mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(asyncUserList.Provider);
        mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(asyncUserList.Expression);
        mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(asyncUserList.ElementType);
        mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => _userList.GetEnumerator());
        mockSet.As<IAsyncEnumerable<User>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
               .Returns(new AsyncEnumeratorMock<User>(_userList.GetEnumerator()));

        _mockContext.Setup(c => c.Users).Returns(mockSet.Object);

        _controller = new UserController(_mockContext.Object, _mockConfig.Object);
    }

    [Fact]
    public async Task GetUsers_ReturnsUsers_WhenUsersExist()
    {
        var result = await _controller.getUsers();

        var actionResult = Assert.IsType<ActionResult<List<User>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedUsers = Assert.IsType<List<User>>(okResult.Value);

        Assert.Single(returnedUsers);
    }

    [Fact]
    public async Task PostUser_CreatesUserSuccessfully()
    {
        var newUser = new User { Identification = "67890", Name = "New User", Password = "securePass", Email = "new@example.com" };

        _mockContext.Setup(c => c.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((User user, CancellationToken token) => null);

        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _controller.postUser(newUser);

        var actionResult = Assert.IsType<ActionResult<User>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
    }

    [Fact]
    public async Task DeleteUser_RemovesUserSuccessfully()
    {
        _mockContext.Setup(c => c.Users.FindAsync("12345"))
                    .ReturnsAsync(_userList[0]);

        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _controller.DeleteUser("12345");

        var actionResult = Assert.IsType<ActionResult<User>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
    }
}

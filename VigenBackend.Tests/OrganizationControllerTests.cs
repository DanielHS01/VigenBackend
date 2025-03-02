using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vigen_Repository.Models;
using Vigen_Repository.Controllers;
using VigenBackend.Tests.Mocks; // Importa las clases de mock

public class OrganizationControllerTests
{
    private readonly Mock<vigendbContext> _mockContext;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly OrganizationController _controller;
    private readonly List<Organization> _organizationList;

    public OrganizationControllerTests()
    {
        _mockContext = new Mock<vigendbContext>();
        _mockConfig = new Mock<IConfiguration>();

        _organizationList = new List<Organization>
        {
            new Organization { Nit = "12345", Name = "Test Organization", Password = "securePass", Phone = "555-1234" }
        };

        var mockSet = new Mock<DbSet<Organization>>();
        var asyncOrganizationList = new AsyncEnumerableMock<Organization>(_organizationList);

        mockSet.As<IQueryable<Organization>>().Setup(m => m.Provider).Returns(asyncOrganizationList.Provider);
        mockSet.As<IQueryable<Organization>>().Setup(m => m.Expression).Returns(asyncOrganizationList.Expression);
        mockSet.As<IQueryable<Organization>>().Setup(m => m.ElementType).Returns(asyncOrganizationList.ElementType);
        mockSet.As<IQueryable<Organization>>().Setup(m => m.GetEnumerator()).Returns(() => _organizationList.GetEnumerator());
        mockSet.As<IAsyncEnumerable<Organization>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
               .Returns(new AsyncEnumeratorMock<Organization>(_organizationList.GetEnumerator()));

        _mockContext.Setup(c => c.Organizations).Returns(mockSet.Object);

        // Simula el cambio de estado en Entity Framework
        _mockContext.Setup(c => c.Entry(It.IsAny<Organization>())).Returns((Organization org) =>
        {
            var mockEntry = new Mock<EntityEntry<Organization>>();
            mockEntry.Setup(e => e.State).Returns(EntityState.Modified);
            return mockEntry.Object;
        });

        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _controller = new OrganizationController(_mockContext.Object, _mockConfig.Object);
    }

    [Fact]
    public async Task GetOrganizations_ReturnsOrganizations_WhenOrganizationsExist()
    {
        var result = await _controller.getOrganizations();

        var actionResult = Assert.IsType<ActionResult<List<Organization>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedOrganizations = Assert.IsType<List<Organization>>(okResult.Value);

        Assert.Single(returnedOrganizations);
    }

    [Fact]
    public async Task GetOrganization_ReturnsNotFound_WhenOrganizationDoesNotExist()
    {
        _mockContext.Setup(c => c.Organizations.FindAsync("99999"))
                    .ReturnsAsync((Organization?)null); // Evita advertencia de posible nulo

        var result = await _controller.getOrganization("99999");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetOrganization_ReturnsOrganization_WhenExists()
    {
        _mockContext.Setup(c => c.Organizations.FindAsync("12345"))
                    .ReturnsAsync(_organizationList[0]!); // Evita advertencia de posible nulo

        var result = await _controller.getOrganization("12345");

        var actionResult = Assert.IsType<ActionResult<Organization>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedOrganization = Assert.IsType<Organization>(okResult.Value);

        Assert.Equal("12345", returnedOrganization.Nit);
    }

    [Fact]
    public async Task PostOrganization_CreatesOrganizationSuccessfully()
    {
        var newOrganization = new Organization { Nit = "67890", Name = "New Org", Password = "securePass", Phone = "555-5678" };

        _mockContext.Setup(c => c.Organizations.AddAsync(It.IsAny<Organization>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Organization org, CancellationToken token) => null);

        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _controller.postOrganization(newOrganization);

        var actionResult = Assert.IsType<ActionResult<Organization>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
    }

    [Fact]
    public async Task DeleteOrganization_RemovesOrganizationSuccessfully()
    {
        _mockContext.Setup(c => c.Organizations.FindAsync("12345"))
                    .ReturnsAsync(_organizationList[0]);

        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _controller.DeleteOrganization("12345");

        var actionResult = Assert.IsType<ActionResult<Organization>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
    }
}

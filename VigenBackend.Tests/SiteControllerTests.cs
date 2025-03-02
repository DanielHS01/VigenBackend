using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vigen_Repository.Models;
using Vigen_Repository.Controllers;
using VigenBackend.Tests.Mocks; // Importa las clases de mock

public class SiteControllerTests
{
    private readonly Mock<vigendbContext> _mockContext;
    private readonly SiteController _controller;
    private readonly List<Site> _siteList;

    public SiteControllerTests()
    {
        _mockContext = new Mock<vigendbContext>();

        _siteList = new List<Site>
    {
        new Site { Id = "1", Nit = "12345", Ubication = "Main Street", Range = 5, CountryCode = "CO", Phone = "5551234", Tel = "5555678" },
        new Site { Id = "2", Nit = "12345", Ubication = "Second Street", Range = 10, CountryCode = "CO", Phone = "5559876", Tel = "5554321" }
    };

        var mockSet = new Mock<DbSet<Site>>();
        var asyncSiteList = new AsyncEnumerableMock<Site>(_siteList);

        mockSet.As<IQueryable<Site>>().Setup(m => m.Provider).Returns(asyncSiteList.Provider);
        mockSet.As<IQueryable<Site>>().Setup(m => m.Expression).Returns(asyncSiteList.Expression);
        mockSet.As<IQueryable<Site>>().Setup(m => m.ElementType).Returns(asyncSiteList.ElementType);
        mockSet.As<IQueryable<Site>>().Setup(m => m.GetEnumerator()).Returns(() => _siteList.GetEnumerator());
        mockSet.As<IAsyncEnumerable<Site>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
               .Returns(new AsyncEnumeratorMock<Site>(_siteList.GetEnumerator()));

        _mockContext.Setup(c => c.Sites).Returns(mockSet.Object);

        // 🔴 **Aquí agregamos la configuración para modificar el estado de la entidad**
        _mockContext.Setup(c => c.Entry(It.IsAny<Site>())).Returns((Site site) =>
        {
            var mockEntry = new Mock<EntityEntry<Site>>();
            mockEntry.Setup(e => e.State).Returns(EntityState.Modified);
            return mockEntry.Object;
        });

        // Simulación del guardado en la base de datos
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _controller = new SiteController(_mockContext.Object);
    }


    [Fact]
    public async Task GetSites_ReturnsSites_WhenSitesExist()
    {
        var result = await _controller.getSites("12345");

        var actionResult = Assert.IsType<ActionResult<List<Site>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedSites = Assert.IsType<List<Site>>(okResult.Value);

        Assert.Equal(2, returnedSites.Count);
    }

    [Fact]
    public async Task GetSite_ReturnsSite_WhenExists()
    {
        _mockContext.Setup(c => c.Sites.FindAsync("1", "12345"))
                    .ReturnsAsync(_siteList[0]);

        var result = await _controller.getSite("12345", "1");

        var actionResult = Assert.IsType<ActionResult<Site>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedSite = Assert.IsType<Site>(okResult.Value);

        Assert.Equal("Main Street", returnedSite.Ubication);
    }

    [Fact]
    public async Task GetSite_ReturnsNotFound_WhenDoesNotExist()
    {
        _mockContext.Setup(c => c.Sites.FindAsync("999", "12345"))
                    .ReturnsAsync((Site?)null);

        var result = await _controller.getSite("12345", "999");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostSite_CreatesSiteSuccessfully()
    {
        var newSite = new Site { Id = "3", Nit = "12345", Ubication = "New Location", Range = 15, CountryCode = "CO", Phone = "5551111", Tel = "5552222" };

        _mockContext.Setup(c => c.Sites.AddAsync(It.IsAny<Site>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Site site, CancellationToken token) => null);

        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _controller.postSite(newSite);

        var actionResult = Assert.IsType<ActionResult<Site>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
    }

    [Fact]
    public async Task DeleteSite_RemovesSiteSuccessfully()
    {
        _mockContext.Setup(c => c.Sites.FindAsync("1", "12345"))
                    .ReturnsAsync(_siteList[0]);

        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _controller.DeleteSite("12345", "1");

        var actionResult = Assert.IsType<ActionResult<Site>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
    }
}

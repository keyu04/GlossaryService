using GlossaryService.Common.DTOs;
using GlossaryService.DTOs;
using GlossaryService.Models;
using GlossaryService.Repository.Interfaces;
using GlossaryService.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace GlossaryService.Tests.Services;

public class TagServiceTests
{
    private readonly Mock<ITagRepository> _repoMock;
    private readonly Mock<ILogger<TagService>> _loggerMock;
    private readonly TagService _service;

    public TagServiceTests()
    {
        _repoMock = new Mock<ITagRepository>();
        _loggerMock = new Mock<ILogger<TagService>>();
        _service = new TagService(_repoMock.Object, _loggerMock.Object);
    }

    // ────────────────────────────────────────────────────────────
    // GetAllAsync
    // ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedResult()
    {
        // Arrange
        var fakeTags = new List<GlossaryTag>
        {
            new() { Id = Guid.NewGuid(), Name = "Fresh",   Slug = "fresh"   },
            new() { Id = Guid.NewGuid(), Name = "Organic", Slug = "organic" }
        };

        _repoMock.Setup(r => r.GetAllAsync(null, 1, 10))
            .ReturnsAsync(new PagedResultDto<GlossaryTag>
            {
                Items = fakeTags,
                TotalCount = 2,
                Page = 1,
                PageSize = 10
            });

        // Act
        var result = await _service.GetAllAsync(null, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal("Fresh", result.Items[0].Name);
    }

    // ────────────────────────────────────────────────────────────
    // GetByIdAsync
    // ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTag_WhenExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var fakeTag = new GlossaryTag { Id = id, Name = "Fresh", Slug = "fresh" };

        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(fakeTag);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Fresh", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((GlossaryTag?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.Null(result);
    }

    // ────────────────────────────────────────────────────────────
    // CreateAsync
    // ────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ShouldCreateTag_WhenSlugIsUnique()
    {
        // Arrange
        var dto = new CreateTagDto
        {
            Name = "Fresh",
            Slug = "fresh",
            Color = "#22c55e"
        };

        _repoMock.Setup(r => r.ExistsBySlugAsync("fresh"))
            .ReturnsAsync(false);

        _repoMock.Setup(r => r.CreateAsync(It.IsAny<GlossaryTag>()))
            .ReturnsAsync((GlossaryTag tag) => tag);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Fresh", result.Name);
        Assert.Equal("fresh", result.Slug);
        Assert.Equal("#22c55e", result.Color);

        _repoMock.Verify(r => r.CreateAsync(It.IsAny<GlossaryTag>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenSlugAlreadyExists()
    {
        // Arrange
        var dto = new CreateTagDto { Name = "Fresh", Slug = "fresh" };

        _repoMock.Setup(r => r.ExistsBySlugAsync("fresh"))
            .ReturnsAsync(true);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAsync(dto));

        Assert.Equal("Slug 'fresh' already exists.", ex.Message);

        _repoMock.Verify(r => r.CreateAsync(It.IsAny<GlossaryTag>()), Times.Never);
    }

    // ────────────────────────────────────────────────────────────
    // UpdateAsync
    // ────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTag_WhenExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var fakeTag = new GlossaryTag { Id = id, Name = "Fresh", Slug = "fresh" };
        var dto = new UpdateTagDto { Name = "Super Fresh", IsActive = true };

        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(fakeTag);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<GlossaryTag>()))
            .ReturnsAsync((GlossaryTag tag) => tag);

        // Act
        var result = await _service.UpdateAsync(id, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Super Fresh", result.Name);

        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<GlossaryTag>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateTagDto { Name = "Fresh", IsActive = true };

        _repoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((GlossaryTag?)null);

        // Act
        var result = await _service.UpdateAsync(id, dto);

        // Assert
        Assert.Null(result);

        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<GlossaryTag>()), Times.Never);
    }

    // ────────────────────────────────────────────────────────────
    // DeleteAsync
    // ────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenTagExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.DeleteAsync(id)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenTagNotExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.DeleteAsync(id)).ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        Assert.False(result);
    }
}
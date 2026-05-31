using GlossaryService.DTOs;
using GlossaryService.Models;
using GlossaryService.Repository.Interfaces;
using GlossaryService.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace GlossaryService.Tests.Services;

public class KeywordServiceTests
{
    private readonly Mock<IKeywordRepository>      _repoMock;
    private readonly Mock<ITagRepository>          _tagRepoMock;
    private readonly Mock<ILogger<KeywordService>> _loggerMock;
    private readonly KeywordService                _service;

    public KeywordServiceTests()
    {
        _repoMock    = new Mock<IKeywordRepository>();
        _tagRepoMock = new Mock<ITagRepository>();
        _loggerMock  = new Mock<ILogger<KeywordService>>();
        _service     = new KeywordService(
            _repoMock.Object,
            _tagRepoMock.Object,
            _loggerMock.Object);
    }

    // ────────────────────────────────────────────────────────────
    // GetByIdAsync
    // ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ShouldReturnKeyword_WhenExists()
    {
        // Arrange
        var id      = Guid.NewGuid();
        var tagId   = Guid.NewGuid();
        var keyword = new GlossaryKeyword
        {
            Id       = id,
            Term     = "tomato",
            Synonyms = "tamatar, tomate",
            TagId    = tagId,
            Tag      = new GlossaryTag { Id = tagId, Name = "Fresh", Slug = "fresh" }
        };

        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(keyword);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("tomato", result.Term);
        Assert.Equal("Fresh",  result.TagName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((GlossaryKeyword?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.Null(result);
    }

    // ────────────────────────────────────────────────────────────
    // CreateAsync
    // ────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ShouldCreateKeyword_WhenTagExists()
    {
        // Arrange
        var tagId  = Guid.NewGuid();
        var dto    = new CreateKeywordDto
        {
            Term     = "tomato",
            Synonyms = "tamatar, tomate",
            TagId    = tagId
        };

        var fakeTag = new GlossaryTag { Id = tagId, Name = "Fresh", Slug = "fresh" };
        var created = new GlossaryKeyword
        {
            Id       = Guid.NewGuid(),
            Term     = dto.Term,
            Synonyms = dto.Synonyms,
            TagId    = tagId,
            Tag      = fakeTag
        };

        _tagRepoMock.Setup(r => r.GetByIdAsync(tagId)).ReturnsAsync(fakeTag);
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<GlossaryKeyword>())).ReturnsAsync(created);
        _repoMock.Setup(r => r.GetByIdAsync(created.Id)).ReturnsAsync(created);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("tomato", result.Term);
        Assert.Equal("Fresh",  result.TagName);

        _repoMock.Verify(r => r.CreateAsync(It.IsAny<GlossaryKeyword>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenTagNotFound()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var dto   = new CreateKeywordDto
        {
            Term     = "tomato",
            Synonyms = "tamatar",
            TagId    = tagId
        };

        _tagRepoMock.Setup(r => r.GetByIdAsync(tagId))
            .ReturnsAsync((GlossaryTag?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.CreateAsync(dto));

        Assert.Equal($"Tag with id '{tagId}' not found.", ex.Message);

        _repoMock.Verify(r => r.CreateAsync(It.IsAny<GlossaryKeyword>()), Times.Never);
    }

    // ────────────────────────────────────────────────────────────
    // DeleteAsync
    // ────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenExists()
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
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
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
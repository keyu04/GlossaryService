using Asp.Versioning;
using GlossaryService.Common.Constants;
using GlossaryService.Common.DTOs;
using GlossaryService.Common.Helpers;
using GlossaryService.DTOs;
using GlossaryService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GlossaryService.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/keywords")]
public class KeywordsController : ControllerBase
{
    private readonly IKeywordService _service;

    public KeywordsController(IKeywordService service) => _service = service;

    // ── GET /api/v1/keywords?search=tomato&tagId=xxx&page=1&pageSize=10 ──
    [HttpGet]
    [EnableRateLimiting("general")]  
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] Guid? tagId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAllAsync(search, tagId, page, pageSize);
        return Ok(ResponseHelper.Success<PagedResultDto<KeywordResponseDto>>(result, LogConst.GLOSSARY_SERVICE + LogConst.GET_KEYWORDS));
    }

    // ── GET /api/v1/keywords/{id} ──
    [HttpGet("{id:guid}")]
    [EnableRateLimiting("general")]  
    public async Task<IActionResult> GetById(Guid id)
    {
        var keyword = await _service.GetByIdAsync(id);
        return keyword is null ? NotFound() : Ok(ResponseHelper.Success(keyword, LogConst.GLOSSARY_SERVICE + LogConst.GET_KEYWORDS));
    }

    // ── POST /api/v1/keywords ──
    [HttpPost]
    [EnableRateLimiting("strict")]
    public async Task<IActionResult> Create([FromBody] CreateKeywordDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ResponseHelper.Success(created, LogConst.GLOSSARY_SERVICE + LogConst.CREATE_KEYWORDS));
    }

    // ── PUT /api/v1/keywords/{id} ──
    [HttpPut("{id:guid}")]
    [EnableRateLimiting("strict")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateKeywordDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(ResponseHelper.Success(updated, LogConst.GLOSSARY_SERVICE + LogConst.UPDATE_KEYWORDS));
    }

    // ── DELETE /api/v1/keywords/{id} ──
    [HttpDelete("{id:guid}")]
    [EnableRateLimiting("strict")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? Ok(ResponseHelper.Success(LogConst.Success, LogConst.GLOSSARY_SERVICE + LogConst.DELETE_KEYWORDS)) : NotFound();
    }
}
using GlossaryService.Common.Constants;
using GlossaryService.Common.DTOs;
using GlossaryService.Common.Helpers;
using GlossaryService.DTOs;
using GlossaryService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GlossaryService.Controllers;

[ApiController]
[Route("api/v1/tags")]
public class TagsController : ControllerBase
{
    private readonly ITagService _service;

    public TagsController(ITagService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAllAsync(search, page, pageSize);
        return Ok(ResponseHelper.Success<PagedResultDto<TagResponseDto>>(result, LogConst.GLOSSARY_SERVICE + LogConst.GET_TAGS));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var tag = await _service.GetByIdAsync(id);
        return tag is null ? NotFound() : Ok(ResponseHelper.Success(tag, LogConst.GLOSSARY_SERVICE + LogConst.GET_TAGS));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTagDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ResponseHelper.Success(created, LogConst.GLOSSARY_SERVICE + LogConst.CREATE_TAGS));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTagDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(ResponseHelper.Success(updated, LogConst.GLOSSARY_SERVICE + LogConst.UPDATE_TAGS));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? Ok(ResponseHelper.Success(LogConst.Success, LogConst.GLOSSARY_SERVICE + LogConst.DELETE_TAGS)) : NotFound();
    }
}
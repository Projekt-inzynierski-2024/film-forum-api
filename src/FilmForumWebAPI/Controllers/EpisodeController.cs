using FilmForumModels.Dtos.EpisodeDtos;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FilmForumWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EpisodeController : ControllerBase
{
    private readonly IEpisodeService _episodeService;

    public EpisodeController(IEpisodeService episodeService)
    {
        _episodeService = episodeService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEpisodeDto createEpisodeDto)
    {
        await _episodeService.CreateAsync(createEpisodeDto);
        return Created(nameof(GetById), createEpisodeDto);
    }

    [Authorize]
    [HttpGet("search/{query}")]
    public async Task<IActionResult> SearchAll(string query) => Ok(await _episodeService.SearchAllAsync(query));

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _episodeService.GetAllAsync());

    [Authorize]
    [HttpGet("details")]
    public async Task<IActionResult> GetDetailedAll() => Ok(await _episodeService.GetDetailedAllAsync());

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
        => await _episodeService.GetAsync(id) is GetEpisodeDto episode ? Ok(episode) : NotFound($"Episode not found");

    [Authorize]
    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetDetailedById(string id)
        => await _episodeService.GetDetailedAsync(id) is GetDetailedEpisodeDto episode ? Ok(episode) : NotFound($"Episode not found");

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateEpisodeDto createEpisodeDto)
    {
        ReplaceOneResult result = await _episodeService.UpdateAsync(id, createEpisodeDto);
        return result.IsModifiedCountAvailable && result.ModifiedCount > 0 ? NoContent() : NotFound($"Episode not found");
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(string id)
    {
        await _episodeService.RemoveAsync(id);
        return NoContent();
    }
}
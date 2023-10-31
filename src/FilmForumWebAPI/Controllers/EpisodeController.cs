using FilmForumModels.Dtos.EpisodeDtos;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEpisodeDto createEpisodeDto)
    {
        await _episodeService.CreateAsync(createEpisodeDto);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _episodeService.GetAllAsync());

    [HttpGet("details")]
    public async Task<IActionResult> GetDetailedAll() => Ok(await _episodeService.GetDetailedAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
        => await _episodeService.GetAsync(id) is GetEpisodeDto episode ? Ok(episode) : NotFound($"Episode not found");

    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetDetailedById(string id)
        => await _episodeService.GetDetailedAsync(id) is GetDetailedEpisodeDto episode ? Ok(episode) : NotFound($"Episode not found");

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateEpisodeDto createEpisodeDto)
    {
        await _episodeService.UpdateAsync(id, createEpisodeDto);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(string id)
    {
        await _episodeService.RemoveAsync(id);
        return Ok();
    }
}

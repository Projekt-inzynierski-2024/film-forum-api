using FilmForumModels.Dtos.ActorDtos;
using FilmForumModels.Dtos.DirectorDtos;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FilmForumWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DirectorController : ControllerBase
{
    private readonly IDirectorService _directorService;

    public DirectorController(IDirectorService directorService)
    {
        _directorService = directorService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDirectorDto createDirectorDto)
    {
        await _directorService.CreateAsync(createDirectorDto);
        return Created(nameof(GetById), createDirectorDto);
    }

    [HttpGet("search/{query}")]
    public async Task<IActionResult> SearchAll(string query) => Ok(await _directorService.SearchAllAsync(query));

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _directorService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
        => await _directorService.GetAsync(id) is GetDirectorDto directorDto ? Ok(directorDto) : NotFound($"Director not found");

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateDirectorDto createDirectorDto)
    {
        ReplaceOneResult result = await _directorService.UpdateAsync(id, createDirectorDto);
        return result.IsModifiedCountAvailable && result.ModifiedCount > 0 ? NoContent() : NotFound($"Director not found");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(string id)
    {
        await _directorService.RemoveAsync(id);
        return NoContent();
    }
}
using FilmForumModels.Dtos.DirectorDtos;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FilmForumWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DirectorController : ControllerBase
{
    private readonly IDirectorService _directorService;

    public DirectorController(IDirectorService directorService) => _directorService = directorService;

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDirectorDto createDirectorDto)
    {
        await _directorService.CreateAsync(createDirectorDto);
        return Created(nameof(GetById), createDirectorDto);
    }

    [Authorize]
    [HttpGet("search/{query}")]
    public async Task<IActionResult> SearchAll(string query) => Ok(await _directorService.SearchAllAsync(query));

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _directorService.GetAllAsync());

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
        => await _directorService.GetAsync(id) is GetDirectorDto directorDto ? Ok(directorDto) : NotFound($"Director not found");

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateDirectorDto createDirectorDto)
    {
        ReplaceOneResult result = await _directorService.UpdateAsync(id, createDirectorDto);
        return result.IsModifiedCountAvailable && result.ModifiedCount > 0 ? NoContent() : NotFound($"Director not found");
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(string id)
    {
        await _directorService.RemoveAsync(id);
        return NoContent();
    }
}
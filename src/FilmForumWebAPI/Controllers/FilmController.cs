using FilmForumModels.Dtos.FilmDtos;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FilmForumWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FilmController : ControllerBase
{
    private readonly IFilmService _filmService;

    public FilmController(IFilmService filmService)
    {
        _filmService = filmService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFilmDto createFilmDto)
    {
        await _filmService.CreateAsync(createFilmDto);
        return Created(nameof(GetById), createFilmDto);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _filmService.GetAllAsync());

    [Authorize]
    [HttpGet("search/{query}")]
    public async Task<IActionResult> SearchAll(string query) => Ok(await _filmService.SearchAllAsync(query));

    [Authorize]
    [HttpGet("details")]
    public async Task<IActionResult> GetDetailedAll() => Ok(await _filmService.GetDetailedAllAsync());

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
        => await _filmService.GetAsync(id) is GetFilmDto film ? Ok(film) : NotFound($"Film not found");

    [Authorize]
    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetDetailedById(string id)
        => await _filmService.GetDetailedAsync(id) is GetDetailedFilmDto film ? Ok(film) : NotFound($"Film not found");

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateFilmDto updatedFilm)
    {
        ReplaceOneResult result = await _filmService.UpdateAsync(id, updatedFilm);
        return result.IsModifiedCountAvailable && result.ModifiedCount > 0 ? NoContent() : NotFound($"Film not found");
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(string id)
    {
        await _filmService.RemoveAsync(id);
        return NoContent();
    }
}
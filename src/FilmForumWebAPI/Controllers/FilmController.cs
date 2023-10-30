using FilmForumModels.Dtos.FilmDtos;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFilmDto createFilmDto)
    {
        await _filmService.CreateAsync(createFilmDto);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _filmService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
        => await _filmService.GetAsync(id) is GetFilmDto film ? Ok(film) : NotFound($"Film not found");

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateFilmDto updatedFilm)
    {
        await _filmService.UpdateAsync(id, updatedFilm);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(string id)
    {
        await _filmService.RemoveAsync(id);
        return Ok();
    }
}
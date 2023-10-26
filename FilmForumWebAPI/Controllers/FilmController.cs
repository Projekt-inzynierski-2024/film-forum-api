using FilmForumWebAPI.Models.Dtos.Film;
using FilmForumWebAPI.Models.Entities;
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
    public async Task<List<Film>> GetAll() => await _filmService.GetAllAsync();
}
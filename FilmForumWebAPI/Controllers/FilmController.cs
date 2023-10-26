using FilmForumWebAPI.Models.Dtos.Film;
using FilmForumWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using FilmForumWebAPI.Models.Entities;

namespace FilmForumWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilmController : ControllerBase
{
    private readonly FilmService filmService;

    public FilmController(FilmService filmService)
    {
        this.filmService = filmService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFilmDto createFilmDto)
    {
        this.filmService.CreateAsync(Film.of(createFilmDto));
        return Ok();
    }

    [HttpGet]
    public async Task<List<Film>> Get(){
        return await filmService.GetAsync();
    }

}

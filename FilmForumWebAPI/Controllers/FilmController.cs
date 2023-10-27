using FilmForumWebAPI.Extensions;
using FilmForumWebAPI.Models.Dtos.FilmDtos;
using FilmForumWebAPI.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace FilmForumWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FilmController : ControllerBase
{
    private readonly IFilmService _filmService;
    private readonly IValidator<CreateFilmDto> _createFilmValidator;

    public FilmController(IFilmService filmService, IValidator<CreateFilmDto> createFilmValidator)
    {
        _filmService = filmService;
        _createFilmValidator = createFilmValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFilmDto createFilmDto)
    {
        ValidationResult validation = _createFilmValidator.Validate(createFilmDto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.GetMessagesAsString());
        }

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
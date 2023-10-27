using FilmForumWebAPI.Models.Dtos.DirectorDtos;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _directorService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
        => await _directorService.GetAsync(id) is GetDirectorDto directorDto ? Ok(directorDto) : NotFound($"Director not found");

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateDirectorDto createDirectorDto)
    {
        await _directorService.UpdateAsync(id, createDirectorDto);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(string id)
    {
        await _directorService.RemoveAsync(id);
        return Ok();
    }
}
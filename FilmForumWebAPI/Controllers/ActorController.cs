using FilmForumWebAPI.Models.Dtos.ActorDtos;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FilmForumWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActorController : ControllerBase
{
    private readonly IActorService _actorService;

    public ActorController(IActorService actorService)
    {
        _actorService = actorService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateActorDto createActorDto)
    {
        await _actorService.CreateAsync(createActorDto);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _actorService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
        => await _actorService.GetAsync(id) is GetActorDto actor ? Ok(actor) : NotFound($"Film not found");

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateActorDto createActorDto)
    {
        await _actorService.UpdateAsync(id, createActorDto);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(string id)
    {
        await _actorService.RemoveAsync(id);
        return Ok();
    }
}
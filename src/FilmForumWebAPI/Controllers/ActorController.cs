using FilmForumModels.Dtos.ActorDtos;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

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
        return Created(nameof(GetById), createActorDto);
    }

    [HttpGet("search/{query}")]
    public async Task<IActionResult> SearchAll(string query) => Ok(await _actorService.SearchAllAsync(query));

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _actorService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
        => await _actorService.GetAsync(id) is GetActorDto actor ? Ok(actor) : NotFound($"Actor not found");

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateActorDto createActorDto)
    {
        ReplaceOneResult result = await _actorService.UpdateAsync(id, createActorDto);
        return result.IsModifiedCountAvailable && result.ModifiedCount > 0 ? NoContent() : NotFound($"Actor not found");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(string id)
    {
        await _actorService.RemoveAsync(id);
        return NoContent();
    }
}
using FilmForumModels.Dtos.ReviewDtos;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FilmForumWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewDto createReviewDto)
    {
        await _reviewService.CreateAsync(createReviewDto);
        return Created(nameof(GetById), createReviewDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _reviewService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
        => await _reviewService.GetAsync(id) is GetReviewDto review ? Ok(review) : NotFound($"Review not found");

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateReviewDto createReviewDto)
    {
        ReplaceOneResult result = await _reviewService.UpdateAsync(id, createReviewDto);
        return result.IsModifiedCountAvailable && result.ModifiedCount > 0 ? NoContent() : NotFound($"Review not found");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(string id)
    {
        await _reviewService.RemoveAsync(id);
        return NoContent();
    }
}
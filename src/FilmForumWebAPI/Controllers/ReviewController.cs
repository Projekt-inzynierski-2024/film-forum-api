using FilmForumModels.Dtos.ReviewDtos;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _reviewService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
        => await _reviewService.GetAsync(id) is GetReviewDto review ? Ok(review) : NotFound($"Episode not found");

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateReviewDto createReviewDto)
    {
        await _reviewService.UpdateAsync(id, createReviewDto);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(string id)
    {
        await _reviewService.RemoveAsync(id);
        return Ok();
    }
}
using FilmForumModels.Dtos.ReviewDtos;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;

namespace FilmForumWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService) => _reviewService = reviewService;

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewDto createReviewDto)
    {
        string userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value!;
        await _reviewService.CreateAsync(userId, createReviewDto);
        return Created(nameof(GetById), createReviewDto);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _reviewService.GetAllAsync());

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
        => await _reviewService.GetAsync(id) is GetReviewDto review ? Ok(review) : NotFound($"Review not found");

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateReviewDto createReviewDto)
    {
        string userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value!;
        ReplaceOneResult result = await _reviewService.UpdateAsync(id, userId, createReviewDto);
        return result.IsModifiedCountAvailable && result.ModifiedCount > 0 ? NoContent() : NotFound($"Review not found");
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(string id)
    {
        await _reviewService.RemoveAsync(id);
        return NoContent();
    }
}
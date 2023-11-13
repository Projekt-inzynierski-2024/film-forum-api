using FilmForumModels.Dtos.ReviewDtos;
using FilmForumWebAPI.Controllers;
using FilmForumWebAPI.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using System.Security.Claims;

namespace FilmForumWebAPI.UnitTests.Controllers;

public class ReviewControllerTests
{
    private readonly ReviewController _reviewController;

    private readonly Mock<IReviewService> _mockReviewService;

    public ReviewControllerTests()
    {
        _mockReviewService = new();
        _reviewController = new(_mockReviewService.Object);
    }

    [Fact]
    public async Task Create_ForValidData_AddsReview()
    {
        // Arrange
        _reviewController.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "1") }))
        };
        CreateReviewDto createReviewDto = new()
        {
            EpisodeId = "2",
            Rate = 5,
            Comment = "Great show"
        };
        _mockReviewService.Setup(x => x.CreateAsync(It.IsAny<string>(), createReviewDto)).Returns(Task.CompletedTask);

        // Act
        IActionResult result = await _reviewController.Create(createReviewDto);

        // Assert
        CreatedResult createdResult = result.Should().BeOfType<CreatedResult>().Subject;
        createdResult.Value.Should().Be(createReviewDto);
    }

    [Fact]
    public async Task GetAll_ForValidData_ReturnsAllReviews()
    {
        // Arrange
        List<GetReviewDto> reviews = new()
        {
            new(new(){ Id = "1", UserId = "1", EpisodeId = "1", Rate = 5, Comment = "Great show"}),
            new(new(){ Id = "2", UserId = "6", EpisodeId = "11", Rate = 1, Comment = "Great episode"}),
            new(new(){ Id = "3", UserId = "6", EpisodeId = "12", Rate = 6, Comment = "Good humour"}),
        };
        _mockReviewService.Setup(x => x.GetAllAsync()).ReturnsAsync(reviews);

        // Act
        IActionResult result = await _reviewController.GetAll();

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().BeEquivalentTo(reviews);
    }

    [Fact]
    public async Task GetById_ForValidData_ReturnsReview()
    {
        // Arrange
        GetReviewDto review = new(new() { Id = "1", UserId = "1", EpisodeId = "1", Rate = 5, Comment = "Great show" });
        _mockReviewService.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(review);

        // Act
        IActionResult result = await _reviewController.GetById(It.IsAny<string>());

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().Be(review);
    }

    [Fact]
    public async Task GetById_ForNonExistingReview_ReturnsNotFound()
    {
        // Arrange
        _mockReviewService.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(() => null);

        // Act
        IActionResult result = await _reviewController.GetById(It.IsAny<string>());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Update_ForValidData_UpdatesReview()
    {
        // Arrange
        _reviewController.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "1") }))
        };
        CreateReviewDto createReviewDto = new()
        {
            EpisodeId = "2",
            Rate = 5,
            Comment = "Great show"
        };
        _mockReviewService.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<string>(), createReviewDto)).ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, It.IsAny<string>()));

        // Act
        IActionResult result = await _reviewController.Update(It.IsAny<string>(), createReviewDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_ForNonExistingReview_ReturnsNotFound()
    {
        // Arrange
        _reviewController.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "1") }))
        };
        CreateReviewDto createReviewDto = new()
        {
            EpisodeId = "2",
            Rate = 5,
            Comment = "Great show"
        };
        _mockReviewService.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<string>(), createReviewDto)).ReturnsAsync(new ReplaceOneResult.Acknowledged(0, null, It.IsAny<string>()));

        // Act
        IActionResult result = await _reviewController.Update(It.IsAny<string>(), createReviewDto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Remove_DeletesReview()
    {
        // Arrange
        _mockReviewService.Setup(x => x.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        IActionResult result = await _reviewController.Remove(It.IsAny<string>());

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}
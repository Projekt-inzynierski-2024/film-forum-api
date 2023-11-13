using FilmForumModels.Dtos.ReviewDtos;
using FilmForumWebAPI.Controllers;
using FilmForumWebAPI.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public async Task Create_ForValidData_ReturnsCreated()
    {
        // Arrange
        CreateReviewDto createReviewDto = new()
        {
            UserId = "1",
            EpisodeId = "2",
            Rate = 5,
            Comment = "Great show"
        };
        _mockReviewService.Setup(x => x.CreateAsync(createReviewDto)).Returns(Task.CompletedTask);

        // Act
        IActionResult result = await _reviewController.Create(createReviewDto);

        // Assert
        CreatedResult createdResult = result.Should().BeOfType<CreatedResult>().Subject;
        createdResult.Value.Should().BeEquivalentTo(createReviewDto);
    }
}

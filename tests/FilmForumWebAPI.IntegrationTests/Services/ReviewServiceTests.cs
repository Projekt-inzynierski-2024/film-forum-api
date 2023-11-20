using FilmForumModels.Dtos.ReviewDtos;
using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.IntegrationTests.HelpersForTests;
using FilmForumWebAPI.Services;
using FluentAssertions;
using MongoDB.Driver;

namespace FilmForumWebAPI.IntegrationTests.Services;

public class ReviewServiceTests
{
    [Fact]
    public async Task CreateAsync_ForGivenData_CreatesReview()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        ReviewService service = new(filmsDatabaseContext);
        CreateReviewDto createReviewDto = new() { EpisodeId = "653cb4c0810cec27c0943b01", Rate = 10, Comment = "Amazing" };

        // Act
        await service.CreateAsync("1", createReviewDto);

        // Assert
        Review? result = await filmsDatabaseContext.ReviewCollection.Find(_ => true).FirstOrDefaultAsync();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetReviewsAsync_ForExistingReviews_ReturnsAllReviews()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        ReviewService service = new(filmsDatabaseContext);
        await filmsDatabaseContext.ReviewCollection.InsertManyAsync(new List<Review>()
        {
            new("1", new CreateReviewDto() { EpisodeId = "653cb4c0810cec27c0943b01", Rate = 5, Comment = "Great movie" }),
            new("1", new CreateReviewDto() { EpisodeId = "653cb4c0810cec27c0943b0b", Rate = 6, Comment = "Great movie" }),
        });

        // Act
        List<GetReviewDto> result = await service.GetAllAsync();

        // Assert
        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task GetAsync_ForExistingReview_ReturnsReview()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        ReviewService service = new(filmsDatabaseContext);
        Review review = new("1", new CreateReviewDto() { EpisodeId = "653cb4c0810cec27c0943b01", Rate = 5, Comment = "Great movie" });
        await filmsDatabaseContext.ReviewCollection.InsertOneAsync(review);

        // Act
        GetReviewDto? result = await service.GetAsync(review.Id);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAsync_ForNonExistingReview_ReturnsNull()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        ReviewService service = new(filmsDatabaseContext);

        // Act
        GetReviewDto? result = await service.GetAsync("653cb4c0850c3c22c0143b01");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ForExistingReview_UpdatesReview()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        ReviewService service = new(filmsDatabaseContext);
        await filmsDatabaseContext.ReviewCollection.InsertOneAsync(new("1", new CreateReviewDto() { EpisodeId = "653cb4c0810cec27c0943b01", Rate = 5, Comment = "Great movie" }));
        string createdReviewId = (await filmsDatabaseContext.ReviewCollection.Find(_ => true).FirstAsync()).Id;
        CreateReviewDto createReviewDto = new() { EpisodeId = "6d32b440820cef5710743801", Rate = 10, Comment = "Amazing" };

        // Act
        await service.UpdateAsync(createdReviewId, "1", createReviewDto);

        // Assert
        Review? result = await filmsDatabaseContext.ReviewCollection.Find(_ => true).FirstOrDefaultAsync();
        result.Should().NotBeNull();
        result?.EpisodeId.Should().Be(createReviewDto.EpisodeId);
        result?.Rate.Should().Be(createReviewDto.Rate);
        result?.Comment.Should().Be(createReviewDto.Comment);
    }

    [Fact]
    public async Task UpdateAsync_ForNonExistingReview_ReturnsNull()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        ReviewService service = new(filmsDatabaseContext);
        CreateReviewDto createReviewDto = new() { EpisodeId = "6d32b440820cef5710743801", Rate = 10, Comment = "Amazing" };

        // Act
        ReplaceOneResult result = await service.UpdateAsync("111111111111111111111111", "1", createReviewDto);

        // Assert
        result.MatchedCount.Should().Be(0);
    }

    [Fact]
    public async Task RemoveAsync_ForExistingReview_RemovesReview()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        ReviewService service = new(filmsDatabaseContext);
        await filmsDatabaseContext.ReviewCollection.InsertOneAsync(new("1", new CreateReviewDto() { EpisodeId = "653cb4c0810cec27c0943b01", Rate = 5, Comment = "Great movie" }));
        string createdReviewId = (await filmsDatabaseContext.ReviewCollection.Find(_ => true).FirstAsync()).Id;

        // Act
        DeleteResult result = await service.RemoveAsync(createdReviewId);

        // Assert
        result.DeletedCount.Should().Be(1);
    }

    [Fact]
    public async Task RemoveAsync_ForNonExistingReview_DoesNotRemoveReview()
    {
        // Arrange
        FilmsDatabaseContext filmsDatabaseContext = await DatabaseHelper.CreateAndPrepareFilmsDatabaseContextForTestingAsync();
        ReviewService service = new(filmsDatabaseContext);

        // Act
        DeleteResult result = await service.RemoveAsync("111111111111111111111111");

        // Assert
        result.DeletedCount.Should().Be(0);
    }
}
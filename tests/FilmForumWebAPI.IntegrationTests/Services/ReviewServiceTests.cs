using FilmForumModels.Dtos.ReviewDtos;
using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.IntegrationTests.HelpersForTests;
using FilmForumWebAPI.Services;
using FluentAssertions;

namespace FilmForumWebAPI.IntegrationTests.Services;

public class ReviewServiceTests
{
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
}
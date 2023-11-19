using FilmForumModels.Dtos.DirectorDtos;
using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.Services;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmForumWebAPI.IntegrationTests.Services;

public class DirectorServiceTests
{
    public class DirectorServiceTests
    {
        private readonly DirectorService _directorService;
        private readonly Mock<IMongoCollection<Director>> _mockCollection = new Mock<IMongoCollection<Director>>();

        public DirectorServiceTests()
        {
            _directorService = new DirectorService(new FilmsDatabaseContext { DirectorCollection = _mockCollection.Object });
        }

        [Fact]
        public async Task SearchAllAsync_ReturnMatchingDirectors()
        {
            // Arrange
            var query = "Kuba";
            var directors = new List<Director>
            {
                new Director { Name = "Arek", Surname = "Czura", description = "Wspanialy rezyser" },
                new Director { Name = "Kuba", Surname = "Baran", description = "Mniej wspanialy rezyser" }
            };

            // Act
            var result = await _directorService.SearchAllAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count); 
        }

        [Fact]
        public async Task GetAsync_ReturnsSpecyfiDirectorDto()
        {
            // Arrange
            var directorId = 1;
            var director = new Director { Id = directorId, Name = "John", Surname = "Doe" };
            _mockCollection.Setup(collection => collection.Find(It.IsAny<FilterDefinition<Director>>(), null, default)).Returns(new FakeFindFluent<Director>(new List<Director> { director }));

            // Act
            var result = await _directorService.GetAsync(directorId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(directorId, result.Id);
        }

        [Fact]
        public async Task CreateAsync_InsertNewDirector()
        {
            // Arrange
            var createDirectorDto = new CreateDirectorDto { Name = "Kuba", Surname = "Polak", description = "Rezyser komediowy" };
            _mockCollection.Setup(collection => collection.InsertOneAsync(It.IsAny<Director>(), null, default)).Returns(Task.CompletedTask);

            // Act
            await _directorService.CreateAsync(createDirectorDto);

            // Assert
            _mockCollection.Verify(collection => collection.InsertOneAsync(It.IsAny<Director>(), null, default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnReplaceOneResult()
        {
            // Arrange
            var directorId = 1;
            var createDirectorDto = new CreateDirectorDto { Name = "Test", Surname = "Testowski", description = "Rezyser testogedonu" };
            var replaceOneResult = new ReplaceOneResult.Acknowledged(1, 1, directorId);
            _mockCollection.Setup(collection => collection.ReplaceOneAsync(It.IsAny<FilterDefinition<Director>>(), It.IsAny<Director>(), null, default)).ReturnsAsync(replaceOneResult);

            // Act
            var result = await _directorService.UpdateAsync(directorId, createDirectorDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(replaceOneResult, result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnNotModified()
        {
            // Arrange
            var DirectorId = 234441;
            var createDirectorDto = new CreateDirectorDto { Name = "Imie", Surname = "Nazwisko", description = "Rezyser"};
            var replaceOneResult = new ReplaceOneResult.Acknowledged(0, 0, DirectorId);
            _mockCollection.Setup(collection => collection.ReplaceOneAsync(It.IsAny<FilterDefinition<Director>>(), It.IsAny<Director>(), null, default)).ReturnsAsync(replaceOneResult);

            // Act
            var result = await _directorService.UpdateAsync(DirectorId, createDirectorDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(replaceOneResult, result);
        }

        [Fact]
        public async Task RemoveAsync_DeleteOnoDirector()
        {
            // Arrange
            var directorId = 1;
            _mockCollection.Setup(collection => collection.DeleteOneAsync(It.IsAny<FilterDefinition<Director>>(), default)).Returns(Task.CompletedTask);

            // Act
            await _directorService.RemoveAsync(directorId);

            // Assert
            _mockCollection.Verify(collection => collection.DeleteOneAsync(It.IsAny<FilterDefinition<Director>>(), default), Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_DeleteNonExistingDircetor()
        {
            // Arrange
            var nonExistingDirectorId = 3211;
            _mockCollection.Setup(collection => collection.DeleteOneAsync(It.IsAny<FilterDefinition<Director>>(), default)).Returns(Task.CompletedTask);

            // Act  // Assert
            await _directorService.RemoveAsync(nonExistingDirectorId);
        }
    }
}
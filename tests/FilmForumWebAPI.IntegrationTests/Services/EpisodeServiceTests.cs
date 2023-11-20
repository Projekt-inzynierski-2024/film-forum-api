//namespace FilmForumWebAPI.IntegrationTests.Services;

//public class EpisodeServiceTests
//{

//    private readonly EpisodeService _episodeService;
//    private readonly Mock<IMongoCollection<Episode>> _mockCollection = new Mock<IMongoCollection<Episode>>();
//    private readonly Mock<IAsyncCursor<Episode>> _mockDetailedEpisodeCursor = new Mock<IAsyncCursor<Episode>>();

//    public EpisodeServiceTests()
//    {
//        var filmsDatabaseContext = new FilmsDatabaseContext { EpisodeCollection = _mockCollection.Object };
//        _episodeService = new EpisodeService(filmsDatabaseContext);
//    }

//    [Fact]
//    public async Task SearchAllAsync_ReturnMatchingEpisodes()
//    {
//        // Arrange
//        var query = "Title";
//        var episodes = new List<Episode>
//            {
//                new Episode { Title = "Title" },
//                new Episode { Title = "Moda na sukces" }
//            };
//        _mockCollection.Setup(collection => collection.FindAsync(It.IsAny<FilterDefinition<Episode>>(), null, default)).ReturnsAsync(new FakeAsyncCursor<Episode>(episodes));

//        // Act
//        var result = await _episodeService.SearchAllAsync(query);

//        // Assert
//        Assert.NotNull(result);
//        Assert.NotEmpty(result);
//        Assert.Equal(1, result.Count); 
//    }

//    [Fact]
//    public async Task GetAllAsync_ReturnAllEpisodes()
//    {
//        // Arrange
//        var episodes = new List<Episode>
//            {
//                new Episode { Title = "Episode 1" },
//                new Episode { Title = "Episode 2" }
//            };
//        _mockCollection.Setup(collection => collection.FindAsync(It.IsAny<FilterDefinition<Episode>>(), null, default)).ReturnsAsync(new FakeAsyncCursor<Episode>(episodes));

//        // Act
//        var result = await _episodeService.GetAllAsync();

//        // Assert
//        Assert.NotNull(result);
//        Assert.NotEmpty(result);
//        Assert.Equal(2, result.Count); /
//    }

//    [Fact]
//    public async Task GetAsync_ReturnsEpisodeDto()
//    {
//        // Arrange
//        var episodeId = 1;
//        var episodeTitle = "Test";
//        var episode = new Episode { Id = episodeId, Title = episodeTitle };
//        _mockCollection.Setup(collection => collection.Find(It.IsAny<FilterDefinition<Episode>>(), null, default)).Returns(new FakeFindFluent<Episode>(new List<Episode> { episode }));

//        // Act
//        var result = await _episodeService.GetAsync(episodeId);

//        // Assert
//        Assert.NotNull(result);
//        Assert.Equal(episodeId, result.Id);
//        Assert.Equal(episodeTitle, result.Title);
//    }

//    [Fact]
//    public async Task GetAsync_ReturnsNull()
//    {
//        // Arrange
//        var nonExistingEpisodeId = 99999;
//        _mockCollection.Setup(collection => collection.Find(It.IsAny<FilterDefinition<Episode>>(), null, default)).Returns(new FakeFindFluent<Episode>(Enumerable.Empty<Episode>()));

//        // Act
//        var result = await _episodeService.GetAsync(nonExistingEpisodeId);

//        // Assert
//        Assert.Null(result);
//    }

//    [Fact]
//    public async Task UpdateAsync_ReturnReplaceOneResult()
//    {
//        // Arrange
//        var episodeId = 1;
//        var createEpisodeDto = new CreateEpisodeDto { Title = "New Title" };
//        var replaceOneResult = new ReplaceOneResult.Acknowledged(1, 1, episodeId);
//        _mockCollection.Setup(collection => collection.ReplaceOneAsync(It.IsAny<FilterDefinition<Episode>>(), It.IsAny<Episode>(), null, default)).ReturnsAsync(replaceOneResult);

//        // Act
//        var result = await _episodeService.UpdateAsync(episodeId, createEpisodeDto);

//        // Assert
//        Assert.NotNull(result);
//        Assert.Equal(replaceOneResult, result);
//    }

//    [Fact]
//    public async Task RemoveAsync_DeleteOne()
//    {
//        // Arrange
//        var episodeId = 1;
//        _mockCollection.Setup(collection => collection.DeleteOneAsync(It.IsAny<FilterDefinition<Episode>>(), default)).Returns(Task.CompletedTask);

//        // Act
//        await _episodeService.RemoveAsync(episodeId);

//        // Assert
//        _mockCollection.Verify(collection => collection.DeleteOneAsync(It.IsAny<FilterDefinition<Episode>>(), default), Times.Once);
//    }
//    [Fact]
//    public async Task CreateAsync_InsertNewEpisode()
//    {
//        // Arrange
//        var createEpisodeDto = new CreateEpisodeDto { Title = "New Episode" };
//        _mockCollection.Setup(collection => collection.InsertOneAsync(It.IsAny<Episode>(), default)).Returns(Task.CompletedTask);

//        // Act
//        await _episodeService.CreateAsync(createEpisodeDto);

//        // Assert
//        _mockCollection.Verify(collection => collection.InsertOneAsync(It.IsAny<Episode>(), default), Times.Once);
//    }

//    [Fact]
//    public async Task GetDetailedAllAsync_DetailedEpisodes()
//    {
//        // Arrange
//        var episodes = new List<Episode>
//    {
//        new Episode { Title = "Episode 1", FilmId = ObjectId.GenerateNewId(), DirectorIds = new List<ObjectId> { ObjectId.GenerateNewId() }, ActorIds = new List<ObjectId> { ObjectId.GenerateNewId() }, Id = ObjectId.GenerateNewId().ToString() },
//        new Episode { Title = "Episode 2", FilmId = ObjectId.GenerateNewId(), DirectorIds = new List<ObjectId> { ObjectId.GenerateNewId() }, ActorIds = new List<ObjectId> { ObjectId.GenerateNewId() }, Id = ObjectId.GenerateNewId().ToString() }
//    };
//        _mockDetailedEpisodeCursor.Setup(cursor => cursor.ToListAsync(default)).ReturnsAsync(episodes);

//        // Act
//        var result = await _episodeService.GetDetailedAllAsync();

//        // Assert
//        Assert.NotNull(result);
//        Assert.NotEmpty(result);
//        Assert.Equal(2, result.Count);
//    }

//    [Fact]
//    public async Task GetDetailedAsync_ReturnsDetailedEpisodeDto()
//    {
//        // Arrange
//        var episodeId = 2;
//        var episode = new Episode { Id = episodeId, Title = "Episode Title For Test" };
//        _mockDetailedEpisodeCursor.Setup(cursor => cursor.ToListAsync(default)).ReturnsAsync(new List<Episode> { episode });

//        // Act
//        var result = await _episodeService.GetDetailedAsync(episodeId);

//        // Assert
//        Assert.NotNull(result);
//        Assert.Equal(episodeId, result?.Id);
//    }

//    [Fact]
//    public async Task GetDetailedAsync_ReturnsNull()
//    {
//        // Arrange
//        var nonExistingEpisodeId = 999999;
//        _mockDetailedEpisodeCursor.Setup(cursor => cursor.ToListAsync(default)).ReturnsAsync(new List<Episode>());

//        // Act
//        var result = await _episodeService.GetDetailedAsync(nonExistingEpisodeId);

//        // Assert
//        Assert.Null(result);
//    }

//}
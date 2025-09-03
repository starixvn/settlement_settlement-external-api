using Fanex.Data.Repository;
using NSubstitute;
using Settlement.External.Criteria;
using Settlement.External.Models;
using Settlement.External.Services;

namespace Settlement.External.UnitTests.Services
{
    public class MatchServiceTests
    {
        [Fact]
        public async Task GetMatchDetailsAsync_ReturnsExpectedMatchDetails()
        {
            // Arrange
            var expected = new List<MatchDetail>
            {
                new MatchDetail { MatchId = 1, LeagueId = 10 },
                new MatchDetail { MatchId = 2, LeagueId = 20 }
            };
            var repository = Substitute.For<IDynamicRepository>();
            repository.FetchAsync<MatchDetail>(Arg.Any<GetMatchDetailCriteria>()).Returns(expected);
            var service = new MatchService(repository);

            // Act
            var result = await service.GetMatchDetailsAsync("1,2");

            // Assert
            Assert.Equal(expected.Count, result.Count);
            Assert.Equal(expected[0].MatchId, result[0].MatchId);
            Assert.Equal(expected[1].LeagueId, result[1].LeagueId);
        }
    }
}

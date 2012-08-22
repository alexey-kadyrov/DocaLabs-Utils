using System.Linq;
using DocaLabs.Storage.Core;
using DocaLabs.Testing.Common.MSpec;

namespace DocaLabs.Testing.Common.Database.RepositoryScenarios
{
    public class UpdateEntitiesScenario : RepositoryTestsScenarioBase
    {
        InterestingPoint Point { get; set; }
        Place Place { get; set; }

        public UpdateEntitiesScenario()
        {
            MsSqlDatabaseBuilder.ExecuteScripts(ConnectionString, @"populate-database.sql");
        }

        public void GetEntities(Tile tile)
        {
            Point = tile.Points.First(x => x.Id == KnownPointBlueId);
            Place = tile.Places.First(x => x.Id == KnownPlace2Id);
        }

        public void UpdateEntities()
        {
            Point.Category = "Purple";
            Place.Name = "Place 42";
        }

        public void ShouldUpdateEntities()
        {
            ShouldContainTileInExpectedState();
            ShouldContainInterestinPointsInExpectedState();
            ShouldContainPlacesInExpectedState();
            ShouldContainsPointPlacesAssosiationsInExpectedState();
        }

        public void Refresh(IRefreshableRepository<Tile> tiles)
        {
            tiles.Refresh(Point);
            tiles.Refresh(Place);
        }

        void ShouldContainTileInExpectedState()
        {
            GetTiles().ShouldContainOnlySimilar((x, y) => x.Id == y, new[] { KnownTileId });
        }

        void ShouldContainInterestinPointsInExpectedState()
        {
            GetPoints().ShouldContainOnlySimilar((x, y) =>
                x.Id == y.Id && x.Category == y.Category && x.Tile_Id == KnownTileId, new[]
            {
                new InterestingPointInDb
                {
                    Id = KnownPointBlueId,
                    Category = "Purple",
                    Tile_Id = KnownTileId
                },    
                new InterestingPointInDb
                {
                    Id = KnownPointRedId,
                    Category = "Red",
                    Tile_Id = KnownTileId
                },    
                new InterestingPointInDb
                {
                    Id = KnownPointGreenId,
                    Category = "Green",
                    Tile_Id = KnownTileId
                }    
            });
        }

        void ShouldContainPlacesInExpectedState()
        {
            GetPlaces().ShouldContainOnlySimilar((x, y) =>
                x.Id == y.Id && x.Name == y.Name && x.Tile_Id == KnownTileId, new[]
            {
                new PlaceInDb
                {
                    Id = KnownPlace0Id,
                    Name = "Place 0",
                    Tile_Id = KnownTileId
                },
                new PlaceInDb
                {
                    Id = KnownPlace1Id,
                    Name = "Place 1",
                    Tile_Id = KnownTileId
                },
                new PlaceInDb
                {
                    Id = KnownPlace2Id,
                    Name = "Place 42",
                    Tile_Id = KnownTileId
                },
                new PlaceInDb
                {
                    Id = KnownPlace3Id,
                    Name = "Place 3",
                    Tile_Id = KnownTileId
                },
                new PlaceInDb
                {
                    Id = KnownPlace4Id,
                    Name = "Place 4",
                    Tile_Id = KnownTileId
                },
                new PlaceInDb
                {
                    Id = KnownPlace5Id,
                    Name = "Place 5",
                    Tile_Id = KnownTileId
                }
            });
        }

        void ShouldContainsPointPlacesAssosiationsInExpectedState()
        {
            GetPointPlaces().ShouldContainOnlySimilar((x, y) =>
                x.InterestingPoint_Id == y.InterestingPoint_Id && x.Place_Id == y.Place_Id, new[]
            {
                new PoinPlaceAssosiationInDb
                {
                    InterestingPoint_Id = KnownPointBlueId,
                    Place_Id = KnownPlace0Id
                },
                new PoinPlaceAssosiationInDb
                {
                    InterestingPoint_Id = KnownPointBlueId,
                    Place_Id = KnownPlace2Id
                },
                new PoinPlaceAssosiationInDb
                {
                    InterestingPoint_Id = KnownPointBlueId,
                    Place_Id = KnownPlace4Id
                },
                new PoinPlaceAssosiationInDb
                {
                    InterestingPoint_Id = KnownPointRedId,
                    Place_Id = KnownPlace1Id
                },
                new PoinPlaceAssosiationInDb
                {
                    InterestingPoint_Id = KnownPointRedId,
                    Place_Id = KnownPlace2Id
                },
                new PoinPlaceAssosiationInDb
                {
                    InterestingPoint_Id = KnownPointRedId,
                    Place_Id = KnownPlace3Id
                },
                new PoinPlaceAssosiationInDb
                {
                    InterestingPoint_Id = KnownPointGreenId,
                    Place_Id = KnownPlace3Id
                }
            });
        }
    }
}
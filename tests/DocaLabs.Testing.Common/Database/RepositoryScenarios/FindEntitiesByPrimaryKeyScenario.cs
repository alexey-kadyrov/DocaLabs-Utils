using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;

namespace DocaLabs.Testing.Common.Database.RepositoryScenarios
{
    public class FindEntitiesByPrimaryKeyScenario : RepositoryTestsScenarioBase
    {
        public FindEntitiesByPrimaryKeyScenario()
        {
            MsSqlDatabaseBuilder.ExecuteScripts(ConnectionString, @"populate-database.sql");
        }

        public void ShouldLoadEntities(Tile tile)
        {
            ShouldLoadTile(tile);
            ShouldLoadInterestingPoints(tile);
            ShouldLoadPlaces(tile);
        }

        void ShouldLoadTile(Tile tile)
        {
            tile.ShouldNotBeNull();
            tile.Id.ShouldEqual(KnownTileId);
        }

        void ShouldLoadInterestingPoints(Tile tile)
        {
            tile.Points.ShouldContainOnlySimilar((x, y) =>
                x.Id == y.Id && x.Category == y.Category && y.Tile_Id == tile.Id, new[]
            {
                new InterestingPointInDb
                {
                    Id = KnownPointBlueId,
                    Category = "Blue",
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

        void ShouldLoadPlaces(Tile tile)
        {
            tile.Places.ShouldContainOnlySimilar((x, y) =>
                x.Id == y.Id && x.Name == y.Name && y.Tile_Id == tile.Id, new[]
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
                    Name = "Place 2",
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
    }
}

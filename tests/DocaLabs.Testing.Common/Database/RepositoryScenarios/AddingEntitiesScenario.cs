using System.Collections.Generic;
using System.Transactions;
using DocaLabs.Testing.Common.MSpec;

namespace DocaLabs.Testing.Common.Database.RepositoryScenarios
{
    public class AddingEntitiesScenario : RepositoryTestsScenarioBase
    {
        public Tile GenerateTile()
        {
            var tile = new Tile { Id = KnownTileId };

            tile.Places = GeneratePlaces(tile);
            tile.Points = GeneratePoints(tile);

            return tile;
        }

        public void ShouldAddEntities()
        {
            // validation code is using READPAST hint that can be used only in certain transaction isolation levels
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                ShouldAddTile();
                ShouldAddInterestingPoints();
                ShouldAddPlaces();
                ShouldAddPointPlaceAssosiations();

                scope.Complete();
            }
        }

        List<Place> GeneratePlaces(Tile tile)
        {
            return new List<Place>
            {
                new Place { Id = KnownPlace0Id, Name = "Place 0", Tile = tile },
                new Place { Id = KnownPlace1Id, Name = "Place 1", Tile = tile },
                new Place { Id = KnownPlace2Id, Name = "Place 2", Tile = tile },
                new Place { Id = KnownPlace3Id, Name = "Place 3", Tile = tile },
                new Place { Id = KnownPlace4Id, Name = "Place 4", Tile = tile },
                new Place { Id = KnownPlace5Id, Name = "Place 5", Tile = tile }
            };
        }

        List<InterestingPoint> GeneratePoints(Tile tile)
        {
            return new List<InterestingPoint>
            {
                new InterestingPoint
                {
                    Id = KnownPointBlueId,
                    Category = "Blue",
                    Places = new List<Place> { tile.Places[0], tile.Places[2], tile.Places[4] },
                    Tile = tile
                },
                new InterestingPoint
                {
                    Id = KnownPointRedId, 
                    Category = "Red",
                    Places = new List<Place> { tile.Places[1], tile.Places[2], tile.Places[3] },
                    Tile = tile
                },
                new InterestingPoint
                {
                    Id = KnownPointGreenId,
                    Category = "Green",
                    Places = new List<Place> { tile.Places[3] },
                    Tile = tile
                }
            };
        }

        void ShouldAddTile()
        {
            GetTiles().ShouldContainOnlySimilar((x, y) => x.Id == y, new[] { KnownTileId });
        }

        void ShouldAddInterestingPoints()
        {
            GetPoints().ShouldContainOnlySimilar((x, y) =>
                x.Id == y.Id && x.Category == y.Category && x.Tile_Id == KnownTileId, new[]
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

        void ShouldAddPlaces()
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

        void ShouldAddPointPlaceAssosiations()
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

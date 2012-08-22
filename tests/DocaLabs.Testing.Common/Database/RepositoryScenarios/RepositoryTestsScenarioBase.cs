using System;
using System.Collections.Generic;
using System.Transactions;
using Machine.Specifications;
using Machine.Specifications.Annotations;

namespace DocaLabs.Testing.Common.Database.RepositoryScenarios
{
    public abstract class RepositoryTestsScenarioBase
    {
        public const string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DocaLabsRepositoryTests;Integrated Security=SSPI;";

        public readonly Guid KnownTileId = Guid.Parse("F8027143-F8B9-41B4-9A49-6A6A74BED529");
        public readonly Guid KnownPlace0Id = Guid.Parse("F4145B48-F796-4373-8AF3-F9F9B8983C09");
        public readonly Guid KnownPlace1Id = Guid.Parse("2AB17DE7-FD5F-4DD4-9F36-DC2ECE6F5A8F");
        public readonly Guid KnownPlace2Id = Guid.Parse("1C92AB7B-FF5F-4588-A304-05F07870B0A6");
        public readonly Guid KnownPlace3Id = Guid.Parse("9BC9770F-2862-42D9-B7F2-1065C2F27EBA");
        public readonly Guid KnownPlace4Id = Guid.Parse("07B7629C-A0C0-46B1-A815-9927A8555E5B");
        public readonly Guid KnownPlace5Id = Guid.Parse("7C2E64CA-5ECC-4D7D-82DB-2593D7293CD8");
        public readonly Guid KnownPointBlueId = Guid.Parse("07709830-323E-4277-B604-7B6980C65919");
        public readonly Guid KnownPointRedId = Guid.Parse("FE3D1F1F-7E4E-4DBE-9894-C063602664A4");
        public readonly Guid KnownPointGreenId = Guid.Parse("1368B5E6-3B87-4078-868F-4A1CA4B50622");

        public static void CleanAfter()
        {
            MsSqlDatabaseBuilder.ExecuteNonQuery(ConnectionString, "delete from PlaceInterestingPoints");
            MsSqlDatabaseBuilder.ExecuteNonQuery(ConnectionString, "delete from Places");
            MsSqlDatabaseBuilder.ExecuteNonQuery(ConnectionString, "delete from InterestingPoints");
            MsSqlDatabaseBuilder.ExecuteNonQuery(ConnectionString, "delete from Tiles");
        }

        public void DatabaseShouldBeEmpty()
        {
            if(!IsDatabaseEmpty())
                throw new SpecificationException("The database is expected to be empty.");
        }

        public bool IsDatabaseEmpty()
        {
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                return GetTiles().Count == 0 && GetPoints().Count == 0 && GetPlaces().Count == 0 && GetPointPlaces().Count == 0;
            }
        }

        protected List<InterestingPointInDb> GetPoints()
        {
            return MsSqlDatabaseBuilder.ExecuteQuery<InterestingPointInDb>(ConnectionString, "select Id, Category, Tile_Id from InterestingPoints (READPAST)");
        }

        protected List<Tile> GetTiles()
        {
            return MsSqlDatabaseBuilder.ExecuteQuery<Tile>(ConnectionString, "select Id from Tiles (READPAST)");
        }

        protected List<PlaceInDb> GetPlaces()
        {
            return MsSqlDatabaseBuilder.ExecuteQuery<PlaceInDb>(ConnectionString, "select Id, Name, Tile_Id from Places (READPAST)");
        }

        protected List<PoinPlaceAssosiationInDb> GetPointPlaces()
        {
            return MsSqlDatabaseBuilder.ExecuteQuery<PoinPlaceAssosiationInDb>(ConnectionString, "select InterestingPoint_Id, Place_Id from PlaceInterestingPoints (READPAST)");
        }

        // ReSharper disable InconsistentNaming
        protected class InterestingPointInDb
        {
            [UsedImplicitly] public Guid Id { get; set; }
            [UsedImplicitly] public string Category { get; set; }
            [UsedImplicitly] public Guid Tile_Id { get; set; }
        }

        protected class PlaceInDb
        {
            [UsedImplicitly] public Guid Id { get; set; }
            [UsedImplicitly] public string Name { get; set; }
            [UsedImplicitly] public Guid Tile_Id { get; set; }
        }

        protected class PoinPlaceAssosiationInDb
        {
            [UsedImplicitly] public Guid InterestingPoint_Id { get; set; }
            [UsedImplicitly] public Guid Place_Id { get; set; }
        }
        // ReSharper restore InconsistentNaming
    }
}
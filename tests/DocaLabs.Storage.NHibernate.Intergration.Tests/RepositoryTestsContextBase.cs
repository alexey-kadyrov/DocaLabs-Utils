using DocaLabs.Testing.Common.Database.RepositoryScenarios;
using Machine.Specifications;
using Machine.Specifications.Annotations;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace DocaLabs.Storage.NHibernate.Intergration.Tests
{
    class RepositoryTestsContextBase
    {
        static protected ISessionFactory session_factory;

        [UsedImplicitly] Cleanup after_each = 
            () => RepositoryTestsScenarioBase.CleanAfter();

        static protected void SetupSessionFactory()
        {
            RepositoryTestsDatabaseSetup.EnsureDatabaseExist();

            var mapper = new ModelMapper();

            mapper.AddMapping<TileMap>();
            mapper.AddMapping<InterestingPointMap>();
            mapper.AddMapping<PlaceMap>();

            var cfg = new Configuration().DataBaseIntegration(c =>
            {
                c.Dialect<MsSql2008Dialect>();
                c.ConnectionString = RepositoryTestsScenarioBase.ConnectionString;
                c.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;

                c.LogFormattedSql = true;
                c.LogSqlInConsole = true;
                c.AutoCommentSql = true;
            });

            cfg.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

            session_factory = cfg.BuildSessionFactory();
        }

        class TileMap : ClassMapping<Tile>
        {
            public TileMap()
            {
                Table("Tiles");

                DynamicUpdate(true);
                Version(x => x.Version, m =>
                {
                    m.Generated(VersionGeneration.Always);
                    m.Type(new BinaryBlobType());
                });

                Id(x => x.Id);

                Bag(x => x.Points, c =>
                {
                    c.Inverse(true);
                    c.Cascade(Cascade.All);
                    c.Key(k => k.Column("Tile_Id"));
                }, m => m.OneToMany(x => x.Class(typeof(InterestingPoint))));

                Bag(x => x.Places, c => 
                {
                    c.Inverse(true);
                    c.Cascade(Cascade.All);
                    c.Key(k => k.Column("Tile_Id"));
                }, m => m.OneToMany(x => x.Class(typeof(Place))));
            }
        }

        class InterestingPointMap : ClassMapping<InterestingPoint>
        {
            public InterestingPointMap()
            {
                Table("InterestingPoints");

                DynamicUpdate(true);
                Version(x => x.Version, m =>
                {
                    m.Generated(VersionGeneration.Always);
                    m.Type(new BinaryBlobType());
                });

                Id(x => x.Id);

                Property(x => x.Category, c =>
                {
                    c.NotNullable(true);
                    c.Length(500);
                });

                Bag(x => x.Places, c =>
                {
                    c.Cascade(Cascade.All);
                    c.Table("PlaceInterestingPoints");
                    c.Key(k => k.Column("InterestingPoint_Id"));
                }, m => m.ManyToMany(x =>
                {
                    x.Column("Place_Id");
                    x.Class(typeof (Place));
                }));

                ManyToOne(x => x.Tile, c =>
                {
                    c.NotNullable(true);
                    c.Column("Tile_Id");
                });
            }
        }

        class PlaceMap : ClassMapping<Place>
        {
            public PlaceMap()
            {
                Table("Places");

                DynamicUpdate(true);
                Version(x => x.Version, m =>
                {
                    m.Generated(VersionGeneration.Always);
                    m.Type(new BinaryBlobType());
                });

                Id(x => x.Id);

                Property(x => x.Name, c =>
                {
                    c.NotNullable(true);
                    c.Length(500);
                });

                Bag(x => x.Points, c =>
                {
                    c.Inverse(true);
                    c.Cascade(Cascade.All);
                    c.Table("PlaceInterestingPoints");
                    c.Key(k => k.Column("Place_Id"));
                }, m => m.ManyToMany(x =>
                {
                    x.Column("InterestingPoint_Id");
                    x.Class(typeof(InterestingPoint));
                }));

                ManyToOne(x => x.Tile, c =>
                {
                    c.NotNullable(true);
                    c.Column("Tile_Id");
                });
            }
        }
    }
}
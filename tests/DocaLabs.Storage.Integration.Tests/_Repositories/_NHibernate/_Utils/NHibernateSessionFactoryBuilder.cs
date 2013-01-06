using DocaLabs.Storage.Integration.Tests._Repositories._Scenarios;
using DocaLabs.Storage.Integration.Tests._Utils;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace DocaLabs.Storage.Integration.Tests._Repositories._NHibernate._Utils
{
    static class NHibernateSessionFactoryBuilder
    {
        public static ISessionFactory Build()
        {
            var mapper = new ModelMapper();

            mapper.AddMapping<BookMap>();
            mapper.AddMapping<PriceMap>();

            var cfg = new Configuration().DataBaseIntegration(c =>
            {
                c.Dialect<MsSql2008Dialect>();
                c.ConnectionString = MsSqlHelper.ConnectionStringSettings.ConnectionString;
                c.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                c.LogFormattedSql = true;
                c.LogSqlInConsole = true;
                c.AutoCommentSql = true;
            });

            cfg.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

            return cfg.BuildSessionFactory();
        }

        class BookMap : ClassMapping<Book>
        {
            public BookMap()
            {
                Table("Books");
                Lazy(false);

                Id(x => x.Id);

                Version(x => x.Version, m =>
                {
                    m.Generated(VersionGeneration.Always);
                    m.Type(new BinaryBlobType());
                });

                Property(x => x.Isbn, c =>
                {
                    c.NotNullable(true);
                    c.Length(13);
                });

                Property(x => x.Title, c =>
                {
                    c.NotNullable(true);
                    c.Length(100);
                });

                Bag(x => x.Prices, c =>
                {
                    c.Cascade(Cascade.All);
                    c.Lazy(CollectionLazy.NoLazy);
                    c.Key(m => m.Column("BookId"));
                }, m => m.OneToMany(x => x.Class(typeof(Price))));
            }
        }

        class PriceMap : ClassMapping<Price>
        {
            public PriceMap()
            {
                Table("BookPrices");
                Lazy(false);

                Id(x => x.Id);

                Property(x => x.Country, c =>
                {
                    c.NotNullable(true);
                    c.Length(2);
                });

                Property(x => x.Currency, c =>
                {
                    c.NotNullable(true);
                    c.Length(3);
                });

                Property(x => x.Value, c => c.NotNullable(true));
            }
        }
    }
}

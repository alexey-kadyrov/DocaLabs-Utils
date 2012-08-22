using System;
using System.Data.Entity;
using DocaLabs.Testing.Common.Database.RepositoryScenarios;
using DocaLabs.Testing.Common.MSpec;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.EntityFramework.Integration.Tests
{
    [Subject(typeof(RepositoryConfiguration)), IntegrationTag]
    class when_on_model_creating_action_is_set_in_repository_configuration
    {
        static Action<DbModelBuilder> action;

        Establish context = () => action = m =>
        {
            m.Entity<Tile>()
                .HasMany(x => x.Points)
                .WithOptional(y => y.Tile)
                .WillCascadeOnDelete();

            m.Entity<Tile>()
                .HasMany(x => x.Places)
                .WithOptional(y => y.Tile)
                .WillCascadeOnDelete();

            m.Entity<Place>()
                .HasMany(x => x.Points)
                .WithMany(y => y.Places);
        };

        Because of =
            () => RepositoryConfiguration.SetOnModelCreatingAction<Place>(action);

        It should_return_the_confogured_action =
            () => RepositoryConfiguration.GetOnModelCreatingAction<Place>().ShouldBeTheSameAs(action);
    }
}

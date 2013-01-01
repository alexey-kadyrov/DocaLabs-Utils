using System;
using System.Collections.Generic;
using DocaLabs.NHibernateStorage;
using DocaLabs.Storage.Core.Repositories;
using DocaLabs.Storage.Integration.Tests._Repositories._NHibernate._Utils;
using DocaLabs.Storage.Integration.Tests._Repositories._Scenarios;
using Machine.Specifications;
using NHibernate;

namespace DocaLabs.Storage.Integration.Tests._Repositories._NHibernate._Partitioned
{
    [Subject(typeof(Repository<>), "with partitioned repository session")]
    class when_adding_an_entity_and_the_transaction_is_committed
    {
        static AddingWithinCommittedTransaction<PartitionedScenarioProvider> scenario;

        Cleanup after_each = 
            () => scenario.Dispose();

        Establish context =
            () => scenario = new AddingWithinCommittedTransaction<PartitionedScenarioProvider>();

        Because of = 
            () => scenario.Run();

        It should_add_the_entity =
            () => scenario.AddedBook.ShouldMatch(scenario.OriginalBook);
    }

    [Subject(typeof(Repository<>), "with partitioned repository session")]
    class when_adding_an_entity_and_the_transaction_is_not_committed
    {
        static AddingWithinNonCommittedTransaction<PartitionedScenarioProvider> scenario;

        Cleanup after_each =
            () => scenario.Dispose();

        Establish context =
            () => scenario = new AddingWithinNonCommittedTransaction<PartitionedScenarioProvider>();

        Because of =
            () => scenario.Run();

        It should_not_add_the_entity =
            () => scenario.AddedBook.ShouldBeNull();
    }

    [Subject(typeof(Repository<>), "with partitioned repository session")]
    class when_adding_list_of_entities
    {
        static AddingRange<PartitionedScenarioProvider> scenario;

        Cleanup after_each =
            () => scenario.Dispose();

        Establish context =
            () => scenario = new AddingRange<PartitionedScenarioProvider>();

        Because of =
            () => scenario.RunEnumerableRange(scenario.OriginalBooks);

        It should_add_the_first_entity =
            () => scenario.GetPersistedBook(scenario.OriginalBooks[0].Id).ShouldMatch(scenario.OriginalBooks[0]);

        It should_add_the_second_entity =
            () => scenario.GetPersistedBook(scenario.OriginalBooks[1].Id).ShouldMatch(scenario.OriginalBooks[1]);
    }

    [Subject(typeof(Repository<>), "with partitioned repository session")]
    class when_adding_variable_param_list_of_entities
    {
        static AddingRange<PartitionedScenarioProvider> scenario;

        Cleanup after_each =
            () => scenario.Dispose();

        Establish context =
            () => scenario = new AddingRange<PartitionedScenarioProvider>();

        Because of =
            () => scenario.RunParamListRange(scenario.OriginalBooks[0], scenario.OriginalBooks[1]);

        It should_add_the_first_entity =
            () => scenario.GetPersistedBook(scenario.OriginalBooks[0].Id).ShouldMatch(scenario.OriginalBooks[0]);

        It should_add_the_second_entity =
            () => scenario.GetPersistedBook(scenario.OriginalBooks[1].Id).ShouldMatch(scenario.OriginalBooks[1]);
    }

    [Subject(typeof(Repository<>), "with partitioned repository session")]
    class when_removing_an_entity
    {
        static Removing<PartitionedScenarioProvider> scenario;

        Cleanup after_each =
            () => scenario.Dispose();

        Establish context =
            () => scenario = new Removing<PartitionedScenarioProvider>();

        Because of =
            () => scenario.Run();

        It should_delete_removed_entity =
            () => scenario.RemovedBook.ShouldBeNull();

        It should_keep_the_entity_which_was_not_removed =
            () => scenario.UnchangedBook.ShouldMatch(scenario.OriginalUnchangedBook);
    }

    [Subject(typeof(Repository<>), "with partitioned repository session")]
    class when_removing_list_of_entities
    {
        static RemovingRange<PartitionedScenarioProvider> scenario;

        Cleanup after_each =
            () => scenario.Dispose();

        Establish context =
            () => scenario = new RemovingRange<PartitionedScenarioProvider>();

        Because of =
            () => scenario.RunEnumerableRange();

        It should_delete_first_entity =
            () => scenario.GetPersistedBook(scenario.OriginalBooks[0].Id).ShouldBeNull();

        It should_delete_second_entity =
            () => scenario.GetPersistedBook(scenario.OriginalBooks[1].Id).ShouldBeNull();

        It should_keep_the_entity_which_was_not_removed =
            () => scenario.UnchangedBook.ShouldMatch(scenario.OriginalUnchangedBook);
    }

    [Subject(typeof(Repository<>), "with partitioned repository session")]
    class when_removing_variable_param_list_of_entities
    {
        static RemovingRange<PartitionedScenarioProvider> scenario;

        Cleanup after_each =
            () => scenario.Dispose();

        Establish context =
            () => scenario = new RemovingRange<PartitionedScenarioProvider>();

        Because of =
            () => scenario.RunParamListRange();

        It should_delete_first_entity =
            () => scenario.GetPersistedBook(scenario.OriginalBooks[0].Id).ShouldBeNull();

        It should_delete_second_entity =
            () => scenario.GetPersistedBook(scenario.OriginalBooks[1].Id).ShouldBeNull();

        It should_keep_the_entity_which_was_not_removed =
            () => scenario.UnchangedBook.ShouldMatch(scenario.OriginalUnchangedBook);
    }

    [Subject(typeof(Repository<>), "with partitioned repository session")]
    class when_updating_an_entity
    {
        static Updating<PartitionedScenarioProvider> scenario;

        Cleanup after_each =
            () => scenario.Dispose();

        Establish context =
            () => scenario = new Updating<PartitionedScenarioProvider>();

        Because of =
            () => scenario.Run("New Title");

        It should_change_only_affected_property =
            () => scenario.UpdatedBook.ShouldMatch(scenario.OriginalUpdatedBook, "New Title");

        It should_keep_the_entity_which_was_not_changed =
            () => scenario.UnchangedBook.ShouldMatch(scenario.OriginalUnchangedBook);
    }

    [Subject(typeof(Repository<>), "with partitioned repository session")]
    class when_getting_entities_by_primary_keys
    {
        static Getting<PartitionedScenarioProvider> scenario;

        Cleanup after_each =
            () => scenario.Dispose();

        Establish context =
            () => scenario = new Getting<PartitionedScenarioProvider>();

        Because of =
            () => scenario.Run();

        It should_get_the_first_entity =
            () => scenario.FirstFoundBook.ShouldMatch(scenario.FirstOriginalBook);

        It should_get_the_second_entity =
            () => scenario.SecondFoundBook.ShouldMatch(scenario.SecondOriginalBook);
    }

    [Subject(typeof(Repository<>), "with partitioned repository session")]
    class when_getting_an_entity_by_null_primary_keys
    {
        static Getting<PartitionedScenarioProvider> scenario;
        static Exception actual_exception;

        Cleanup after_each =
            () => scenario.Dispose();

        Establish context =
            () => scenario = new Getting<PartitionedScenarioProvider>();

        Because of =
            () => actual_exception = Catch.Exception(() => scenario.Books.Get(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_key_values_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("keyValues");
    }

    [Subject(typeof(Repository<>), "with partitioned repository session")]
    class when_getting_an_entity_using_more_than_one_value
    {
        static Getting<PartitionedScenarioProvider> scenario;
        static Exception actual_exception;

        Cleanup after_each =
            () => scenario.Dispose();

        Establish context =
            () => scenario = new Getting<PartitionedScenarioProvider>();

        Because of =
            () => actual_exception = Catch.Exception(() => scenario.Books.Get(Guid.NewGuid(), Guid.NewGuid()));

        It should_throw_argument_exception =
            () => actual_exception.ShouldBeOfType<ArgumentException>();

        It should_report_key_values_argument =
            () => ((ArgumentException)actual_exception).ParamName.ShouldEqual("keyValues");
    }

    [Subject(typeof(Repository<>), "with partitioned repository session")]
    class when_querying_for_entities
    {
        static Querying<PartitionedScenarioProvider> scenario;

        Cleanup after_each =
            () => scenario.Dispose();

        Establish context =
            () => scenario = new Querying<PartitionedScenarioProvider>();

        Because of =
            () => scenario.Run();

        It should_return_expected_number_of_entities =
            () => scenario.FoundBooks.Count.ShouldEqual(2);

        It should_find_first_entity =
            () => scenario.FoundBooks[0].ShouldMatch(scenario.GetOriginalBook(scenario.FoundBooks[0].Id));

        It should_find_second_entity =
            () => scenario.FoundBooks[1].ShouldMatch(scenario.GetOriginalBook(scenario.FoundBooks[1].Id));
    }

    [Subject(typeof(Repository<>), "with partitioned repository session")]
    class when_executing_a_query
    {
        static Executing<PartitionedScenarioProvider> scenario;
        static IQuery<Book> query;

        Cleanup after_each =
            () => scenario.Dispose();

        Establish context = () =>
        {
            scenario = new Executing<PartitionedScenarioProvider>();
            query = new TestQuery(scenario.OriginalBooks[0].Id, scenario.OriginalBooks[1].Id);
        };

        Because of =
            () => scenario.Run(query);

        It should_return_expected_number_of_entities =
            () => scenario.FoundBooks.Count.ShouldEqual(2);

        It should_find_first_entity =
            () => scenario.FoundBooks[0].ShouldMatch(scenario.GetOriginalBook(scenario.FoundBooks[0].Id));

        It should_find_second_entity =
            () => scenario.FoundBooks[1].ShouldMatch(scenario.GetOriginalBook(scenario.FoundBooks[1].Id));

        class TestQuery : Query<Book>
        {
            readonly Guid _id1;
            readonly Guid _id2;

            public TestQuery(Guid id1, Guid id2)
            {
                _id1 = id1;
                _id2 = id2;
            }

            protected override IList<Book> Execute(ISession session)
            {
                return session.QueryOver<Book>()
                       .Where(x => x.Id == _id1 || x.Id == _id2)
                       .List();
            }
        }
    }

    [Subject(typeof(Repository<>), "with partitioned repository session")]
    class when_executing_a_null_query
    {
        static Executing<PartitionedScenarioProvider> scenario;
        static Exception actual_exception;

        Cleanup after_each =
            () => scenario.Dispose();

        Establish context = 
            () => scenario = new Executing<PartitionedScenarioProvider>();

        Because of =
            () => actual_exception = Catch.Exception(() => scenario.Run(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_query_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("query");
    }

    [Subject(typeof(Repository<>), "with partitioned repository session")]
    class when_newing_repository_with_null_session
    {
        static Exception actual_exception;

        Because of = 
            () => actual_exception = Catch.Exception(() => new Repository<Book>(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_session_argument =
            () => ((ArgumentNullException)actual_exception).ParamName.ShouldEqual("session");
    }
}

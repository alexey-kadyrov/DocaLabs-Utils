﻿using System;
using DocaLabs.AzureStorage.Tables;
using DocaLabs.Storage.Integration.Tests._Repositories._Azure._Utils;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace DocaLabs.Storage.Integration.Tests._Repositories._Azure
{
    [Subject(typeof(AzureQuery<>))]
    class when_azure_scalar_query_is_executed_with_null_repository
    {
        static Mock<AzureScalarQuery<AzureRepositoryTestProduct, int>> query;
        static Exception actual_exception;

        Establish context =
            () => query = new Mock<AzureScalarQuery<AzureRepositoryTestProduct, int>> { CallBase = true };

        Because of =
            () => actual_exception = Catch.Exception(() => query.Object.Execute(null));

        It should_throw_argument_null_exception =
            () => actual_exception.ShouldBeOfType<ArgumentNullException>();

        It should_report_repository_argument =
            () => ((ArgumentNullException) actual_exception).ParamName.ShouldEqual("repository");
    }
}

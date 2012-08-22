using DocaLabs.Utils.Configuration;
using Moq;
using NUnit.Framework;

namespace DocaLabs.Utils.Tests.Configuration
{
    [TestFixture]
    public class CurrentConfigurationManagerTests
    {
        [TearDown]
        public void Teardown()
        {
            CurrentConfigurationManager.Current = null;
        }

        [Test]
        public void CurrentIsInstanceOfDefaultConfigurationManager()
        {
            Assert.IsInstanceOf<DefaultConfigurationManager>(CurrentConfigurationManager.Current);
        }

        [Test]
        public void ReplacingCurrentConfigurationManager()
        {
            var mockConfigurationManager = new Mock<IConfigurationManager>();

            var original = CurrentConfigurationManager.Current;

            // act
            CurrentConfigurationManager.Current = mockConfigurationManager.Object;

            Assert.IsNotNull(CurrentConfigurationManager.Current);
            Assert.AreNotSame(original, CurrentConfigurationManager.Current);
            Assert.AreSame(mockConfigurationManager.Object, CurrentConfigurationManager.Current);
        }

        [Test]
        public void ResettingCurrentToNullForcesToUseTheDefault()
        {
            var mockConfigurationManager = new Mock<IConfigurationManager>();

            var original = CurrentConfigurationManager.Current;

            // act
            CurrentConfigurationManager.Current = mockConfigurationManager.Object;
            CurrentConfigurationManager.Current = null;

            Assert.IsNotNull(CurrentConfigurationManager.Current);
            Assert.AreSame(original, CurrentConfigurationManager.Current);
            Assert.AreNotSame(mockConfigurationManager.Object, CurrentConfigurationManager.Current);
            Assert.IsInstanceOf<DefaultConfigurationManager>(CurrentConfigurationManager.Current);
        }
    }
}

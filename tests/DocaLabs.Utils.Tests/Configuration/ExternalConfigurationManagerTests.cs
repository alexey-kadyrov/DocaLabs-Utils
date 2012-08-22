using System.IO;
using DocaLabs.Utils.Configuration;
using Moq;
using NUnit.Framework;

namespace DocaLabs.Utils.Tests.Configuration
{
    [TestFixture]
    public class ExternalConfigurationManagerTests
    {
        [TearDown]
        public void Teardown()
        {
            CurrentConfigurationManager.Current = null;
        }

        #region .ctor

        [Test]
        public void CtorThrowsFileNotFoundException()
        {
            Assert.Catch<FileNotFoundException>(() => new ExternalConfiguration("very unknown file"));
        }

        #endregion

        #region ReplaceCurrentBy

        [Test]
        public void ReplacesCurrentByConfigurationManagerWithSpecifiedConfigFile()
        {
            var original = CurrentConfigurationManager.Current;

            var result = ExternalConfiguration.ReplaceCurrentBy("DocaLabs.Utils.Tests.External.config");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ExternalConfiguration>(result);
            Assert.AreSame(result, CurrentConfigurationManager.Current);
            Assert.AreNotSame(original, result);
        }

        [Test]
        public void ReplaceCurrentByThrowsFileNotFoundExceptionAndLeavesCurrentManagerUnchanged()
        {
            var mockConfigurationManager = new Mock<IConfigurationManager>();

            CurrentConfigurationManager.Current = mockConfigurationManager.Object;

            Assert.Catch<FileNotFoundException>(() => ExternalConfiguration.ReplaceCurrentBy("very unknown file"));

            Assert.AreSame(mockConfigurationManager.Object, CurrentConfigurationManager.Current);
        }

        #endregion ReplaceCurrentBy
        
        #region GetAppSetting

        [Test]
        public void GetAppSettingReturnsFoundSetting()
        {
            var target = new ExternalConfiguration("DocaLabs.Utils.Tests.External.config");

            var result = target.GetAppSetting("externalKey1");

            Assert.AreEqual("externalValue1", result);
        }

        [Test]
        public void GetAppSettingReturnsNullForNotFoundSetting()
        {
            var target = new ExternalConfiguration("DocaLabs.Utils.Tests.External.config");

            var result = target.GetAppSetting("very unknown name");

            Assert.IsNull(result);
        }

        #endregion

        #region GetConnectionString

        [Test]
        public void GetConnectionStringReturnsFoundString()
        {
            var target = new ExternalConfiguration("DocaLabs.Utils.Tests.External.config");

            var result = target.GetConnectionString("externalName1");

            Assert.AreEqual("System.Data.SqlClient", result.ProviderName);
            Assert.AreEqual("externalString1", result.ConnectionString);
        }

        [Test]
        public void GetConnectionStringReturnsNullForNotFoundString()
        {
            var target = new ExternalConfiguration("DocaLabs.Utils.Tests.External.config");

            var result = target.GetConnectionString("very unknown name");

            Assert.IsNull(result);
        }

        #endregion

        #region GetSection

        [Test]
        public void NonGenericGetSectionReturnsFoundSection()
        {
            var target = new ExternalConfiguration("DocaLabs.Utils.Tests.External.config");

            var result = target.GetSection("externalSection1");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<TestConfigurationSection>(result);
            Assert.AreEqual("externalTestValue1", ((TestConfigurationSection)result).TestValue);
        }

        [Test]
        public void NonGenericGetSectionReturnsNullForNotFoundSection()
        {
            var target = new ExternalConfiguration("DocaLabs.Utils.Tests.External.config");

            var result = target.GetSection("very unknown name");

            Assert.IsNull(result);
        }

        [Test]
        public void GenericGetSectionReturnsFoundSection()
        {
            var target = new ExternalConfiguration("DocaLabs.Utils.Tests.External.config");

            var result = target.GetSection<TestConfigurationSection>("externalSection1");

            Assert.IsNotNull(result);
            Assert.AreEqual("externalTestValue1", result.TestValue);
        }

        [Test]
        public void GenericGetSectionReturnsNullForNotFoundSection()
        {
            var target = new ExternalConfiguration("DocaLabs.Utils.Tests.External.config");

            var result = target.GetSection<TestConfigurationSection>("very unknown name");

            Assert.IsNull(result);
        }

        [Test]
        public void GenericGetSectionReturnsNullForExistingSectionOfWrongType()
        {
            var target = new ExternalConfiguration("DocaLabs.Utils.Tests.External.config");

            var result = target.GetSection<TestConfigurationSection>("anotherTestConfigurationSection");

            Assert.IsNull(result);
        }

        #endregion
    }
}

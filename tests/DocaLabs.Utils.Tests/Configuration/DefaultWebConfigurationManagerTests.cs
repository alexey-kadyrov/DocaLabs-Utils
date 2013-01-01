using DocaLabs.Utils.Configuration;
using DocaLabs.Utils.Tests.Configuration._Utils;
using NUnit.Framework;

namespace DocaLabs.Utils.Tests.Configuration
{
    [TestFixture(Description = "in absence of web.config the app.config (DocaLabs.Utils.Tests.dll.config) is used, it impossible to have both web.config and app.config in test run.")]
    public class DefaultWebConfigurationManagerTests
    {
        [TearDown]
        public void Teardown()
        {
            CurrentConfigurationManager.Current = null;
        }

        #region ReplaceCurrentBy

        [Test]
        public void ReplacesCurrentByConfigurationManagerWithSpecifiedConfigFile()
        {
            var original = CurrentConfigurationManager.Current;

            var result = DefaultWebConfigurationManager.ReplaceCurrentBy();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<DefaultWebConfigurationManager>(result);
            Assert.AreSame(result, CurrentConfigurationManager.Current);
            Assert.AreNotSame(original, result);
        }

        #endregion ReplaceCurrentBy

        #region GetAppSetting

        [Test]
        public void GetAppSettingReturnsFoundSetting()
        {
            var target = new DefaultWebConfigurationManager();

            var result = target.GetAppSetting("defaultKey1");

            Assert.AreEqual("defaultValue1", result);
        }

        [Test]
        public void GetAppSettingReturnsNullForNotFoundSetting()
        {
            var target = new DefaultWebConfigurationManager();

            var result = target.GetAppSetting("very unknown name");

            Assert.IsNull(result);
        }

        #endregion

        #region GetConnectionString

        [Test]
        public void GetConnectionStringReturnsFoundString()
        {
            var target = new DefaultWebConfigurationManager();

            var result = target.GetConnectionString("defaultName1");

            Assert.AreEqual("System.Data.SqlClient", result.ProviderName);
            Assert.AreEqual("defaultString1", result.ConnectionString);
        }

        [Test]
        public void GetConnectionStringReturnsNullForNotFoundString()
        {
            var target = new DefaultWebConfigurationManager();

            var result = target.GetConnectionString("very unknown name");

            Assert.IsNull(result);
        }

        #endregion

        #region GetSection

        [Test]
        public void NonGenericGetSectionReturnsFoundSection()
        {
            var target = new DefaultWebConfigurationManager();

            var result = target.GetSection("defaultSection1");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<TestConfigurationSection>(result);
            Assert.AreEqual("defaultTestValue1", ((TestConfigurationSection)result).TestValue);
        }

        [Test]
        public void NonGenericGetSectionReturnsNullForNotFoundSection()
        {
            var target = new DefaultWebConfigurationManager();

            var result = target.GetSection("very unknown name");

            Assert.IsNull(result);
        }

        [Test]
        public void GenericGetSectionReturnsFoundSection()
        {
            var target = new DefaultWebConfigurationManager();

            var result = target.GetSection<TestConfigurationSection>("defaultSection1");

            Assert.IsNotNull(result);
            Assert.AreEqual("defaultTestValue1", result.TestValue);
        }

        [Test]
        public void GenericGetSectionReturnsNullForNotFoundSection()
        {
            var target = new DefaultWebConfigurationManager();

            var result = target.GetSection<TestConfigurationSection>("very unknown name");

            Assert.IsNull(result);
        }

        [Test]
        public void GenericGetSectionReturnsNullForExistingSectionOfWrongType()
        {
            var target = new DefaultWebConfigurationManager();

            var result = target.GetSection<TestConfigurationSection>("anotherTestConfigurationSection");

            Assert.IsNull(result);
        }

        #endregion
    }
}

using DocaLabs.Utils.Configuration;
using DocaLabs.Utils.Tests.Configuration._Utils;
using NUnit.Framework;

namespace DocaLabs.Utils.Tests.Configuration
{
    [TestFixture]
    public class DefaultConfigurationManagerTests
    {
        #region GetAppSetting

        [Test]
        public void GetAppSettingReturnsFoundSetting()
        {
            var target = new DefaultConfigurationManager();

            var result = target.GetAppSetting("defaultKey1");

            Assert.AreEqual("defaultValue1", result);
        }

        [Test]
        public void GetAppSettingReturnsNullForNotFoundSetting()
        {
            var target = new DefaultConfigurationManager();

            var result = target.GetAppSetting("very unknown name");

            Assert.IsNull(result);
        }

        #endregion

        #region GetConnectionString

        [Test]
        public void GetConnectionStringReturnsFoundString()
        {
            var target = new DefaultConfigurationManager();

            var result = target.GetConnectionString("defaultName1");

            Assert.AreEqual("System.Data.SqlClient", result.ProviderName);
            Assert.AreEqual("defaultString1", result.ConnectionString);
        }

        [Test]
        public void GetConnectionStringReturnsNullForNotFoundString()
        {
            var target = new DefaultConfigurationManager();

            var result = target.GetConnectionString("very unknown name");

            Assert.IsNull(result);
        }

        #endregion

        #region GetSection

        [Test]
        public void NonGenericGetSectionReturnsFoundSection()
        {
            var target = new DefaultConfigurationManager();

            var result = target.GetSection("defaultSection1");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<TestConfigurationSection>(result);
            Assert.AreEqual("defaultTestValue1", ((TestConfigurationSection)result).TestValue);
        }

        [Test]
        public void NonGenericGetSectionReturnsNullForNotFoundSection()
        {
            var target = new DefaultConfigurationManager();

            var result = target.GetSection("very unknown name");

            Assert.IsNull(result);
        }

        [Test]
        public void GenericGetSectionReturnsFoundSection()
        {
            var target = new DefaultConfigurationManager();

            var result = target.GetSection<TestConfigurationSection>("defaultSection1");

            Assert.IsNotNull(result);
            Assert.AreEqual("defaultTestValue1", result.TestValue);
        }

        [Test]
        public void GenericGetSectionReturnsNullForNotFoundSection()
        {
            var target = new DefaultConfigurationManager();

            var result = target.GetSection<TestConfigurationSection>("very unknown name");

            Assert.IsNull(result);
        }

        [Test]
        public void GenericGetSectionReturnsNullForExistingSectionOfWrongType()
        {
            var target = new DefaultConfigurationManager();

            var result = target.GetSection<TestConfigurationSection>("anotherTestConfigurationSection");

            Assert.IsNull(result);
        }

        #endregion
    }
}

using System.Configuration;

namespace DocaLabs.Utils.Tests.Configuration
{
    public class TestConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("testValue", IsRequired = true)]
        public string TestValue
        {
            get { return (string)this["testValue"]; }
            set { this["testValue"] = value; }
        }
    }

    public class AnotherTestConfigurationSection : ConfigurationSection
    {
        
    }
}

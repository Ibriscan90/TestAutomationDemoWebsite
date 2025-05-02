using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAutomationDemoWebsite.Helpers
{
    public static class Config
    {
        private static IConfigurationRoot configuration;

        static Config()
        {
            configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .       Build();

        }

        public static string BaseUrl => configuration["TestSettings:BaseUrl"];
        public static string Username => configuration["TestSettings:Credentials:Username"];
        public static string Password => configuration["TestSettings:Credentials:Password"];

    }
}

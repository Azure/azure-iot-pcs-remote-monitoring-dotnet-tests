using System;
using System.Collections.Generic;
using System.Text;

namespace E2E_Tests.Helpers
{
    public class Constants
    {

        public static string CHROME_DRIVER = Environment.GetEnvironmentVariable("ChromeWebDriver");
        public static string IE_DRIVER = Environment.GetEnvironmentVariable("IEWebDriver");
        public static string FIREFOX_DRIVER = Environment.GetEnvironmentVariable("GeckoWebDriver");

        internal const string appURL = "http://rm-aks-test.eastus.cloudapp.azure.com/";

        public const string TEST = "Type";
        public const string E2E_TESTS = "E2E";

    }
}

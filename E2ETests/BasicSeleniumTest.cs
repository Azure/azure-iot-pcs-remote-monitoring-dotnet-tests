using Xunit;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using E2E_Tests.Helpers;

namespace E2E_Tests
{
    [Collection("ASA Manager Tests")]
    public  class BasicSeleniumTest : IDisposable
    {
        private IWebDriver driver;
        private string appURL = Constants.appURL;

        public BasicSeleniumTest()
        {
            appURL = "http://www.bing.com/";

            string browser = "Chrome";
            switch (browser)
            {
                case "Chrome":
                    driver = new ChromeDriver(Constants.CHROME_DRIVER);
                    break;
                case "Firefox":
                    driver = new FirefoxDriver(Constants.FIREFOX_DRIVER);
                    break;
                case "IE":
                    driver = new InternetExplorerDriver(Constants.IE_DRIVER);
                    break;
                default:
                    driver = new ChromeDriver(Constants.CHROME_DRIVER);
                    break;
            }
        }


        [Fact, Trait(Constants.TEST, Constants.E2E_TESTS)]
        public void TheBingSearchTest()
        {
            driver.Navigate().GoToUrl(appURL + "/");
            driver.FindElement(By.Id("sb_form_q")).SendKeys("Azure Pipelines");
            driver.FindElement(By.Id("sb_form_go")).Click();
            driver.FindElement(By.XPath("//ol[@id='b_results']/li/h2/a/strong[3]")).Click();
            Assert.True(driver.Title.Contains("Azure Pipelines"), "Verified title of the page");
        }

        public void Dispose()
        {
            driver.Quit();
        }
    }
}
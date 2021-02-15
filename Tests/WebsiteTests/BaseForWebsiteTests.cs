using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Tests.WebsiteTests
{
    public class BaseForWebsiteTests
    {
        protected readonly string _baseAppUrl = "http://pqc-debug:5000/";

        // protected readonly string _baseAppUrl = "http://localhost:5000/";
        //protected readonly string _seleniumUrl = "http://selenium:4444/wd/hub/";
        protected readonly string _seleniumUrl = "http://localhost:4444/wd/hub/";

        public IWebDriver GetDriver()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddAdditionalOption("version", "latest");
            options.AddAdditionalOption("platform", "WIN10");

            IWebDriver driver = new RemoteWebDriver(new Uri(_seleniumUrl), options.ToCapabilities());
            return driver;
        }
    }
}
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using Xunit;

namespace Tests.WebsiteTests
{
    public class WebPagesTests : BaseForWebsiteTests
    {
        [Fact]
        public void HomePageTest()
        {
            var driver = GetDriver();
            driver.Navigate().GoToUrl(_baseAppUrl);
            driver.Manage().Window.Maximize();

            Assert.Equal("Home Page - PQC", driver.Title);
            var images = driver.FindElements(By.ClassName("img-fluid"));
            Assert.Equal(3, images.Count);

            driver.Quit();
        }

        [Fact]
        public void MenuLinksTest()
        {
            var driver = GetDriver();
            driver.Navigate().GoToUrl(_baseAppUrl);
            driver.Manage().Window.Maximize();

            Assert.Equal("Home Page - PQC", driver.Title);

            var menuLinks = driver.FindElements(By.XPath("/html/body/header/nav/div/div/ul/li/a"));
            var urls = menuLinks.Select(l => l.GetAttribute("href")).ToList();

            Assert.Equal(3, urls.Count());

            foreach (var link in urls)
            {
                Assert.NotEmpty(link);
                Assert.Contains(_baseAppUrl, link);
                driver.Navigate().GoToUrl(link);
                Thread.Sleep(4000);

                Assert.Equal(link, driver.Url);
                try
                {
                    driver.FindElement(By.XPath("//*[@id=\"error-information-popup\"]"));
                    Assert.False(true, "Broken Page Link: " + link);
                }
                catch (NoSuchElementException)
                {
                }

                driver.Navigate().Back();
                Thread.Sleep(3000);
            }

            driver.Quit();
        }
    }
}
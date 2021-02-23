using OpenQA.Selenium;
using Xunit;

namespace Tests.WebsiteTests
{
    public class ErrorPageTest : BaseForWebsiteTests
    {
        [Fact]
        public void Error404PageTest()
        {
            var linkToTestPage = _baseAppUrl + "randomerror";
            var driver = GetDriver();
            driver.Navigate().GoToUrl(linkToTestPage);
            driver.Manage().Window.Maximize();

            Assert.Equal("404 Page - PQC", driver.Title);

            var image = driver.FindElements(By.ClassName("img-fluid"));
            var errorMessage = driver.FindElement(By.Id("errormessage="));

            Assert.NotNull(image);
            Assert.NotNull(errorMessage);
            driver.Quit();
        }
    }
}
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace Tests.WebsiteTests
{
    public class ResultsPage : BaseForWebsiteTests
    {
        private readonly string _linkToTestPage;

        public ResultsPage()
        {
            _linkToTestPage = _baseAppUrl + "Result";
        }

        [Fact]
        public void SelectRepositoryWithoutDataTest()
        {
            var driver = GetDriver();
            driver.Navigate().GoToUrl(_linkToTestPage);
            driver.Manage().Window.Maximize();

            Assert.Equal("Result - PQC", driver.Title);
            SelectElement projectList = new SelectElement(driver.FindElement(By.Id("repository")));
            projectList.SelectByText("noDataTest");

            IWebElement button = driver.FindElement(By.Name("repository_confirm"));

            button.Click();
            Thread.Sleep(2000);

            var errorMessage = driver.FindElement(By.XPath("//*[@id=\"chartImageError\"]/div[1]/h3"));
            var errorImage = driver.FindElement(By.XPath("//*[@id=\"chartImageError\"]/div[2]/img"));

            Assert.NotNull(errorMessage);
            Assert.NotNull(errorImage);
            Assert.True(errorImage.Displayed);
            driver.Quit();
        }

        [Fact]
        public void SelectRepositoryWithDataTest()
        {
            var driver = GetDriver();
            driver.Navigate().GoToUrl(_linkToTestPage);
            driver.Manage().Window.Maximize();

            Assert.Equal("Result - PQC", driver.Title);
            SelectElement projectList = new SelectElement(driver.FindElement(By.Id("repository")));
            projectList.SelectByText("flex-layout");

            IWebElement button = driver.FindElement(By.Name("repository_confirm"));
            button.Click();

            Thread.Sleep(2000);
            var charts = driver.FindElements(By.ClassName("chartAreaWrapper"));
            var labels = driver.FindElements(By.ClassName("badge"));

            Assert.Equal(8, charts.Count);
            Assert.Equal(8, labels.Count);
            driver.Quit();
        }
    }
}
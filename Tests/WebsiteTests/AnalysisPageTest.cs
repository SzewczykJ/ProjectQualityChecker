using System.Threading;
using OpenQA.Selenium;
using Xunit;

namespace Tests.WebsiteTests
{
    public class AnalysisPageTest : BaseForWebsiteTests
    {
        private readonly string _linkToTestPage;

        public AnalysisPageTest()
        {
            _linkToTestPage = _baseAppUrl + "Analysis";
        }

        [Fact]
        public void FormValidation_EmptyFieldsTest()
        {
            var driver = GetDriver();
            driver.Navigate().GoToUrl(_linkToTestPage);
            driver.Manage().Window.Maximize();

            Assert.Equal("Analyze the project - PQC", driver.Title);
            IWebElement projectName = driver.FindElement(By.Id("name"));
            IWebElement projectUrl = driver.FindElement(By.Id("link"));
            IWebElement formSentButton = driver.FindElement(By.Id("submit"));
            formSentButton.Submit();

            var nameErrorInfo = driver.FindElement(By.XPath("//*[@id=\"name-error\"]"));
            var linkErrorInfo = driver.FindElement(By.XPath("//*[@id=\"link-error\"]"));

            Assert.True(nameErrorInfo.Displayed);
            Assert.True(linkErrorInfo.Displayed);

            driver.Quit();
        }

        [Fact]
        public void FormValidation_EmptyName_FilledUrl_Test()
        {
            var linkForTest = "http://github.com/account/repo";

            var driver = GetDriver();
            driver.Navigate().GoToUrl(_linkToTestPage);
            driver.Manage().Window.Maximize();

            Assert.Equal("Analyze the project - PQC", driver.Title);

            IWebElement projectUrl = driver.FindElement(By.Id("link"));
            IWebElement formSentButton = driver.FindElement(By.Id("submit"));

            projectUrl.SendKeys(linkForTest);

            formSentButton.Submit();

            var nameErrorInfo = driver.FindElement(By.Id("name-error"));

            Assert.True(nameErrorInfo.Displayed);

            driver.Quit();
        }

        [Theory]
        [InlineData("http://github.com/account/repo")]
        public void FormValidation_ValidUrlTest(string linkToTest)
        {
            var driver = GetDriver();
            driver.Navigate().GoToUrl(_linkToTestPage);
            driver.Manage().Window.Maximize();

            Assert.Equal("Analyze the project - PQC", driver.Title);
            IWebElement projectName = driver.FindElement(By.Id("name"));
            IWebElement projectUrl = driver.FindElement(By.Id("link"));
            IWebElement formSentButton = driver.FindElement(By.Id("submit"));

            projectName.SendKeys("any text");
            projectUrl.SendKeys(linkToTest);
            formSentButton.Submit();

            IWebElement nameErrorInfo = null;
            IWebElement linkErrorInfo = null;
            try
            {
                nameErrorInfo = driver.FindElement(By.Id("name-error"));
                linkErrorInfo = driver.FindElement(By.Id("link-error"));
                Assert.False(nameErrorInfo.Displayed);
                Assert.False(linkErrorInfo.Displayed);
            }
            catch (NoSuchElementException)
            {
                Assert.Null(nameErrorInfo);
                Assert.Null(linkErrorInfo);
            }
            finally
            {
                driver.Quit();
            }
        }

        [Theory]
        [InlineData("randomtext")]
        [InlineData("randomtext.com")]
        [InlineData("www.randomtext.com")]
        [InlineData("http://randomtextom")]
        public void FormValidation_InvalidUrlTest(string linkToTest)
        {
            var driver = GetDriver();
            driver.Navigate().GoToUrl(_linkToTestPage);
            driver.Manage().Window.Maximize();

            Assert.Equal("Analyze the project - PQC", driver.Title);
            IWebElement projectName = driver.FindElement(By.Id("name"));
            IWebElement projectUrl = driver.FindElement(By.Id("link"));
            IWebElement formSentButton = driver.FindElement(By.Id("submit"));

            projectName.SendKeys("any text");
            projectUrl.SendKeys(linkToTest);
            formSentButton.Submit();

            IWebElement nameErrorInfo = null;
            IWebElement linkErrorInfo = null;
            try
            {
                linkErrorInfo = driver.FindElement(By.Id("link-error"));
                Assert.True(linkErrorInfo.Displayed);
                nameErrorInfo = driver.FindElement(By.Id("name-error"));
            }
            catch (NoSuchElementException)
            {
                Assert.Null(nameErrorInfo);
                Assert.NotNull(linkErrorInfo);
            }
            finally
            {
                driver.Quit();
            }
        }

        [Fact]
        public void FormValidation_ModalPopupOnFormSendTest()
        {
            var linkToTest = "http://github.com/account/repo";
            var driver = GetDriver();
            driver.Navigate().GoToUrl(_linkToTestPage);
            driver.Manage().Window.Maximize();

            Assert.Equal("Analyze the project - PQC", driver.Title);

            IWebElement projectName = driver.FindElement(By.Id("name"));
            IWebElement projectUrl = driver.FindElement(By.Id("link"));
            IWebElement formSentButton = driver.FindElement(By.Id("submit"));

            projectName.SendKeys("any text");
            projectUrl.SendKeys(linkToTest);
            formSentButton.Submit();

            Thread.Sleep(1000);
            IWebElement popup = driver.FindElement(By.CssSelector(".swal2-popup.swal2-modal"));
            Assert.NotNull(popup);

            var popupErrorIcon = driver.FindElement(By.CssSelector(".swal2-icon.swal2-error.swal2-icon-show"));
            Assert.NotNull(popupErrorIcon);
        }
    }
}
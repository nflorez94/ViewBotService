using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace ViewBot.Services
{
    public interface IViewBotServices
    {
        Task<FirefoxDriverService> GetDriverServiceAsync(FirefoxProfile profile);
        Task<FirefoxOptions> GetOptionsAsync();
        Task<FirefoxProfile> GetProfileAsync();
        Task<IWebDriver> PassFirstValidation(IWebDriver driver, FirefoxDriverService firefoxDriverService, FirefoxOptions firefoxOptions);
        Task PlayVideo(string url);
        Task<IWebDriver> SetupVideo(IWebDriver driver);
        Task<FirefoxDriver> StartDriverAsync(FirefoxDriverService firefoxDriverService, FirefoxOptions firefoxOptions);
    }
}
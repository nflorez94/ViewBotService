using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace ViewBot.Services
{
    public class ViewBotServices : IViewBotServices
    {
        public async Task<FirefoxProfile> GetProfileAsync()
        {
            string profilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Tor Browser\\Browser\\TorBrowser\\Data\\Browser\\profile.default";
            FirefoxProfile profile = new FirefoxProfile(profilePath);
            profile.SetPreference("network.proxy.type", 1);
            profile.SetPreference("network.proxy.socks", "127.0.0.1");
            profile.SetPreference("network.proxy.socks_port", 9153);
            profile.SetPreference("network.proxy.socks_remote_dns", false);
            return profile;
        }

        public async Task<FirefoxDriverService> GetDriverServiceAsync(FirefoxProfile profile)
        {
            string torPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Tor Browser\\Browser\\firefox.exe";

            FirefoxDriverService firefoxDriverService = FirefoxDriverService.CreateDefaultService("C:\\geckodriver-v0.30.0-win64", "geckodriver.exe");
            firefoxDriverService.FirefoxBinaryPath = torPath;
            firefoxDriverService.BrowserCommunicationPort = 2828;
            return firefoxDriverService;
        }

        public async Task<FirefoxOptions> GetOptionsAsync()
        {
            string profilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Tor Browser\\Browser\\TorBrowser\\Data\\Browser\\profile.default";
            var firefoxOptions = new FirefoxOptions
            {
                Profile = null,
                LogLevel = FirefoxDriverLogLevel.Trace
            };
            firefoxOptions.AddArguments("-profile", profilePath);
            return firefoxOptions;
        }

        public async Task<FirefoxDriver> StartDriverAsync(FirefoxDriverService firefoxDriverService, FirefoxOptions firefoxOptions)
        {
            FirefoxDriver driver = new FirefoxDriver(firefoxDriverService, firefoxOptions);
            Thread.Sleep(10000);
            driver.Navigate().GoToUrl("https://www.youtube.com/watch?v=Iu220UjOHZc?autoplay=1");
            Thread.Sleep(5000);
            driver= (FirefoxDriver)await PassFirstValidation(driver, firefoxDriverService, firefoxOptions);
            return driver;
        }

        public async Task<IWebDriver> PassFirstValidation(IWebDriver driver, FirefoxDriverService firefoxDriverService, FirefoxOptions firefoxOptions)
        {
            try
            {
                var agreeButtom = driver.FindElement(By.CssSelector("ytd-button-renderer.style-scope:nth-child(2) > a:nth-child(1) > tp-yt-paper-button:nth-child(1)"));
                if (agreeButtom != null)
                    agreeButtom.Click();
                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                try
                {
                    var googleError = driver.FindElement(By.CssSelector("body > div:nth-child(1) > div:nth-child(3)")).Text;
                    while (googleError.Contains("unusual traffic from your computer network"))
                    {
                        driver.Close();
                        try
                        {
                            driver = await StartDriverAsync(firefoxDriverService, firefoxOptions);
                            googleError = string.Empty;
                        }
                        catch (Exception ex2)
                        {
                            Console.WriteLine(ex2.ToString());
                            googleError = driver.FindElement(By.CssSelector("body > div:nth-child(1) > div:nth-child(3)")).Text;
                        }
                    }
                }
                catch (Exception catchEx)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return driver;
        }

        public async Task<IWebDriver> SetupVideo(IWebDriver driver)
        {
            var firefoxDriverService = await GetDriverServiceAsync(await GetProfileAsync());
            var firefoxOptions = await GetOptionsAsync();
            try
            {
                driver.FindElement(By.ClassName("ytp-mute-button")).Click();
                driver.FindElement(By.ClassName("ytp-play-button")).Click();
                driver.FindElement(By.ClassName("ytp-autonav-toggle-button")).Click();
            }
            catch (Exception ex)
            {
                try
                {
                    var existError = driver.FindElement(By.ClassName("ytp-mute-button"));
                    while (existError == null)
                    {
                        existError = driver.FindElement(By.ClassName("ytp-mute-button"));
                    }
                    driver.FindElement(By.ClassName("ytp-mute-button")).Click();
                    driver.FindElement(By.ClassName("ytp-play-button")).Click();
                    driver.FindElement(By.ClassName("ytp-autonav-toggle-button")).Click();
                }
                catch (Exception ex3)
                {
                    driver.Close();
                    driver = await StartDriverAsync(firefoxDriverService, firefoxOptions);
                }

            }
            return driver;
        }
        public async Task PlayVideo(string url)
        {

            var firefoxDriverService = await GetDriverServiceAsync(await GetProfileAsync());
            var firefoxOptions = await GetOptionsAsync();
            var driver =await StartDriverAsync(firefoxDriverService, firefoxOptions);

            var currentTime = driver.FindElement(By.ClassName("ytp-time-current")).Text;
            var finalTime = driver.FindElement(By.ClassName("ytp-time-duration")).Text;

            while (currentTime != finalTime)
            {
                var seconds = (int.Parse(finalTime.Split(':')[0]) * 60) + int.Parse(finalTime.Split(':')[1]);
                driver = (FirefoxDriver)await SetupVideo(driver);
                Thread.Sleep(seconds * 1000);
                driver.Close();
                await StartDriverAsync(firefoxDriverService, firefoxOptions);
            }
        }

        public async Task StartViewers(string url)
        {
            var Threads = new List<Thread>();
            for(var x=0; x<7; x++)
            {
                Threads.Add(new Thread(async () => await PlayVideo(url)));
            }

            foreach(var thread in Threads)
            {
                thread.Start();
            }


        }


    }
}
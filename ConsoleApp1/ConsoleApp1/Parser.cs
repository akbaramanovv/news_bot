using System;
using System.Collections.Generic;
using System.Text;

namespace NewsBotProject
{
    public class Parser
    {
        public static OpenQA.Selenium.IWebDriver driver;

        public static void StartBrowser()
        {
            //var chromeOptions = new OpenQA.Selenium.Chrome.ChromeOptions();
            //chromeOptions.AddArguments(new System.Collections.Generic.List<string>() {
            //"--silent-launch",
            //"--no-startup-window",
            //"no-sandbox",
            //"--start-maximized",
            //"--window-size=1920,1080",
            //"headless",});

            //var chromeDriverService = OpenQA.Selenium.Chrome.ChromeDriverService.CreateDefaultService();
            //chromeDriverService.HideCommandPromptWindow = true;
            driver = new OpenQA.Selenium.Chrome.ChromeDriver();
        }
        public static void CloseBrowser()
        {
            driver.Close();
        }
       
        public  virtual string GetLastRowFromDB()
        {
            return "";
        }
        public virtual void InsertNewsToDB(News news)
        {
           
        }
    }
}

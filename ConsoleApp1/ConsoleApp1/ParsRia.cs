using System;
using System.Collections.Generic;
using System.Text;

namespace NewsBotProject
{
    public class ParsRia : Parser, IBot
    {
        public void BotForAdminPanel(News news)
        {
            try
            {
                StartBrowser();

                driver.Navigate().GoToUrl("https://oldfor.com/admin.php?mod=addnews&action=addnews");

                System.Threading.Thread.Sleep(2000);
                driver.FindElement(OpenQA.Selenium.
               By.Name("username")).SendKeys("f3hm1@outlook.com");
                driver.FindElement(OpenQA.Selenium.
                    By.Name("password")).SendKeys("orxan12345");
                driver.Manage().Window.Maximize();
                driver.FindElement(OpenQA.Selenium.
                    By.ClassName("legitRipple")).Click();
                System.Threading.Thread.Sleep(1000);

                driver.FindElement(OpenQA.Selenium.
                    By.Name("title")).SendKeys(news.Header);
                System.Threading.Thread.Sleep(1000);
                var divElement = driver.FindElement(OpenQA.Selenium.
                    By.Id("category_chosen"));

                OpenQA.Selenium.Interactions.Actions actions = new OpenQA.Selenium.Interactions.Actions(driver);

                actions.MoveToElement(divElement).Click().Perform();
                OpenQA.Selenium.IJavaScriptExecutor js = (OpenQA.Selenium.IJavaScriptExecutor)driver;
                System.Threading.Thread.Sleep(1000);

                var item = driver.FindElement(OpenQA.Selenium.By
                    .CssSelector(".chosen-results > li:nth-child(2)"));
                actions.MoveToElement(item).Click().Perform();

                driver.SwitchTo().DefaultContent();

                System.Threading.Thread.Sleep(2000);

                driver.FindElement(OpenQA.Selenium.
                    By.Id("mceu_7-button")).Click();
                var mediaUploadFrame = driver.FindElement(OpenQA.Selenium.By
                    .CssSelector("#mediauploadframe"));
                driver.SwitchTo().Frame(mediaUploadFrame);
                System.Threading.Thread.Sleep(3000);

                driver.FindElement(OpenQA.Selenium.
                 By.XPath("//button[.='Select All']")).Click();

                System.Threading.Thread.Sleep(2000);

                driver.FindElement(OpenQA.Selenium.
                 By.XPath("//button[.='Delete files']")).Click();
                driver.SwitchTo().DefaultContent();
                System.Threading.Thread.Sleep(1000);

                driver.FindElement(OpenQA.Selenium.
               By.XPath("//button[.='Yes']")).Click();
                System.Threading.Thread.Sleep(2000);

                driver.SwitchTo().Frame(mediaUploadFrame);
                System.Threading.Thread.Sleep(1000);

                driver.FindElement(OpenQA.Selenium.
                    By.Id("copyurl")).SendKeys(news.ImageLink);

                System.Threading.Thread.Sleep(2000);

                driver.FindElement(OpenQA.Selenium.
                    By.XPath("//button[@class='edit' and contains(@onclick,'upload_from_url')]")).Click();

                System.Threading.Thread.Sleep(5000);

                driver.FindElement(OpenQA.Selenium.
                   By.XPath("//button[.='Select All']")).Click();
                System.Threading.Thread.Sleep(2000);

                driver.FindElement(OpenQA.Selenium.
                  By.XPath("//button[.='Insert selected']")).Click();
                driver.SwitchTo().DefaultContent();
                System.Threading.Thread.Sleep(2000);
                var editorFrame = driver.FindElement(OpenQA.Selenium.By.CssSelector("#short_story_ifr"));
                driver.SwitchTo().Frame(editorFrame);

                driver.FindElement(OpenQA.Selenium.
                    By.XPath("//*[@data-id='short_story']")).FindElement(OpenQA.Selenium.
                    By.TagName("p")).SendKeys(news.ShortStory);
                System.Threading.Thread.Sleep(2000);

                driver.SwitchTo().DefaultContent();
                System.Threading.Thread.Sleep(1000);
                driver.SwitchTo().DefaultContent();
                driver.FindElement(OpenQA.Selenium.
                  By.Id("mceu_75-button")).Click();

                var editorFrame2 = driver.FindElement(OpenQA.Selenium.
                    By.Id("full_story_ifr"));
                driver.SwitchTo().Frame(editorFrame2);
                System.Threading.Thread.Sleep(1000);

                driver.FindElement(OpenQA.Selenium.
                   By.XPath("//*[@data-id='full_story']")).FindElement(OpenQA.Selenium.
                   By.TagName("p")).SendKeys(news.FullStory);
                System.Threading.Thread.Sleep(2000);

                driver.SwitchTo().DefaultContent();
                System.Threading.Thread.Sleep(2000);
                var mediaUploadFrame2 = driver.FindElement(OpenQA.Selenium.By.Name("mediauploadframe"));
                driver.SwitchTo().Frame(mediaUploadFrame2);
                driver.FindElement(OpenQA.Selenium.
                 By.XPath("//button[.='Select All']")).Click();
                System.Threading.Thread.Sleep(2000);

                driver.FindElement(OpenQA.Selenium.
                 By.XPath("//button[.='Insert selected']")).Click();
                System.Threading.Thread.Sleep(2000);
                driver.SwitchTo().DefaultContent();
                System.Threading.Thread.Sleep(1000);
                driver.FindElement(OpenQA.Selenium.
                 By.Id("addnews")).Submit();
            }
            catch (System.TimeoutException e)
            {
                System.Console.WriteLine("Driver timeoout exception occured -- " + e);

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("another exception occured -- " + e);

            }
        }
        public News GetLasNewsFromSite(string link)
        {
            StartBrowser();
            OpenQA.Selenium.Support.UI.WebDriverWait wait =
                new OpenQA.Selenium.Support.UI.WebDriverWait(driver, System.TimeSpan.FromSeconds(120));

            wait.Until(webDriver => ((OpenQA.Selenium.IJavaScriptExecutor)webDriver)
            .ExecuteScript("return document.readyState").Equals("complete"));
            driver.Navigate().GoToUrl(link);

            System.Threading.Thread.Sleep(5000);
            string newsHeader = driver.FindElement(OpenQA.Selenium.By
                .ClassName("article__header"))
                .FindElement(OpenQA.Selenium.By
                .ClassName("article__title")).Text;

            string imgLink = driver.FindElement(
                OpenQA.Selenium.By
                .ClassName("photoview__open"))
                .FindElement(OpenQA.Selenium.By
                .TagName("img"))
                .GetAttribute("src");

            var texts = driver.FindElements(OpenQA.Selenium.By
                .ClassName("article__text"));
            OpenQA.Selenium.IJavaScriptExecutor js =
                (OpenQA.Selenium.IJavaScriptExecutor)driver;

            var sb = new System.Text.StringBuilder();

            string shortText = texts[0].Text;

            foreach (var text in texts)
            {
                if (text != null)

                    sb.Append(text.Text);
                else
                    sb.Append("");
            }


            News news = new News()
            {
                Date = System.DateTime.Now.ToString("yyyy-dd-dd hh:ss:ff"),
                ShortStory = Translation.Translate("en", shortText),
                FullStory = Translation.Translate("en", sb.ToString()),
                Link = link,
                Header = Translation.Translate("en", newsHeader),
                ImageLink = imgLink
            };
            return news;
        }
        public override string GetLastRowFromDB()
        {
            string conString = System.String.Format("Server={0};Database={1};Uid={2};Pwd={3};Port={4}",
                             "127.0.0.1", "news_bot", "root", "", "3306");

            string queryString = "SELECT  link  FROM world_news ORDER BY id DESC LIMIT 1";

            string link = "";
            try
            {
                using (MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(conString))
                {
                    using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(queryString, connection))
                    {
                        connection.Open();

                        MySql.Data.MySqlClient.MySqlDataReader data = cmd.ExecuteReader();
                        if (data.Read())
                        {
                            link = data.GetValue(0).ToString();
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e);
            }
            return link;
        }
        public override void InsertNewsToDB(News news)
        {
            try
            {
                string conString = System.String.Format("Server={0};Database={1};Uid={2};Pwd={3};Port={4}",
                           "127.0.0.1", "news_bot", "root", "", "3306");
                string queryString = "INSERT INTO world_news (link) VALUES (@link)";

                using (MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(conString))
                {
                    using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(queryString, connection))
                    {
                        connection.Open();


                        cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@link", news.Link));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (System.Exception e)
            {

                System.Console.WriteLine(e);
            }
        }
    }
}

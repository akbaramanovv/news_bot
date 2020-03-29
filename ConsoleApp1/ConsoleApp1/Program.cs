using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NewsBotProject
{
    class Program
    {
        public static string conString = "";
        public static string queryString = "";
        public class Translation
        {
            public int code { get; set; }
            public string lang { get; set; }
            public System.Collections.Generic.List<string> text { get; set; }
        }

        public class News
        {
            public int Id { get; set; }
            public string Link { get; set; }
            public string ImageLink { get; set; }

            public string Header { get; set; }
            public string ShortStory { get; set; }
            public string FullStory { get; set; }
            public string Date { get; set; }
            public string Category { get; set; } = "World";
            public string Fields { get; set; }
            public string Keyword { get; set; }
        }
        static string Translate(string lang, string newsText)
        {
            using (var wb = new System.Net.WebClient())
            {
                var reqData = new System.Collections.Specialized.NameValueCollection();
                reqData["text"] = newsText; // text to translate
                reqData["lang"] = lang; // target language
                reqData["key"] = "trnsl.1.1.20200322T204929Z.1b2f191026b631ad.d9409db5e96691aa2f2dd5b410cc3cc6bc9f0f59";

                dynamic rootObject = null;
                try
                {
                    var response = wb.UploadValues("https://translate.yandex.net/api/v1.5/tr.json/translate", "POST", reqData);
                    string responseInString = System.Text.Encoding.UTF8.GetString(response);

                    rootObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Translation>(responseInString);
                }

                catch (System.Exception ex)
                {
                    System.Console.WriteLine("ERROR!!! " + ex.Message);

                }
                string[] arr = new string[] { "sex", "porn", "ria", "RIA Novosti", "Novosti" };
                System.Text.StringBuilder s = new System.Text.StringBuilder(rootObject.text[0].ToString());

                foreach (var item in arr)
                {
                    int index1 = s.ToString().ToLower().IndexOf(item.ToLower());
                    if (index1 != -1)
                    {
                        s.Replace(item, "");

                    }
                }
                return s.ToString();
            }
        }

        static string GetLastRowFromDB()
        {
            conString = System.String.Format("Server={0};Database={1};Uid={2};Pwd={3};Port={4}",
                           "localhost", "newsbot", "root", "3303399e", "3306");

            queryString = "SELECT  NewsLink  FROM dle_post ORDER BY id DESC LIMIT 1";

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

        static News GetLasNewsFromSite(string link)
        {
            StartBrowser();
            driver.Navigate().GoToUrl(link);
            driver.Manage().Timeouts().PageLoad = System.TimeSpan.FromSeconds(120);

            //string linkText = driver.FindElements(OpenQA.Selenium.By.ClassName("list-item__title"))[0].GetAttribute("href");

            //driver.Url = driver.FindElements(OpenQA.Selenium.By.ClassName("list-item__title"))[0].GetAttribute("href");
            System.Threading.Thread.Sleep(5000);
            string newsHeader = driver.FindElement(OpenQA.Selenium.By
                .ClassName("article__header"))
                .FindElement(OpenQA.Selenium.By
                .ClassName("article__title")).Text;

            string imgLink = driver.FindElement(OpenQA.Selenium.By
                .ClassName("photoview__open"))
                .FindElement(OpenQA.Selenium.By.TagName("img"))
                .GetAttribute("src");

            var texts = driver.FindElements(OpenQA.Selenium.By.ClassName("article__text"));
            OpenQA.Selenium.IJavaScriptExecutor js = (OpenQA.Selenium.IJavaScriptExecutor)driver;
            //string title2 = (string)js.ExecuteScript("var div = document.getElementsByClassName('article__text');" +
            //    "var strong = document.getElementsByTagName('strong');" +
            //    "for (var i = 0; i < strong.length; i++){strong[i].remove()}");
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
                ShortStory = Translate("en", shortText),
                FullStory = Translate("en", sb.ToString()),
                Link = link,
                Header = Translate("en", newsHeader),
                ImageLink = imgLink
            };
            return news;
        }
        static void InsertNewsToDB(News news)
        {
            try
            {
                conString = System.String.Format("Server={0};Database={1};Uid={2};Pwd={3};Port={4}",
                          "localhost", "newsbot", "root", "3303399e", "3306");
                queryString = "INSERT INTO dle_post (date ,short_story, full_story , category , NewsLink , Header ) VALUES (@date , @short_story, @full_story , @category , @link , @header)";

                using (MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(conString))
                {
                    using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(queryString, connection))
                    {
                        connection.Open();

                        cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@date", news.Date));
                        cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@short_story", news.ShortStory));
                        cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@full_story", news.FullStory));
                        cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@category", news.Category));
                        cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@link", news.Link));
                        cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@header", news.Header));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (System.Exception e)
            {

                System.Console.WriteLine(e);
            }
        }

        public static OpenQA.Selenium.IWebDriver driver;


        static void StartBrowser()
        {

            driver = new OpenQA.Selenium.Chrome.ChromeDriver();

        }


        static void CloseBrowser()
        {
            driver.Close();

        }

        static void Main(string[] args)
        {

            for (; ; )
            {

                StartBrowser();
                driver.Navigate().GoToUrl("https://ria.ru/world/");
                driver.Manage().Timeouts().PageLoad = System.TimeSpan.FromSeconds(120);
                string link1 = driver.FindElements(OpenQA.Selenium.By.ClassName("list-item__title"))[0].GetAttribute("href").ToLower();

                string link2 = "asdasd";


                driver.Close();

                if (link1 != link2)
                {
                    News news = GetLasNewsFromSite(link1);
                    CloseBrowser();
                  

                    BotForAdminPanel(news);

                    InsertNewsToDB(news);
                    CloseBrowser();
                }


                System.Threading.Thread.Sleep(10 * 6000);
            }


        }

        static void BotForAdminPanel(News news)
        {

            StartBrowser();

            driver.Navigate().GoToUrl("https://oldfor.com/admin.php?mod=addnews&action=addnews");
            driver.Manage().Timeouts().PageLoad = System.TimeSpan.FromSeconds(120);

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

            var item = driver.FindElement(By.CssSelector(".chosen-results > li:nth-child(2)"));
            actions.MoveToElement(item).Click().Perform();

            //OpenQA.Selenium.IJavaScriptExecutor js3 = (OpenQA.Selenium.IJavaScriptExecutor)driver;
            //string title3 = (string)js.ExecuteScript("var drop2 = document.getElementsByClassName('chosen-container-multi')[0];" +
            //    "drop2.classList.add('chosen-container-active');");

            //OpenQA.Selenium.IJavaScriptExecutor js4 = (OpenQA.Selenium.IJavaScriptExecutor)driver;
            //string title4 = (string)js.ExecuteScript("var ul = document.getElementsByClassName('chosen-results')[0];" +
            //    "var li = document.createElement('li');" +
            //    "li.classList.add('result-selected');" +
            //    "li.innerHTML = 'World';" +
            //    "li.setAttribute('data-option-array-index', '2');" +
            //    "ul.appendChild(li);");
            //OpenQA.Selenium.IJavaScriptExecutor js5 = (OpenQA.Selenium.IJavaScriptExecutor)driver;
            //string title5 = (string)js.ExecuteScript("var ul = document.getElementsByClassName('chosen-choices')[0];" +
            //    "var li = document.createElement('li');" +
            //    "li.classList.add('search-choice');" +
            //    "var span =document.createElement('span') ;" +
            //    "span.innerHTML = 'World' ; " +
            //    "var a = document.createElement('a');" +
            //    "a.classList.add('search-choice-close');" +
            //    "a.setAttribute('data-option-array-index', '2');" +
            //    "li.appendChild(span);" +
            //    "li.appendChild(a);" +
            //    "ul.appendChild(li);");





            driver.SwitchTo().DefaultContent();

            System.Threading.Thread.Sleep(2000);

            driver.FindElement(OpenQA.Selenium.
                By.Id("mceu_7-button")).Click();
            var mediaUploadFrame = driver.FindElement(OpenQA.Selenium.By.CssSelector("#mediauploadframe"));
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

            var editorFrame2 = driver.FindElement(OpenQA.Selenium.By.Id("full_story_ifr"));
            driver.SwitchTo().Frame(editorFrame2);
            System.Threading.Thread.Sleep(1000);

            driver.FindElement(OpenQA.Selenium.
               By.XPath("//*[@data-id='full_story']")).FindElement(OpenQA.Selenium.
               By.TagName("p")).SendKeys(news.FullStory);
            System.Threading.Thread.Sleep(2000);



            //driver.SwitchTo().Frame(mediaUploadFrame);


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

        //static void ImageDownloadAndUpload(string imgLink)
        //{
        //    string imageUrl = imgLink;
        //    string localFilename = System.IO.Directory.GetCurrentDirectory() + "/Images/" + System.DateTime.Now.ToString("yyyymmddHHmmssttfff") + ".jpg";
        //    try
        //    {
        //        using (System.Net.WebClient client = new System.Net.WebClient())
        //        {
        //            client.DownloadFile(imageUrl, localFilename);
        //        }
        //    }
        //    catch (System.Net.WebException e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //    try
        //    {
        //        string filename = localFilename;
        //        System.String ftpServerIP = "ftp://oldfor.com/";
        //        System.IO.FileInfo fileInf = new System.IO.FileInfo(filename);
        //        System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(ftpServerIP + "/public_html/uploads/news_bot_temp/" + fileInf.Name);
        //        request.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
        //        request.Credentials = new System.Net.NetworkCredential("ljxmwyvi", "BIwpz98@27JP!e");
        //        request.UsePassive = true;
        //        request.UseBinary = true;
        //        request.KeepAlive = false;
        //        //Load the file
        //        System.IO.FileStream stream = System.IO.File.OpenRead(filename);
        //        byte[] buffer = new byte[stream.Length];

        //        stream.Read(buffer, 0, buffer.Length);

        //        System.IO.Stream reqStream = request.GetRequestStream();
        //        reqStream.Write(buffer, 0, buffer.Length);
        //        stream.Close();

        //        reqStream.Close();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        System.Console.WriteLine(ex.Message);
        //    }
        //    System.Threading.Thread.Sleep(3000);
        //    if (File.Exists(localFilename))
        //        File.Delete(localFilename);
        //}
    }
}



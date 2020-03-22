namespace NewsBotProject
{
    class Program
    {
        public class Translation
        {
            public int code { get; set; }
            public string lang { get; set; }
            public System.Collections.Generic.List<string> text { get; set; }
        }

        static void Translate(string lang, string text)
        {
            using (var wb = new System.Net.WebClient())
            {
                var reqData = new System.Collections.Specialized.NameValueCollection();
                reqData["text"] = text; // text to translate
                reqData["lang"] = lang; // target language
                reqData["key"] = "trnsl.1.1.20200322T204929Z.1b2f191026b631ad.d9409db5e96691aa2f2dd5b410cc3cc6bc9f0f59";

                try
                {
                    var response = wb.UploadValues("https://translate.yandex.net/api/v1.5/tr.json/translate", "POST", reqData);
                    string responseInString = System.Text.Encoding.UTF8.GetString(response);

                    var rootObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Translation>(responseInString);
                    System.Console.WriteLine($"Original text: {reqData["text"]}\n" +
                        $"Translated text: {rootObject.text[0]}\n" +
                        $"Lang: {rootObject.lang}");

                    System.Console.ReadLine();
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine("ERROR!!! " + ex.Message);
                    throw;
                }
            }
        }

        //bazadan cekilen son xeberin linki burda olmalidir
        // string hrefstr = newsLinks[0].FindElement(OpenQA.Selenium.By.ClassName("list-item__title")).GetAttribute("href");
        static void Main(string[] args)
        {
            OpenQA.Selenium.Chrome.ChromeDriverService service = OpenQA.Selenium.Chrome.ChromeDriverService.CreateDefaultService();
            var chromeOptions = new OpenQA.Selenium.Chrome.ChromeOptions();
            chromeOptions.AddArgument("headless");

            service.HideCommandPromptWindow = true;
            OpenQA.Selenium.IWebDriver driver = new OpenQA.Selenium.Chrome.ChromeDriver(service , chromeOptions);
          
            driver.Navigate().GoToUrl("https://ria.ru/world/");

            var newsLinks = driver.FindElements(OpenQA.Selenium.By.ClassName("list-item"));

            foreach (var item in newsLinks)
            {
                var link = item.FindElement(OpenQA.Selenium.By.TagName("a"));
                string linkText = link.GetAttribute("href");
                link.Click();

                string newsHeader = driver.FindElement(OpenQA.Selenium.By
                    .ClassName("article__header"))
                    .FindElement(OpenQA.Selenium.By
                    .ClassName("article__title")).Text;

                string imgLink = driver.FindElement(OpenQA.Selenium.By
                    .ClassName("photoview__open"))
                    .FindElement(OpenQA.Selenium.By.TagName("img"))
                    .GetAttribute("src");

                var texts = driver.FindElements(OpenQA.Selenium.By.ClassName("article__text"));
                var sb = new System.Text.StringBuilder();
                foreach (var text in texts)
                {
                    if (text != null)

                        sb.Append(text.Text);
                    else
                        sb.Append(" ");
                }
                Translate("en", sb.ToString());


                System.Console.WriteLine();

            }


        }
    }
}


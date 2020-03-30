using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NewsBotProject
{
    class Program
    {
       

        static void Main(string[] args)
        {

            for (; ; )
            {
                ParsRia parsRia = new ParsRia();
                string link1 = "";
                try
                {
                    Parser.StartBrowser();

                    Parser.driver.Navigate().GoToUrl("https://ria.ru/world/");
                    System.Threading.Thread.Sleep(2000);
                    link1 = Parser.driver.FindElements(OpenQA.Selenium.By.ClassName("list-item__title"))[0].GetAttribute("href").ToLower();

                }
                catch (System.Exception e)
                {
                    System.Console.WriteLine(e);
                };

                System.Threading.Thread.Sleep(2000);
                string link2 = parsRia.GetLastRowFromDB();


                Parser.driver.Close();

                if (link1 != link2)
                {
                    News news = parsRia.GetLasNewsFromSite(link1);
                    Parser.CloseBrowser();


                    parsRia.BotForAdminPanel(news);

                    parsRia.InsertNewsToDB(news);
                    Parser.CloseBrowser();
                }


                System.Threading.Thread.Sleep(10 * 5000);
            }


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



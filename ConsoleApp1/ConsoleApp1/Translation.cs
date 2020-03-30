using System;
using System.Collections.Generic;
using System.Text;

namespace NewsBotProject
{
    class Translation
    {
        public int code { get; set; }
        public string lang { get; set; }
        public System.Collections.Generic.List<string> text { get; set; }

        public static string Translate(string lang, string newsText)
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
                string[] arr = new string[] { "sex", "porn", "ria", "RIA Novosti", "Novosti", "Radio Sputnik", "RIA NOVOSTI", "RADIO SPUTNIK" };
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
    }
}

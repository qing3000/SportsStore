using System.IO;
using System.Net;
using System.Text.RegularExpressions;


namespace SportsStore.Domain.Entities
{
    public class Translator
    {
        public static string ManualTranslate(string sentence)
        {
            string ret;
            if (Regex.IsMatch(sentence, @"\d*-\d*y"))
            {
                ret = sentence.Substring(0, sentence.Length - 1) + @"岁";
            }
            else
            {
                ret = Translate(sentence);
            }

            return ret;
        }

        public static string Translate(string sentence, bool language = false)
        {
            string from_language = @"en";
            string to_language = @"zh";

            if (language == true)
            {
                from_language = @"zh";
                to_language = @"en";
            }

            string translationString = GoogleTranslate(sentence, from_language, to_language);
            return WebUtility.HtmlDecode(translationString);
        }

        private static string GoogleTranslate(string word, string from_language, string to_language)
        {
            WebClient client = new WebClient();

            // Prepare uri Google transaltion string 
            string uri = string.Format(@"http://translate.google.com/m?hl={0}&sl={1}&q={2}", to_language, from_language, word);

            string data = readURI(uri);

            // Locate the result
            string before_trans = @"class=""t0"">";
            int index = data.IndexOf(before_trans);
            string ret = data.Remove(0, index + before_trans.Length).Split('<')[0];
            return ret;
        }

        private static string readURI(string uri)
        {
            WebClient client = new WebClient();
            // Add a user agent header in case the requested URI contains a query.
            // Do the translation
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            Stream data = client.OpenRead(uri);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            data.Close();
            reader.Close();
            return s;
        }
    }
}
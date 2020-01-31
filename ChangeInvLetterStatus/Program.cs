using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChangeInvLetterStatus
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("進入Console");
            string oData = GetJsonContent($"http://invoice.rocket-coding.com/InvTables/ChangeInvStatus?Now={DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd")}");
            //string oData = GetJsonContent($"http://invoice.rocket-coding.com/InvTables/ChangeInvStatus?Now=2020-01-01");

            Console.WriteLine("跑完API");

            //Console.ReadKey();


        }

        //static async Task Main(string[] args)
        //{

        //    HttpClient client = new HttpClient();
        //    var fooResult = await client.GetStringAsync("http://invoice.rocket-coding.com/InvAccounts/CheckAcEm?Guid=40d1c52b-b977-4312-89bf-ffe11c1faa09");
        //    Console.WriteLine($"{fooResult}");
        //    Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
        //    Console.ReadKey();

        //}

        private static string GetJsonContent(string Url)
        {
            try
            {
                string targetURI = Url;
                var request = WebRequest.Create(targetURI);
                request.ContentType = "application/json; charset=utf-8";

                Console.WriteLine("準備接資料");

                string text;
                var response = (HttpWebResponse)request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    text = sr.ReadToEnd();
                }
                Console.WriteLine("接到資料了");

                return text;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}

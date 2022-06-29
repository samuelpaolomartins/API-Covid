using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace APICovid
{
    public class MetodosApi
    {
        private const string Url = "https://covid-193.p.rapidapi.com"; //const a variável é constatnte, não pode alterar
        private const string Key = "d7a0543448mshd4c0461dfd90571p149c59jsn063447df72e7";

        public static async Task<CountryModel> GetCountries(string country = null)
        {
            var client = new HttpClient();

            var url = Url + "/countries";

            if (!string.IsNullOrEmpty(country))
                url += $"?search={country}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "X-RapidAPI-Key", Key },
                    { "X-RapidAPI-Host", "covid-193.p.rapidapi.com" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                CountryModel info = JsonConvert.DeserializeObject<CountryModel>(body);
                for (int i = 0; i < info.Response.Length; i++)
                {
                    info.Response[i] = HttpUtility.HtmlDecode(info.Response[i]);
                }
                return info;
            }
        }

        public static async Task<StatisticsModel> GetEstatistica(string country = null)
        {
            var client = new HttpClient();

            var url = Url + "/statistics";

            if (!string.IsNullOrEmpty(country))
                url += $"?country={country}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "X-RapidAPI-Key", Key },
                    { "X-RapidAPI-Host", "covid-193.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(body);
                StatisticsModel info = JsonConvert.DeserializeObject<StatisticsModel>(body);
                return info;
            }
        }
        public static async Task<StatisticsModel> GetHistorico(string country, DateTime? date = null)
        {
            var client = new HttpClient();

            var url = Url + "/history";

            if (!string.IsNullOrEmpty(country))
            {
                url += $"?country={country}";

                if (date != null)
                {
                    url += $"&day={date?.ToString("yyyy-MM-dd")}";
                }
            }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "X-RapidAPI-Key", Key },
                    { "X-RapidAPI-Host", "covid-193.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(body);
                StatisticsModel info = JsonConvert.DeserializeObject<StatisticsModel>(body);
                return info;
            }
        }
    }
}

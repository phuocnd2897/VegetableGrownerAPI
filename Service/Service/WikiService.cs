
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VG.Model.Model;
using VG.Model.ResponseModel;

namespace VG.Service.Service
{
    public interface IWikiService
    {
        IEnumerable<VegetableResponseModel> LeakInfoFromWikiByTitle(string title);
    }
    public class WikiService : IWikiService
    {
        private const string APISearchList = "http://vi.wikipedia.org/w/api.php?action=query&format=json&list=search&srsearch=";
        private const string APIGetInfo = "http://vi.wikipedia.org/w/api.php?action=parse&prop=text&format=json&page=";
        private const string APIGetInfo2 = "http://vi.wikipedia.org/w/api.php?action=parse&prop=wikitext&format=json&page=";
        public IEnumerable<VegetableResponseModel> LeakInfoFromWikiByTitle(string title)
        {
            string resultSearch = null;
            DataTable dt = null;
            List<string> plainText = new List<string>();
            string description = "";
            string feature = "";
            List<VegetableImage> postImages = new List<VegetableImage>();
            List<VegetableResponseModel> vegetables = new List<VegetableResponseModel>();
            char[] separators = new char[] { '[', ']', '\\' };
            Regex pattern = new Regex("[\\[\\]\\\\]");
            using (var httpRequestSearch = new HttpRequestMessage(HttpMethod.Get, APISearchList+ title))
            {
                using (var httpClient = new HttpClient())
                {
                    var responseSearch = httpClient.SendAsync(httpRequestSearch).Result;

                    if (responseSearch.IsSuccessStatusCode)
                    {
                        resultSearch = responseSearch.Content.ReadAsStringAsync().Result;
                        JObject myObj = JsonConvert.DeserializeObject<JObject>(resultSearch);
                        string value = myObj.SelectToken("query.search").ToString();
                        dt = JsonConvert.DeserializeObject<DataTable>(value);
                    }
                    else
                    {
                    }
                }
            }
            foreach (DataRow dr in dt.Rows)
            {
                using (var httpRequestSearch = new HttpRequestMessage(HttpMethod.Get, APIGetInfo2 + dr["title"]))
                {
                    using (var httpClient = new HttpClient())
                    {
                        var responseSearch = httpClient.SendAsync(httpRequestSearch).Result;

                        if (responseSearch.IsSuccessStatusCode)
                        {
                            resultSearch = responseSearch.Content.ReadAsStringAsync().Result;
                            JObject myObj = JsonConvert.DeserializeObject<JObject>(resultSearch);
                            string value = myObj.SelectToken("parse.wikitext").ToString();
                            string plantext = value.Replace(@"\n","\n\r");
                            var split = plantext.Split("\n\r\n\r==");
                            split = split.Select(s => Regex.Replace(s, @"[|]", string.Empty)).ToArray();
                            description = split[0].Substring(split[0].IndexOf("'''" + dr["title"].ToString() + "'''"), split[0].Length - split[0].IndexOf("'''" + dr["title"].ToString() + "'''") - 1);
                            feature = split.Where(s => s.Contains("Công dụng") || s.Contains("Tác dụng") || s.Contains("Ứng dụng")).FirstOrDefault();
                            vegetables.Add(new VegetableResponseModel
                            {
                                Name = dr["title"].ToString() ,
                                Description = description != null ? pattern.Replace(description, string.Empty) : pattern.Replace(plantext, string.Empty),
                                Feature = feature!= null ? pattern.Replace(feature, string.Empty) : pattern.Replace(plantext, string.Empty),
                            });
                        }
                        else
                        {
                        }
                    }
                }
            }
            return vegetables;
        }
    }
}

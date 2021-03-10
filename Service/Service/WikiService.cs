
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
            string tittle = "";
            List<VegetableImage> postImages = new List<VegetableImage>();
            List<VegetableResponseModel> vegetables = new List<VegetableResponseModel>();
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
                using (var httpRequestSearch = new HttpRequestMessage(HttpMethod.Get, APIGetInfo + dr["title"]))
                {
                    using (var httpClient = new HttpClient())
                    {
                        var responseSearch = httpClient.SendAsync(httpRequestSearch).Result;

                        if (responseSearch.IsSuccessStatusCode)
                        {
                            resultSearch = responseSearch.Content.ReadAsStringAsync().Result;
                            JObject myObj = JsonConvert.DeserializeObject<JObject>(resultSearch);
                            string value = myObj.SelectToken("parse.text").ToString();
                            var document = new HtmlDocument();
                            document.LoadHtml(value);
                            var innerHtml = document.DocumentNode.SelectNodes("div")[0];
                            var test1 = innerHtml.Descendants().Where(s => s.Name == "img");
                        }
                        else
                        {
                        }
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
                            description = split[0].Substring(split[0].IndexOf(dr["title"].ToString()), split[0].Length - split[0].IndexOf(dr["title"].ToString()) - 1);
                            feature = split.Where(s => s.Contains("Công dụng") || s.Contains("Tác dụng") || s.Contains("Ứng dụng")).FirstOrDefault();
                            vegetables.Add(new VegetableResponseModel
                            {
                                Name = dr["title"].ToString(),
                                Description = description,
                                Feature = feature
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
        public string RemoveSpecialCharacters(string str)
        {
            string strResult = "";
            string except = "\n\r";
            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] >= '0' && str[i] <= '9') || (str[i] >= 'a' && str[i] <= 'z') || (str[i] >= 'A' && str[i] <= 'Z') || (except.IndexOf(str[i]) > -1))
                {
                    strResult += str[i];
                }
            }
            return strResult;
        }
        public string DoIt(string htmlString)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlString);
            document.DocumentNode.Descendants("img")
                                .Where(e =>
                                {
                                    string src = e.GetAttributeValue("src", null) ?? "";
                                    return !string.IsNullOrEmpty(src) && src.StartsWith("data:image");
                                })
                                .ToList()
                                .ForEach(x =>
                                {
                                    string currentSrcValue = x.GetAttributeValue("src", null);
                                    currentSrcValue = currentSrcValue.Split(',')[1];//Base64 part of string
                                byte[] imageData = Convert.FromBase64String(currentSrcValue);
                                    string contentId = Guid.NewGuid().ToString();
                                    LinkedResource inline = new LinkedResource(new MemoryStream(imageData), "image/jpeg");
                                    inline.ContentId = contentId;
                                    inline.TransferEncoding = TransferEncoding.Base64;

                                    x.SetAttributeValue("src", "cid:" + inline.ContentId);
                                });


            string result = document.DocumentNode.OuterHtml;
            return result;
        }
    }
}

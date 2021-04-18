
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VG.Common.Helper;
using VG.Data.Repository;
using VG.Model.Model;
using VG.Model.ResponseModel;

namespace VG.Service.Service
{
    public interface IWikiService
    {
        IEnumerable<WikiTitleResponseModel> LeakInfoFromWikiByTitle(string title);
        WikiResponseModel GetDescription(string title);
        IEnumerable<Keyword> TestGetKeyWord(string text);
    }
    public class WikiService : IWikiService
    {
        private const string APISearchList = "http://vi.wikipedia.org/w/api.php?action=query&format=json&list=search&srsearch=";
        private const string APIGetHTML = "http://vi.wikipedia.org/w/api.php?action=parse&prop=text&format=json&page=";
        private const string APIGetInfo2 = "http://vi.wikipedia.org/w/api.php?action=parse&prop=wikitext&format=json&page=";
        private const string APIGetDescription = "https://vi.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro&explaintext&redirects=1&titles=";
        private ILabelRepository _labelRepository;
        public WikiService(ILabelRepository labelRepository)
        {
            _labelRepository = labelRepository;
        }

        public WikiResponseModel GetDescription(string title)
        {
            List<string> texts = new List<string>();
            string description = "";
            string feature = "";
            List<WikiResponseModel> vegetables = new List<WikiResponseModel>();
            var listLabel = _labelRepository.GetMulti(s => s.VegCompositionId == 3 && s.StandsFor == "CD").ToArray();
            char[] separators = new char[] { '[', ']', '\\' };
            Regex pattern = new Regex("[\\[\\]\\\\]");
            using (var httpRequestSearch = new HttpRequestMessage(HttpMethod.Get, APIGetDescription + title))
            {
                using (var httpClient = new HttpClient())
                {
                    var responseSearch = httpClient.SendAsync(httpRequestSearch).Result;
                    if (responseSearch.IsSuccessStatusCode)
                    {
                        var resultSearch = responseSearch.Content.ReadAsStringAsync().Result;
                        JObject myObj = JsonConvert.DeserializeObject<JObject>(resultSearch);
                        description = myObj.SelectToken("query.pages").FirstOrDefault().Children<JToken>().FirstOrDefault().SelectToken("extract").ToString();
                    }
                }
            }
            using (var httpRequestSearch = new HttpRequestMessage(HttpMethod.Get, APIGetInfo2 + title))
            {
                using (var httpClient = new HttpClient())
                {
                    var responseSearch = httpClient.SendAsync(httpRequestSearch).Result;

                    if (responseSearch.IsSuccessStatusCode)
                    {
                        var resultSearch = responseSearch.Content.ReadAsStringAsync().Result;
                        JObject myObj = JsonConvert.DeserializeObject<JObject>(resultSearch);
                        string value = myObj.SelectToken("parse.wikitext").ToString();
                        string plantext = value.Replace(@"\n", "\n\r");
                        var split = plantext.IndexOf("===") > 0 ? plantext.Split("\n\r\n\r== ").ToList() : plantext.Split("\n\r\n\r==").ToList();
                        split.RemoveAt(0);
                        split = split.Select(s => Regex.Replace(s, @"[[=\']", string.Empty)).ToList();
                        split = split.Select(s => Regex.Replace(s, @"[]]", string.Empty)).ToList();
                        foreach (var item in listLabel.Select(s => s.LabelName))
                        {
                            foreach (var s in split)
                            {
                                var test = IdentityHelper.RemoveUnicode(s.Split("\n\r")[0]).ToLower();
                                var test2 = IdentityHelper.RemoveUnicode(item).ToLower();
                                if (test.Contains(test2))
                                {
                                    feature = Regex.Replace(string.Join("\n\r", s.Split("\n\r").Skip(1).ToArray()), "<.*?>.*?<.*?>", String.Empty);
                                    break;
                                }
                            }
                            if (feature != "")
                            {
                                break;
                            }
                            else
                            {
                                texts = split.Select(s => Regex.Replace(s, "<.*?>.*?<.*?>", String.Empty)).ToList();
                            }
                            vegetables.Add(new WikiResponseModel
                            {
                                Name = title,
                                Description = description,
                                Feature = feature,
                                ListText = texts
                            });
                        }
                    }
                }
            }
            
            return vegetables.GroupBy(s => s.Name).Select(s => s.First()).FirstOrDefault();
        }

        public IEnumerable<WikiTitleResponseModel> LeakInfoFromWikiByTitle(string title)
        {
            string resultSearch = null;
            DataTable dt = null;
            List<WikiTitleResponseModel> wikiTitleResponseModels = new List<WikiTitleResponseModel>();
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
            for (int i = 0; i < (dt.Rows.Count) / 2 + 1; i++)
            {
                using (var httpRequestSearch = new HttpRequestMessage(HttpMethod.Get, APIGetHTML + dt.Rows[i]["title"].ToString()))
                {
                    using (var httpClient = new HttpClient())
                    {
                        var responseSearch = httpClient.SendAsync(httpRequestSearch).Result;
                        if (responseSearch.IsSuccessStatusCode)
                        {
                            resultSearch = responseSearch.Content.ReadAsStringAsync().Result;
                            JObject myObj = JsonConvert.DeserializeObject<JObject>(resultSearch);
                            var value = myObj.SelectTokens("parse.text.*").First().ToString();
                            List<string> listOfImgdata = new List<string>();
                            string regexImgSrc = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
                            MatchCollection matchesImgSrc = Regex.Matches(value, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            var link = matchesImgSrc.Count > 0 ? matchesImgSrc.ElementAt(0).Groups[1].Value : "";
                            wikiTitleResponseModels.Add(new WikiTitleResponseModel
                            {
                                Title = dt.Rows[i]["title"].ToString(),
                                Image = link != null && link != "" ? link : ""
                            });
                        }
                    }
                }
            }
            return wikiTitleResponseModels;
        }

        public IEnumerable<Keyword> TestGetKeyWord(string text)
        {
            //object Obj = new object()
            //{
            //    Text = ""
            //};
            return null;
        }
    }
}

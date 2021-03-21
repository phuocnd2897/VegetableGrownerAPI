using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using VG.Data.Repository;
using VG.Model.Model;
using VG.Model.RequestModel;
using VG.Model.ResponseModel;

namespace VG.Service.Service
{
    public interface IVegetableService
    {
        VegetableRequestModel Add(VegetableRequestModel newItem, string phoneNumber, string savePath, string url);
        VegetableRequestModel Update(VegetableRequestModel newItem, string phoneNumber, string savePath, string url);
        void Delete(int noVeg, int gardenId);
        public VegetableResponseModel Get(int noVeg, int gardenId);
        public VegetableResponseModel Get(string Id);
        public IEnumerable<VegetableResponseModel> GetByGardenId(int GardenId);
        public IEnumerable<VegetableResponseModel> SearchByDescription(string searchValue);
        public IEnumerable<VegetableResponseModel> SearchByName(string searchValue);
        public IEnumerable<VegetableResponseModel> SearchByKeyword(string keyword);
    }
    public class VegetableService : IVegetableService
    {
        private IVegetableRepository _vegetableRepository;
        private IVegetableDescriptionRepository _vegetableDescriptionRepository;
        private IVegetableCompositionRepository _vegetableCompositionRepository;
        private IVegetableImageService _vegetableImageService;
        private IAccountRepository _acccountRepository;
        private IWikiService _wikiService;
        private IKeywordRepository _keywordRepository;
        private IHttpClientFactory _factory;
        private const string uri = "https://extractkeywords.herokuapp.com";
        public VegetableService(IVegetableRepository vegetableRepository, IVegetableDescriptionRepository vegetableDescriptionRepository, 
            IVegetableCompositionRepository vegetableCompositionRepository, IVegetableImageService vegetableImageService, 
            IAccountRepository acccountRepository, IWikiService wikiService, IKeywordRepository keywordRepository, IHttpClientFactory factory)
        {
            _vegetableRepository = vegetableRepository;
            _vegetableDescriptionRepository = vegetableDescriptionRepository;
            _vegetableCompositionRepository = vegetableCompositionRepository;
            _vegetableImageService = vegetableImageService;
            _acccountRepository = acccountRepository;
            _wikiService = wikiService;
            _keywordRepository = keywordRepository;
            _factory = factory;
        }
        public VegetableRequestModel Add(VegetableRequestModel newItem, string phoneNumber, string savePath, string url)
        {
            int num = 1;
            var item = this._vegetableRepository.GetMulti(s => s.GardenId == newItem.GardenId);
            if (item.Count() > 0)
            {
                num = item.Select(s => s.No).Max() + 1;
            }
            if (newItem.NewFeatture != "")
            {
                var account = this._acccountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
                if (account == null)
                {
                    throw new Exception("Có lỗi xảy ra. Vui lòng thử lại");
                }
                var vegDetailName = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = newItem.Title,
                    VegetableCompositionId = 1,
                    AccountId = account.Id,
                });
                var vegName = this._vegetableRepository.Add(new Vegetable
                {
                    No = num,
                    GardenId = newItem.GardenId,
                    Quantity = newItem.Quantity,
                    VegetableDescriptionId = vegDetailName.Id
                });
                var vegDetailDes = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = newItem.Description,
                    VegetableCompositionId = 2,
                    AccountId = account.Id,
                });
                var vegDes = this._vegetableRepository.Add(new Vegetable
                {
                    No = num,
                    GardenId = newItem.GardenId,
                    Quantity = newItem.Quantity,
                    VegetableDescriptionId = vegDetailDes.Id
                });
                var vegDetailFeature = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = newItem.NewFeatture,
                    VegetableCompositionId = 3,
                    AccountId = account.Id,
                });
                var vegFeature = this._vegetableRepository.Add(new Vegetable
                {
                    No = num,
                    Quantity = newItem.Quantity,
                    GardenId = newItem.GardenId,
                    VegetableDescriptionId = vegDetailFeature.Id
                });
                var vegDetailImg = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = "Image",
                    VegetableCompositionId = 4,
                    AccountId = account.Id,
                });
                var vegImg = this._vegetableRepository.Add(new Vegetable
                {
                    No = num,
                    Quantity = newItem.Quantity,
                    GardenId = newItem.GardenId,
                    VegetableDescriptionId = vegDetailImg.Id
                });
                if (newItem.NewImages != null)
                {
                    foreach (IFormFile img in newItem.NewImages)
                    {
                        this._vegetableImageService.UploadImage(img, vegDetailImg.Id, savePath, url);
                    }
                }
                newItem.IdName = vegName.Id;
                newItem.IdDescription = vegDes.Id;
                newItem.IdFeature = vegFeature.Id;
                newItem.IdImage = vegImg.Id;

                HttpClient client = _factory.CreateClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(uri + "/keywords"),
                    Content = new StringContent(JsonConvert.SerializeObject(new KeywordsRequestModel { text = vegDetailFeature.VegContent }), Encoding.UTF8, "application/json"),
                };
                var response = client.SendAsync(request).Result;
                if (response.IsSuccessStatusCode)
                {
                    var comp = this._vegetableCompositionRepository.Add(new VegetableComposition
                    {
                        CompositionName = vegDetailName.VegContent,
                        VegetableDescriptionId = vegDetailName.Id
                    });
                    string jsonData = response.Content.ReadAsStringAsync().Result;
                    var keywords = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonData).FirstOrDefault();
                    var max = this._vegetableCompositionRepository.GetMaxStt(s => s.Id) + 1;
                    foreach (var value in keywords.Value)
                    {
                        this._keywordRepository.Add(new Keyword
                        {
                            KeywordName = value,
                            VegCompositionId = max,
                            AccountId = account.Id
                        });
                    }
                }

            }
            else
            {
                var vegDetailName = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = newItem.Title,
                    VegetableCompositionId = 1,
                });
                var vegName = this._vegetableRepository.Add(new Vegetable
                {
                    No = num,
                    Quantity = newItem.Quantity,
                    GardenId = newItem.GardenId,
                    VegetableDescriptionId = vegDetailName.Id
                });
                var vegDetailDes = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = newItem.Description,
                    VegetableCompositionId = 2,
                });
                var vegDes = this._vegetableRepository.Add(new Vegetable
                {
                    No = num,
                    Quantity = newItem.Quantity,
                    GardenId = newItem.GardenId,
                    VegetableDescriptionId = vegDetailDes.Id
                });
                var vegDetailFeature = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = newItem.Featture,
                    VegetableCompositionId = 3,
                });
                var vegFeature = this._vegetableRepository.Add(new Vegetable
                {
                    No = num,
                    Quantity = newItem.Quantity,
                    GardenId = newItem.GardenId,
                    VegetableDescriptionId = vegDetailFeature.Id
                });
                var vegDetailImg = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = "Image",
                    VegetableCompositionId = 4,
                });
                var vegImg = this._vegetableRepository.Add(new Vegetable
                {
                    No = num,
                    Quantity = newItem.Quantity,
                    GardenId = newItem.GardenId,
                    VegetableDescriptionId = vegDetailImg.Id
                });
                if (newItem.NewImages != null)
                {
                    foreach (IFormFile img in newItem.NewImages)
                    {
                        this._vegetableImageService.UploadImage(img, vegDetailImg.Id, savePath, url);
                    }
                }
                newItem.IdName = vegName.Id;
                newItem.IdDescription = vegDes.Id;
                newItem.IdFeature = vegFeature.Id;
                newItem.IdImage = vegImg.Id;
                HttpClient client = _factory.CreateClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(uri + "/keywords"),
                    Content = new StringContent(JsonConvert.SerializeObject(new KeywordsRequestModel { text = vegDetailFeature.VegContent}), Encoding.UTF8, "application/json"),
                };
                var response = client.SendAsync(request).Result;
                if (response.IsSuccessStatusCode)
                {
                    var comp = this._vegetableCompositionRepository.Add(new VegetableComposition
                    {
                        CompositionName = vegDetailName.VegContent,
                        VegetableDescriptionId = vegDetailName.Id
                    });
                    var max = this._vegetableCompositionRepository.GetMaxStt(s => s.Id) + 1;
                    string jsonData = response.Content.ReadAsStringAsync().Result;
                    var keywords = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonData).FirstOrDefault();
                    foreach (var value in keywords.Value)
                    {
                        this._keywordRepository.Add(new Keyword
                        {
                            KeywordName = value,
                            VegCompositionId = max
                        });
                    }
                }
            }

            this._vegetableDescriptionRepository.Commit();
            return newItem;
        }

        public void Delete(int noVeg, int gardenId)
        {
            var result = this._vegetableRepository.GetMulti(s => s.GardenId == gardenId && s.No == noVeg, new string[] { "VegetableDescription" });
            foreach (var item in result)
            {
                this._vegetableRepository.Delete(item.Id);
            }
            this._vegetableRepository.Commit();
        }

        public VegetableResponseModel Get(int noVeg, int gardenId)
        {
            var result = this._vegetableRepository.GetMulti(s => s.No == noVeg && s.GardenId == gardenId, new string[] { "VegetableDescription" });
            var name = result.Where(s => s.VegetableDescription.VegetableCompositionId == 1);
            var des = result.Where(s => s.VegetableDescription.VegetableCompositionId == 2);
            var feat = result.Where(s => s.VegetableDescription.VegetableCompositionId == 3);
            var img = result.Where(s => s.VegetableDescription.VegetableCompositionId == 4);
            return new VegetableResponseModel
            {
                No = noVeg,
                Name = name.Count() > 0 ? name.FirstOrDefault().VegetableDescription.VegContent : "",
                Description = des.Count() > 0 ? des.FirstOrDefault().VegetableDescription.VegContent : "",
                Feature = feat.Count() > 0 ? feat.FirstOrDefault().VegetableDescription.VegContent : "",
                Images = img.Count() > 0 ? this._vegetableImageService.Get(img.FirstOrDefault().VegetableDescription.Id).ToList() : null,
                IdName = name.Count() > 0 ? name.FirstOrDefault().VegetableDescription.Id : "",
                IdDescription = des.Count() > 0 ? des.FirstOrDefault().VegetableDescription.Id : "",
                IdFeature = feat.Count() > 0 ? feat.FirstOrDefault().VegetableDescription.Id : "",
                IdImage = img.Count() > 0 ? img.FirstOrDefault().VegetableDescription.Id : "",
            };
        }

        public VegetableResponseModel Get(string Id)
        {
            var veg = this._vegetableRepository.GetSingle(s => s.Id == Id);
            var result = this._vegetableRepository.GetMulti(s => s.No == veg.No && s.GardenId == veg.GardenId, new string[] { "VegetableDescription" });
            var name = result.Where(s => s.VegetableDescription.VegetableCompositionId == 1).ToList();
            var des = result.Where(s => s.VegetableDescription.VegetableCompositionId == 2).ToList();
            var feat = result.Where(s => s.VegetableDescription.VegetableCompositionId == 3).ToList();
            var img = result.Where(s => s.VegetableDescription.VegetableCompositionId == 4).ToList();
            return new VegetableResponseModel
            {
                No = veg.No,
                Name = name.Count() > 0 ? name.FirstOrDefault().VegetableDescription.VegContent : "",
                Description = des.Count() > 0 ? des.FirstOrDefault().VegetableDescription.VegContent : "",
                Feature = feat.Count() > 0 ? feat.FirstOrDefault().VegetableDescription.VegContent : "",
                Images = img.Count() > 0 ? this._vegetableImageService.Get(img.FirstOrDefault().VegetableDescription.Id).ToList() : null,
                IdName = name.Count() > 0 ? name.FirstOrDefault().Id : "",
                IdDescription = des.Count() > 0 ? des.FirstOrDefault().Id : "",
                IdFeature = feat.Count() > 0 ? feat.FirstOrDefault().Id : "",
                IdImage = img.Count() > 0 ? img.FirstOrDefault().Id : "",
            };
        }

        public IEnumerable<VegetableResponseModel> GetByGardenId(int GardenId)
        {
            List<VegetableResponseModel> vegetableResponseModels = new List<VegetableResponseModel>();
            List <VegetableImage> vegetableImages = new List<VegetableImage>();
            var result = this._vegetableRepository.GetMulti(s => s.GardenId == GardenId, new string[] { "VegetableDescription" }).OrderBy(s => s.No);
            for (int i = 0; i < result.Max(s => s.No); i++)
            {
                var name = result.Where(s => s.No == (i + 1)).Where(s => s.VegetableDescription.VegetableCompositionId == 1);
                var des = result.Where(s => s.No == (i + 1)).Where(s => s.VegetableDescription.VegetableCompositionId == 2);
                var feat = result.Where(s => s.No == (i + 1)).Where(s => s.VegetableDescription.VegetableCompositionId == 3);
                var img = result.Where(s => s.No == (i + 1)).Where(s => s.VegetableDescription.VegetableCompositionId == 4);
                vegetableResponseModels.Add(new VegetableResponseModel
                {
                    No = i + 1,
                    Name = name.Count() > 0 ? name.FirstOrDefault().VegetableDescription.VegContent : "",
                    Description = des.Count() > 0 ? des.FirstOrDefault().VegetableDescription.VegContent : "",
                    Feature = feat.Count() > 0 ? feat.FirstOrDefault().VegetableDescription.VegContent : "",
                    Images = img.Count() > 0 ? this._vegetableImageService.Get(img.FirstOrDefault().VegetableDescription.Id).ToList() : null,
                    IdName = name.Count() > 0 ? name.FirstOrDefault().Id : "",
                    IdDescription = des.Count() > 0 ? des.FirstOrDefault().Id : "",
                    IdFeature = feat.Count() > 0? feat.FirstOrDefault().Id : "",
                    IdImage = img.Count() > 0 ? img.FirstOrDefault().Id : "",
                }); 
            }
            return vegetableResponseModels;
        }

        public IEnumerable<VegetableResponseModel> SearchByDescription(string searchValue)
        {
            List<VegetableResponseModel> vegetableResponseModels = new List<VegetableResponseModel>();
            var listId = this._vegetableDescriptionRepository.SearchByDescription(searchValue).ToList();
            foreach (var item in listId)
            {
                vegetableResponseModels.Add(this.Get(item));
            }
            return vegetableResponseModels.OrderBy(s => s.No);
        }

        public IEnumerable<VegetableResponseModel> SearchByKeyword(string keyword)
        {
            List<VegetableResponseModel> vegetableResponseModels = new List<VegetableResponseModel>();
            var key = this._keywordRepository.GetMulti(s => s.KeywordName.Contains(keyword)).Select(s => s.VegCompositionId).ToArray();
            if (key.Length > 0)
            {
                var desids = this._vegetableCompositionRepository.GetMulti(s => key.Contains(s.Id)).Select(s => s.VegetableDescriptionId).ToArray();
                var result = this._vegetableRepository.GetMulti(s => desids.Contains(s.VegetableDescriptionId)).Select(s => s.Id).ToArray();

                if (true)
                {
                    foreach (var item in result)
                    {
                        vegetableResponseModels.Add(this.Get(item));
                    }
                }
            }
            else
            {
                HttpClient client = _factory.CreateClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(uri + "/searchplant"),
                    Content = new StringContent(JsonConvert.SerializeObject(new KeywordsRequestModel { text = keyword }), Encoding.UTF8, "application/json"),
                };
                var response = client.SendAsync(request).Result;
                if (response.IsSuccessStatusCode)
                {
                    string jsonData = response.Content.ReadAsStringAsync().Result;
                    var keywords = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonData).FirstOrDefault();
                    if (keywords.Value.Count > 0)
                    {
                        var desids = this._vegetableDescriptionRepository.GetMulti(s => s.VegetableCompositionId == 1 && keywords.Value.Contains(s.VegContent)).Select(s => s.Id).ToArray();
                        var result = this._vegetableRepository.GetMulti(s => desids.Contains(s.VegetableDescriptionId)).Select(s => s.Id).ToArray();

                        if (true)
                        {
                            foreach (var item in result)
                            {
                                vegetableResponseModels.Add(this.Get(item));
                            }
                        }
                    }
                }
            }
            return vegetableResponseModels;
        }

        public IEnumerable<VegetableResponseModel> SearchByName(string searchValue)
        {
            List<VegetableResponseModel> vegetableResponseModels = new List<VegetableResponseModel>();
            var listId = this._vegetableDescriptionRepository.SearchByName(searchValue).ToList();
            foreach (var item in listId)
            {
                vegetableResponseModels.Add(this.Get(item));
            }
            if (vegetableResponseModels.Count > 0)
            {
                return vegetableResponseModels.OrderBy(s => s.No);
            }
            else
            {
                return this._wikiService.LeakInfoFromWikiByTitle(searchValue);
            }
            
        }

        public VegetableRequestModel Update(VegetableRequestModel newItem, string phoneNumber, string savePath, string url)
        {
            int num = 1;
            var account = this._acccountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
            var item = this._vegetableRepository.GetMulti(s => s.GardenId == newItem.GardenId);
            if (item != null)
            {
                num = item.Select(s => s.No).Max() + 1;
            }
            var vegDetailDes = this._vegetableDescriptionRepository.GetSingle(s => s.Id == newItem.IdDescription);
            var vegDetailFeature = this._vegetableDescriptionRepository.GetSingle(s => s.Id == newItem.IdFeature);
            var vegDetailImg = this._vegetableDescriptionRepository.GetSingle(s => s.Id == newItem.IdFeature);
            if (newItem.NewFeatture != "")
            {
                if (vegDetailFeature.AccountId != "")
                {
                    vegDetailFeature.VegContent = newItem.NewFeatture;
                    this._vegetableDescriptionRepository.Update(vegDetailFeature);
                    if (newItem.Images != null)
                {
                    foreach (IFormFile img in newItem.Images)
                    {
                        this._vegetableImageService.UploadImage(img, vegDetailImg.Id, savePath, url);
                    }
                }
                }
                else
                {
                    this._vegetableDescriptionRepository.Add(new VegetableDescription
                    {
                        VegContent = newItem.NewFeatture,
                        AccountId = account.Id,
                        VegetableCompositionId = 3
                    });
                }
                vegDetailDes.VegContent = newItem.Description;
                this._vegetableDescriptionRepository.Update(vegDetailDes);
            }
            this._vegetableDescriptionRepository.Commit();
            return newItem;
        }

    }
}

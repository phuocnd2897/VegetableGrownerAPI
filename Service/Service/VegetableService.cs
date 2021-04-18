using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using VG.Common.Helper;
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
        void Delete(string Id);
        public VegetableResponseModel Get(string Id);
        public IEnumerable<VegetableResponseModel> GetByGardenId(int GardenId);
        public IEnumerable<VegetableResponseModel> SearchByDescription(string searchValue);
        public IEnumerable<VegetableResponseModel> SearchByName(string searchValue);
        public IEnumerable<VegetableResponseModel> SearchByKeyword(string keyword);
        public IEnumerable<VegetableResponseModel> CheckVegetableInGarden(string Id, string Name, string PhoneNumber);
        public IEnumerable<SelectedResponseModel> GetAllVegetable();
        public IEnumerable<VegetableResponseModel> GetAllVegetableUnapproved();
        public void IsAccept(string Id, int status);
        public IEnumerable<VegetableResponseModel> SearchVegetableSharedByDescription(string searchValue);
        public IEnumerable<VegetableResponseModel> SearchVegetableSharedByName(string searchValue);
        public IEnumerable<VegetableResponseModel> SearchVegetableSharedByKeyword(string keyword);
    }
    public class VegetableService : IVegetableService
    {
        private IVegetableRepository _vegetableRepository;
        private IVegetableDescriptionRepository _vegetableDescriptionRepository;
        private IVegetableCompositionRepository _vegetableCompositionRepository;
        private IVegetableImageService _vegetableImageService;
        private IShareDetailRepository _shareDetailRepository;
        private IAccountRepository _acccountRepository;
        private IWikiService _wikiService;
        private IKeywordRepository _keywordRepository;
        private ILabelRepository _labelRepository;
        private IHttpClientFactory _factory;
        private const string uri = "https://extractkeywords.herokuapp.com";
        public VegetableService(IVegetableRepository vegetableRepository, IVegetableDescriptionRepository vegetableDescriptionRepository,
            IVegetableCompositionRepository vegetableCompositionRepository, IVegetableImageService vegetableImageService, IShareDetailRepository shareDetailRepository,
            IAccountRepository acccountRepository, IWikiService wikiService, IKeywordRepository keywordRepository, IHttpClientFactory factory, ILabelRepository labelRepository)
        {
            _vegetableRepository = vegetableRepository;
            _vegetableDescriptionRepository = vegetableDescriptionRepository;
            _vegetableCompositionRepository = vegetableCompositionRepository;
            _vegetableImageService = vegetableImageService;
            _shareDetailRepository = shareDetailRepository;
            _acccountRepository = acccountRepository;
            _wikiService = wikiService;
            _keywordRepository = keywordRepository;
            _labelRepository = labelRepository;
            _factory = factory;
        }
        public VegetableRequestModel Add(VegetableRequestModel newItem, string phoneNumber, string savePath, string url)
        {
            int num = 1;
            var item = this._vegetableRepository.GetMulti(s => s.GardenId == newItem.GardenId);
            var account = this._acccountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
            if (item.Count() > 0)
            {
                num = item.Select(s => s.No).Max() + 1;
            }

            if (newItem.IdDescription != null && newItem.IdDescription != "")
            {
                if (account == null)
                {
                    throw new Exception("Có lỗi xảy ra. Vui lòng thử lại");
                }
                if (newItem.IsFixed)
                {
                    var vegDetailFeature = this._vegetableDescriptionRepository.GetSingle(s => s.VegDesCommonId == newItem.IdDescription && s.VegetableCompositionId == 3);
                    var vegName = this._vegetableDescriptionRepository.Add(new VegetableDescription
                    {
                        VegContent = newItem.Title,
                        VegetableCompositionId = 1,
                        Status = true,
                        AccountId = account.Id
                    });
                    vegName.VegDesCommonId = vegName.Id;
                    var vegDescription = this._vegetableDescriptionRepository.Add(new VegetableDescription
                    {
                        VegContent = newItem.Description,
                        VegetableCompositionId = 2,
                        Status = true,
                        VegDesCommonId = vegName.VegDesCommonId,
                        AccountId = account.Id
                    });
                    var vegFeature = this._vegetableDescriptionRepository.Add(new VegetableDescription
                    {
                        VegContent = newItem.Featture,
                        VegetableCompositionId = 3,
                        Status = true,
                        VegDesCommonId = vegName.VegDesCommonId,
                        AccountId = account.Id
                    });
                    var vegImage = this._vegetableDescriptionRepository.Add(new VegetableDescription
                    {
                        VegContent = "Image",
                        VegetableCompositionId = 4,
                        Status = true,
                        VegDesCommonId = vegName.VegDesCommonId,
                        AccountId = account.Id
                    });
                    var veg = this._vegetableRepository.Add(new Vegetable
                    {
                        No = num,
                        Quantity = newItem.Quantity,
                        GardenId = newItem.GardenId,
                        VegetableDescriptionId = vegName.Id
                    });
                    if (newItem.NewImages != null)
                    {
                        foreach (IFormFile img in newItem.NewImages)
                        {
                            this._vegetableImageService.UploadImage(img, vegImage.Id, savePath, url, account.Id);
                        }
                    }
                    newItem.Id = veg.Id;
                    if (vegDetailFeature != null)
                    {
                        if (vegDetailFeature.VegContent != newItem.Featture)
                        {
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
                                string jsonData = response.Content.ReadAsStringAsync().Result;
                                var newkeywords = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonData).FirstOrDefault();
                                var oldKeywords = this._vegetableCompositionRepository.GetSingle(s => s.CompositionName == newItem.Title, new string[] { "Keyword" });
                                var keywords = newkeywords.Value.Except(oldKeywords.Keywords.Select(s => s.KeywordName));
                                if (keywords.Count() > 0)
                                {
                                    foreach (var value in keywords)
                                    {
                                        this._keywordRepository.Add(new Keyword
                                        {
                                            KeywordName = value,
                                            VegCompositionId = oldKeywords.Id,
                                            AccountId = account.Id
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    var vegImage = this._vegetableDescriptionRepository.Add(new VegetableDescription
                    {
                        VegContent = "Image",
                        VegetableCompositionId = 4,
                        Status = true,
                        VegDesCommonId = newItem.IdDescription,
                        AccountId = account.Id
                    });
                    var veg = this._vegetableRepository.Add(new Vegetable
                    {
                        No = num,
                        Quantity = newItem.Quantity,
                        GardenId = newItem.GardenId,
                        VegetableDescriptionId = newItem.IdDescription
                    });
                    if (newItem.NewImages != null)
                    {
                        foreach (IFormFile img in newItem.NewImages)
                        {
                            this._vegetableImageService.UploadImage(img, vegImage.Id, savePath, url, account.Id);
                        }
                    }
                    newItem.Id = veg.Id;
                }
            }
            else
            {
                if (newItem.SynonymOfFeature != "" && newItem.SynonymOfFeature != null)
                {
                    if (this._labelRepository.GetMulti(s => s.LabelName == newItem.SynonymOfFeature).Count() > 0)
                    {
                        this._labelRepository.Add(new Label
                        {
                            LabelName = newItem.SynonymOfFeature,
                            VegCompositionId = 3,
                            StandsFor = "CD"
                        });
                    }
                }
                if (newItem.NameSearch != "" && newItem.NameSearch != null)
                {
                    var ListVegName = this._labelRepository.GetMulti(s => s.LabelName == newItem.Title);
                    if (ListVegName.Count() > 0)
                    {

                        this._labelRepository.Add(new Label
                        {
                            LabelName = newItem.NameSearch,
                            StandsFor = "VEG",
                            VegCompositionId = ListVegName.FirstOrDefault().VegCompositionId
                        });
                        var composition = this._vegetableCompositionRepository.GetSingle(s => s.Id == ListVegName.FirstOrDefault().VegCompositionId, new string[] { "VegetableDescription" });
                        if (composition != null && (composition.VegetableDescription.AccountId == null || composition.VegetableDescription.AccountId == ""))
                        {
                            var veg = this._vegetableRepository.Add(new Vegetable
                            {
                                No = num,
                                Quantity = newItem.Quantity,
                                GardenId = newItem.GardenId,
                                VegetableDescriptionId = composition.VegetableDescriptionId
                            });
                        }
                    }
                    else
                    {
                        var vegNamePending = this._vegetableDescriptionRepository.Add(new VegetableDescription
                        {
                            VegContent = newItem.Title,
                            VegetableCompositionId = 1,
                            Status = false,
                        });
                        vegNamePending.VegDesCommonId = vegNamePending.Id;
                        var vegDescriptionPending = this._vegetableDescriptionRepository.Add(new VegetableDescription
                        {
                            VegContent = newItem.Description,
                            VegetableCompositionId = 2,
                            Status = false,
                            VegDesCommonId = vegNamePending.VegDesCommonId
                        });
                        var vegFeaturePending = this._vegetableDescriptionRepository.Add(new VegetableDescription
                        {
                            VegContent = newItem.Featture,
                            VegetableCompositionId = 3,
                            Status = false,
                            VegDesCommonId = vegNamePending.VegDesCommonId
                        });
                        var vegImagePending = this._vegetableDescriptionRepository.Add(new VegetableDescription
                        {
                            VegContent = "Image",
                            VegetableCompositionId = 4,
                            Status = false,
                            VegDesCommonId = vegNamePending.VegDesCommonId
                        });
                        if (newItem.NewImages != null)
                        {
                            foreach (IFormFile img in newItem.NewImages)
                            {
                                this._vegetableImageService.UploadImage(img, vegImagePending.Id, savePath, url, "");
                            }
                        }
                        else if (newItem.Images != null)
                        {
                            this._vegetableImageService.Add(newItem.Images, vegImagePending.Id, "");

                        }
                        var vegName = this._vegetableDescriptionRepository.Add(new VegetableDescription
                        {
                            VegContent = newItem.Title,
                            VegetableCompositionId = 1,
                            Status = false,
                            AccountId = account.Id
                        });
                        vegName.VegDesCommonId = vegName.Id;
                        var vegDescription = this._vegetableDescriptionRepository.Add(new VegetableDescription
                        {
                            VegContent = newItem.Description,
                            VegetableCompositionId = 2,
                            Status = false,
                            VegDesCommonId = vegName.VegDesCommonId,
                            AccountId = account.Id
                        });
                        var vegFeature = this._vegetableDescriptionRepository.Add(new VegetableDescription
                        {
                            VegContent = newItem.Featture,
                            VegetableCompositionId = 3,
                            Status = false,
                            VegDesCommonId = vegName.VegDesCommonId,
                            AccountId = account.Id
                        });
                        var vegImage = this._vegetableDescriptionRepository.Add(new VegetableDescription
                        {
                            VegContent = "Image",
                            VegetableCompositionId = 4,
                            Status = false,
                            VegDesCommonId = vegName.VegDesCommonId,
                            AccountId = account.Id
                        });
                        var veg = this._vegetableRepository.Add(new Vegetable
                        {
                            No = num,
                            Quantity = newItem.Quantity,
                            GardenId = newItem.GardenId,
                            VegetableDescriptionId = vegName.Id
                        });
                        if (newItem.NewImages != null)
                        {
                            foreach (IFormFile img in newItem.NewImages)
                            {
                                this._vegetableImageService.UploadImage(img, vegImage.Id, savePath, url, account.Id);
                            }
                        }
                        else if (newItem.Images != null)
                        {
                            this._vegetableImageService.Add(newItem.Images, vegImage.Id, account.Id);

                        }
                        newItem.Id = veg.Id;
                        newItem.IdDescription = vegName.VegDesCommonId;
                        var comName = this._vegetableCompositionRepository.Add(new VegetableComposition
                        {
                            CompositionName = vegName.VegContent,
                            VegetableDescriptionId = vegName.VegDesCommonId,
                        });

                        HttpClient client = _factory.CreateClient();
                        var request = new HttpRequestMessage
                        {
                            Method = HttpMethod.Get,
                            RequestUri = new Uri(uri + "/keywords"),
                            Content = new StringContent(JsonConvert.SerializeObject(new KeywordsRequestModel { text = vegFeature.VegContent }), Encoding.UTF8, "application/json"),
                        };
                        var response = client.SendAsync(request).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var comKey = this._vegetableCompositionRepository.Add(new VegetableComposition
                            {
                                CompositionName = vegName.VegContent,
                                VegetableDescriptionId = vegFeature.Id,
                            });
                            this._vegetableCompositionRepository.Commit();
                            string jsonData = response.Content.ReadAsStringAsync().Result;
                            var newkeywords = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonData).FirstOrDefault();
                            if (newkeywords.Value.Count() > 0)
                            {
                                foreach (var value in newkeywords.Value)
                                {
                                    this._keywordRepository.Add(new Keyword
                                    {
                                        KeywordName = value,
                                        VegCompositionId = comKey.Id,
                                        AccountId = account.Id
                                    });
                                }
                            }
                        }
                        this._labelRepository.Add(new Label
                        {
                            LabelName = newItem.NameSearch,
                            VegCompositionId = comName.Id,
                        });
                    }
                }
                else
                {
                        var vegNamePending = this._vegetableDescriptionRepository.Add(new VegetableDescription
                        {
                            VegContent = newItem.Title,
                            VegetableCompositionId = 1,
                            Status = false,
                        });
                        vegNamePending.VegDesCommonId = vegNamePending.Id;
                        var vegDescriptionPending = this._vegetableDescriptionRepository.Add(new VegetableDescription
                        {
                            VegContent = newItem.Description,
                            VegetableCompositionId = 2,
                            Status = false,
                            VegDesCommonId = vegNamePending.VegDesCommonId
                        });
                        var vegFeaturePending = this._vegetableDescriptionRepository.Add(new VegetableDescription
                        {
                            VegContent = newItem.Featture,
                            VegetableCompositionId = 3,
                            Status = false,
                            VegDesCommonId = vegNamePending.VegDesCommonId
                        });
                        var vegImagePending = this._vegetableDescriptionRepository.Add(new VegetableDescription
                        {
                            VegContent = "Image",
                            VegetableCompositionId = 4,
                            Status = false,
                            VegDesCommonId = vegNamePending.VegDesCommonId
                        });
                        if (newItem.NewImages != null)
                        {
                            foreach (IFormFile img in newItem.NewImages)
                            {
                                this._vegetableImageService.UploadImage(img, vegImagePending.Id, savePath, url, "");
                            }
                        }
                        else if (newItem.Images != null)
                        {
                            this._vegetableImageService.Add(newItem.Images, vegImagePending.Id, "");

                        }
                        var vegName = this._vegetableDescriptionRepository.Add(new VegetableDescription
                        {
                            VegContent = newItem.Title,
                            VegetableCompositionId = 1,
                            Status = false,
                            AccountId = account.Id
                        });
                        vegName.VegDesCommonId = vegName.Id;
                        var vegDescription = this._vegetableDescriptionRepository.Add(new VegetableDescription
                        {
                            VegContent = newItem.Description,
                            VegetableCompositionId = 2,
                            Status = false,
                            VegDesCommonId = vegName.VegDesCommonId,
                            AccountId = account.Id
                        });
                        var vegFeature = this._vegetableDescriptionRepository.Add(new VegetableDescription
                        {
                            VegContent = newItem.Featture,
                            VegetableCompositionId = 3,
                            Status = false,
                            VegDesCommonId = vegName.VegDesCommonId,
                            AccountId = account.Id
                        });
                        var vegImage = this._vegetableDescriptionRepository.Add(new VegetableDescription
                        {
                            VegContent = "Image",
                            VegetableCompositionId = 4,
                            Status = false,
                            VegDesCommonId = vegName.VegDesCommonId,
                            AccountId = account.Id
                        });
                        var veg = this._vegetableRepository.Add(new Vegetable
                        {
                            No = num,
                            Quantity = newItem.Quantity,
                            GardenId = newItem.GardenId,
                            VegetableDescriptionId = vegName.Id
                        });
                        if (newItem.NewImages != null)
                        {
                            foreach (IFormFile img in newItem.NewImages)
                            {
                                this._vegetableImageService.UploadImage(img, vegImage.Id, savePath, url, account.Id);
                            }
                        }
                        else if (newItem.Images != null)
                        {
                            this._vegetableImageService.Add(newItem.Images, vegImage.Id, account.Id);

                        }
                        newItem.Id = veg.Id;
                        newItem.IdDescription = vegName.VegDesCommonId;
                        var comName = this._vegetableCompositionRepository.Add(new VegetableComposition
                        {
                            CompositionName = vegNamePending.VegContent,
                            VegetableDescriptionId = vegNamePending.VegDesCommonId,
                        });

                        HttpClient client = _factory.CreateClient();
                        var request = new HttpRequestMessage
                        {
                            Method = HttpMethod.Get,
                            RequestUri = new Uri(uri + "/keywords"),
                            Content = new StringContent(JsonConvert.SerializeObject(new KeywordsRequestModel { text = vegFeature.VegContent }), Encoding.UTF8, "application/json"),
                        };
                        var response = client.SendAsync(request).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var comKey = this._vegetableCompositionRepository.Add(new VegetableComposition
                            {
                                CompositionName = vegNamePending.VegContent,
                                VegetableDescriptionId = vegFeaturePending.Id,
                            });
                            this._vegetableCompositionRepository.Commit();
                            string jsonData = response.Content.ReadAsStringAsync().Result;
                            var newkeywords = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonData).FirstOrDefault();
                            if (newkeywords.Value.Count() > 0)
                            {
                                foreach (var value in newkeywords.Value)
                                {
                                    this._keywordRepository.Add(new Keyword
                                    {
                                        KeywordName = value,
                                        VegCompositionId = comKey.Id,
                                    });
                                }
                            }
                        }
                        this._labelRepository.Add(new Label
                        {
                            LabelName = newItem.Title,
                            VegCompositionId = comName.Id,
                        });
                    
                }
            }

            this._vegetableDescriptionRepository.Commit();
            return newItem;
        }

        public void Delete(string Id)
        {
            var result = this._vegetableRepository.GetSingle(s => s.Id == Id, new string[] { "ShareDetails" });
            var details = this._vegetableDescriptionRepository.GetMulti(s => s.VegDesCommonId == result.VegetableDescriptionId && (s.AccountId != null && s.AccountId != ""), new string[] { "VegetableImages" }).Distinct().ToList();
            if (result.ShareDetails.Count > 0)
            {
                throw new Exception("Vui lòng xoá bài viết trước!");
            }
            this._vegetableRepository.Delete(result);

            foreach (var des in details)
            {
                var composition = this._vegetableCompositionRepository.GetMulti(s => s.VegetableDescriptionId == des.Id, new string[] { "Keywords", "Labels" });
                this._vegetableCompositionRepository.Delete(composition);
                if (des.VegetableCompositionId == 4)
                {
                    foreach (var img in des.VegetableImages.Where(s => s.AccountId != "" && s.AccountId != null))
                    {
                        this._vegetableImageService.DeleteByIdImg(img);
                    }
                }
                if (des.AccountId != "" && des.AccountId != null)
                {
                    this._vegetableDescriptionRepository.Delete(des);
                }
            }
            this._vegetableRepository.Commit();
        }
        public VegetableResponseModel Get(string Id)
        {
            var veg = this._vegetableRepository.GetSingle(s => s.Id == Id);
            var result = this._vegetableDescriptionRepository.GetMulti(s => s.VegDesCommonId == veg.VegetableDescriptionId);
            var name = result.Where(s => s.VegetableCompositionId == 1).FirstOrDefault().VegContent;
            var des = result.Where(s => s.VegetableCompositionId == 2).FirstOrDefault().VegContent;
            var feat = result.Where(s => s.VegetableCompositionId == 3).FirstOrDefault().VegContent;
            var img = result.Where(s => s.VegetableCompositionId == 4).FirstOrDefault().Id;
            if (name.Count() == 0 && des.Count() == 0 && feat.Count() == 0 && img.Count() == 0) return null;
            return new VegetableResponseModel
            {
                Id = Id,
                No = veg.No,
                Name = name,
                Description = des,
                Feature = feat,
                Quantity = veg.Quantity,
                Images = this._vegetableImageService.Get(img).ToList(),
                IdDescription = veg.VegetableDescriptionId
            };
        }

        public IEnumerable<VegetableResponseModel> GetByGardenId(int GardenId)
        {
            List<VegetableResponseModel> vegetableResponseModels = new List<VegetableResponseModel>();
            var result = this._vegetableRepository.GetMulti(s => s.GardenId == GardenId, new string[] { "Garden" }).OrderBy(s => s.No);
            if (result.Count() > 0)
            {
                foreach (var item in result)
                {
                    var detail = this._vegetableDescriptionRepository.GetMulti(s => s.VegDesCommonId == item.VegetableDescriptionId);

                    vegetableResponseModels.Add(new VegetableResponseModel
                    {
                        Id = item.Id,
                        No = item.No,
                        Name = detail.Where(s => s.VegetableCompositionId == 1).FirstOrDefault().VegContent,
                        Description = detail.Where(s => s.VegetableCompositionId == 2).FirstOrDefault().VegContent,
                        Feature = detail.Where(s => s.VegetableCompositionId == 3).FirstOrDefault().VegContent,
                        Quantity = item.Quantity,
                        GardenId = item.GardenId,
                        Images = this._vegetableImageService.Get(detail.Where(s => s.VegetableCompositionId == 4).FirstOrDefault().Id).Where(s => s.AccountId != "" && s.AccountId != null).ToList(),
                        IdDescription = item.VegetableDescriptionId
                    });
                }
            }
            return vegetableResponseModels;
        }

        public IEnumerable<VegetableResponseModel> SearchByDescription(string searchValue)
        {
            List<VegetableResponseModel> vegetableResponseModels = new List<VegetableResponseModel>();
            var vegDetails = this._vegetableDescriptionRepository.GetMulti(s => s.VegetableCompositionId == 2 && (s.AccountId == "" || s.AccountId == null)).Distinct().ToList();
            foreach (var detail in vegDetails)
            {
                if (IdentityHelper.RemoveUnicode(detail.VegContent).ToUpper().Trim().Contains(IdentityHelper.RemoveUnicode(searchValue).ToUpper().Trim()))
                {
                    var name = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 1 && s.VegDesCommonId == detail.VegDesCommonId);
                    var des = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 2 && s.VegDesCommonId == detail.VegDesCommonId);
                    var feat = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 3 && s.VegDesCommonId == detail.VegDesCommonId);
                    var img = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 4 && s.VegDesCommonId == detail.VegDesCommonId);
                    vegetableResponseModels.Add(new VegetableResponseModel
                    {
                        Name = name.VegContent,
                        Description = des.VegContent,
                        Feature = feat.VegContent,
                        Images = this._vegetableImageService.Get(img.Id).Where(s => s.AccountId == "" || s.AccountId == null).ToList(),
                        IdDescription = name.Id,
                    });
                }
            }
            return vegetableResponseModels;
        }

        public IEnumerable<VegetableResponseModel> SearchByKeyword(string keyword)
        {
            List<VegetableResponseModel> vegetableResponseModels = new List<VegetableResponseModel>();
            List<string> desids = new List<string>();
            var keys = this._keywordRepository.GetAll().ToList();
            foreach (var key in keys)
            {
                var t = key.KeywordName.Contains('_') ? IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim().Replace("_", string.Empty) : IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim();
                if (key.KeywordName.Contains('_') ?
                    (IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim().Replace("_", string.Empty).Contains(IdentityHelper.RemoveUnicode(keyword).ToUpper().Trim())
                    || IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim().Replace("_", string.Empty) == (IdentityHelper.RemoveUnicode(keyword).ToUpper().Trim()))
                    : IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim().Contains(IdentityHelper.RemoveUnicode(keyword).ToUpper().Trim())
                    || IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim() == IdentityHelper.RemoveUnicode(keyword).ToUpper().Trim())
                {
                    desids.AddRange(this._vegetableCompositionRepository.GetMulti(s => key.VegCompositionId == s.Id && (s.VegetableDescription.AccountId == "" || s.VegetableDescription.AccountId == null), new string[] { "VegetableDescription" }).Select(s => s.VegetableDescriptionId).Distinct().ToList());
                }
            }
            if (desids.Count > 0)
            {
                desids = desids.Distinct().ToList();
                foreach (var item in desids)
                {
                    if (item != null)
                    {
                        var idcommon = this._vegetableDescriptionRepository.GetSingle(s => s.Id == item && s.Status == true).VegDesCommonId;
                        var name = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 1 && s.VegDesCommonId == idcommon);
                        var des = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 2 && s.VegDesCommonId == idcommon);
                        var feat = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 3 && s.VegDesCommonId == idcommon);
                        var img = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 4 && s.VegDesCommonId == idcommon);
                        vegetableResponseModels.Add(new VegetableResponseModel
                        {
                            Name = name.VegContent,
                            Description = des.VegContent,
                            Feature = feat.VegContent,
                            Images = this._vegetableImageService.Get(img.Id).Where(s => s.AccountId == "" || s.AccountId == null).ToList(),
                            IdDescription = name.Id,
                        });
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
                        foreach (var item in keywords.Value)
                        {
                            vegetableResponseModels.AddRange(SearchByName(item));
                        }

                    }
                }
            }
            return vegetableResponseModels;
        }

        public IEnumerable<VegetableResponseModel> SearchByName(string searchValue)
        {
            List<VegetableResponseModel> vegetableResponseModels = new List<VegetableResponseModel>();
            var Labels = this._labelRepository.GetMulti(s => (s.StandsFor == "" || s.StandsFor == null), new string[] { "VegetableComposition" }).ToList();
            foreach (var label in Labels)
            {
                if (IdentityHelper.RemoveUnicode(label.LabelName).ToUpper().Trim().Contains(IdentityHelper.RemoveUnicode(searchValue).ToUpper().Trim()))
                {
                    var vegDetail = this._vegetableDescriptionRepository.GetSingle(s => s.Id == label.VegetableComposition.VegetableDescriptionId);
                    var listId = this._vegetableDescriptionRepository.GetMulti(s => s.VegDesCommonId == vegDetail.VegDesCommonId).Select(s => s.VegDesCommonId).Distinct().ToList();
                    foreach (var item in listId)
                    {
                        var name = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 1 && s.VegDesCommonId == item);
                        var des = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 2 && s.VegDesCommonId == item);
                        var feat = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 3 && s.VegDesCommonId == item);
                        var img = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 4 && s.VegDesCommonId == item, new string[] { "VegetableImages" });
                        vegetableResponseModels.Add(new VegetableResponseModel
                        {
                            Name = name.VegContent,
                            Description = des.VegContent,
                            Feature = feat.VegContent,
                            Images = img.VegetableImages.Where(s => s.AccountId == "" || s.AccountId == null).ToList(),
                            IdDescription = name.Id,
                        });
                    }
                }
            }
            return vegetableResponseModels;
        }

        public VegetableRequestModel Update(VegetableRequestModel newItem, string phoneNumber, string savePath, string url)
        {
            var account = this._acccountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
            var veg = this._vegetableRepository.GetSingle(s => s.Id == newItem.Id);
            var detail = this._vegetableDescriptionRepository.GetMulti(s => s.VegDesCommonId == veg.VegetableDescriptionId);
            veg.Quantity = newItem.Quantity;
            if (detail.Where(s => s.VegetableCompositionId == 1).FirstOrDefault().AccountId != null)
            {
                detail.Where(s => s.VegetableCompositionId == 3).FirstOrDefault().VegContent = newItem.Featture;
                detail.Where(s => s.VegetableCompositionId == 2).FirstOrDefault().VegContent = newItem.Description;
                if (newItem.NewImages != null)
                {
                    var idImg = detail.Where(s => s.VegetableCompositionId == 2).FirstOrDefault().Id;
                    this._vegetableImageService.Delete(idImg);
                    foreach (IFormFile img in newItem.NewImages)
                    {
                        this._vegetableImageService.UploadImage(img, idImg, savePath, url, account.Id);
                    }

                }
                this._vegetableRepository.Update(veg);
                this._vegetableDescriptionRepository.Update(detail);
                this._vegetableDescriptionRepository.Commit();
            }
            else
            {
                int num = 1;
                var item = this._vegetableRepository.GetMulti(s => s.GardenId == newItem.GardenId);
                if (item.Count() > 0)
                {
                    num = item.Select(s => s.No).Max() + 1;
                }
                var vegDetailFeature = this._vegetableDescriptionRepository.GetSingle(s => s.VegDesCommonId == newItem.IdDescription && s.VegetableCompositionId == 3);
                var vegName = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = newItem.Title,
                    VegetableCompositionId = 1,
                    Status = true,
                    AccountId = account.Id
                });
                vegName.VegDesCommonId = vegName.Id;
                var vegDescription = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = newItem.Description,
                    VegetableCompositionId = 2,
                    Status = true,
                    VegDesCommonId = vegName.VegDesCommonId,
                    AccountId = account.Id
                });
                var vegFeature = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = newItem.Featture,
                    VegetableCompositionId = 3,
                    Status = true,
                    VegDesCommonId = vegName.VegDesCommonId,
                    AccountId = account.Id
                });
                var vegImage = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = "Image",
                    VegetableCompositionId = 4,
                    Status = true,
                    VegDesCommonId = vegName.VegDesCommonId,
                    AccountId = account.Id
                });
                var vegetable = this._vegetableRepository.Add(new Vegetable
                {
                    No = num,
                    Quantity = newItem.Quantity,
                    GardenId = newItem.GardenId,
                    VegetableDescriptionId = vegName.Id
                });
                if (newItem.NewImages != null)
                {
                    foreach (IFormFile img in newItem.NewImages)
                    {
                        this._vegetableImageService.UploadImage(img, vegImage.Id, savePath, url, account.Id);
                    }
                }
                newItem.Id = vegetable.Id;
                if (vegDetailFeature != null)
                {
                    if (vegDetailFeature.VegContent != newItem.Featture)
                    {
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
                            string jsonData = response.Content.ReadAsStringAsync().Result;
                            var newkeywords = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonData).FirstOrDefault();
                            var oldKeywords = this._vegetableCompositionRepository.GetSingle(s => s.CompositionName == newItem.Title, new string[] { "Keyword" });
                            var keywords = newkeywords.Value.Except(oldKeywords.Keywords.Select(s => s.KeywordName));
                            if (keywords.Count() > 0)
                            {
                                foreach (var value in keywords)
                                {
                                    this._keywordRepository.Add(new Keyword
                                    {
                                        KeywordName = value,
                                        VegCompositionId = oldKeywords.Id,
                                        AccountId = account.Id
                                    });
                                }
                            }
                        }
                    }
                }
            }
            return newItem;
        }

        public IEnumerable<VegetableResponseModel> CheckVegetableInGarden(string Id, string Name, string PhoneNumber)
        {
            List<VegetableResponseModel> vegetableResponseModels = new List<VegetableResponseModel>();

            var account = this._acccountRepository.GetSingle(s => s.PhoneNumber == PhoneNumber, new string[] { "Gardens" });
            var yourVeg = this._vegetableRepository.GetMulti(s => s.VegetableDescriptionId == Id).ToList();
            if (account.Gardens.Count() > 0)
            {
                if (yourVeg.Count() > 0)
                {
                    foreach (var item in yourVeg)
                    {
                        var vegDetail = this._vegetableDescriptionRepository.GetMulti(s => s.VegDesCommonId == item.VegetableDescriptionId);
                        var name = vegDetail.Where(s => s.VegetableCompositionId == 1).FirstOrDefault().VegContent;
                        var des = vegDetail.Where(s => s.VegetableCompositionId == 2).FirstOrDefault().VegContent;
                        var feat = vegDetail.Where(s => s.VegetableCompositionId == 3).FirstOrDefault().VegContent;
                        var img = vegDetail.Where(s => s.VegetableCompositionId == 4).FirstOrDefault().Id;
                        vegetableResponseModels.Add(new VegetableResponseModel
                        {
                            Id = item.Id,
                            No = item.No,
                            Name = name,
                            Description = des,
                            Feature = feat,
                            Quantity = item.Quantity,
                            GardenId = item.GardenId,
                            Images = this._vegetableImageService.Get(img).ToList(),
                            IdDescription = item.VegetableDescriptionId
                        });
                    }
                }
                else
                {
                    var ListName = this._labelRepository.GetMulti(s => s.VegetableComposition.VegetableDescriptionId == Id, new string[] { "VegetableComposition" }).Distinct().ToList();
                    if (ListName.Count() > 0)
                    {
                        var vegDetailName = this._vegetableDescriptionRepository.GetMulti(s => s.VegetableCompositionId == 1 && account.Id == s.AccountId).ToList();
                        foreach (var label in ListName)
                        {
                            foreach (var item in vegDetailName)
                            {
                                if (IdentityHelper.RemoveUnicode(label.LabelName).ToLower() == IdentityHelper.RemoveUnicode(item.VegContent).ToLower())
                                {
                                    var veg = this._vegetableRepository.GetSingle(s => s.VegetableDescriptionId == item.Id);
                                    var vegDetail = this._vegetableDescriptionRepository.GetMulti(s => s.VegDesCommonId == item.VegDesCommonId);
                                    var name = vegDetail.Where(s => s.VegetableCompositionId == 1).FirstOrDefault().VegContent;
                                    var des = vegDetail.Where(s => s.VegetableCompositionId == 2).FirstOrDefault().VegContent;
                                    var feat = vegDetail.Where(s => s.VegetableCompositionId == 3).FirstOrDefault().VegContent;
                                    var img = vegDetail.Where(s => s.VegetableCompositionId == 4).FirstOrDefault().Id;
                                    vegetableResponseModels.Add(new VegetableResponseModel
                                    {
                                        Id = veg.Id,
                                        No = veg.No,
                                        Name = name,
                                        Description = des,
                                        Feature = feat,
                                        Quantity = veg.Quantity,
                                        GardenId = veg.GardenId,
                                        Images = this._vegetableImageService.Get(img).ToList(),
                                        IdDescription = veg.VegetableDescriptionId
                                    });
                                }
                            }
                        }

                    }
                }
            }
            else
                throw new Exception("Bạn chưa sở hữu bất kì loại rau nào!");
            return vegetableResponseModels.GroupBy(s => s.Id).Select(s => s.FirstOrDefault());
        }

        public IEnumerable<SelectedResponseModel> GetAllVegetable()
        {
            var result = this._vegetableDescriptionRepository.GetMulti(s => s.VegetableCompositionId == 1).Select(s => new SelectedResponseModel
            {
                Id = s.Id,
                Text = s.VegContent
            });
            return result;
        }

        public IEnumerable<VegetableResponseModel> GetAllVegetableUnapproved()
        {
            List<VegetableResponseModel> vegetableResponseModels = new List<VegetableResponseModel>();
            var DesId = this._vegetableDescriptionRepository.GetMulti(s => (s.AccountId == "" || s.AccountId == null) && s.Status == false).Select(s => s.VegDesCommonId).Distinct().ToList();
            foreach (var item in DesId)
            {
                var name = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 1 && s.VegDesCommonId == item);
                var des = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 2 && s.VegDesCommonId == item);
                var feat = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 3 && s.VegDesCommonId == item);
                var img = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 4 && s.VegDesCommonId == item, new string[] { "VegetableImages" });
                vegetableResponseModels.Add(new VegetableResponseModel
                {
                    Name = name.VegContent,
                    Description = des.VegContent,
                    Feature = feat.VegContent,
                    Images = img.VegetableImages.Where(s => s.AccountId == "" || s.AccountId == null).ToList(),
                    IdDescription = name.Id,
                });
            }
            return vegetableResponseModels;
        }

        public void IsAccept(string Id, int status)
        {
            var name = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 1 && s.VegDesCommonId == Id);
            var des = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 2 && s.VegDesCommonId == Id);
            var feat = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 3 && s.VegDesCommonId == Id);
            var img = this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 4 && s.VegDesCommonId == Id, new string[] { "VegetableImages" });
            if (status == 2)
            {
                name.Status = true;
                des.Status = true;
                feat.Status = true;
                img.Status = true;
                this._vegetableDescriptionRepository.Update(name);
                this._vegetableDescriptionRepository.Update(des);
                this._vegetableDescriptionRepository.Update(feat);
                this._vegetableDescriptionRepository.Update(img);
                this._vegetableDescriptionRepository.Commit();
            }
            else
            {
                var idimgs = img.VegetableImages.Where(s => s.AccountId == "" || s.AccountId == null);
                foreach (var item in idimgs)
                {
                    this._vegetableImageService.DeleteByIdImg(item);
                }
                this._vegetableDescriptionRepository.Delete(name);
                this._vegetableDescriptionRepository.Delete(des);
                this._vegetableDescriptionRepository.Delete(feat);
                this._vegetableDescriptionRepository.Delete(img);
            }
        }

        public IEnumerable<VegetableResponseModel> SearchVegetableSharedByDescription(string searchValue)
        {
            List<string> listId = new List<string>();
            List<VegetableResponseModel> vegetableResponseModels = new List<VegetableResponseModel>();
            var listVegetableShared = this._shareDetailRepository.GetMulti(s => s.Quantity > 0).Select(s => s.VegetableId).Distinct().ToList();
            var vegDetails = this._vegetableDescriptionRepository.GetMulti(s => s.VegetableCompositionId == 2 && s.AccountId != "" && s.AccountId != null).Distinct().ToList();
            foreach (var detail in vegDetails)
            {
                if (IdentityHelper.RemoveUnicode(detail.VegContent).ToUpper().Trim().Contains(IdentityHelper.RemoveUnicode(searchValue).ToUpper().Trim()))
                {
                    var vegDetail = this._vegetableDescriptionRepository.GetSingle(s => s.VegDesCommonId == detail.VegDesCommonId && s.VegetableCompositionId == 1, new string[] { "Vegetables" });
                    listId = vegDetail.Vegetables.Select(s => s.Id).ToList();
                    foreach (var item in listId)
                    {
                        if (listVegetableShared.Contains(item))
                        {
                            var result = this._vegetableDescriptionRepository.GetMulti(s => s.VegDesCommonId == vegDetail.VegDesCommonId);
                            var name = result.Where(s => s.VegetableCompositionId == 1).FirstOrDefault().VegContent;
                            var des = result.Where(s => s.VegetableCompositionId == 2).FirstOrDefault().VegContent;
                            var feat = result.Where(s => s.VegetableCompositionId == 3).FirstOrDefault().VegContent;
                            var img = result.Where(s => s.VegetableCompositionId == 4).FirstOrDefault().Id;
                            if (name.Count() == 0 && des.Count() == 0 && feat.Count() == 0 && img.Count() == 0) return null;
                            vegetableResponseModels.Add(new VegetableResponseModel
                            {
                                Name = name,
                                Description = des,
                                Feature = feat,
                                Images = this._vegetableImageService.Get(img).ToList(),
                                IdDescription = vegDetail.VegDesCommonId
                            });
                        }
                    }
                }
            }
            return vegetableResponseModels;
        }

        public IEnumerable<VegetableResponseModel> SearchVegetableSharedByName(string searchValue)
        {
            List<string> listId = new List<string>();
            List<VegetableResponseModel> vegetableResponseModels = new List<VegetableResponseModel>();
            var Labels = this._labelRepository.GetMulti(s => (s.StandsFor == "" || s.StandsFor == null), new string[] { "VegetableComposition" }).ToList();
            var listVegetableShared = this._shareDetailRepository.GetMulti(s => s.Quantity > 0).Select(s => s.VegetableId).Distinct().ToList();
            foreach (var label in Labels)
            {
                if (IdentityHelper.RemoveUnicode(label.LabelName).ToUpper().Trim().Contains(IdentityHelper.RemoveUnicode(searchValue).ToUpper().Trim()))
                {
                    var vegDetail = this._vegetableDescriptionRepository.GetSingle(s => s.Id == label.VegetableComposition.VegetableDescriptionId);
                    listId = this._vegetableDescriptionRepository.GetMulti(s => s.VegContent == vegDetail.VegContent && s.Vegetables.Count() > 0, new string[] { "Vegetables" })
                        .Select(s => s.Vegetables).FirstOrDefault().Select(s => s.Id).Distinct().ToList();
                    foreach (var item in listId)
                    {
                        if (listVegetableShared.Contains(item))
                        {
                            var result = this._vegetableDescriptionRepository.GetMulti(s => s.VegDesCommonId == vegDetail.VegDesCommonId);
                            var name = result.Where(s => s.VegetableCompositionId == 1).FirstOrDefault().VegContent;
                            var des = result.Where(s => s.VegetableCompositionId == 2).FirstOrDefault().VegContent;
                            var feat = result.Where(s => s.VegetableCompositionId == 3).FirstOrDefault().VegContent;
                            var img = result.Where(s => s.VegetableCompositionId == 4).FirstOrDefault().Id;
                            if (name.Count() == 0 && des.Count() == 0 && feat.Count() == 0 && img.Count() == 0) return null;
                            vegetableResponseModels.Add(new VegetableResponseModel
                            {
                                Name = name,
                                Description = des,
                                Feature = feat,
                                Images = this._vegetableImageService.Get(img).ToList(),
                                IdDescription = vegDetail.VegDesCommonId
                            });
                        }
                    }
                }
            }
            return vegetableResponseModels;
        }

        public IEnumerable<VegetableResponseModel> SearchVegetableSharedByKeyword(string keyword)
        {
            List<string> listId = new List<string>();
            List<VegetableResponseModel> vegetableResponseModels = new List<VegetableResponseModel>();
            var listVegetableShared = this._shareDetailRepository.GetMulti(s => s.Quantity > 0).Select(s => s.VegetableId).Distinct().ToList();
            var keys = this._keywordRepository.GetAll().ToList();
            foreach (var key in keys)
            {
                var t = key.KeywordName.Contains('_') ? IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim().Replace("_", string.Empty) : IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim();
                if (key.KeywordName.Contains('_') ?
                    (IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim().Replace("_", string.Empty).Contains(IdentityHelper.RemoveUnicode(keyword).ToUpper().Trim())
                    || IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim().Replace("_", string.Empty) == (IdentityHelper.RemoveUnicode(keyword).ToUpper().Trim()))
                    : IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim().Contains(IdentityHelper.RemoveUnicode(keyword).ToUpper().Trim())
                    || IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim() == IdentityHelper.RemoveUnicode(keyword).ToUpper().Trim())
                {
                    var nameVeg = this._vegetableCompositionRepository.GetSingle(s => key.VegCompositionId == s.Id, new string[] { "VegetableDescription" }).VegetableDescription;
                    nameVeg = nameVeg.VegetableCompositionId == 3 ? this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 1 && s.VegDesCommonId == nameVeg.VegDesCommonId) : nameVeg;
                    listId.AddRange(this._vegetableDescriptionRepository.GetMulti(s => s.VegContent == nameVeg.VegContent && s.Vegetables.Count() > 0, new string[] { "Vegetables" })
                        .Select(s => s.Vegetables).FirstOrDefault().Select(s => s.Id).Distinct().ToList());
                    foreach (var item in listId.ToList())
                    {
                        if (listVegetableShared.Contains(item))
                        {
                            var result = this._vegetableDescriptionRepository.GetMulti(s => s.VegDesCommonId == nameVeg.VegDesCommonId);
                            var name = result.Where(s => s.VegetableCompositionId == 1).FirstOrDefault().VegContent;
                            var des = result.Where(s => s.VegetableCompositionId == 2).FirstOrDefault().VegContent;
                            var feat = result.Where(s => s.VegetableCompositionId == 3).FirstOrDefault().VegContent;
                            var img = result.Where(s => s.VegetableCompositionId == 4).FirstOrDefault().Id;
                            if (name.Count() == 0 && des.Count() == 0 && feat.Count() == 0 && img.Count() == 0) return null;
                            vegetableResponseModels.Add(new VegetableResponseModel
                            {
                                Name = name,
                                Description = des,
                                Feature = feat,
                                Images = this._vegetableImageService.Get(img).ToList(),
                                IdDescription = nameVeg.VegDesCommonId
                            });
                        }
                    }
                }
            }

            if (listId.Count() <= 0)
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
                        var Labels = this._labelRepository.GetMulti(s => (s.StandsFor == "" || s.StandsFor == null), new string[] { "VegetableComposition" }).ToList();
                        foreach (var item in keywords.Value)
                        {
                            foreach (var label in Labels)
                            {
                                if (IdentityHelper.RemoveUnicode(label.LabelName).ToUpper().Trim().Contains(IdentityHelper.RemoveUnicode(item).ToUpper().Trim()))
                                {
                                    var nameVeg = this._vegetableDescriptionRepository.GetSingle(s => s.Id == label.VegetableComposition.VegetableDescriptionId);
                                    listId.AddRange(this._vegetableDescriptionRepository.GetMulti(s => s.VegContent == nameVeg.VegContent && s.Vegetables.Count() > 0, new string[] { "Vegetables" })
                                        .Select(s => s.Vegetables).FirstOrDefault().Select(s => s.Id).Distinct().ToList());
                                    foreach (var id in listId.ToList())
                                    {
                                        if (listVegetableShared.Contains(id))
                                        {
                                            var result = this._vegetableDescriptionRepository.GetMulti(s => s.VegDesCommonId == nameVeg.VegDesCommonId);
                                            var name = result.Where(s => s.VegetableCompositionId == 1).FirstOrDefault().VegContent;
                                            var des = result.Where(s => s.VegetableCompositionId == 2).FirstOrDefault().VegContent;
                                            var feat = result.Where(s => s.VegetableCompositionId == 3).FirstOrDefault().VegContent;
                                            var img = result.Where(s => s.VegetableCompositionId == 4).FirstOrDefault().Id;
                                            if (name.Count() == 0 && des.Count() == 0 && feat.Count() == 0 && img.Count() == 0) return null;
                                            vegetableResponseModels.Add(new VegetableResponseModel
                                            {
                                                Name = name,
                                                Description = des,
                                                Feature = feat,
                                                Images = this._vegetableImageService.Get(img).ToList(),
                                                IdDescription = nameVeg.VegDesCommonId
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return vegetableResponseModels.GroupBy(s => s.IdDescription).Select(s => s.First());
        }
    }
}

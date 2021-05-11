using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using VG.Common.Constant;
using VG.Common.Helper;
using VG.Data.Repository;
using VG.Model.Model;
using VG.Model.RequestModel;
using VG.Model.ResponseModel;

namespace VG.Service.Service
{
    public interface IPostService
    {
        PostRequestModel Add(PostRequestModel newItem, string phoneNumber);
        IEnumerable<PostResponseModel> GetShareByAccountId(string Id);
        IEnumerable<PostResponseModel> GetAll(string phoneNumber);
        PostResponseModel Get(string Id);
        PostRequestModel Update(PostRequestModel newItem, string phoneNumber, string savePath, string domain);
        void Delete(string Id);
        IEnumerable<PostResponseModel> SearchShareByName(string valueSearch, string phoneNumer);
        IEnumerable<PostResponseModel> SearchShareByKeyword(string valueSearch, string phoneNumer);
        IEnumerable<PostResponseModel> SearchShareByDescription(string valueSearch, string phoneNumer);
    }
    public class PostService : IPostService
    {
        private IPostRepository _postRepository;
        private IAccountRepository _accountRepository;
        private IVegetableService _vegetableService;
        private IAccountDetailRepository _accountDetailRepository;
        private IAccountFriendRepository _accountFriendRepository;
        private IVegetableExchangeRepository _vegetableExchangeRepository;
        private IVegetableDescriptionRepository _vegetableDescriptionRepository;
        private IVegetableCompositionRepository _vegetableCompositionRepository;
        private ILabelRepository _labelRepository;
        private IKeywordRepository _keywordRepository;
        private IHttpClientFactory _factory;
        private IExchangeDetailRepository _exchangeDetail;
        private IQRCodeRepository _qrCodeRepository;
        private const string uri = "https://extractkeywords.herokuapp.com";
        public PostService(IPostRepository postRepository, IAccountRepository accountRepository,
            IVegetableService vegetableService, IAccountDetailRepository accountDetailRepository,
            IAccountFriendRepository accountFriendRepository, IHttpClientFactory factory,
            IVegetableExchangeRepository vegetableExchangeRepository, IVegetableDescriptionRepository vegetableDescriptionRepository,
            IVegetableCompositionRepository vegetableCompositionRepository, ILabelRepository labelRepository, IKeywordRepository keywordRepository, 
            IExchangeDetailRepository exchangeDetail, IQRCodeRepository qrCodeRepository)
        {
            _postRepository = postRepository;
            _accountRepository = accountRepository;
            _vegetableService = vegetableService;
            _accountDetailRepository = accountDetailRepository;
            _accountFriendRepository = accountFriendRepository;
            _vegetableExchangeRepository = vegetableExchangeRepository;
            _vegetableDescriptionRepository = vegetableDescriptionRepository;
            _labelRepository = labelRepository;
            _keywordRepository = keywordRepository;
            _factory = factory;
            _vegetableCompositionRepository = vegetableCompositionRepository;
            _exchangeDetail = exchangeDetail;
            _qrCodeRepository = qrCodeRepository;
        }
        public PostRequestModel Add(PostRequestModel newItem, string phoneNumber)
        {
            string accountId = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber).Id;
            var veg = this._vegetableService.Get(newItem.VegetableId);
            if (veg.Quantity < newItem.Quantity)
            {
                throw new Exception("Số lượng cho vượt quá số lượng bạn đang sở hữu");
            }
            var post = this._postRepository.Add(new Post
            {
                PostContent = newItem.Content,
                ProvinceId = newItem.ProvinceId,
                DistrictId = newItem.DistrictId,
                WardId = newItem.WardId,
                Address = newItem.Address,
                DateShare = DateTime.Now,
                AccountId = accountId,
                Type = newItem.Type,
                Status = true,
                Quantity = newItem.Quantity,
                VegetableId = newItem.VegetableId,
                Lock = false
            });
            if (newItem.VegetableNeedId != null)
            {
                if (newItem.VegetableNeedId.Count() > 0)
                {
                    foreach (var item in newItem.VegetableNeedId)
                    {
                        this._vegetableExchangeRepository.Add(new VegetableExchange
                        {
                            VegetableDesciptionId = item,
                            PostId = post.Id
                        });
                    }
                }
            }
            this._postRepository.Commit();
            newItem.Id = post.Id;
            return newItem;
        }

        public void Delete(string Id)
        {
            var post = this._postRepository.GetSingle(s => s.Id == Id, new string[] { "ExchangeDetails", "VegetableShares" });
            var vegshare = this._vegetableExchangeRepository.GetMulti(s => s.PostId == post.Id);
            if (vegshare.Count() > 0)
            {
                this._vegetableExchangeRepository.Delete(vegshare);
            }
            var exchange = this._exchangeDetail.GetMulti(s => s.PostId == post.Id);
            if (exchange.Count() > 0)
            {
                var qrCodeExchange = this._qrCodeRepository.GetMulti(s => exchange.Select(q => q.Id).Contains(s.ExchangeId));
                if (qrCodeExchange.Count() > 0)
                {
                    this._qrCodeRepository.Delete(qrCodeExchange);
                }
                this._exchangeDetail.Delete(exchange);
            }
            if (post.ExchangeDetails.Where(s => s.PostId == Id && s.Status == (int)EnumStatusRequest.Accept).Count() > 0)
            {
                throw new Exception("Có đơn hàng đang được vận chuyển, không thể xoá bài viết này");
            }
            this._postRepository.Delete(post);
            this._postRepository.Commit();
        }

        public PostResponseModel Get(string Id)
        {
            var result = this._postRepository.GetSingle(s => s.Id == Id, new string[] { "VegetableShares.VegetableDescription" });
            List<VegetableExchangeResponseModel> vegetableExchanges = new List<VegetableExchangeResponseModel>();
            if (result.VegetableExchanges.Count > 0)
            {
                foreach (var item in result.VegetableExchanges)
                {
                    vegetableExchanges.Add(new VegetableExchangeResponseModel
                    {
                        VegetableExchangeId = item.VegetableDesciptionId,
                        VegetableExchangeName = item.VegetableDescription.VegContent
                    });
                }
            }
            var veg = this._vegetableService.Get(result.VegetableId);
            var accountDetail = _accountDetailRepository.GetSingle(s => s.AccountId == result.AccountId, new string[] { "AppAccount" });
            return new PostResponseModel
            {
                Id = result.Id,
                VegetableId = veg.Id,
                CreatedDate = result.DateShare,
                AccountId = result.AccountId,
                VegName = veg.Name,
                Content = result.PostContent,
                VegDescription = veg.Description,
                VegFeature = veg.Feature,
                FullName = accountDetail.FullName,
                PhoneNumber = accountDetail.AppAccount.PhoneNumber,
                Avatar = accountDetail.Avatar,
                Quantity = result.Quantity,
                QuantityVeg = (int)veg.Quantity,
                Type = result.Type,
                Images = veg.Images,
                VegetableExchange = vegetableExchanges
            };
        }

        public IEnumerable<PostResponseModel> GetAll(string phoneNumber)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
            var listFriend = this._accountFriendRepository.GetMulti(s => s.Account_one_Id == account.Id).Select(s => s.Account_two_Id).ToList();
            var listFriend2 = this._accountFriendRepository.GetMulti(s => s.Account_two_Id == account.Id).Select(s => s.Account_one_Id).ToList();
            listFriend.AddRange(listFriend2);
            var shareFriend = this._postRepository.GetAll(listFriend).ToList();
            var allShare = this._postRepository.GetAllExcept(listFriend, account.Id).ToList();
            var result = shareFriend.Union(allShare);
            return result.GroupBy(s => s.Id).Select(s => s.FirstOrDefault());
        }

        public IEnumerable<PostResponseModel> GetShareByAccountId(string Id)
        {
            List<PostResponseModel> postResponseModels = new List<PostResponseModel>();

            var result = this._postRepository.GetMulti(s => s.AccountId == Id, new string[] { "VegetableShares.VegetableDescription", "Provice", "District", "Ward" });
            var accountDetail = _accountDetailRepository.GetSingle(s => s.AccountId == Id, new string[] { "AppAccount" });
            foreach (var item in result.ToList())
            {
                List<VegetableExchangeResponseModel> vegetableExchanges = new List<VegetableExchangeResponseModel>();
                if (item.VegetableExchanges.Count > 0)
                {
                    foreach (var share in item.VegetableExchanges)
                    {
                        vegetableExchanges.Add(new VegetableExchangeResponseModel
                        {
                            VegetableExchangeId = share.VegetableDesciptionId,
                            VegetableExchangeName = share.VegetableDescription.VegContent
                        });
                    }
                }
                var veg = this._vegetableService.Get(item.VegetableId);
                postResponseModels.Add(new PostResponseModel
                {
                    Id = item.Id,
                    VegetableId = veg.Id,
                    CreatedDate = item.DateShare,
                    AccountId = item.AccountId,
                    VegName = veg.Name,
                    Content = item.PostContent,
                    VegDescription = veg.Description,
                    VegFeature = veg.Feature,
                    FullName = accountDetail.FullName,
                    PhoneNumber = accountDetail.AppAccount.PhoneNumber,
                    Address = item.Address + ", " + item.Ward.Name + ", " + item.District.Name + ", " + item.Province.Name,
                    Avatar = accountDetail.Avatar,
                    Quantity = item.Quantity,
                    QuantityVeg = (int)veg.Quantity,
                    Type = item.Type,
                    Images = veg.Images,
                    VegetableExchange = vegetableExchanges
                });
            }
            return postResponseModels.OrderByDescending(s => s.CreatedDate).ToList();
        }

        public IEnumerable<PostResponseModel> SearchShareByName(string valueSearch, string phoneNumer)
        {
            List<string> listId = new List<string>();
            var accountId = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumer).Id;
            var Labels = this._labelRepository.GetMulti(s => (s.StandsFor == "VEG"), new string[] { "VegetableComposition" }).ToList();
            foreach (var label in Labels)
            {
                if (IdentityHelper.RemoveUnicode(label.LabelName).ToUpper().Trim().Contains(IdentityHelper.RemoveUnicode(valueSearch).ToUpper().Trim()))
                {
                    var nameVeg = this._vegetableDescriptionRepository.GetSingle(s => s.Id == label.VegetableComposition.VegetableDescriptionId).VegContent;
                    var Veg = this._vegetableDescriptionRepository.GetMulti(s => s.VegContent == nameVeg && s.AccountId != "" && s.AccountId != null && s.Vegetables.Count() > 0, new string[] { "Vegetables" }).Select(s => s.Vegetables).FirstOrDefault();
                    if (Veg != null)
                    {
                        listId.AddRange(this._vegetableDescriptionRepository.GetMulti(s => s.VegContent == nameVeg && s.AccountId != "" && s.AccountId != null && s.Vegetables.Count() > 0, new string[] { "Vegetables" })
                        .Select(s => s.Vegetables).FirstOrDefault().Select(s => s.Id));
                    }
                }
            }
            var result = this._postRepository.SearchShare(listId.Distinct().ToList(), accountId);
            return result;
        }
        public IEnumerable<PostResponseModel> SearchShareByKeyword(string valueSearch, string phoneNumer)
        {
            List<string> listId = new List<string>();
            var accountId = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumer).Id;
            var keys = this._keywordRepository.GetAll().ToList();
            var db = keys.GroupBy(q => q.VegCompositionId).Select(s =>
            new Dictionary<string, string>() { { s.Key, string.Join(',', s.Select(q => q.KeywordName)) } });
            var json = JsonConvert.SerializeObject(new KeywordsRequestModel
            {
                text = valueSearch,
                db = db
            });
            HttpClient client = _factory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri + "/searchplat_withowndatabase"),
                Content = new StringContent(JsonConvert.SerializeObject(
                    new KeywordsRequestModel
                    {
                        text = valueSearch,
                        db = db
                    }),
                    Encoding.UTF8, "application/json"),
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
                        var composition = this._vegetableCompositionRepository.GetSingle(s => s.Id == item);
                        var nameVeg = this._vegetableDescriptionRepository.GetSingle(s => s.Id == composition.VegetableDescriptionId);
                        nameVeg = nameVeg.VegetableCompositionId == "3" ? this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == "1" && s.VegDesCommonId == nameVeg.VegDesCommonId) : nameVeg;
                        var veg = this._vegetableDescriptionRepository.GetMulti(s => s.VegContent == nameVeg.VegContent && s.Vegetables.Count() > 0, new string[] { "Vegetables" })
                            .Select(s => s.Vegetables);
                        if (veg.Count() > 0)
                        {
                            listId.AddRange(veg.FirstOrDefault().Select(s => s.Id));
                        }
                    }
                }

            }
            var result = this._postRepository.SearchShare(listId.Distinct().ToList(), accountId);
            return result;
        }
        public IEnumerable<PostResponseModel> SearchShareByDescription(string valueSearch, string phoneNumer)
        {
            List<string> listId = new List<string>();
            var accountId = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumer).Id;
            var vegDetails = this._vegetableDescriptionRepository.GetMulti(s => s.VegetableCompositionId == "2" && s.AccountId != "" && s.AccountId != null).Distinct().ToList();
            foreach (var detail in vegDetails)
            {
                if (IdentityHelper.RemoveUnicode(detail.VegContent).ToUpper().Trim().Contains(IdentityHelper.RemoveUnicode(valueSearch).ToUpper().Trim()))
                {
                    var vegDetail = this._vegetableDescriptionRepository.GetSingle(s => s.VegDesCommonId == detail.VegDesCommonId && s.VegetableCompositionId == "1", new string[] { "Vegetables" });
                    listId.AddRange(vegDetail.Vegetables.Select(s => s.Id));
                }
            }
            var result = this._postRepository.SearchShare(listId.Distinct().ToList(), accountId);
            return result;
        }
        public PostRequestModel Update(PostRequestModel newItem, string phoneNumber, string savePath, string domain)
        {
            var result = this._postRepository.GetSingle(s => s.Id == newItem.Id);
            var veg = this._vegetableService.Get(newItem.VegetableId);
            if (newItem.Quantity > veg.Quantity)
            {
                throw new Exception("Số lượng bạn cập nhật đang lớn hơn số lượng có sẵn trong vườn");
            }
            result.PostContent = newItem.Content;
            result.Quantity = newItem.Quantity;
            this._postRepository.Update(result);
            this._postRepository.Commit();
            return newItem;
        }
    }
}

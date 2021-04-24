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
    public interface IShareDetailService
    {
        ShareDetail Add(ShareDetailRequestModel newItem, string phoneNumber);
        IEnumerable<ShareDetailResponseModel> GetShareByAccountId(string Id);
        IEnumerable<ShareDetailResponseModel> GetAll(string phoneNumber);
        ShareDetailResponseModel Get(string Id);
        ShareDetailRequestModel Update(ShareDetailRequestModel newItem, string phoneNumber, string savePath, string domain);
        void Delete(string Id);
        IEnumerable<ShareDetailResponseModel> SearchShareByName(string valueSearch, string phoneNumer);
        IEnumerable<ShareDetailResponseModel> SearchShareByKeyword(string valueSearch, string phoneNumer);
        IEnumerable<ShareDetailResponseModel> SearchShareByDescription(string valueSearch, string phoneNumer);
    }
    public class ShareDetailService : IShareDetailService
    {
        private IShareDetailRepository _shareRepository;
        private IAccountRepository _accountRepository;
        private IVegetableService _vegetableService;
        private IAccountDetailRepository _accountDetailRepository;
        private IAccountFriendRepository _accountFriendRepository;
        private IVegetableShareRepository _vegetableShareRepository;
        private IVegetableDescriptionRepository _vegetableDescriptionRepository;
        private IVegetableCompositionRepository _vegetableCompositionRepository;
        private ILabelRepository _labelRepository;
        private IKeywordRepository _keywordRepository;
        private IHttpClientFactory _factory;
        private const string uri = "https://extractkeywords.herokuapp.com";
        public ShareDetailService(IShareDetailRepository shareRepository, IAccountRepository accountRepository,
            IVegetableService vegetableService, IAccountDetailRepository accountDetailRepository,
            IAccountFriendRepository accountFriendRepository, IHttpClientFactory factory,
            IVegetableShareRepository vegetableShareRepository, IVegetableDescriptionRepository vegetableDescriptionRepository,
            IVegetableCompositionRepository vegetableCompositionRepository, ILabelRepository labelRepository, IKeywordRepository keywordRepository)
        {
            _shareRepository = shareRepository;
            _accountRepository = accountRepository;
            _vegetableService = vegetableService;
            _accountDetailRepository = accountDetailRepository;
            _accountFriendRepository = accountFriendRepository;
            _vegetableShareRepository = vegetableShareRepository;
            _vegetableDescriptionRepository = vegetableDescriptionRepository;
            _labelRepository = labelRepository;
            _keywordRepository = keywordRepository;
            _factory = factory;
            _vegetableCompositionRepository = vegetableCompositionRepository;
        }
        public ShareDetail Add(ShareDetailRequestModel newItem, string phoneNumber)
        {
            string accountId = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber).Id;
            var veg = this._vegetableService.Get(newItem.VegetableId);
            if (veg.Quantity < newItem.Quantity)
            {
                throw new Exception("Số lượng cho vượt quá số lượng bạn đang sở hữu");
            }
            var share = this._shareRepository.Add(new ShareDetail
            {
                ShareContent = newItem.Content,
                DateShare = DateTime.Now,
                AccountId = accountId,
                Status = newItem.Status,
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
                        this._vegetableShareRepository.Add(new VegetableShare
                        {
                            VegetableDesciptionId = item,
                            ShareDetailId = share.Id
                        });
                    }
                }
            }
            this._shareRepository.Commit();
            return share;
        }

        public void Delete(string Id)
        {
            var share = this._shareRepository.GetSingle(s => s.Id == Id, new string[] { "ExchangeDetails", "VegetableShares" });
            if (share.ExchangeDetails.Where(s => s.ShareDetailId == Id && s.Status == (int)EnumStatusRequest.Accept).Count() > 0)
            {
                throw new Exception("Có đơn hàng đang được vận chuyển, không thể xoá bài viết này");
            }
            this._shareRepository.Delete(share);
            this._shareRepository.Commit();
        }

        public ShareDetailResponseModel Get(string Id)
        {
            var result = this._shareRepository.GetSingle(s => s.Id == Id, new string[] { "VegetableShares.VegetableDescription" });
            List<VegetableShareResponseModel> vegetableShares = new List<VegetableShareResponseModel>();
            if (result.VegetableShares.Count > 0)
            {
                foreach (var item in result.VegetableShares)
                {
                    vegetableShares.Add(new VegetableShareResponseModel
                    {
                        VegetableShareId = item.VegetableDesciptionId,
                        VegetableShareName = item.VegetableDescription.VegContent
                    });
                }
            }
            var veg = this._vegetableService.Get(result.VegetableId);
            var accountDetail = _accountDetailRepository.GetSingle(s => s.AccountId == result.AccountId, new string[] { "AppAccount" });
            return new ShareDetailResponseModel
            {
                Id = result.Id,
                CreatedDate = result.DateShare,
                AccountId = result.AccountId,
                VegName = veg.Name,
                Content = result.ShareContent,
                VegDescription = veg.Description,
                VegFeature = veg.Feature,
                FullName = accountDetail.FullName,
                PhoneNumber = accountDetail.AppAccount.PhoneNumber,
                Quantity = result.Quantity,
                Statius = result.Status,
                Images = veg.Images,
                VegetableShare = vegetableShares
            };
        }

        public IEnumerable<ShareDetailResponseModel> GetAll(string phoneNumber)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
            var listFriend = this._accountFriendRepository.GetMulti(s => s.Account_one_Id == account.Id).Select(s => s.Account_two_Id).ToList();
            var listFriend2 = this._accountFriendRepository.GetMulti(s => s.Account_two_Id == account.Id).Select(s => s.Account_one_Id).ToList();
            listFriend.AddRange(listFriend2);
            var shareFriend = this._shareRepository.GetAll(listFriend).ToList();
            var allShare = this._shareRepository.GetAllExcept(listFriend, account.Id).ToList();
            var result = shareFriend.Union(allShare);
            return result.GroupBy(s => s.Id).Select(s => s.FirstOrDefault());
        }

        public IEnumerable<ShareDetailResponseModel> GetShareByAccountId(string Id)
        {
            List<ShareDetailResponseModel> shareDetailResponseModels = new List<ShareDetailResponseModel>();

            var result = this._shareRepository.GetMulti(s => s.AccountId == Id, new string[] { "VegetableShares.VegetableDescription" });
            var accountDetail = _accountDetailRepository.GetSingle(s => s.AccountId == Id, new string[] { "AppAccount" });
            foreach (var item in result.ToList())
            {
                List<VegetableShareResponseModel> vegetableShares = new List<VegetableShareResponseModel>();
                if (item.VegetableShares.Count > 0)
                {
                    foreach (var share in item.VegetableShares)
                    {
                        vegetableShares.Add(new VegetableShareResponseModel
                        {
                            VegetableShareId = share.VegetableDesciptionId,
                            VegetableShareName = share.VegetableDescription.VegContent
                        });
                    }
                }
                var veg = this._vegetableService.Get(item.VegetableId);
                shareDetailResponseModels.Add(new ShareDetailResponseModel
                {
                    Id = item.Id,
                    CreatedDate = item.DateShare,
                    AccountId = item.AccountId,
                    VegName = veg.Name,
                    Content = item.ShareContent,
                    VegDescription = veg.Description,
                    VegFeature = veg.Feature,
                    FullName = accountDetail.FullName,
                    PhoneNumber = accountDetail.AppAccount.PhoneNumber,
                    Quantity = item.Quantity,
                    Statius = item.Status,
                    Images = veg.Images,
                    VegetableShare = vegetableShares
                });
            }
            return shareDetailResponseModels.OrderByDescending(s => s.CreatedDate).ToList();
        }

        public IEnumerable<ShareDetailResponseModel> SearchShareByName(string valueSearch, string phoneNumer)
        {
            List<string> listId = new List<string>();
            var accountId = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumer).Id;
            var Labels = this._labelRepository.GetMulti(s => (s.StandsFor == "" || s.StandsFor == null), new string[] { "VegetableComposition" }).ToList();
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
            var result = this._shareRepository.SearchShare(listId.Distinct().ToList(), accountId);
            return result;
        }
        public IEnumerable<ShareDetailResponseModel> SearchShareByKeyword(string valueSearch, string phoneNumer)
        {
            List<string> listId = new List<string>();
            var accountId = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumer).Id;
            var keys = this._keywordRepository.GetAll().ToList();
            //foreach (var key in keys)
            //{
            //    var t = key.KeywordName.Contains('_') ? IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim().Replace("_", string.Empty) : IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim();
            //    if (key.KeywordName.Contains('_') ? 
            //        (IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim().Replace("_", string.Empty).Contains(IdentityHelper.RemoveUnicode(valueSearch).ToUpper().Trim()) 
            //        || IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim().Replace("_", string.Empty) == (IdentityHelper.RemoveUnicode(valueSearch).ToUpper().Trim()))
            //        : IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim().Contains(IdentityHelper.RemoveUnicode(valueSearch).ToUpper().Trim())
            //        || IdentityHelper.RemoveUnicode(key.KeywordName).ToUpper().Trim() == IdentityHelper.RemoveUnicode(valueSearch).ToUpper().Trim())
            //    {
            //        var nameVeg = this._vegetableCompositionRepository.GetSingle(s => key.VegCompositionId == s.Id, new string[] { "VegetableDescription" }).VegetableDescription.VegContent;
            //        var Veg = this._vegetableDescriptionRepository.GetMulti(s => s.VegContent == nameVeg && s.Vegetables.Count() > 0, new string[] { "Vegetables" })
            //            .Select(s => s.Vegetables).FirstOrDefault();
            //        if (Veg != null)
            //        {
            //            listId.AddRange(Veg.Select(s => s.Id));
            //        }
            //    }
            //}
            if (listId.Count() <= 0)
            {
                var db = keys.GroupBy(q => q.VegCompositionId).Select(s =>
                new Dictionary<int, string>() { { s.Key, string.Join(',', s.Select(q => q.KeywordName))} });         
                var json = JsonConvert.SerializeObject( new KeywordsRequestModel
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
                        var Labels = this._labelRepository.GetMulti(s => (s.StandsFor == "" || s.StandsFor == null), new string[] { "VegetableComposition" }).ToList();
                        foreach (var item in keywords.Value)
                        {
                            foreach (var label in Labels)
                            {
                                if (IdentityHelper.RemoveUnicode(label.LabelName).ToUpper().Trim().Contains(IdentityHelper.RemoveUnicode(item).ToUpper().Trim()))
                                {
                                    var nameVeg = this._vegetableDescriptionRepository.GetSingle(s => s.Id == label.VegetableComposition.VegetableDescriptionId);
                                    nameVeg = nameVeg.VegetableCompositionId == 3 ? this._vegetableDescriptionRepository.GetSingle(s => s.VegetableCompositionId == 1 && s.VegDesCommonId == nameVeg.VegDesCommonId) : nameVeg;
                                    listId.AddRange(this._vegetableDescriptionRepository.GetMulti(s => s.VegContent == nameVeg.VegContent && s.Vegetables.Count() > 0, new string[] { "Vegetables" })
                                        .Select(s => s.Vegetables).FirstOrDefault().Select(s => s.Id));
                                }
                            }
                        }
                    }
                }
            }
            var result = this._shareRepository.SearchShare(listId.Distinct().ToList(), accountId);
            return result;
        }
        public IEnumerable<ShareDetailResponseModel> SearchShareByDescription(string valueSearch, string phoneNumer)
        {
            List<string> listId = new List<string>();
            var accountId = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumer).Id;
            var vegDetails = this._vegetableDescriptionRepository.GetMulti(s => s.VegetableCompositionId == 2 && s.AccountId != "" && s.AccountId != null).Distinct().ToList();
            foreach (var detail in vegDetails)
            {
                if (IdentityHelper.RemoveUnicode(detail.VegContent).ToUpper().Trim().Contains(IdentityHelper.RemoveUnicode(valueSearch).ToUpper().Trim()))
                {
                    var vegDetail = this._vegetableDescriptionRepository.GetSingle(s => s.VegDesCommonId == detail.VegDesCommonId && s.VegetableCompositionId == 1, new string[] { "Vegetables" });
                    listId.AddRange(vegDetail.Vegetables.Select(s => s.Id));
                }
            }
            var result = this._shareRepository.SearchShare(listId.Distinct().ToList(), accountId);
            return result;
        }
        public ShareDetailRequestModel Update(ShareDetailRequestModel newItem, string phoneNumber, string savePath, string domain)
        {
            var result = this._shareRepository.GetSingle(s => s.Id == newItem.Id);
            result.ShareContent = newItem.Content;
            result.Quantity = newItem.Quantity;
            result.Status = newItem.Status;
            result.VegetableId = newItem.VegetableId;
            this._shareRepository.Update(result);
            this._shareRepository.Commit();
            return newItem;
        }


    }
}

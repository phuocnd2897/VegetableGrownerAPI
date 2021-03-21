using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VG.Data.Repository;
using VG.Model.Model;
using VG.Model.RequestModel;
using VG.Model.ResponseModel;

namespace VG.Service.Service
{
    public interface IShareDetailService
    {
        ShareDetail Add(ShareDetailRequestModel newItem, string phoneNumber);
        IEnumerable<ShareDetailResponseModel> GetShareByAccountId(string phoneNumber);
        ShareDetailResponseModel Get(string Id);
        ShareDetailRequestModel Update(ShareDetailRequestModel newItem, string phoneNumber, string savePath, string domain);
        void Delete(string Id);
    }
    public class ShareDetailService : IShareDetailService
    {
        private IShareDetailRepository _shareRepository;
        private IAccountRepository _accountRepository;
        private IVegetableService _vegetableService;
        private IAccountDetailRepository _accountDetailRepository;
        public ShareDetailService(IShareDetailRepository shareRepository, IAccountRepository accountRepository, IVegetableService vegetableService, IAccountDetailRepository accountDetailRepository)
        {
            _shareRepository = shareRepository;
            _accountRepository = accountRepository;
            _vegetableService = vegetableService;
            _accountDetailRepository = accountDetailRepository;
        }
        public ShareDetail Add(ShareDetailRequestModel newItem, string phoneNumber)
        {
            string accountId = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber).Id;
            var share = this._shareRepository.Add(new ShareDetail
            {
                ShareContent = newItem.Content,
                DateShare = DateTime.Now,
                AccountId = accountId,
                Status = newItem.Status,
                Quantity = newItem.Quantity,
                VegetableId = newItem.VegetableId,
            });
            this._shareRepository.Commit();
            return share;
        }

        public void Delete(string Id)
        {
            var share = this._shareRepository.GetSingle(s => s.Id == Id);
            this._shareRepository.Delete(share);
            this._shareRepository.Commit();
        }

        public ShareDetailResponseModel Get(string Id)
        {
            var result = this._shareRepository.GetSingle(s => s.Id == Id);
            var veg = this._vegetableService.Get(result.VegetableId);
            var accountDetail = _accountDetailRepository.GetSingle(s => s.AccountId == result.AccountId);
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
                Quantity = result.Quantity,
                Statius = result.Status,
                Images = veg.Images
            };
        }

        public IEnumerable<ShareDetailResponseModel> GetShareByAccountId(string phoneNumber)
        {
            List<ShareDetailResponseModel> shareDetailResponseModels = new List<ShareDetailResponseModel>();
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
            var result = this._shareRepository.GetMulti(s => s.AccountId == account.Id);
            var accountDetail = _accountDetailRepository.GetSingle(s => s.AccountId == account.Id);
            foreach (var item in result.ToList())
            {

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
                    Quantity = item.Quantity,
                    Statius = item.Status,
                    Images = veg.Images
                });
            }
            return shareDetailResponseModels.ToList();
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

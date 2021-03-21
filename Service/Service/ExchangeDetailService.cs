using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VG.Common.Constant;
using VG.Data.Repository;
using VG.Model.Model;
using VG.Model.RequestModel;
using VG.Model.ResponseModel;

namespace VG.Service.Service
{
    public interface IExchangeDetailService
    {
        ExchangeDetailRequestModel Add(ExchangeDetailRequestModel newItem);
        ExchangeDetailRequestModel Update(ExchangeDetailRequestModel newItem);
        ExchangeDetail IsAccept(string Id, int Status);
        void Delete(string Id);
        ExchangeDetailResponseModel Get(string Id);
        IEnumerable<ExchangeDetailResponseModel> GetByAccountId(string phoneNumber);
    }
    public class ExchangeDetailService : IExchangeDetailService
    {
        private IExchangeDetailRepository _exchangeDetailRepository;
        private IAccountRepository _accountRepository;
        private IShareDetailRepository _shareDetailRepository;
        private IVegetableService _vegetableService;
        private IAccountDetailRepository _accountDetailRepository;
        public ExchangeDetailService(IAccountRepository accountRepository, IExchangeDetailRepository exchangeDetailRepository, IShareDetailRepository shareDetailRepository, IVegetableService vegetableService, IAccountDetailRepository accountDetailRepository)
        {
            _accountRepository = accountRepository;
            _exchangeDetailRepository = exchangeDetailRepository;
            _shareDetailRepository = shareDetailRepository;
            _vegetableService = vegetableService;
            _accountDetailRepository = accountDetailRepository;
        }

        public ExchangeDetail IsAccept(string Id, int Status)
        {
            var result = this._exchangeDetailRepository.GetSingle(s => s.Id == Id);
            result.Status = Status;
            this._exchangeDetailRepository.Update(result);
            this._exchangeDetailRepository.Commit();
            return result;
        }

        public ExchangeDetailRequestModel Add(ExchangeDetailRequestModel newItem)
        {
            var share = this._shareDetailRepository.GetSingle(s => s.Id == newItem.ShareDetailId);
            if (newItem.Quantity > share.Quantity)
            {
                return null;
            }
            var result = this._exchangeDetailRepository.Add(new ExchangeDetail
            {
                Status = (int)EnumStatusRequest.Pending,
                DateExchange = DateTime.UtcNow.AddHours(7),
                Quantity = newItem.Quantity,
                ReceiveBy = newItem.ReceivedBy,
                ShareDetailId = newItem.ShareDetailId
            });
            this._exchangeDetailRepository.Commit();
            newItem.Id = result.Id;
            return newItem;
        }

        public ExchangeDetailRequestModel Update(ExchangeDetailRequestModel newItem)
        {
            var result = this._exchangeDetailRepository.GetSingle(s => s.Id == newItem.Id);
            result.Quantity = newItem.Quantity;
            this._exchangeDetailRepository.Update(result);
            this._exchangeDetailRepository.Commit();
            return newItem;
        }

        public void Delete(string Id)
        {
            var result = this._exchangeDetailRepository.GetSingle(s => s.Id == Id);
            this._exchangeDetailRepository.Delete(result);
            this._exchangeDetailRepository.Commit();
        }

        public ExchangeDetailResponseModel Get(string Id)
        {
            var result = this._exchangeDetailRepository.GetSingle(S => S.Id == Id, new string[] { "ShareDetail" });
            var accountHost = this._accountDetailRepository.GetSingle(s => s.AccountId == result.ShareDetail.AccountId);
            var accountReceiver = this._accountDetailRepository.GetSingle(s => s.AccountId == result.ReceiveBy);
            return new ExchangeDetailResponseModel
            {
                Id = result.Id,
                Quantity = result.Quantity,
                Status = result.Status,
                AccountHostId = result.ShareDetail.AccountId,
                ReceiverId = result.ReceiveBy,
                FullNameReceiver = accountReceiver.FullName,
                VegName = _vegetableService.Get(result.ShareDetail.VegetableId).Name,
                CreatedDate = result.DateExchange,
                FullNameHost = accountHost.FullName,
                ShareDetailId = result.ShareDetailId
            };
        }

        public IEnumerable<ExchangeDetailResponseModel> GetByAccountId(string phoneNumber)
        {
            var result = this._exchangeDetailRepository.GetByAccountId(phoneNumber);
            return result;
        }
    }
}

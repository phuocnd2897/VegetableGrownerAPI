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
        IEnumerable<ExchangeDetailResponseModel> Add(ExchangeDetailRequestModel newItem, string phoneNumber);
        ExchangeDetailRequestModel Update(ExchangeDetailRequestModel newItem);
        void IsAccept(string Id, int Status, string baseUrl);
        void Delete(string Id);
        ExchangeDetailResponseModel Get(string Id);
        IEnumerable<ExchangeDetailResponseModel> GetByAccountId(string phoneNumber);
        IEnumerable<ExchangeDetailResponseModel> GetExchangeRequest(string phoneNumber);
    }
    public class ExchangeDetailService : IExchangeDetailService
    {
        private IExchangeDetailRepository _exchangeDetailRepository;
        private IAccountRepository _accountRepository;
        private IShareDetailRepository _shareDetailRepository;
        private IVegetableRepository _vegetableRepository;
        private IQRCodeService _qrCodeService;
        public ExchangeDetailService(IAccountRepository accountRepository, IExchangeDetailRepository exchangeDetailRepository, IShareDetailRepository shareDetailRepository,
            IVegetableRepository vegetableRepository, IQRCodeService qrCodeService)
        {
            _accountRepository = accountRepository;
            _exchangeDetailRepository = exchangeDetailRepository;
            _shareDetailRepository = shareDetailRepository;
            _vegetableRepository = vegetableRepository;
            _qrCodeService = qrCodeService;
        }

        public void IsAccept(string Id, int Status, string baseUrl)
        {
            var exchange = this._exchangeDetailRepository.GetSingle(s => s.Id == Id, new string[] { "ShareDetail" });
            if (exchange.Quantity <= exchange.ShareDetail.Quantity)
            {
                
                var listId = this._exchangeDetailRepository.GetMulti(s => s.ShareDetailId == exchange.ShareDetailId).Select(s => s.Id).ToList();
                if (Status == (int)EnumStatusRequest.Accept)
                {
                    exchange.ShareDetail.Quantity = exchange.ShareDetail.Quantity - exchange.Quantity;
                }
                foreach (var item in listId)
                {
                    var result = this._exchangeDetailRepository.GetSingle(s => s.Id == item, new string[] { "Vegetable" });
                    result.Status = Status;
                    if (Status == (int)EnumStatusRequest.Accept)
                    {
                        var veg = this._vegetableRepository.GetSingle(s => s.Id == result.VegetableId);
                        veg.Quantity = veg.Quantity - result.Quantity;
                        this._vegetableRepository.Update(veg);
                    }
                    this._exchangeDetailRepository.Update(result);
                    if (Status == (int)EnumStatusRequest.Accept)
                    {
                        var qrCode1 = this._qrCodeService.Add(item, baseUrl);
                    }
                }
                this._shareDetailRepository.Update(exchange.ShareDetail);
                this._exchangeDetailRepository.Commit();

            }
            else
            {
                throw new Exception("Số lượng rau nhận lớn hơn số lượng rau được chia sẻ");
            }
        }

        public IEnumerable<ExchangeDetailResponseModel> Add(ExchangeDetailRequestModel newItem, string phoneNumber)
        {
            List<ExchangeDetailResponseModel> exchangeDetailResponseModels = new List<ExchangeDetailResponseModel>();
            var share = this._shareDetailRepository.GetSingle(s => s.Id == newItem.ShareDetailId, new string[] { "Vegetable.VegetableDescription" });
            var accountHost = this._accountRepository.GetSingle(s => s.Id == share.AccountId, new string[] { "Members" });
            var accountReceiver = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber, new string[] { "Members" });
            if (newItem.VegetableId != "")
            {
                var resullt = this._exchangeDetailRepository.Add(new ExchangeDetail
                {
                    Status = (int)EnumStatusRequest.Pending,
                    DateExchange = DateTime.UtcNow.AddHours(7),
                    Quantity = newItem.Quantity,
                    Sender = accountHost.Id,
                    ReceiveBy = accountReceiver.Id,
                    ShareDetailId = newItem.ShareDetailId,
                    VegetableId = share.VegetableId
                });
                exchangeDetailResponseModels.Add(new ExchangeDetailResponseModel
                {
                    Id = resullt.Id,
                    Quantity = newItem.Quantity,
                    Status = (int)EnumStatusRequest.Pending,
                    FullNameHost = accountHost.Members.FirstOrDefault().FullName,
                    FullNameReceiver = accountReceiver.Members.FirstOrDefault().FullName,
                    AccountHostId = accountHost.Id,
                    ReceiverId = accountReceiver.Id,
                    ShareDetailId = share.Id,
                    CreatedDate = resullt.DateExchange
                });
                resullt = this._exchangeDetailRepository.Add(new ExchangeDetail
                {
                    Status = (int)EnumStatusRequest.Pending,
                    DateExchange = DateTime.UtcNow.AddHours(7),
                    Quantity = newItem.Quantity,
                    Sender = accountReceiver.Id,
                    ReceiveBy = accountHost.Id,
                    ShareDetailId = newItem.ShareDetailId,
                    VegetableId = newItem.VegetableId
                });
                exchangeDetailResponseModels.Add(new ExchangeDetailResponseModel
                {
                    Id = resullt.Id,
                    Quantity = newItem.QuantityExchange,
                    Status = (int)EnumStatusRequest.Pending,
                    FullNameHost = accountReceiver.Members.FirstOrDefault().FullName,
                    FullNameReceiver = accountHost.Members.FirstOrDefault().FullName,
                    AccountHostId = accountReceiver.Id,
                    ReceiverId = accountHost.Id,
                    ShareDetailId = share.Id,
                    CreatedDate = resullt.DateExchange
                });
            }
            else
            {
                var resullt = this._exchangeDetailRepository.Add(new ExchangeDetail
                {
                    Status = (int)EnumStatusRequest.Pending,
                    DateExchange = DateTime.UtcNow.AddHours(7),
                    Quantity = newItem.Quantity,
                    Sender = accountHost.Id,
                    ReceiveBy = accountReceiver.Id,
                    ShareDetailId = newItem.ShareDetailId,
                    VegetableId = share.VegetableId
                });
                exchangeDetailResponseModels.Add(new ExchangeDetailResponseModel
                {
                    Id = resullt.Id,
                    Quantity = newItem.Quantity,
                    Status = (int)EnumStatusRequest.Pending,
                    FullNameHost = accountHost.Members.FirstOrDefault().FullName,
                    FullNameReceiver = accountReceiver.Members.FirstOrDefault().FullName,
                    AccountHostId = accountHost.Id,
                    ReceiverId = accountReceiver.Id,
                    ShareDetailId = share.Id,
                    CreatedDate = resullt.DateExchange
                });
            }
            this._exchangeDetailRepository.Commit();
            return exchangeDetailResponseModels;
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
            var accountHost = this._accountRepository.GetSingle(s => s.Id == result.ShareDetail.AccountId, new string[] { "Members" });
            var accountReceiver = this._accountRepository.GetSingle(s => s.Id == result.ReceiveBy, new string[] { "Members" });
            return new ExchangeDetailResponseModel
            {
                Id = result.Id,
                Quantity = result.Quantity,
                Status = result.Status,
                AccountHostId = result.ShareDetail.AccountId,
                ReceiverId = result.ReceiveBy,
                FullNameReceiver = accountReceiver.Members.FirstOrDefault().FullName,
                CreatedDate = result.DateExchange,
                FullNameHost = accountHost.Members.FirstOrDefault().FullName,
                ShareDetailId = result.ShareDetailId
            };
        }

        public IEnumerable<ExchangeDetailResponseModel> GetByAccountId(string phoneNumber)
        {
            var result = this._exchangeDetailRepository.GetByAccountId(phoneNumber);
            return result;
        }

        public IEnumerable<ExchangeDetailResponseModel> GetExchangeRequest(string phoneNumber)
        {
            var result = this._exchangeDetailRepository.GetExchangeRequest(phoneNumber);
            return result;
        }
    }
}

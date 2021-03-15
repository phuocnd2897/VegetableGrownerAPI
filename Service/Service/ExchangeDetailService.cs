using System;
using System.Collections.Generic;
using System.Text;
using VG.Common.Constant;
using VG.Data.Repository;
using VG.Model.Model;
using VG.Model.RequestModel;

namespace VG.Service.Service
{
    public interface IExchangeDetailService
    {
        ExchangeDetailRequestModel Add(ExchangeDetailRequestModel newItem);
        ExchangeDetailRequestModel Update(ExchangeDetailRequestModel newItem);
        ExchangeDetail IsAccept(string Id, int Status);
        void Delete(string Id);
        ExchangeDetail Get(string Id);
        IEnumerable<ExchangeDetail> GetByAccountId(string phoneNumber);
    }
    public class ExchangeDetailService : IExchangeDetailService
    {
        private IExchangeDetailRepository _exchangeDetailRepository;
        private IAccountRepository _accountRepository;
        public ExchangeDetailService(IAccountRepository accountRepository, IExchangeDetailRepository exchangeDetailRepository)
        {
            _accountRepository = accountRepository;
            _exchangeDetailRepository = exchangeDetailRepository;
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

        public ExchangeDetail Get(string Id)
        {
            return this._exchangeDetailRepository.GetSingle(S => S.Id == Id);
        }

        public IEnumerable<ExchangeDetail> GetByAccountId(string phoneNumber)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
            var result = this._exchangeDetailRepository.GetMulti(s => s.ReceiveBy == account.Id, new string[] { "ShareDetail" });
            return result;
        }
    }
}

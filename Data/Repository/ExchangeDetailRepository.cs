using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;
using VG.Model.ResponseModel;
using System.Linq;
using VG.Common.Constant;

namespace VG.Data.Repository
{
    public interface IExchangeDetailRepository : IRepository<ExchangeDetail, string>
    {
        IEnumerable<ExchangeDetailResponseModel> GetByAccountId(string phoneNumber);
        IEnumerable<ExchangeDetailResponseModel> GetExchangeRequest(string phoneNumber);

    }
    public class ExchangeDetailRepository : RepositoryBase<ExchangeDetail, string>, IExchangeDetailRepository
    {
        private VGContext dbContext;
        public ExchangeDetailRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            dbContext = unitOfWork.dbContext;
        }

        public IEnumerable<ExchangeDetailResponseModel> GetByAccountId(string phoneNumber)
        {
            var result = from exchange in dbContext.ExchangeDetails
                         join share in dbContext.ShareDetails on exchange.ShareDetailId equals share.Id
                         join accountDetailReceiver in dbContext.Members on exchange.ReceiveBy equals accountDetailReceiver.AccountId into _accountDetailReceiver
                         from accountDetailReceiver in _accountDetailReceiver.DefaultIfEmpty()
                         join accountDetailHost in dbContext.Members on exchange.Sender equals accountDetailHost.AccountId into _accountDetailHost
                         from accountDetailHost in _accountDetailHost.DefaultIfEmpty()
                         join vegShare in dbContext.Vegetables on share.VegetableId equals vegShare.Id
                         join vegReceive in dbContext.VegetableDescriptions on share.VegetableNeedId equals vegReceive.Id into vegReceiveSen
                         from vegReceive in vegReceiveSen.DefaultIfEmpty()
                         where accountDetailReceiver.Account.PhoneNumber == phoneNumber || share.AppAccount.PhoneNumber == phoneNumber
                         select new ExchangeDetailResponseModel
                         {
                             Id = exchange.Id,
                             Quantity = exchange.Quantity,
                             Status = exchange.Status,
                             AccountHostId = exchange.Sender == "" ? share.AccountId : exchange.Sender,
                             VegNameSend = vegShare.VegetableDescription.VegContent,
                             VegNameReceive = vegReceive.VegContent,
                             CreatedDate = exchange.DateExchange,
                             FullNameHost = accountDetailHost != null ? accountDetailHost.FullName : share.AppAccount.Members.FirstOrDefault().FullName,
                             FullNameReceiver = accountDetailReceiver.FullName,
                             ReceiverId = exchange.ReceiveBy,
                             ShareDetailId = share.Id
                         };
            return result;

        }

        public IEnumerable<ExchangeDetailResponseModel> GetExchangeRequest(string phoneNumber)
        {
            var result = from share in dbContext.ShareDetails
                         join exchange in dbContext.ExchangeDetails on share.Id equals exchange.ShareDetailId
                         join accountDetailReceiver in dbContext.Members on exchange.ReceiveBy equals accountDetailReceiver.AccountId into _accountDetailReceiver
                         from accountDetailReceiver in _accountDetailReceiver.DefaultIfEmpty()
                         join accountDetailHost in dbContext.Members on exchange.Sender equals accountDetailHost.AccountId into _accountDetailHost
                         from accountDetailHost in _accountDetailHost.DefaultIfEmpty()
                         join vegShare in dbContext.Vegetables on share.VegetableId equals vegShare.Id
                         join vegReceive in dbContext.VegetableDescriptions on share.VegetableNeedId equals vegReceive.Id into vegReceiveSen
                         from vegReceive in vegReceiveSen.DefaultIfEmpty()
                         where share.AppAccount.PhoneNumber == phoneNumber && exchange.Status == (int)EnumStatusRequest.Pending
                         select new ExchangeDetailResponseModel
                         {
                             Id = exchange.Id,
                             Quantity = exchange.Quantity,
                             Status = exchange.Status,
                             AccountHostId = exchange.Sender ==  "" ? share.AccountId : exchange.Sender,
                             VegNameSend = vegShare.VegetableDescription.VegContent,
                             VegNameReceive = vegReceive.VegContent,
                             CreatedDate = exchange.DateExchange,
                             FullNameHost = accountDetailHost != null ? accountDetailHost.FullName : share.AppAccount.Members.FirstOrDefault().FullName,
                             FullNameReceiver = accountDetailReceiver.FullName,
                             ReceiverId = exchange.ReceiveBy,
                             ShareDetailId = share.Id
                         };
            return result;
        }
    }
}

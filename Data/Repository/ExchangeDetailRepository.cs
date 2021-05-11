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
                         join post in dbContext.Posts on exchange.PostId equals post.Id
                         join accountDetailReceiver in dbContext.Members on exchange.ReceiveBy equals accountDetailReceiver.AccountId into _accountDetailReceiver
                         from accountDetailReceiver in _accountDetailReceiver.DefaultIfEmpty()
                         join accountDetailHost in dbContext.Members on exchange.Sender equals accountDetailHost.AccountId into _accountDetailHost
                         from accountDetailHost in _accountDetailHost.DefaultIfEmpty()
                         join vegReceive in dbContext.Vegetables on exchange.VegetableId equals vegReceive.Id into vegReceiveSen
                         from vegReceive in vegReceiveSen.DefaultIfEmpty()
                         where accountDetailReceiver.AppAccount.PhoneNumber == phoneNumber || accountDetailHost.AppAccount.PhoneNumber == phoneNumber && (accountDetailReceiver.AppAccount.Status == true || accountDetailHost.AppAccount.Status == true)
                         select new ExchangeDetailResponseModel
                         {
                             Id = exchange.Id,
                             Quantity = exchange.Quantity,
                             Status = exchange.Status,
                             AccountHostId = exchange.Sender == "" ? post.AccountId : exchange.Sender,
                             VegNameReceive = vegReceive.VegetableDescription.VegContent,
                             CreatedDate = exchange.DateExchange,
                             FullNameHost = accountDetailHost != null ? accountDetailHost.FullName : post.AppAccount.Members.FirstOrDefault().FullName,
                             FullNameReceiver = accountDetailReceiver.FullName,
                             ReceiverId = exchange.ReceiveBy,
                             PostId = post.Id,
                             TypeShare = post.Type
                         };
            return result;

        }

        public IEnumerable<ExchangeDetailResponseModel> GetExchangeRequest(string phoneNumber)
        {
            var result = from post in dbContext.Posts
                         join exchange in dbContext.ExchangeDetails on post.Id equals exchange.PostId
                         join accountDetailReceiver in dbContext.Members on exchange.ReceiveBy equals accountDetailReceiver.AccountId into _accountDetailReceiver
                         from accountDetailReceiver in _accountDetailReceiver.DefaultIfEmpty()
                         join accountDetailHost in dbContext.Members on exchange.Sender equals accountDetailHost.AccountId into _accountDetailHost
                         from accountDetailHost in _accountDetailHost.DefaultIfEmpty()
                         join vegReceive in dbContext.Vegetables on exchange.VegetableId equals vegReceive.Id into vegReceiveSen
                         from vegReceive in vegReceiveSen.DefaultIfEmpty()
                         join exchangeResponse in dbContext.ExchangeDetails on exchange.Stt equals exchangeResponse.Stt  into _exchangeResponse
                         from exchangeResponse in _exchangeResponse.DefaultIfEmpty()
                         join vegReceiveExchangeResponse in dbContext.Vegetables on exchangeResponse.VegetableId equals vegReceiveExchangeResponse.Id into _vegReceiveExchangeResponse
                         from vegReceiveExchangeResponse in _vegReceiveExchangeResponse.DefaultIfEmpty()
                         where post.AppAccount.PhoneNumber == phoneNumber && exchange.Sender == post.AccountId && exchange.VegetableId != exchangeResponse.VegetableId && exchange.Status == (int)EnumStatusRequest.Pending && (accountDetailReceiver.AppAccount.Status == true && accountDetailHost.AppAccount.Status == true)
                         select new ExchangeDetailResponseModel
                         {
                             Id = exchange.Id,
                             Quantity = exchange.Quantity,
                             Status = exchange.Status,
                             AccountHostId = exchange.Sender ==  "" ? post.AccountId : exchange.Sender,
                             VegNameReceive = vegReceive.VegetableDescription.VegContent,
                             CreatedDate = exchange.DateExchange,
                             FullNameHost = accountDetailHost != null ? accountDetailHost.FullName : post.AppAccount.Members.FirstOrDefault().FullName,
                             FullNameReceiver = accountDetailReceiver.FullName,
                             ReceiverId = exchange.ReceiveBy,
                             PostId = post.Id,
                             VegNameReceiveExchangeResponse = vegReceiveExchangeResponse.VegetableDescription.VegContent,
                             QuantityReceiveExchangeResponse = exchangeResponse.Quantity,
                             TypeShare = post.Type
                         };
            return result;
        }
    }
}

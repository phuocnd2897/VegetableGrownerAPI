using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;
using VG.Model.ResponseModel;
using System.Linq;

namespace VG.Data.Repository
{
    public interface IExchangeDetailRepository : IRepository<ExchangeDetail, string>
    {
        IEnumerable<ExchangeDetailResponseModel> GetByAccountId(string phoneNumber);
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
                         join accountDetailReceiver in dbContext.Members on exchange.ReceiveBy equals accountDetailReceiver.AccountId
                         join accountDetailHost in dbContext.Members on share.AccountId equals accountDetailHost.AccountId
                         join veg in dbContext.Vegetables on share.VegetableId equals veg.Id
                         join vegInfo in dbContext.Vegetables.Where(s => s.VegetableDescription.VegetableCompositionId == 1) on new { GardenId = veg.GardenId, NoVeg = veg.No } equals new { GardenId = vegInfo.GardenId, NoVeg = vegInfo.No }
                         where accountDetailReceiver.Account.PhoneNumber == phoneNumber
                         select new ExchangeDetailResponseModel
                         {
                             Id = exchange.Id,
                             Quantity = exchange.Quantity,
                             Status = exchange.Status,
                             AccountHostId = share.AccountId,
                             VegName = vegInfo.VegetableDescription.VegContent,
                             CreatedDate = exchange.DateExchange,
                             FullNameHost = accountDetailHost.FullName,
                             FullNameReceiver = accountDetailReceiver.FullName,
                             ReceiverId = exchange.ReceiveBy,
                             ShareDetailId = share.Id
                         };
            return result;

        }
    }
}

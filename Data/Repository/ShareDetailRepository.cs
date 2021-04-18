using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;
using VG.Model.ResponseModel;
using System.Linq;

namespace VG.Data.Repository
{
    public interface IShareDetailRepository : IRepository<ShareDetail, string>
    {
        IEnumerable<ShareDetailResponseModel> GetAll(List<string> listId);
        IEnumerable<ShareDetailResponseModel> GetAllExcept(List<string> listId, string accountId);
        IEnumerable<ShareDetailResponseModel> SearchShare(List<string> valueSearch);
    }
    public class ShareDetailRepository : RepositoryBase<ShareDetail, string>, IShareDetailRepository
    {
        private VGContext dbContext;
        public ShareDetailRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            dbContext = unitOfWork.dbContext;
        }

        public IEnumerable<ShareDetailResponseModel> GetAll(List<string> listId)
        {
            var result = from share in dbContext.ShareDetails
                         join account in dbContext.Accounts on share.AccountId equals account.Id
                         join accountDetail in dbContext.Members on account.Id equals accountDetail.AccountId
                         join veg in dbContext.Vegetables on share.VegetableId equals veg.Id
                         join vegDetailName in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == 1) on veg.VegetableDescriptionId equals vegDetailName.VegDesCommonId
                         join vegDetailDes in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == 2) on veg.VegetableDescriptionId equals vegDetailDes.VegDesCommonId
                         join vegDetailFeat in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == 3) on veg.VegetableDescriptionId equals vegDetailFeat.VegDesCommonId
                         join vegDetailImg in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == 4) on veg.VegetableDescriptionId equals vegDetailImg.VegDesCommonId
                         where listId.Contains(share.AccountId) && share.Quantity > 0 && account.Status == true
                         orderby share.DateShare descending
                         select new ShareDetailResponseModel
                         {
                             Id = share.Id,
                             CreatedDate = share.DateShare,
                             AccountId = share.AccountId,
                             VegName = vegDetailName.VegContent,
                             Content = share.ShareContent,
                             VegDescription = vegDetailDes.VegContent,
                             VegFeature = vegDetailFeat.VegContent,
                             FullName = accountDetail.FullName,
                             VegetableShare = (from vegNeed in dbContext.VegetableShares where vegNeed.ShareDetailId == share.Id 
                                              select new VegetableShareResponseModel 
                                              { 
                                                  VegetableShareId = vegNeed.VegetableDesciptionId, 
                                                  VegetableShareName = vegNeed.VegetableDescription.VegContent
                                              }).ToList(),
                             Quantity = share.Quantity,
                             Statius = share.Status,
                             Images = (from image in dbContext.VegetableImages where vegDetailImg.Id == image.VegetableDescriptionId && vegDetailImg.AccountId == account.Id select image).ToList()
                         };
            return result;
        }

        public IEnumerable<ShareDetailResponseModel> GetAllExcept(List<string> listId, string accountId)
        {
            var result = from share in dbContext.ShareDetails
                         join account in dbContext.Accounts on share.AccountId equals account.Id
                         join accountDetail in dbContext.Members on account.Id equals accountDetail.AccountId
                         join veg in dbContext.Vegetables on share.VegetableId equals veg.Id
                         join vegDetailName in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == 1) on veg.VegetableDescriptionId equals vegDetailName.VegDesCommonId
                         join vegDetailDes in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == 2) on veg.VegetableDescriptionId equals vegDetailDes.VegDesCommonId
                         join vegDetailFeat in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == 3) on veg.VegetableDescriptionId equals vegDetailFeat.VegDesCommonId
                         join vegDetailImg in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == 4) on veg.VegetableDescriptionId equals vegDetailImg.VegDesCommonId
                         where !(listId.Contains(share.AccountId)) && share.AccountId != accountId && share.Quantity > 0 && account.Status == true
                         orderby share.DateShare descending
                         select new ShareDetailResponseModel
                         {
                             Id = share.Id,
                             CreatedDate = share.DateShare,
                             AccountId = share.AccountId,
                             VegName = vegDetailName.VegContent,
                             Content = share.ShareContent,
                             VegDescription = vegDetailDes.VegContent,
                             VegFeature = vegDetailFeat.VegContent,
                             FullName = accountDetail.FullName,
                             VegetableShare = (from vegNeed in dbContext.VegetableShares
                                               where vegNeed.ShareDetailId == share.Id
                                               select new VegetableShareResponseModel
                                               {
                                                   VegetableShareId = vegNeed.VegetableDesciptionId,
                                                   VegetableShareName = vegNeed.VegetableDescription.VegContent
                                               }).ToList(),
                             Quantity = share.Quantity,
                             Statius = share.Status,
                             Images = (from image in dbContext.VegetableImages where vegDetailImg.Id == image.VegetableDescriptionId && vegDetailImg.AccountId == account.Id select image).ToList()
                         };
            return result;
        }

        public IEnumerable<ShareDetailResponseModel> SearchShare(List<string> listId)
        {
            var result = from share in dbContext.ShareDetails
                         join account in dbContext.Accounts on share.AccountId equals account.Id
                         join accountDetail in dbContext.Members on account.Id equals accountDetail.AccountId
                         join veg in dbContext.Vegetables on share.VegetableId equals veg.Id
                         join vegDetailName in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == 1) on veg.VegetableDescriptionId equals vegDetailName.VegDesCommonId
                         join vegDetailDes in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == 2) on veg.VegetableDescriptionId equals vegDetailDes.VegDesCommonId
                         join vegDetailFeat in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == 3) on veg.VegetableDescriptionId equals vegDetailFeat.VegDesCommonId
                         join vegDetailImg in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == 4) on veg.VegetableDescriptionId equals vegDetailImg.VegDesCommonId
                         where listId.Contains(share.VegetableId) && share.Quantity > 0 && account.Status == true
                         orderby share.DateShare descending
                         select new ShareDetailResponseModel
                         {
                             Id = share.Id,
                             CreatedDate = share.DateShare,
                             AccountId = share.AccountId,
                             VegName = vegDetailName.VegContent,
                             Content = share.ShareContent,
                             VegDescription = vegDetailDes.VegContent,
                             VegFeature = vegDetailFeat.VegContent,
                             FullName = accountDetail.FullName,
                             VegetableShare = (from vegNeed in dbContext.VegetableShares
                                               where vegNeed.ShareDetailId == share.Id
                                               select new VegetableShareResponseModel
                                               {
                                                   VegetableShareId = vegNeed.VegetableDesciptionId,
                                                   VegetableShareName = vegNeed.VegetableDescription.VegContent
                                               }).ToList(),
                             Quantity = share.Quantity,
                             Statius = share.Status,
                             Images = (from image in dbContext.VegetableImages where vegDetailImg.Id == image.VegetableDescriptionId && vegDetailImg.AccountId == account.Id select image).ToList()
                         };
            return result.AsEnumerable().GroupBy(s => s.Id).Select(s => s.First());
        }
    }
}

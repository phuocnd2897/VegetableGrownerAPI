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
        IEnumerable<ShareDetailResponseModel> GetAll();
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
                         join vegDetailName in dbContext.Vegetables.Where(s => s.VegetableDescription.VegetableCompositionId == 1) on new { GardenId = veg.GardenId, NoVeg = veg.No } equals new { GardenId = vegDetailName.GardenId, NoVeg = vegDetailName.No }
                         join vegDetailDes in dbContext.Vegetables.Where(s => s.VegetableDescription.VegetableCompositionId == 2) on new { GardenId = veg.GardenId, NoVeg = veg.No } equals new { GardenId = vegDetailDes.GardenId, NoVeg = vegDetailDes.No }
                         join vegDetailFeat in dbContext.Vegetables.Where(s => s.VegetableDescription.VegetableCompositionId == 3) on new { GardenId = veg.GardenId, NoVeg = veg.No } equals new { GardenId = vegDetailFeat.GardenId, NoVeg = vegDetailFeat.No }
                         join vegDetailImg in dbContext.Vegetables.Where(s => s.VegetableDescription.VegetableCompositionId == 4) on new { GardenId = veg.GardenId, NoVeg = veg.No } equals new { GardenId = vegDetailImg.GardenId, NoVeg = vegDetailImg.No }
                         //join img in dbContext.VegetableImages on vegDetailImg.VegetableDescriptionId equals img.VegetableDescriptionId into g
                         where listId.Contains(share.AccountId)
                         orderby share.DateShare
                         select new ShareDetailResponseModel
                         {
                             Id = share.Id,
                             CreatedDate = share.DateShare,
                             AccountId = share.AccountId,
                             VegName = vegDetailName.VegetableDescription.VegContent,
                             Content = share.ShareContent,
                             VegDescription = vegDetailDes.VegetableDescription.VegContent,
                             VegFeature = vegDetailFeat.VegetableDescription.VegContent,
                             FullName = accountDetail.FullName,
                             Quantity = share.Quantity,
                             Statius = share.Status,
                             Images = (from image in dbContext.VegetableImages where vegDetailImg.VegetableDescriptionId == image.VegetableDescriptionId select image).ToList()
                         };
            return result;
        }

        public IEnumerable<ShareDetailResponseModel> GetAll()
        {
            var result = from share in dbContext.ShareDetails
                         join account in dbContext.Accounts on share.AccountId equals account.Id
                         join accountDetail in dbContext.Members on account.Id equals accountDetail.AccountId
                         join veg in dbContext.Vegetables on share.VegetableId equals veg.Id
                         join vegDetailName in dbContext.Vegetables.Where(s => s.VegetableDescription.VegetableCompositionId == 1) on new { GardenId = veg.GardenId, NoVeg = veg.No } equals new { GardenId = vegDetailName.GardenId, NoVeg = vegDetailName.No }
                         join vegDetailDes in dbContext.Vegetables.Where(s => s.VegetableDescription.VegetableCompositionId == 2) on new { GardenId = veg.GardenId, NoVeg = veg.No } equals new { GardenId = vegDetailDes.GardenId, NoVeg = vegDetailDes.No }
                         join vegDetailFeat in dbContext.Vegetables.Where(s => s.VegetableDescription.VegetableCompositionId == 3) on new { GardenId = veg.GardenId, NoVeg = veg.No } equals new { GardenId = vegDetailFeat.GardenId, NoVeg = vegDetailFeat.No }
                         join vegDetailImg in dbContext.Vegetables.Where(s => s.VegetableDescription.VegetableCompositionId == 4) on new { GardenId = veg.GardenId, NoVeg = veg.No } equals new { GardenId = vegDetailImg.GardenId, NoVeg = vegDetailImg.No }
                         //join img in dbContext.VegetableImages on vegDetailImg.VegetableDescriptionId equals img.VegetableDescriptionId into g
                         orderby share.DateShare
                         select new ShareDetailResponseModel
                         {
                             Id = share.Id,
                             CreatedDate = share.DateShare,
                             AccountId = share.AccountId,
                             VegName = vegDetailName.VegetableDescription.VegContent,
                             Content = share.ShareContent,
                             VegDescription = vegDetailDes.VegetableDescription.VegContent,
                             VegFeature = vegDetailFeat.VegetableDescription.VegContent,
                             FullName = accountDetail.FullName,
                             Quantity = share.Quantity,
                             Statius = share.Status,
                             Images = (from image in dbContext.VegetableImages where vegDetailImg.VegetableDescriptionId == image.VegetableDescriptionId select image).ToList()
                         };
            return result;
        }
    }
}

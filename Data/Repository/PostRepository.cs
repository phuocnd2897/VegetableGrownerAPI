using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;
using VG.Model.ResponseModel;
using System.Linq;

namespace VG.Data.Repository
{
    public interface IPostRepository : IRepository<Post, string>
    {
        IEnumerable<PostResponseModel> GetAll(List<string> listId);
        IEnumerable<PostResponseModel> GetAllExcept(List<string> listId, string accountId);
        IEnumerable<PostResponseModel> SearchShare(List<string> valueSearch, string accountId);
    }
    public class PostRepository : RepositoryBase<Post, string>, IPostRepository
    {
        private VGContext dbContext;
        public PostRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            dbContext = unitOfWork.dbContext;
        }

        public IEnumerable<PostResponseModel> GetAll(List<string> listId)
        {
            var result = from post in dbContext.Posts
                         join account in dbContext.Accounts on post.AccountId equals account.Id
                         join accountDetail in dbContext.Members on account.Id equals accountDetail.AccountId
                         join veg in dbContext.Vegetables on post.VegetableId equals veg.Id
                         join vegDetailName in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == "1") on veg.VegetableDescriptionId equals vegDetailName.VegDesCommonId
                         join vegDetailDes in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == "2") on veg.VegetableDescriptionId equals vegDetailDes.VegDesCommonId
                         join vegDetailFeat in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == "3") on veg.VegetableDescriptionId equals vegDetailFeat.VegDesCommonId
                         join vegDetailImg in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == "4") on veg.VegetableDescriptionId equals vegDetailImg.VegDesCommonId
                         join province in dbContext.Provinces on post.ProvinceId equals province.Id
                         join district in dbContext.Districts on post.DistrictId equals district.Id
                         join ward in dbContext.Wards on post.WardId equals ward.Id
                         where listId.Contains(post.AccountId) && post.Quantity > 0 && account.Status == true
                         orderby post.DateShare descending
                         select new PostResponseModel
                         {
                             Id = post.Id,
                             CreatedDate = post.DateShare,
                             AccountId = post.AccountId,
                             VegName = vegDetailName.VegContent,
                             Content = post.PostContent,
                             VegDescription = vegDetailDes.VegContent,
                             VegFeature = vegDetailFeat.VegContent,
                             FullName = accountDetail.FullName,
                             PhoneNumber = account.PhoneNumber,
                             Address = post.Address + ", " + ward.Name + ", " + district.Name + ", " + province.Name,
                             Avatar = accountDetail.Avatar,
                             VegetableExchange = (from vegNeed in dbContext.VegetableExchanges where vegNeed.PostId == post.Id 
                                              select new VegetableExchangeResponseModel 
                                              { 
                                                  VegetableExchangeId = vegNeed.VegetableDesciptionId, 
                                                  VegetableExchangeName = vegNeed.VegetableDescription.VegContent
                                              }).ToList(),
                             Quantity = post.Quantity,
                             Type = post.Type,
                             Images = (from image in dbContext.VegetableImages where vegDetailImg.Id == image.VegetableDescriptionId && vegDetailImg.AccountId == account.Id select image).ToList()
                         };
            return result;
        }

        public IEnumerable<PostResponseModel> GetAllExcept(List<string> listId, string accountId)
        {
            var result = from post in dbContext.Posts
                         join account in dbContext.Accounts on post.AccountId equals account.Id
                         join accountDetail in dbContext.Members on account.Id equals accountDetail.AccountId
                         join veg in dbContext.Vegetables on post.VegetableId equals veg.Id
                         join vegDetailName in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == "1") on veg.VegetableDescriptionId equals vegDetailName.VegDesCommonId
                         join vegDetailDes in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == "2") on veg.VegetableDescriptionId equals vegDetailDes.VegDesCommonId
                         join vegDetailFeat in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == "3") on veg.VegetableDescriptionId equals vegDetailFeat.VegDesCommonId
                         join vegDetailImg in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == "4") on veg.VegetableDescriptionId equals vegDetailImg.VegDesCommonId
                         join province in dbContext.Provinces on post.ProvinceId equals province.Id
                         join district in dbContext.Districts on post.DistrictId equals district.Id
                         join ward in dbContext.Wards on post.WardId equals ward.Id
                         where !(listId.Contains(post.AccountId)) && post.AccountId != accountId && post.Quantity > 0 && account.Status == true
                         orderby post.DateShare descending
                         select new PostResponseModel
                         {
                             Id = post.Id,
                             CreatedDate = post.DateShare,
                             AccountId = post.AccountId,
                             VegName = vegDetailName.VegContent,
                             Content = post.PostContent,
                             VegDescription = vegDetailDes.VegContent,
                             VegFeature = vegDetailFeat.VegContent,
                             FullName = accountDetail.FullName,
                             PhoneNumber = account.PhoneNumber,
                             Address = post.Address + ", " + ward.Name + ", " + district.Name + ", " + province.Name,
                             Avatar = accountDetail.Avatar,
                             VegetableExchange = (from vegNeed in dbContext.VegetableExchanges
                                               where vegNeed.PostId == post.Id
                                               select new VegetableExchangeResponseModel
                                               {
                                                   VegetableExchangeId = vegNeed.VegetableDesciptionId,
                                                   VegetableExchangeName = vegNeed.VegetableDescription.VegContent
                                               }).ToList(),
                             Quantity = post.Quantity,
                             Type = post.Type,
                             Images = (from image in dbContext.VegetableImages where vegDetailImg.Id == image.VegetableDescriptionId && vegDetailImg.AccountId == account.Id select image).ToList()
                         };
            return result;
        }

        public IEnumerable<PostResponseModel> SearchShare(List<string> listId, string accountId)
        {
            var result = from post in dbContext.Posts
                         join account in dbContext.Accounts on post.AccountId equals account.Id
                         join accountDetail in dbContext.Members on account.Id equals accountDetail.AccountId
                         join veg in dbContext.Vegetables on post.VegetableId equals veg.Id
                         join vegDetailName in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == "1") on veg.VegetableDescriptionId equals vegDetailName.VegDesCommonId
                         join vegDetailDes in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == "2") on veg.VegetableDescriptionId equals vegDetailDes.VegDesCommonId
                         join vegDetailFeat in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == "3") on veg.VegetableDescriptionId equals vegDetailFeat.VegDesCommonId
                         join vegDetailImg in dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == "4") on veg.VegetableDescriptionId equals vegDetailImg.VegDesCommonId
                         join province in dbContext.Provinces on post.ProvinceId equals province.Id
                         join district in dbContext.Districts on post.DistrictId equals district.Id
                         join ward in dbContext.Wards on post.WardId equals ward.Id
                         where listId.Contains(post.VegetableId) && post.Quantity > 0 && account.Status == true && post.AccountId != accountId
                         orderby post.DateShare descending
                         select new PostResponseModel
                         {
                             Id = post.Id,
                             CreatedDate = post.DateShare,
                             AccountId = post.AccountId,
                             VegName = vegDetailName.VegContent,
                             Content = post.PostContent,
                             VegDescription = vegDetailDes.VegContent,
                             VegFeature = vegDetailFeat.VegContent,
                             FullName = accountDetail.FullName,
                             PhoneNumber = account.PhoneNumber,
                             Address = post.Address + ", " + ward.Name + ", " + district.Name + ", " + province.Name,
                             Avatar = accountDetail.Avatar,
                             VegetableExchange = (from vegNeed in dbContext.VegetableExchanges
                                               where vegNeed.PostId == post.Id
                                               select new VegetableExchangeResponseModel
                                               {
                                                   VegetableExchangeId = vegNeed.VegetableDesciptionId,
                                                   VegetableExchangeName = vegNeed.VegetableDescription.VegContent
                                               }).ToList(),
                             Quantity = post.Quantity,
                             Type = post.Type,
                             Images = (from image in dbContext.VegetableImages where vegDetailImg.Id == image.VegetableDescriptionId && vegDetailImg.AccountId == account.Id select image).ToList()
                         };
            return result.AsEnumerable().GroupBy(s => s.Id).Select(s => s.First());
        }
    }
}

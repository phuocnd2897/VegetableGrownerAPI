using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VG.Data.Repository;
using VG.Model.Model;
using VG.Model.RequestModel;
using VG.Model.ResponseModel;

namespace VG.Service.Service
{
    public interface IVegetableService
    {
        VegetableRequestModel Add(VegetableRequestModel newItem, string phoneNumber, string savePath);
        VegetableRequestModel Update(VegetableRequestModel newItem, string phoneNumber, string savePath);
        void Delete(int noVeg, int gardenId);
        public VegetableResponseModel Get(int noVeg, int gardenId);
        public IEnumerable<VegetableResponseModel> GetAll();
    }
    public class VegetableService : IVegetableService
    {
        private IVegetableRepository _vegetableRepository;
        private IVegetableDescriptionRepository _vegetableDescriptionRepository;
        private IVegetableCompositionRepository _vegetableCompositionRepository;
        private IVegetableImageService _vegetableImageService;
        private IAccountRepository _acccountRepository;
        public VegetableService(IVegetableRepository vegetableRepository, IVegetableDescriptionRepository vegetableDescriptionRepository, IVegetableCompositionRepository vegetableCompositionRepository, IVegetableImageService vegetableImageService, IAccountRepository acccountRepository)
        {
            _vegetableRepository = vegetableRepository;
            _vegetableDescriptionRepository = vegetableDescriptionRepository;
            _vegetableCompositionRepository = vegetableCompositionRepository;
            _vegetableImageService = vegetableImageService;
            _acccountRepository = acccountRepository;
        }
        public VegetableRequestModel Add(VegetableRequestModel newItem, string phoneNumber, string savePath)
        {
            int num = 1;
            var item = this._vegetableRepository.GetMulti(s => s.GardenId == newItem.GardenId);
            if (item != null)
            {
                num = item.Select(s => s.No).Max() + 1;
            }
            if (newItem.NewFeatture != "")
            {
                var account = this._acccountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
                if (account == null)
                {
                    throw new Exception("Có lỗi xảy ra. Vui lòng thử lại");
                }
                var vegDetailName = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = newItem.Title,
                    VegetableCompositionId = 1,
                    AccountId = account.Id,
                });
                var vegName = this._vegetableRepository.Add(new Vegetable
                {
                    No = num,
                    GardenId = newItem.GardenId,
                    VegetableDescriptionId = vegDetailName.Id
                });
                var vegDetailDes = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = newItem.Description,
                    VegetableCompositionId = 2,
                    AccountId = account.Id,
                });
                var vegDes = this._vegetableRepository.Add(new Vegetable
                {
                    No = num,
                    GardenId = newItem.GardenId,
                    VegetableDescriptionId = vegDetailDes.Id
                });
                var vegDetailFeature = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = newItem.NewFeatture,
                    VegetableCompositionId = 3,
                    AccountId = account.Id,
                });
                var vegFeature = this._vegetableRepository.Add(new Vegetable
                {
                    No = num,
                    GardenId = newItem.GardenId,
                    VegetableDescriptionId = vegDetailFeature.Id
                });
                var vegDetailImg = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = "Image",
                    VegetableCompositionId = 4,
                    AccountId = account.Id,
                });
                var vegImg = this._vegetableRepository.Add(new Vegetable
                {
                    No = num,
                    GardenId = newItem.GardenId,
                    VegetableDescriptionId = vegDetailImg.Id
                });
                if (newItem.Images != null)
                {
                    foreach (IFormFile img in newItem.Images)
                    {
                        this._vegetableImageService.UploadImage(img, vegDetailImg.Id, savePath);
                    }
                }
                newItem.IdName = vegName.Id;
                newItem.IdDescription = vegDes.Id;
                newItem.IdFeature = vegFeature.Id;
                newItem.IdImage = vegImg.Id;
            }
            else
            {
                var vegDetailName = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = newItem.Title,
                    VegetableCompositionId = 1,
                });
                var vegName = this._vegetableRepository.Add(new Vegetable
                {
                    No = num,
                    GardenId = newItem.GardenId,
                    VegetableDescriptionId = vegDetailName.Id
                });
                var vegDetailDes = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = newItem.Description,
                    VegetableCompositionId = 2,
                });
                var vegDes = this._vegetableRepository.Add(new Vegetable
                {
                    No = num,
                    GardenId = newItem.GardenId,
                    VegetableDescriptionId = vegDetailDes.Id
                });
                var vegDetailFeature = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = newItem.Featture,
                    VegetableCompositionId = 3,
                });
                var vegFeature = this._vegetableRepository.Add(new Vegetable
                {
                    No = num,
                    GardenId = newItem.GardenId,
                    VegetableDescriptionId = vegDetailFeature.Id
                });
                var vegDetailImg = this._vegetableDescriptionRepository.Add(new VegetableDescription
                {
                    VegContent = "Image",
                    VegetableCompositionId = 4,
                });
                var vegImg = this._vegetableRepository.Add(new Vegetable
                {
                    No = num,
                    GardenId = newItem.GardenId,
                    VegetableDescriptionId = vegDetailImg.Id
                });
                if (newItem.Images != null)
                {
                    foreach (IFormFile img in newItem.Images)
                    {
                        this._vegetableImageService.UploadImage(img, vegDetailImg.Id, savePath);
                    }
                }
                newItem.IdName = vegName.Id;
                newItem.IdDescription = vegDes.Id;
                newItem.IdFeature = vegFeature.Id;
                newItem.IdImage = vegImg.Id;
            }
            this._vegetableDescriptionRepository.Commit();
            
            return newItem;
        }

        public void Delete(int noVeg, int gardenId)
        {
            var result = this._vegetableRepository.GetMulti(s => s.GardenId == gardenId && s.No == noVeg, new string[] { "VegetableDescription" });
            foreach (var item in result)
            {
                this._vegetableRepository.Delete(item.Id);
            }
        }

        public VegetableResponseModel Get(int noVeg, int gardenId)
        {
            var result = this._vegetableRepository.GetMulti(s => s.GardenId == gardenId && s.No == noVeg, new string[] { "VegetableDescription" });
            return new VegetableResponseModel
            {
                No = noVeg,
                Name = result.Select(s => s.VegetableDescription).Where(s => s.VegetableCompositionId == 1).FirstOrDefault().VegContent,
                Description = result.Select(s => s.VegetableDescription).Where(s => s.VegetableCompositionId == 2).FirstOrDefault().VegContent,
                Feature = result.Select(s => s.VegetableDescription).Where(s => s.VegetableCompositionId == 3).FirstOrDefault().VegContent,
                Images = this._vegetableImageService.Get(result.Select(s => s.VegetableDescription).Where(s => s.VegetableCompositionId == 4).FirstOrDefault().Id),
                IdName = result.Select(s => s.VegetableDescription).Where(s => s.VegetableCompositionId == 1).FirstOrDefault().Id,
                IdDescription = result.Select(s => s.VegetableDescription).Where(s => s.VegetableCompositionId == 2).FirstOrDefault().Id,
                IdFeature = result.Select(s => s.VegetableDescription).Where(s => s.VegetableCompositionId == 3).FirstOrDefault().Id,
                IdImage = result.Select(s => s.VegetableDescription).Where(s => s.VegetableCompositionId == 4).FirstOrDefault().Id
            };
        }

        public IEnumerable<VegetableResponseModel> GetAll()
        {
            int tmpVeg = 1;
            List<VegetableResponseModel> vegetableResponseModels = new List<VegetableResponseModel>();
            string Name = "";
            string Description = "";
            string Feature = "";
            string IdName = "";
            string IdDescription = "";
            string IdFeature = "";
            string IdImage = "";
            int GardenId = 0;
            List <VegetableImage> vegetableImages = new List<VegetableImage>();
            var result = this._vegetableRepository.GetAll(new string[] { "VegetableDescription" }).OrderBy(s => s.No);
            for (int i = 0; i < result.Max(s => s.No); i++)
            {
                vegetableResponseModels.Add(new VegetableResponseModel
                {
                    No = i + 1,
                    Name = result.Where(s => s.No == (i+1)).Select(s => s.VegetableDescription).Where(s => s.VegetableCompositionId == 1).FirstOrDefault().VegContent,
                    Description = result.Where(s => s.No == (i + 1)).Select(s => s.VegetableDescription).Where(s => s.VegetableCompositionId == 2).FirstOrDefault().VegContent,
                    Feature = result.Where(s => s.No == (i + 1)).Select(s => s.VegetableDescription).Where(s => s.VegetableCompositionId == 3).FirstOrDefault().VegContent,
                    //Images = this._vegetableImageService.Get(result.Where(s => s.No == (i + 1)).Select(s => s.VegetableDescription).Where(s => s.VegetableCompositionId == 4).FirstOrDefault().Id),
                    IdName = result.Where(s => s.No == (i + 1)).Select(s => s.VegetableDescription).Where(s => s.VegetableCompositionId == 1).FirstOrDefault().Id,
                    IdDescription = result.Where(s => s.No == (i + 1)).Select(s => s.VegetableDescription).Where(s => s.VegetableCompositionId == 2).FirstOrDefault().Id,
                    IdFeature = result.Where(s => s.No == (i + 1)).Select(s => s.VegetableDescription).Where(s => s.VegetableCompositionId == 3).FirstOrDefault().Id,
                    //IdImage = result.Where(s => s.No == (i + 1)).Select(s => s.VegetableDescription).Where(s => s.VegetableCompositionId == 4).FirstOrDefault().Id,
                });
            }
            return vegetableResponseModels;
        }

        public VegetableRequestModel Update(VegetableRequestModel newItem, string phoneNumber, string savePath)
        {
            int num = 1;
            var account = this._acccountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
            var item = this._vegetableRepository.GetMulti(s => s.GardenId == newItem.GardenId);
            if (item != null)
            {
                num = item.Select(s => s.No).Max() + 1;
            }
            var vegDetailDes = this._vegetableDescriptionRepository.GetSingle(s => s.Id == newItem.IdDescription);
            var vegDetailFeature = this._vegetableDescriptionRepository.GetSingle(s => s.Id == newItem.IdFeature);
            if (newItem.NewFeatture != "")
            {
                if (vegDetailFeature.AccountId != "")
                {
                    vegDetailFeature.VegContent = newItem.NewFeatture;
                    this._vegetableDescriptionRepository.Update(vegDetailFeature);
                }
                else
                {
                    this._vegetableDescriptionRepository.Add(new VegetableDescription
                    {
                        VegContent = newItem.NewFeatture,
                        AccountId = account.Id,
                        VegetableCompositionId = 3
                    });
                }
                vegDetailDes.VegContent = newItem.Description;
                this._vegetableDescriptionRepository.Update(vegDetailDes);
            }
            this._vegetableDescriptionRepository.Commit();
            return newItem;
        }

    }
}

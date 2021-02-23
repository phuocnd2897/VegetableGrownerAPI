using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using VG.Data.Repository;
using VG.Model.Model;
using VG.Model.RequestModel;

namespace VG.Service.Service
{
    public interface IVegetableService
    {
        void Add(VegetableRequestModel newItem, string savePath);
    }
    public class VegetableService : IVegetableService
    {
        private IVegetableRepository _vegetableRepository;
        private IVegetableDescriptionRepository _vegetableDescriptionRepository;
        private IVegetableCompositionRepository _vegetableCompositionRepository;
        private IPostImageService _postImageService;
        public VegetableService(IVegetableRepository vegetableRepository, IVegetableDescriptionRepository vegetableDescriptionRepository, IVegetableCompositionRepository vegetableCompositionRepository, IPostImageService postImageService)
        {
            _vegetableRepository = vegetableRepository;
            _vegetableDescriptionRepository = vegetableDescriptionRepository;
            _vegetableCompositionRepository = vegetableCompositionRepository;
            _postImageService = postImageService;
        }
        public void Add(VegetableRequestModel newItem, string savePath)
        {
            var veg = this._vegetableRepository.Add(new Vegetable
            {
                VegName = newItem.Title,
                GardenId = newItem.GardenId
            });
            this._vegetableDescriptionRepository.Add(new VegetableDescription
            {
                VegContent = newItem.Title,
                VegCompositionId = 1,
                VegetableId = veg.Id
            });
            this._vegetableDescriptionRepository.Add(new VegetableDescription
            {
                VegContent = newItem.Description,
                VegCompositionId = 2,
                VegetableId = veg.Id
            });
            this._vegetableDescriptionRepository.Add(new VegetableDescription
            {
                VegContent = newItem.Featture,
                VegCompositionId = 3,
                VegetableId = veg.Id
            });
            var vegImg = this._vegetableDescriptionRepository.Add(new VegetableDescription
            {
                VegContent = "Image",
                VegCompositionId = 4,
                VegetableId = veg.Id
            });
            foreach (IFormFile item in newItem.Images)
            {
                this._postImageService.UploadImage(item, veg.Id, savePath);
            } 
        }
    }
}

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
    public interface IGardenService
    {
        Garden Add(GardenRequestModel newItem, string phoneNumber);
        GardenRequestModel Update(GardenRequestModel newItem);
        IEnumerable<GardenResponseModel> GetByAccountId(string phoneNumber);
        Garden Get(int Id);
        void Delete(int Id);
    }
    public class GardenService : IGardenService
    {
        private IGardenRepository _gardenRepository;
        private IAccountRepository _accountRepository;
        public GardenService(IGardenRepository gardenRepository, IAccountRepository accountRepository)
        {
            _gardenRepository = gardenRepository;
            _accountRepository = accountRepository;
        }
        public Garden Add(GardenRequestModel newItem, string phoneNumber)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
            var result = this._gardenRepository.Add(new Garden
            {
                Name = newItem.Name,
                ProvinceId = newItem.ProvinceId,
                DistrictId = newItem.DistrictId,
                WardId = newItem.WardId,
                Address = newItem.Address,
                AccountId = account.Id
            });
            this._gardenRepository.Commit();
            return result;
        }

        public void Delete(int Id)
        {
            var result = this._gardenRepository.GetSingle(s => s.Id == Id);
            this._gardenRepository.Delete(result);
            this._gardenRepository.Commit();
        }

        public Garden Get(int Id)
        {
            return this._gardenRepository.GetSingle(s => s.Id == Id);
        }

        public IEnumerable<GardenResponseModel> GetByAccountId(string phoneNumber)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
            return this._gardenRepository.GetMulti(s => s.AccountId == account.Id, new string[] { "Province", "District", "Ward" }).Select(s => new GardenResponseModel
            {
                Id = s.Id,
                Name = s.Name,
                ProvinceId = s.ProvinceId,
                ProvinceName = s.Province.Name,
                DistrictId = s.DistrictId,
                DistrictName = s.District.Name,
                WardId = s.WardId,
                WardName = s.Ward.Name,
                Address = s.Address
            });
        }

        public GardenRequestModel Update(GardenRequestModel newItem)
        {
            var result = this._gardenRepository.GetSingle(s => s.Id == newItem.Id);
            result.Name = newItem.Name;
            result.ProvinceId = newItem.ProvinceId;
            result.DistrictId = newItem.DistrictId;
            result.WardId = newItem.WardId;
            result.Address = newItem.Address;
            this._gardenRepository.Update(result);
            this._gardenRepository.Commit();
            return newItem;
        }

    }
}

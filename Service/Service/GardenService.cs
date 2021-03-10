using System;
using System.Collections.Generic;
using System.Text;
using VG.Data.Repository;
using VG.Model.Model;
using VG.Model.RequestModel;

namespace VG.Service.Service
{
    public interface IGardenService
    {
        Garden Add(GardenRequestModel newItem, string phoneNumber);
        GardenRequestModel Update(GardenRequestModel newItem);
        IEnumerable<Garden> GetAll();
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

        public IEnumerable<Garden> GetAll()
        {
            return this._gardenRepository.GetAll();
        }

        public GardenRequestModel Update(GardenRequestModel newItem)
        {
            var result = this._gardenRepository.GetSingle(s => s.Id == newItem.Id);
            result.Name = newItem.Name;
            result.Address = newItem.Address;
            this._gardenRepository.Update(result);
            this._gardenRepository.Commit();
            return newItem;
        }

    }
}

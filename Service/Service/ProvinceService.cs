using System;
using System.Collections.Generic;
using System.Text;
using VG.Data.Repository;
using VG.Model.Model;

namespace VG.Service.Service
{
    public interface IProvinceService
    {
        IEnumerable<Province> GetAll();
    }
    public class ProvinceService : IProvinceService
    {
        private IProvinceRepository _provinceRepository;
        public ProvinceService(IProvinceRepository provinceRepository)
        {
            _provinceRepository = provinceRepository;
        }
        public IEnumerable<Province> GetAll()
        {
            return this._provinceRepository.GetAll();
        }
    }
}

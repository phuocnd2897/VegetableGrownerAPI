using System;
using System.Collections.Generic;
using System.Text;
using VG.Data.Repository;
using VG.Model.Model;

namespace VG.Service.Service
{
    public interface IDistrictService
    {
        IEnumerable<District> GetByProvinceId(int Id);
    }
    public class DistrictService : IDistrictService
    {
        private IDistrictRepository _districtRepository;
        public DistrictService(IDistrictRepository districtRepository)
        {
            _districtRepository = districtRepository;
        }
        public IEnumerable<District> GetByProvinceId(int Id)
        {
            return this._districtRepository.GetMulti(s => s.ProvinceID == Id);
        }
    }
}

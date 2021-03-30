using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IProvinceRepository : IRepository<Province, int>
    {
    }
    public class ProvinceRepository : RepositoryBase<Province, int>, IProvinceRepository
    {
        public ProvinceRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

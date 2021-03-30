using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IDistrictRepository : IRepository<District, int>
    {
    }
    public class DistrictRepository : RepositoryBase<District, int>, IDistrictRepository
    {
        public DistrictRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

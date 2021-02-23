using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IGardenRepository : IRepository<Garden, int>
    {

    }
    public class GardenRepository : RepositoryBase<Garden, int>, IGardenRepository
    {
        public GardenRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

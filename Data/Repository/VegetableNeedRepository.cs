using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IVegetableShareRepository : IRepository<VegetableShare, int>
    {
    }
    public class VegetableShareRepository : RepositoryBase<VegetableShare, int>, IVegetableShareRepository
    {
        public VegetableShareRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;
using System.Linq;

namespace VG.Data.Repository
{
    public interface IVegetableDescriptionRepository : IRepository<VegetableDescription, string>
    {
    }
    public class VegetableDescriptionRepository : RepositoryBase<VegetableDescription, string>, IVegetableDescriptionRepository
    {
        private VGContext dbContext;
        public VegetableDescriptionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            dbContext = unitOfWork.dbContext;
        }

    }
}

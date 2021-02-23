using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IVegetableCompositionRepository : IRepository<VegetableComposition, int>
    {

    }
    public class VegetableCompositionRepository : RepositoryBase<VegetableComposition, int>, IVegetableCompositionRepository
    {
        public VegetableCompositionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

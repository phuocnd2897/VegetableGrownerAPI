using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IVegetableRepository: IRepository<Vegetable, int>
    {

    }
    public class VegetableRepository : RepositoryBase<Vegetable, int>, IVegetableRepository
    {
        public VegetableRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

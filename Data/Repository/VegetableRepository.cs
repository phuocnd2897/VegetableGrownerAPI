using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IVegetableRepository: IRepository<Vegetable, string>
    {

    }
    public class VegetableRepository : RepositoryBase<Vegetable, string>, IVegetableRepository
    {
        public VegetableRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

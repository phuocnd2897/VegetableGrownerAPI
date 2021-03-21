using Korzh.EasyQuery.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;
using VG.Model.ResponseModel;
using System.Linq;

namespace VG.Data.Repository
{
    public interface IVegetableRepository: IRepository<Vegetable, string>
    {
    }
    public class VegetableRepository : RepositoryBase<Vegetable, string>, IVegetableRepository
    {
        private VGContext dbContext;
        public VegetableRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            dbContext = unitOfWork.dbContext;
        }
    }
}

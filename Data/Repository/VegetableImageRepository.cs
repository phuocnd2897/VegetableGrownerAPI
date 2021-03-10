using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IVegetableImageRepository : IRepository<VegetableImage, string>
    {

    }
    public class VegetableImageRepository : RepositoryBase<VegetableImage, string>, IVegetableImageRepository
    {
        public VegetableImageRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

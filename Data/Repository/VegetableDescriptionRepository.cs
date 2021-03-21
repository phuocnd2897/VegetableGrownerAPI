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
        IEnumerable<string> SearchByDescription(string searchValue);
        IEnumerable<string> SearchByName(string searchValue);
    }
    public class VegetableDescriptionRepository : RepositoryBase<VegetableDescription, string>, IVegetableDescriptionRepository
    {
        private VGContext dbContext;
        public VegetableDescriptionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            dbContext = unitOfWork.dbContext;
        }

        public IEnumerable<string> SearchByDescription(string searchValue)
        {
            var result = from veg in this.dbContext.Vegetables
                         join vegDes in this.dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == 2) on veg.VegetableDescriptionId equals vegDes.Id
                         where vegDes.VegContent.Contains(searchValue)
                         select veg.Id;
            return result;


        }

        public IEnumerable<string> SearchByName(string searchValue)
        {
            var result = from veg in this.dbContext.Vegetables
                         join vegDes in this.dbContext.VegetableDescriptions.Where(s => s.VegetableCompositionId == 1) on veg.VegetableDescriptionId equals vegDes.Id
                         where vegDes.VegContent.ToLower().Contains(searchValue.ToLower())
                         select veg.Id;
            return result;
        }
    }
}

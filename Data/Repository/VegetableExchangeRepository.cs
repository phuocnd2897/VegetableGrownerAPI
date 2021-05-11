using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IVegetableExchangeRepository : IRepository<VegetableExchange, int>
    {
    }
    public class VegetableExchangeRepository : RepositoryBase<VegetableExchange, int>, IVegetableExchangeRepository
    {
        public VegetableExchangeRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

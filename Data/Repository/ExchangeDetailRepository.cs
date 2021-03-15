using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IExchangeDetailRepository : IRepository<ExchangeDetail, string>
    {

    }
    public class ExchangeDetailRepository : RepositoryBase<ExchangeDetail, string>, IExchangeDetailRepository
    {
        public ExchangeDetailRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

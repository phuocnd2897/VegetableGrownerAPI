using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IAccountDetailRepository : IRepository<AccountDetail, int>
    {
    }
    public class AccountDetailRepository : RepositoryBase<AccountDetail, int>, IAccountDetailRepository
    {
        public AccountDetailRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

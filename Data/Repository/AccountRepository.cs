using VG.Context;
using VG.Model.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace VG.Data.Repository
{
    public interface IAccountRepository : IRepository<Account, string>
    {
    }
    public class AccountRepository : RepositoryBase<Account, string>, IAccountRepository
    {
        public AccountRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

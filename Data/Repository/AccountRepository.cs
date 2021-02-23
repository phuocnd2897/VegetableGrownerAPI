using VG.Context;
using VG.Model.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace VG.Data.Repository
{
    public interface IAccountRepository : IRepository<AppAccount, string>
    {
    }
    public class AccountRepository : RepositoryBase<AppAccount, string>, IAccountRepository
    {
        public AccountRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

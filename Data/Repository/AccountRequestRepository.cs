using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IAccountRequestRepository : IRepository<AccountRequest, int>
    {

    }
    public class AccountRequestRepository : RepositoryBase<AccountRequest, int>, IAccountRequestRepository
    {
        public AccountRequestRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

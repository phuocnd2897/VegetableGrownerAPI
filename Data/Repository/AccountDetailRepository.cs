using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IAccountDetailRepository : IRepository<Member, int>
    {
    }
    public class AccountDetailRepository : RepositoryBase<Member, int>, IAccountDetailRepository
    {
        public AccountDetailRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

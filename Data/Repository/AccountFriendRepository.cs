using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IAccountFriendRepository : IRepository<AccountFriend, int>
    {

    }
    public class AccountFriendRepository : RepositoryBase<AccountFriend, int>, IAccountFriendRepository
    {
        public AccountFriendRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

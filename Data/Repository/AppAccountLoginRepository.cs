using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IAppAccountLoginRepository : IRepository<AppAccountLogin, string>
    {
    }
    public class AppAccountLoginRepository : RepositoryBase<AppAccountLogin, string>, IAppAccountLoginRepository
    {
        public AppAccountLoginRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

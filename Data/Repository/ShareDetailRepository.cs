using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IShareDetailRepository : IRepository<ShareDetail, string>
    {

    }
    public class ShareDetailRepository : RepositoryBase<ShareDetail, string>, IShareDetailRepository
    {
        public ShareDetailRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

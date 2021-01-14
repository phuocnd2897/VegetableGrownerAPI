using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IPostRepository : IRepository<Post, string>
    {

    }
    public class PostRepository : RepositoryBase<Post, string>, IPostRepository
    {
        public PostRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

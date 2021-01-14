using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IPostImageRepository :IRepository<PostImage, string>
    {

    }
    public class PostImageRepository : RepositoryBase<PostImage, string>, IPostImageRepository
    {
        public PostImageRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

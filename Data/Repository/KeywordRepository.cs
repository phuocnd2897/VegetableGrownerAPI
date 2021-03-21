using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IKeywordRepository : IRepository<Keyword, int>
    {

    }
    public class KeywordRepository : RepositoryBase<Keyword, int>, IKeywordRepository
    {
        public KeywordRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

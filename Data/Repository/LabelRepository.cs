using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface ILabelRepository : IRepository<Label, int>
    {

    }
    public class LabelRepository : RepositoryBase<Label, int>, ILabelRepository
    {
        public LabelRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

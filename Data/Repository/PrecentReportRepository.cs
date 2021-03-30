using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IPrecentReportRepository : IRepository<PrecentReport, int>
    {

    }
    public class PrecentReportRepository : RepositoryBase<PrecentReport, int>, IPrecentReportRepository
    {
        public PrecentReportRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IReportRepository : IRepository<Report, string>
    {

    }
    public class ReportRepository : RepositoryBase<Report, string>, IReportRepository
    {
        public ReportRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

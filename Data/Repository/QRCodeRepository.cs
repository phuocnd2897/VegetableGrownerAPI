using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IQRCodeRepository : IRepository<QRCodeExchange, string>
    {

    }
    public class QRCodeRepository : RepositoryBase<QRCodeExchange, string>, IQRCodeRepository
    {
        public QRCodeRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

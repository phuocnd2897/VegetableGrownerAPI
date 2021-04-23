using System;
using System.Collections.Generic;
using System.Text;
using VG.Context;
using VG.Model.Model;

namespace VG.Data.Repository
{
    public interface IQRCodeForShipperRepository : IRepository<QRCodeForShipper, string>
    {

    }
    public class QRCodeForShipperRepository : RepositoryBase<QRCodeForShipper, string>, IQRCodeForShipperRepository
    {
        public QRCodeForShipperRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}

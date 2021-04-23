using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using VG.Data.Repository;
using VG.Model.Model;

namespace VG.Service.Service
{
    public interface IQRCodeForShipperService
    {
        QRCodeForShipper Add(string ExchangeId, string baseUrl);
        QRCodeForShipper Get(string ExchangeId);
    }
    public class QRCodeForShipperService : IQRCodeForShipperService
    {
        private IQRCodeForShipperRepository _qrCodeForShipperRepository;
        private const string URI = "http://54.179.74.214:5400/";
        public QRCodeForShipperService(IQRCodeForShipperRepository qrCodeForShipperRepository)
        {
            _qrCodeForShipperRepository = qrCodeForShipperRepository;
        }
        public QRCodeForShipper Add(string ExchangeId, string baseUrl)
        {
            string imageName = "";
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(URI + "?Id=" + ExchangeId, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                imageName = DateTime.Now.ToString("yyyyMmddHHmmssfff");
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                qrCodeImage.Save("wwwroot/QRCodeForShipper/" + imageName + ".png", ImageFormat.Png);
            }
            var result = this._qrCodeForShipperRepository.Add(new QRCodeForShipper
            {
                Name = imageName,
                Url = baseUrl + "/QRCodeForShipper//" + imageName + ".png",
                ExchangeId = ExchangeId
            });
            this._qrCodeForShipperRepository.Commit();
            return result;
        }

        public QRCodeForShipper Get(string ExchangeId)
        {
            return this._qrCodeForShipperRepository.GetSingle(s => s.ExchangeId == ExchangeId);
        }
    }
}

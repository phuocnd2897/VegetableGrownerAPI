using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using VG.Data.Repository;
using VG.Model.Model;

namespace VG.Service.Service
{
    public interface IQRCodeService
    {
        QRCodeExchange Add(string ExchangeId, string baseUrl);
        QRCodeExchange Get(string ExchangeId);
    }
    public class QRCodeService : IQRCodeService
    {
        private IQRCodeRepository _qrCodeRepository;
        public QRCodeService(IQRCodeRepository qrCodeRepository)
        {
            _qrCodeRepository = qrCodeRepository;
        }
        public QRCodeExchange Add(string ExchangeId, string baseUrl)
        {
            string imageName = "";
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(ExchangeId, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                imageName = DateTime.Now.ToString("yyyyMmddHHmmssfff");
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                qrCodeImage.Save("wwwroot/QRCode/" + imageName + ".png", ImageFormat.Png);
            }
            var result = this._qrCodeRepository.Add(new QRCodeExchange
            {
                Name = imageName,
                Url = baseUrl + "/QRCode//" + imageName + ".png",
                ExchangeId = ExchangeId
            });
            this._qrCodeRepository.Commit();
            return result;
        }

        public QRCodeExchange Get(string ExchangeId)
        {
            return this._qrCodeRepository.GetSingle(s => s.ExchangeId == ExchangeId);
        }
    }
}

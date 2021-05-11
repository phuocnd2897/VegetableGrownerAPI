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
        QRCodeExchange Add(string ExchangeId, int type, string baseUrl);
        string Get(string ExchangeId, int type, string phoneNumber);
    }
    public class QRCodeService : IQRCodeService
    {
        private IQRCodeRepository _qrCodeRepository;
        private IAccountRepository _accountRepository;
        private const string URI = "http://54.179.74.214:5400/";
        public QRCodeService(IQRCodeRepository qrCodeRepository, IAccountRepository accountRepository)
        {
            _qrCodeRepository = qrCodeRepository;
            _accountRepository = accountRepository;
        }
        public QRCodeExchange Add(string ExchangeId, int type, string baseUrl)
        {
            QRCodeExchange result = null;
            string imageName = "";
            switch (type)
            {
                case 1:
                    using (MemoryStream ms = new MemoryStream())
                    {
                        QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(ExchangeId, QRCodeGenerator.ECCLevel.Q);
                        QRCode qrCode = new QRCode(qrCodeData);
                        imageName = DateTime.Now.ToString("yyyyMmddHHmmssfff");
                        Bitmap qrCodeImage = qrCode.GetGraphic(20);
                        qrCodeImage.Save("wwwroot/QRCode/" + imageName + ".png", ImageFormat.Png);
                    }
                    result = this._qrCodeRepository.Add(new QRCodeExchange
                    {
                        Name = imageName,
                        Type = type,
                        Url = baseUrl + "/QRCode//" + imageName + ".png",
                        ExchangeId = ExchangeId
                    });
                    break;
                case 2:
                    using (MemoryStream ms = new MemoryStream())
                    {
                        QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(URI + "?Id=" + ExchangeId, QRCodeGenerator.ECCLevel.Q);
                        QRCode qrCode = new QRCode(qrCodeData);
                        imageName = DateTime.Now.ToString("yyyyMmddHHmmssfff");
                        Bitmap qrCodeImage = qrCode.GetGraphic(20);
                        qrCodeImage.Save("wwwroot/QRCodeForShipper/" + imageName + ".png", ImageFormat.Png);
                    }
                    result = this._qrCodeRepository.Add(new QRCodeExchange
                    {
                        Name = imageName,
                        Type = type,
                        Url = baseUrl + "/QRCodeForShipper//" + imageName + ".png",
                        ExchangeId = ExchangeId
                    });
                    break;
            }
            this._qrCodeRepository.Commit();
            return result;
        }

        public string Get(string ExchangeId, int type, string phoneNumber)
        {
            var QrCode = this._qrCodeRepository.GetSingle(s => s.ExchangeId == ExchangeId && s.Type == type, new string[] { "ExchangeDetail" });
            if (type == 2 && phoneNumber != "")
            {
                var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
                if (account.Id != QrCode.ExchangeDetail.Sender)
                {
                    throw new Exception("Bạn không phải là người gửi. Không thể xem QR Code của giao dịch này");
                }
            }
            return QrCode.Url;
        }
    }
}

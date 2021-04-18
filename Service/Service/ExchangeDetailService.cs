using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VG.Common.Constant;
using VG.Common.Helper;
using VG.Data.Repository;
using VG.Model.Model;
using VG.Model.RequestModel;
using VG.Model.ResponseModel;

namespace VG.Service.Service
{
    public interface IExchangeDetailService
    {
        IEnumerable<ExchangeDetailResponseModel> Add(ExchangeDetailRequestModel newItem, string phoneNumber, string baseUrl);
        ExchangeDetailRequestModel Update(ExchangeDetailRequestModel newItem);
        void IsAccept(string Id, int Status, string baseUrl);
        void Delete(string Id);
        ExchangeDetailResponseModel Get(string Id);
        IEnumerable<ExchangeDetailResponseModel> GetByAccountId(string phoneNumber);
        IEnumerable<ExchangeDetailResponseModel> GetExchangeRequest(string phoneNumber);
    }
    public class ExchangeDetailService : IExchangeDetailService
    {
        private IExchangeDetailRepository _exchangeDetailRepository;
        private IAccountRepository _accountRepository;
        private IShareDetailRepository _shareDetailRepository;
        private IVegetableRepository _vegetableRepository;
        private IQRCodeService _qrCodeService;
        public ExchangeDetailService(IAccountRepository accountRepository, IExchangeDetailRepository exchangeDetailRepository, IShareDetailRepository shareDetailRepository,
            IVegetableRepository vegetableRepository, IQRCodeService qrCodeService)
        {
            _accountRepository = accountRepository;
            _exchangeDetailRepository = exchangeDetailRepository;
            _shareDetailRepository = shareDetailRepository;
            _vegetableRepository = vegetableRepository;
            _qrCodeService = qrCodeService;
        }

        public void IsAccept(string Id, int Status, string baseUrl)
        {
            string vegNameSend = "";
            string vegNameReceive = "";
            var exchange = this._exchangeDetailRepository.GetSingle(s => s.Id == Id, new string[] { "ShareDetail" });
            var account = this._accountRepository.GetSingle(s => s.Id == exchange.ReceiveBy, new string[] { "Members" });
            if (account.Status == false)
            {
                throw new Exception("Tài khoản " + account.Members.FirstOrDefault().FullName + " đã bị khoá");
            }
            if (exchange.Quantity <= exchange.ShareDetail.Quantity)
            {
                var exchangeResponse = this._exchangeDetailRepository.GetMulti(s => s.ShareDetailId == exchange.ShareDetailId && s.ReceiveBy == exchange.Sender && s.Sender == exchange.ReceiveBy);
                if (exchangeResponse.Count() > 0)
                {
                    exchangeResponse.FirstOrDefault().Status = Status;
                    if (Status == (int)EnumStatusRequest.Accept)
                    {
                        var veg = this._vegetableRepository.GetSingle(s => s.Id == exchangeResponse.FirstOrDefault().VegetableId, new string[] { "VegetableDescription", "ShareDetails" });
                        if (veg.ShareDetails.Count() > 0)
                        {
                            foreach (var share in veg.ShareDetails)
                            {
                                share.Quantity = share.Quantity - exchangeResponse.FirstOrDefault().Quantity;
                                this._shareDetailRepository.Update(share);
                            }
                        }
                        veg.Quantity = veg.Quantity - exchangeResponse.FirstOrDefault().Quantity;
                        this._vegetableRepository.Update(veg);
                        var qrCode = this._qrCodeService.Add(exchangeResponse.FirstOrDefault().Id, baseUrl);
                        vegNameReceive = veg.VegetableDescription.VegContent;
                    }
                    this._exchangeDetailRepository.Update(exchangeResponse);
                }
                exchange.Status = Status;
                if (Status == (int)EnumStatusRequest.Accept)
                {
                    exchange.ShareDetail.Quantity = exchange.ShareDetail.Quantity - exchange.Quantity;
                    var veg = this._vegetableRepository.GetSingle(s => s.Id == exchange.VegetableId, new string[] { "VegetableDescription" });
                    veg.Quantity = veg.Quantity - exchange.Quantity;
                    this._vegetableRepository.Update(veg);
                    var qrCode = this._qrCodeService.Add(exchange.Id, baseUrl);
                    vegNameSend = veg.VegetableDescription.VegContent;
                    var appAccountLogin = this._accountRepository.GetSingle(s => s.Id == exchange.ReceiveBy, new string[] { "AppAccountLogins", "Members" });
                    var accountHost = this._accountRepository.GetSingle(s => s.Id == exchange.Sender, new string[] { "Members" }).Members.FirstOrDefault();
                    var mess = IdentityHelper.NotifyAsync(appAccountLogin.AppAccountLogins.Select(s => s.DeviceToken).ToArray(),
                        "Yêu cầu đã được đồng ý",
                        accountHost.FullName + " đã đồng ý gửi cho bạn " + exchange.Quantity + " " + vegNameSend +
                        vegNameReceive != "" ? " và nhận lại " + exchangeResponse.FirstOrDefault().Quantity + " " + vegNameReceive : "");
                }
                else if (Status == (int)EnumStatusRequest.Reject)
                {
                    var appAccountLogin = this._accountRepository.GetSingle(s => s.Id == exchange.ReceiveBy, new string[] { "AppAccountLogins", "Members" });
                    var accountHost = this._accountRepository.GetSingle(s => s.Id == exchange.Sender, new string[] { "Members" }).Members.FirstOrDefault();
                    var mess = IdentityHelper.NotifyAsync(appAccountLogin.AppAccountLogins.Select(s => s.DeviceToken).ToArray(),
                        "Yêu cầu bị từ chối",
                        accountHost.FullName + " đã từ chối yêu cầu của bạn");
                }
                else if (Status == (int)EnumStatusRequest.Finish)
                {
                    var appAccountLogin = this._accountRepository.GetSingle(s => s.Id == exchange.Sender, new string[] { "AppAccountLogins", "Members" });
                    var accountReceive = this._accountRepository.GetSingle(s => s.Id == exchange.ReceiveBy, new string[] { "Members" }).Members.FirstOrDefault();
                    var mess = IdentityHelper.NotifyAsync(appAccountLogin.AppAccountLogins.Select(s => s.DeviceToken).ToArray(),
                        "Yêu cầu đã hoàn thành",
                        "Yêu cầu của " + accountReceive.FullName + " đã được hoàn thành");
                }
                this._exchangeDetailRepository.Update(exchange);
                this._shareDetailRepository.Update(exchange.ShareDetail);
                this._exchangeDetailRepository.Commit();
            }
            else
            {
                throw new Exception("Số lượng rau nhận lớn hơn số lượng rau được chia sẻ");
            }
        }

        public IEnumerable<ExchangeDetailResponseModel> Add(ExchangeDetailRequestModel newItem, string phoneNumber, string baseUrl)
        {
            List<ExchangeDetailResponseModel> exchangeDetailResponseModels = new List<ExchangeDetailResponseModel>();
            var share = this._shareDetailRepository.GetSingle(s => s.Id == newItem.ShareDetailId, new string[] { "Vegetable.VegetableDescription" });
            if (share.Quantity > 0)
            {
                var accountHost = this._accountRepository.GetSingle(s => s.Id == share.AccountId, new string[] { "Members" });
                var accountReceiver = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber, new string[] { "Members" });
                if (newItem.VegetableId != "")
                {
                    var exchangeSend = this._exchangeDetailRepository.Add(new ExchangeDetail
                    {
                        Status = (int)EnumStatusRequest.Pending,
                        DateExchange = DateTime.UtcNow.AddHours(7),
                        Quantity = newItem.Quantity,
                        Sender = accountHost.Id,
                        ReceiveBy = accountReceiver.Id,
                        ShareDetailId = newItem.ShareDetailId,
                        VegetableId = share.VegetableId
                    });
                    exchangeDetailResponseModels.Add(new ExchangeDetailResponseModel
                    {
                        Id = exchangeSend.Id,
                        Quantity = newItem.Quantity,
                        Status = (int)EnumStatusRequest.Pending,
                        FullNameHost = accountHost.Members.FirstOrDefault().FullName,
                        FullNameReceiver = accountReceiver.Members.FirstOrDefault().FullName,
                        AccountHostId = accountHost.Id,
                        ReceiverId = accountReceiver.Id,
                        ShareDetailId = share.Id,
                        CreatedDate = exchangeSend.DateExchange
                    });
                    var exchangeResponse = this._exchangeDetailRepository.Add(new ExchangeDetail
                    {
                        Status = (int)EnumStatusRequest.Pending,
                        DateExchange = DateTime.UtcNow.AddHours(7),
                        Quantity = newItem.QuantityExchange,
                        Sender = accountReceiver.Id,
                        ReceiveBy = accountHost.Id,
                        ShareDetailId = newItem.ShareDetailId,
                        VegetableId = newItem.VegetableId
                    });
                    exchangeDetailResponseModels.Add(new ExchangeDetailResponseModel
                    {
                        Id = exchangeResponse.Id,
                        Quantity = newItem.QuantityExchange,
                        Status = (int)EnumStatusRequest.Pending,
                        FullNameHost = accountReceiver.Members.FirstOrDefault().FullName,
                        FullNameReceiver = accountHost.Members.FirstOrDefault().FullName,
                        AccountHostId = accountReceiver.Id,
                        ReceiverId = accountHost.Id,
                        ShareDetailId = share.Id,
                        CreatedDate = exchangeResponse.DateExchange
                    });
                    var veg = this._vegetableRepository.GetSingle(s => s.Id == exchangeResponse.VegetableId, new string[] { "VegetableDescription" });
                    var appAccountLogin = this._accountRepository.GetSingle(s => s.Id == accountHost.Id, new string[] { "AppAccountLogins", "Members" });
                    var mess = IdentityHelper.NotifyAsync(appAccountLogin.AppAccountLogins.Select(s => s.DeviceToken).ToArray(),
                        "Bạn nhận được yêu cầu mới",
                        accountReceiver.Members.FirstOrDefault().FullName + " yêu cầu nhận " + newItem.Quantity + " " + share.Vegetable.VegetableDescription.VegContent + " từ bạn. " +
                        " và gửi lại " + exchangeResponse.Quantity + " " + veg.VegetableDescription.VegContent);
                }
                else
                {
                    var resullt = this._exchangeDetailRepository.Add(new ExchangeDetail
                    {
                        Status = (int)EnumStatusRequest.Accept,
                        DateExchange = DateTime.UtcNow.AddHours(7),
                        Quantity = newItem.Quantity,
                        Sender = accountHost.Id,
                        ReceiveBy = accountReceiver.Id,
                        ShareDetailId = newItem.ShareDetailId,
                        VegetableId = share.VegetableId
                    });
                    exchangeDetailResponseModels.Add(new ExchangeDetailResponseModel
                    {
                        Id = resullt.Id,
                        Quantity = newItem.Quantity,
                        Status = (int)EnumStatusRequest.Accept,
                        FullNameHost = accountHost.Members.FirstOrDefault().FullName,
                        FullNameReceiver = accountReceiver.Members.FirstOrDefault().FullName,
                        AccountHostId = accountHost.Id,
                        ReceiverId = accountReceiver.Id,
                        ShareDetailId = share.Id,
                        CreatedDate = resullt.DateExchange
                    });
                    share.Quantity = share.Quantity - resullt.Quantity;
                    var veg = this._vegetableRepository.GetSingle(s => s.Id == resullt.VegetableId, new string[] { "VegetableDescription" });
                    veg.Quantity = veg.Quantity - resullt.Quantity;
                    this._vegetableRepository.Update(veg);
                    this._shareDetailRepository.Update(share);
                    var qrCode = this._qrCodeService.Add(resullt.Id, baseUrl);
                    var appAccountLogin = this._accountRepository.GetSingle(s => s.Id == accountHost.Id, new string[] { "AppAccountLogins", "Members" });
                    var mess = IdentityHelper.NotifyAsync(appAccountLogin.AppAccountLogins.Select(s => s.DeviceToken).ToArray(),
                        "Có tài khoản nhận rau từ bạn",
                        accountReceiver.Members.FirstOrDefault().FullName + " đã nhận " + newItem.Quantity + " " + share.Vegetable.VegetableDescription.VegContent + " từ bạn.");
                }
                this._exchangeDetailRepository.Commit();
            }
            else
            {
                throw new Exception("Số lượng cho đã hết.");
            }
            return exchangeDetailResponseModels;
        }

        public ExchangeDetailRequestModel Update(ExchangeDetailRequestModel newItem)
        {
            var result = this._exchangeDetailRepository.GetSingle(s => s.Id == newItem.Id);
            result.Quantity = newItem.Quantity;
            this._exchangeDetailRepository.Update(result);
            this._exchangeDetailRepository.Commit();
            return newItem;
        }

        public void Delete(string Id)
        {
            var result = this._exchangeDetailRepository.GetSingle(s => s.Id == Id);
            this._exchangeDetailRepository.Delete(result);
            this._exchangeDetailRepository.Commit();
        }

        public ExchangeDetailResponseModel Get(string Id)
        {
            var result = this._exchangeDetailRepository.GetSingle(S => S.Id == Id, new string[] { "ShareDetail" });
            var accountHost = this._accountRepository.GetSingle(s => s.Id == result.ShareDetail.AccountId, new string[] { "Members" });
            var accountReceiver = this._accountRepository.GetSingle(s => s.Id == result.ReceiveBy, new string[] { "Members" });
            return new ExchangeDetailResponseModel
            {
                Id = result.Id,
                Quantity = result.Quantity,
                Status = result.Status,
                AccountHostId = result.ShareDetail.AccountId,
                ReceiverId = result.ReceiveBy,
                FullNameReceiver = accountReceiver.Members.FirstOrDefault().FullName,
                CreatedDate = result.DateExchange,
                FullNameHost = accountHost.Members.FirstOrDefault().FullName,
                ShareDetailId = result.ShareDetailId
            };
        }

        public IEnumerable<ExchangeDetailResponseModel> GetByAccountId(string phoneNumber)
        {
            var result = this._exchangeDetailRepository.GetByAccountId(phoneNumber);
            return result.OrderByDescending(s => s.CreatedDate);
        }

        public IEnumerable<ExchangeDetailResponseModel> GetExchangeRequest(string phoneNumber)
        {
            var result = this._exchangeDetailRepository.GetExchangeRequest(phoneNumber);
            return result.OrderByDescending(s => s.CreatedDate);
        }
    }
}

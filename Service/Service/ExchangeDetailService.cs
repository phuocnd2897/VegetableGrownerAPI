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
        void Finish(string Id, string phoneNumber);
        void Delete(string Id);
        ExchangeDetailResponseModel Get(string Id);
        IEnumerable<ExchangeDetailResponseModel> GetByAccountId(string phoneNumber);
        IEnumerable<ExchangeDetailResponseModel> GetExchangeRequest(string phoneNumber);
        ExchangeDetailResponseModel GetInformationExchange(string Id);
        IEnumerable<ExchangeDetailResponseModel> CheckInstance(ExchangeDetailRequestModel newItem, string phoneNumber, string baseUrl);
    }
    public class ExchangeDetailService : IExchangeDetailService
    {
        private IExchangeDetailRepository _exchangeDetailRepository;
        private IAccountRepository _accountRepository;
        private IPostRepository _postRepository;
        private IVegetableRepository _vegetableRepository;
        private IQRCodeService _qrCodeService;
        public ExchangeDetailService(IAccountRepository accountRepository, IExchangeDetailRepository exchangeDetailRepository, IPostRepository postRepository,
            IVegetableRepository vegetableRepository, IQRCodeService qrCodeService)
        {
            _accountRepository = accountRepository;
            _exchangeDetailRepository = exchangeDetailRepository;
            _postRepository = postRepository;
            _vegetableRepository = vegetableRepository;
            _qrCodeService = qrCodeService;
        }
       
        public void IsAccept(string Id, int Status, string baseUrl)
        {
            string vegNameSend = "";
            string vegNameReceive = "";
            var exchange = this._exchangeDetailRepository.GetSingle(s => s.Id == Id, new string[] { "postDetail" });
            var account = this._accountRepository.GetSingle(s => s.Id == exchange.ReceiveBy, new string[] { "Members" });
            if (account.Status == false)
            {
                throw new Exception("Tài khoản " + account.Members.FirstOrDefault().FullName + " đã bị khoá");
            }

            var exchangeResponse = this._exchangeDetailRepository.GetMulti(s => s.PostId == exchange.PostId && s.Stt == exchange.Stt && s.ReceiveBy == exchange.Sender && s.Sender == exchange.ReceiveBy);
            if (exchangeResponse.Count() > 0)
            {
                exchangeResponse.FirstOrDefault().Status = Status;
                if (Status == (int)EnumStatusRequest.Accept)
                {
                    var veg = this._vegetableRepository.GetSingle(s => s.Id == exchangeResponse.FirstOrDefault().VegetableId, new string[] { "VegetableDescription", "postDetails" });
                    if (veg.Quantity >= exchangeResponse.FirstOrDefault().Quantity)
                    {
                        if (veg.Posts.Count() > 0)
                        {
                            foreach (var post in veg.Posts)
                            {
                                post.Quantity = post.Quantity - exchangeResponse.FirstOrDefault().Quantity;
                                this._postRepository.Update(post);
                            }
                        }
                        veg.Quantity = veg.Quantity - exchangeResponse.FirstOrDefault().Quantity;
                        this._vegetableRepository.Update(veg);
                        var qrCodeForShipper = this._qrCodeService.Add(exchangeResponse.FirstOrDefault().Id, 2, baseUrl);
                        var qrCodeExchange = this._qrCodeService.Add(exchangeResponse.FirstOrDefault().Id, 1, baseUrl);
                        vegNameReceive = veg.VegetableDescription.VegContent;
                    }
                    else
                    {
                        throw new Exception("Tài khoản " + account.Members.FirstOrDefault().FullName + " không còn đủ số lượng " + veg.VegetableDescription.VegContent );
                    }
                }
                this._exchangeDetailRepository.Update(exchangeResponse);
            }
            exchange.Status = Status;

            if (Status == (int)EnumStatusRequest.Accept)
            {
                if (exchange.Post.Quantity >= exchange.Quantity)
                {
                    exchange.Post.Quantity = exchange.Post.Quantity - exchange.Quantity;
                    var veg = this._vegetableRepository.GetSingle(s => s.Id == exchange.VegetableId, new string[] { "VegetableDescription" });
                    veg.Quantity = veg.Quantity - exchange.Quantity;
                    this._vegetableRepository.Update(veg);
                    var qrCodeForShipper = this._qrCodeService.Add(exchangeResponse.FirstOrDefault().Id, 1, baseUrl);
                    var qrCodeExchange = this._qrCodeService.Add(exchangeResponse.FirstOrDefault().Id, 2, baseUrl);
                    vegNameSend = veg.VegetableDescription.VegContent;
                    var appAccountLogin = this._accountRepository.GetSingle(s => s.Id == exchange.ReceiveBy, new string[] { "AppAccountLogins", "Members" });
                    var accountHost = this._accountRepository.GetSingle(s => s.Id == exchange.Sender, new string[] { "Members" }).Members.FirstOrDefault();
                    string body = accountHost.FullName + " đã đồng ý gửi cho bạn " + exchange.Quantity + " " + vegNameSend + " ";
                    body += vegNameReceive != "" ? "và nhận lại " + exchangeResponse.FirstOrDefault().Quantity + " " + vegNameReceive : "";
                    var mess = IdentityHelper.NotifyAsync(appAccountLogin.AppAccountLogins.Select(s => s.DeviceToken).ToArray(),
                        "Yêu cầu đã được đồng ý", body);
                }
                else
                {
                    throw new Exception("Số lượng rau nhận lớn hơn số lượng rau được chia sẻ");
                }
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
            this._postRepository.Update(exchange.Post);
            this._exchangeDetailRepository.Commit();
        }

        public IEnumerable<ExchangeDetailResponseModel> Add(ExchangeDetailRequestModel newItem, string phoneNumber, string baseUrl)
        {
            List<ExchangeDetailResponseModel> exchangeDetailResponseModels = new List<ExchangeDetailResponseModel>();
            var stt = this._exchangeDetailRepository.GetMaxStt(s => s.Stt) + 1;
            var post = this._postRepository.GetSingle(s => s.Id == newItem.PostId, new string[] { "Vegetable.VegetableDescription" });
            if (post.Quantity > 0)
            {
                var accountHost = this._accountRepository.GetSingle(s => s.Id == post.AccountId, new string[] { "Members", "AppAccountLogins" });
                var accountReceiver = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber, new string[] { "Members" });
                if (newItem.VegetableId != "")
                {
                    var veg = this._vegetableRepository.GetSingle(s => s.Id == newItem.VegetableId, new string[] { "VegetableDescription" });
                    if (newItem.Quantity > post.Quantity)
                    {
                        throw new Exception("Số lượng cho không đủ. Vui lòng nhập lại");
                    }
                    if (newItem.QuantityExchange > veg.Quantity)
                    {
                        throw new Exception("Số lượng bạn sở hữu không đủ. Vui lòng nhập lại");
                    }
                    var exchangeSend = this._exchangeDetailRepository.Add(new ExchangeDetail
                    {
                        Stt = stt,
                        Status = (int)EnumStatusRequest.Pending,
                        DateExchange = DateTime.UtcNow.AddHours(7),
                        Quantity = newItem.Quantity,
                        Sender = accountHost.Id,
                        ReceiveBy = accountReceiver.Id,
                        ProvinceId = newItem.ProvinceId,
                        DistrictId = newItem.DistrictId,
                        WardId = newItem.WardId,
                        Address = newItem.Address,
                        PostId = newItem.PostId,
                        VegetableId = post.VegetableId
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
                        PostId = post.Id,
                        CreatedDate = exchangeSend.DateExchange
                    });
                    var exchangeResponse = this._exchangeDetailRepository.Add(new ExchangeDetail
                    {
                        Stt = stt,
                        Status = (int)EnumStatusRequest.Pending,
                        DateExchange = DateTime.UtcNow.AddHours(7),
                        Quantity = newItem.QuantityExchange,
                        Sender = accountReceiver.Id,
                        ReceiveBy = accountHost.Id,
                        ProvinceId = post.ProvinceId,
                        DistrictId = post.DistrictId,
                        WardId = post.WardId,
                        Address = post.Address,
                        PostId = newItem.PostId,
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
                        PostId = post.Id,
                        CreatedDate = exchangeResponse.DateExchange
                    });
                    var mess = IdentityHelper.NotifyAsync(accountHost.AppAccountLogins.Select(s => s.DeviceToken).ToArray(),
                        "Bạn nhận được yêu cầu mới",
                        accountReceiver.Members.FirstOrDefault().FullName + " yêu cầu nhận " + newItem.Quantity + " " + post.Vegetable.VegetableDescription.VegContent + " từ bạn" +
                        " và gửi lại " + exchangeResponse.Quantity + " " + veg.VegetableDescription.VegContent);
                }
                else
                {
                    if (newItem.Quantity > post.Quantity)
                    {
                        throw new Exception("Số lượng cho không đủ. Vui lòng nhập lại");
                    }
                    var resullt = this._exchangeDetailRepository.Add(new ExchangeDetail
                    {
                        Stt = stt,
                        Status = (int)EnumStatusRequest.Accept,
                        DateExchange = DateTime.UtcNow.AddHours(7),
                        Quantity = newItem.Quantity,
                        Sender = accountHost.Id,
                        ReceiveBy = accountReceiver.Id,
                        ProvinceId = newItem.ProvinceId,
                        DistrictId = newItem.DistrictId,
                        WardId = newItem.WardId,
                        Address = newItem.Address,
                        PostId = newItem.PostId,
                        VegetableId = post.VegetableId
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
                        PostId = post.Id,
                        CreatedDate = resullt.DateExchange
                    });
                    post.Quantity = post.Quantity - resullt.Quantity;
                    var veg = this._vegetableRepository.GetSingle(s => s.Id == resullt.VegetableId, new string[] { "VegetableDescription" });
                    veg.Quantity = veg.Quantity - resullt.Quantity;
                    this._vegetableRepository.Update(veg);
                    this._postRepository.Update(post);
                    var qrCodeForShipper = this._qrCodeService.Add(resullt.Id, 2, baseUrl);
                    var qrCodeExchange = this._qrCodeService.Add(resullt.Id, 1, baseUrl);
                    var mess = IdentityHelper.NotifyAsync(accountHost.AppAccountLogins.Select(s => s.DeviceToken).ToArray(),
                        "Có tài khoản nhận rau từ bạn",
                        accountReceiver.Members.FirstOrDefault().FullName + " đã nhận " + newItem.Quantity + " " + post.Vegetable.VegetableDescription.VegContent + " từ bạn.");
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
            var result = this._exchangeDetailRepository.GetSingle(S => S.Id == Id, new string[] { "postDetail" });
            var accountHost = this._accountRepository.GetSingle(s => s.Id == result.Post.AccountId, new string[] { "Members" });
            var accountReceiver = this._accountRepository.GetSingle(s => s.Id == result.ReceiveBy, new string[] { "Members" });
            return new ExchangeDetailResponseModel
            {
                Id = result.Id,
                Quantity = result.Quantity,
                Status = result.Status,
                AccountHostId = result.Post.AccountId,
                ReceiverId = result.ReceiveBy,
                FullNameReceiver = accountReceiver.Members.FirstOrDefault().FullName,
                CreatedDate = result.DateExchange,
                FullNameHost = accountHost.Members.FirstOrDefault().FullName,
                PostId = result.PostId
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

        public void Finish(string Id, string phoneNumber)
        {
            var exchange = this._exchangeDetailRepository.GetSingle(s => s.Id == Id);
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
            if (exchange.ReceiveBy != account.Id)
            {
                throw new Exception("Bạn không phải là người nhận của đơn hàng này");
            }
            exchange.Status = (int)EnumStatusRequest.Finish;
            this._exchangeDetailRepository.Update(exchange);
            var appAccountLogin = this._accountRepository.GetSingle(s => s.Id == exchange.Sender, new string[] { "AppAccountLogins", "Members" });
            var accountReceive = this._accountRepository.GetSingle(s => s.Id == exchange.ReceiveBy, new string[] { "Members" }).Members.FirstOrDefault();
            var mess = IdentityHelper.NotifyAsync(appAccountLogin.AppAccountLogins.Select(s => s.DeviceToken).ToArray(),
                "Yêu cầu đã hoàn thành",
                accountReceive.FullName + " đã nhận hàng.");
            this._exchangeDetailRepository.Commit();
        }

        public ExchangeDetailResponseModel GetInformationExchange(string Id)
        {
            var exchange = this._exchangeDetailRepository.GetSingle(s => s.Id == Id, new string[] { "Vegetable.VegetableDescription", "postDetail" });
            var qrCodeExchange = this._qrCodeService.Get(exchange.Id, 1, "");
            var accountHost = this._accountRepository.GetSingle(s => s.Id == exchange.Sender, new string[] { "Members" });
            var accountReceiver = this._accountRepository.GetSingle(s => s.Id == exchange.ReceiveBy, new string[] { "Members" });
            return new ExchangeDetailResponseModel
            {
                Id = exchange.Id,
                Quantity = exchange.Quantity,
                FullNameReceiver = accountReceiver.Members.FirstOrDefault().FullName,
                CreatedDate = exchange.DateExchange,
                FullNameHost = accountHost.Members.FirstOrDefault().FullName,
                ReceiverAddress = accountReceiver.Members.FirstOrDefault().Address,
                VegNameReceive = exchange.Vegetable.VegetableDescription.VegContent,
                ReceiverPhoneNumber = accountReceiver.PhoneNumber,
                QRCode = qrCodeExchange
            };
        }

        public IEnumerable<ExchangeDetailResponseModel> CheckInstance(ExchangeDetailRequestModel newItem, string phoneNumber, string baseUrl)
        {

            var share = this._postRepository.GetSingle(s => s.Id == newItem.PostId, new string[] { "Province", "District", "Ward" });
            var addressSender = share.Address + ", " + share.Ward.Name + ", " + share.District.Name + "," + share.Province.Name;
            var distance = IdentityHelper.GetDistance(addressSender, newItem.FullAddress);
            if (distance > 15)
            {
                throw new Exception("Khoảng cách quá xa. Bạn có muốn tiếp tục không ?");
            }
            else
            {
                var result = this.Add(newItem, phoneNumber, baseUrl);
                return result;
            }
        }
    }
}

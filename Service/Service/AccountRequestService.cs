using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VG.Common.Constant;
using VG.Common.Helper;
using VG.Data.Repository;
using VG.Model.Model;
using VG.Model.RequestModel;
using VG.Model.ResponseModel;

namespace VG.Service.Service
{
    public interface IAccountRequestService
    {
        Task<AccountRequest> Add(AccountFriendRequestModel newItem);
        Task IsComfirm(int Id, int status);
        IEnumerable<AccountRequestResponseModel> GetAccountRequest(string phoneNumber);
    }
    public class AccountRequestService : IAccountRequestService
    {
        private IAccountRepository _accountRepository;
        private IAccountRequestRepository _accountRequestRepository;
        private IAccountFriendRepository _accountFriendRepository;
        public AccountRequestService(IAccountRequestRepository accountRequestRepository, IAccountFriendRepository accountFriendRepository, IAccountRepository accountRepository)
        {
            _accountRequestRepository = accountRequestRepository;
            _accountFriendRepository = accountFriendRepository;
            _accountRepository = accountRepository;
        }
        public async Task<AccountRequest> Add(AccountFriendRequestModel newItem)
        {
            var accountReq = this._accountRequestRepository.GetMulti(s => s.AccountSend == newItem.AccountSend && s.AccountReceived == newItem.AccountReceived && s.Status == (int)EnumStatusRequest.Pending);
            if (accountReq.Count() <= 0)
            {
                var appAccountLogin = this._accountRepository.GetSingle(s => s.Id == newItem.AccountReceived, new string[] { "AppAccountLogins", "Members" });
                var FullNameAccountReceived = appAccountLogin.Members.FirstOrDefault().FullName;
                var result = this._accountRequestRepository.Add(new AccountRequest
                {
                    AccountReceived = newItem.AccountReceived,
                    AccountSend = newItem.AccountSend,
                    RequestedDate = DateTime.UtcNow.AddHours(7),
                    Status = (int)EnumStatusRequest.Pending
                });
                var mess = IdentityHelper.NotifyAsync(appAccountLogin.AppAccountLogins.Select(s => s.DeviceToken).ToArray(), "Bạn có lời mời kết bạn mới", FullNameAccountReceived + " đã gửi cho bạn một lời kết không bạn. Bạn có đồng ý không ?");
                this._accountRequestRepository.Commit();
                return await Task.FromResult(result);
            }
            return null;
        }

        public IEnumerable<AccountRequestResponseModel> GetAccountRequest(string phoneNumber)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
            var result = this._accountRequestRepository.GetMulti(s => s.AccountReceived == account.Id && s.Status == (int)EnumFriendRequest.Pending, new string[] { "AppAccountSend.Members", "AppAccountReceived.Members" })
                .Select(s => new AccountRequestResponseModel
                {
                    Id = s.Id,
                    AccountSend = s.AccountSend,
                    AccountSendName = s.AppAccountSend.Members.FirstOrDefault().FullName,
                    AccountReceived = s.AccountReceived,
                    AccountReceivedName = s.AppAccountReceived.Members.FirstOrDefault().FullName,
                    RequestedDate = s.RequestedDate,
                });
            return result;
        }

        public async Task IsComfirm(int Id, int status)
        {
            var result = this._accountRequestRepository.GetSingle(s => s.Id == Id);
            result.Status = status;
            this._accountRequestRepository.Update(result);
            this._accountFriendRepository.Add(new AccountFriend
            {
                Account_one_Id = result.AccountSend,
                Account_two_Id = result.AccountReceived,
                AcceptedDate = DateTime.Now
            });
            if (status == (int)EnumStatusRequest.Accept)
            {
                var appAccountLogin = this._accountRepository.GetSingle(s => s.Id == result.AccountSend, new string[] { "AppAccountLogins", "Members" });
                var FullNameAccountSend = appAccountLogin.Members.FirstOrDefault().FullName;
                var mess = IdentityHelper.NotifyAsync(appAccountLogin.AppAccountLogins.Select(s => s.DeviceToken).ToArray(), "Kết bạn thành công", FullNameAccountSend + " đã đồng ý kết bạn. ");
            }
            this._accountRequestRepository.Commit();
            await Task.CompletedTask;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using VG.Common.Constant;
using VG.Data.Repository;
using VG.Model.Model;

namespace VG.Service.Service
{
    public interface IAccountRequestService
    {
        AccountRequest Add(AccountFriendRequestModel newItem);
        void IsComfirm(int Id, int status);
        IEnumerable<AccountRequest> GetAccountRequest(string phoneNumber);
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
        public AccountRequest Add(AccountFriendRequestModel newItem)
        {
            var result = this._accountRequestRepository.Add(new AccountRequest 
            {
                AccountReceived = newItem.AccountReceived,
                AccountSend = newItem.AccountSend,
                RequestedDate = DateTime.UtcNow.AddHours(7),
                Status = (int)EnumStatusRequest.Pending
            });
            this._accountRequestRepository.Commit();
            return result;
        }

        public IEnumerable<AccountRequest> GetAccountRequest(string phoneNumber)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
            var result = this._accountRequestRepository.GetMulti(s => s.AccountReceived == account.Id && s.Status == (int)EnumFriendRequest.Accept);
            return result;
        }

        public void IsComfirm(int Id, int status)
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
            this._accountRequestRepository.Commit();
        }
    }
}

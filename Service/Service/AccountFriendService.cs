using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VG.Data.Repository;
using VG.Model.ResponseModel;

namespace VG.Service.Service
{
    public interface IAccountFriendService
    {
        IEnumerable<AccountFiendResponseModel> GetAllFriend(string phoneNumber);
        void Unfriend(int Id);
    }
    public class AccountFriendService : IAccountFriendService
    {
        private IAccountFriendRepository _accountFriendRepository;
        private IAccountRepository _accountRepository;
        public AccountFriendService(IAccountFriendRepository accountFriendRepository, IAccountRepository accountRepository)
        {
            _accountFriendRepository = accountFriendRepository;
            _accountRepository = accountRepository;
        }

        public IEnumerable<AccountFiendResponseModel> GetAllFriend(string phoneNumber)
        {
            var account = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber);
            var friend1 = this._accountFriendRepository.GetMulti(s => s.Account_one_Id == account.Id, new string[] { "AppAccountReceive.Members" })
                .Select(s => new AccountFiendResponseModel
                {
                    Id = s.Id,
                    FriendName = s.AppAccountReceive.Members.FirstOrDefault().FullName
                });
            var friend2 = this._accountFriendRepository.GetMulti(s => s.Account_two_Id == account.Id, new string[] { "AppAccountSend.Members" })
                .Select(s => new AccountFiendResponseModel
                {
                    Id = s.Id,
                    FriendName = s.AppAccountSend.Members.FirstOrDefault().FullName
                });
            var result = friend1.Union(friend2);
            return result;
        }

        public void Unfriend(int Id)
        {
            var result = this._accountFriendRepository.GetSingle(s => s.Id == Id);
            this._accountFriendRepository.Delete(result);
            this._accountFriendRepository.Commit();
        }
    }
}

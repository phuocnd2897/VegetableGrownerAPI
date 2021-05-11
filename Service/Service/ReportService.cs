using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VG.Data.Repository;
using VG.Model.Model;
using VG.Model.RequestModel;

namespace VG.Service.Service
{
    public interface IReportService
    {
        void Report(ReportRequestModel newItem);
        void Unlock(string AccountId);
    }
    public class ReportService : IReportService
    {
        private IReportRepository _reportRepository;
        private IPostRepository _postRepository;
        private IAccountRepository _accountRepository;
        private IPrecentReportRepository _precentReportRepository;
        public ReportService(IReportRepository reportRepository, IPostRepository postRepository, IAccountRepository accountRepository, IPrecentReportRepository precentReportRepository)
        {
            _reportRepository = reportRepository;
            _postRepository = postRepository;
            _accountRepository = accountRepository;
            _precentReportRepository = precentReportRepository;
        }
        public void Report(ReportRequestModel newItem)
        {
            var report = this._reportRepository.GetMulti(s => s.AccountReport == newItem.AccountReport && s.PostId == newItem.PostId);
            if (report.Count() > 0)
            {
                return;
            }
            this._reportRepository.Add(new Report
            {
                ReportContent = newItem.ReportContent,
                PostId = newItem.PostId,
                DateReport = DateTime.UtcNow.AddHours(7),
                AccountReport = newItem.AccountReport
            });
            var precent = _precentReportRepository.GetAll();
            var totalAccount = this._accountRepository.GetMulti(s => s.Status == true).Count();
            var countReport = this._reportRepository.GetMulti(s => s.PostId == newItem.PostId).Count() + 1;
            var share = this._postRepository.GetSingle(s => s.Id == newItem.PostId);
            if (countReport >= ((precent.Where(s => s.Id == 1).FirstOrDefault().Precent * totalAccount) / 100))
            {
                share.Lock = false;
                this._postRepository.Update(share);
            }
            this._postRepository.Commit();
            var countShare = this._postRepository.GetMulti(s => s.AccountId == share.AccountId && s.Lock == false).Count();
            var totalShare = this._postRepository.GetMulti(s => s.AccountId == share.AccountId).Count();
            if (countShare >= ((precent.Where(s => s.Id == 2).FirstOrDefault().Precent * totalShare) / 100))
            {
                var account = this._accountRepository.GetSingle(s => s.Id == share.AccountId);
                account.Status = false;
                var dateNow = DateTime.UtcNow.AddHours(7);
                var dateLock = dateNow.AddDays(3);
                if (dateNow.Hour >= 0 && dateNow.Hour <= 4)
                {
                    account.DateUnlock = new DateTime(dateLock.Year, dateLock.Month, dateLock.Day, 04, 00, 00);
                }
                else if (dateNow.Hour > 4 && dateNow.Hour <= 8)
                {
                    account.DateUnlock = new DateTime(dateLock.Year, dateLock.Month, dateLock.Day, 08, 00, 00);
                }
                else if (dateNow.Hour > 8 && dateNow.Hour <= 12)
                {
                    account.DateUnlock = new DateTime(dateLock.Year, dateLock.Month, dateLock.Day, 12, 00, 00);
                }
                else if (dateNow.Hour > 12 && dateNow.Hour <= 16)
                {
                    account.DateUnlock = new DateTime(dateLock.Year, dateLock.Month, dateLock.Day, 16, 00, 00);
                }
                else if (dateNow.Hour > 16 && dateNow.Hour <= 20)
                {
                    account.DateUnlock = new DateTime(dateLock.Year, dateLock.Month, dateLock.Day, 20, 00, 00);
                }
                else if (dateNow.Hour > 20 && dateNow.Hour <= 23)
                {
                    account.DateUnlock = new DateTime(dateLock.Year, dateLock.Month, dateLock.Day, 23, 59, 59);
                }
                this._accountRepository.Update(account);
            }
            this._reportRepository.Commit();
        }

        public void Unlock(string AccountId)
        {
            var account = this._accountRepository.GetSingle(s => s.Id == AccountId);
            account.Status = true;
            this._accountRepository.Update(account);
            this._accountRepository.Commit();
        }
    }
}

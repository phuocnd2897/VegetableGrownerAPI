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
    }
    public class ReportService : IReportService
    {
        private IReportRepository _reportRepository;
        private IShareDetailRepository _shareDetailRepository;
        private IAccountRepository _accountRepository;
        private IPrecentReportRepository _precentReportRepository;
        public ReportService(IReportRepository reportRepository, IShareDetailRepository shareDetailRepository, IAccountRepository accountRepository, IPrecentReportRepository precentReportRepository)
        {
            _reportRepository = reportRepository;
            _shareDetailRepository= shareDetailRepository;
            _accountRepository = accountRepository;
            _precentReportRepository = precentReportRepository;
        }
        public void Report(ReportRequestModel newItem)
        {
            var report = this._reportRepository.GetMulti(s => s.AccountReport == newItem.AccountReport && s.ShareDetailId == newItem.ShareDetailId);
            if (report.Count() > 0)
            {
                return;
            }
            this._reportRepository.Add(new Report
            {
                ReportContent = newItem.ReportContent,
                ShareDetailId = newItem.ShareDetailId,
                DateReport = DateTime.UtcNow.AddHours(7),
                AccountReport = newItem.AccountReport
            });
            var precent = _precentReportRepository.GetAll();
            var totalAccount = this._accountRepository.GetMulti(s => s.Status == true).Count();
            var countReport = this._reportRepository.GetMulti(s => s.ShareDetailId == newItem.ShareDetailId).Count() + 1;
            var share = this._shareDetailRepository.GetSingle(s => s.Id == newItem.ShareDetailId);
            if (countReport >= ((precent.Where(s => s.Id == 1).FirstOrDefault().Precent * totalAccount) / 100))
            {
                share.Lock = false;
                this._shareDetailRepository.Update(share);
            }
            this._shareDetailRepository.Commit();
            var countShare = this._shareDetailRepository.GetMulti(s => s.AccountId == share.AccountId && s.Lock == false).Count();
            var totalShare = this._shareDetailRepository.GetMulti(s => s.AccountId == share.AccountId).Count();
            if (countShare >= ((precent.Where(s => s.Id == 2).FirstOrDefault().Precent * totalShare) / 100))
            {
                var account = this._accountRepository.GetSingle(s => s.Id == share.AccountId);
                account.Status = false;
                this._accountRepository.Update(account);
            }
            this._reportRepository.Commit();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using VG.Data.Repository;
using VG.Model.Model;

namespace VG.Service.Service
{
    public interface IPrecentReportService
    {
        IEnumerable<PrecentReport> GetAll();
        PrecentReport Get(int Id);
        PrecentReport UpdatePrecent(PrecentReport newItem);
    }
    public class PrecentReportService : IPrecentReportService
    {
        private IPrecentReportRepository _precentReportRepository;
        public PrecentReportService(IPrecentReportRepository precentReportRepository)
        {
            _precentReportRepository = precentReportRepository;
        }

        public PrecentReport Get(int Id)
        {
            return this._precentReportRepository.GetSingle(s => s.Id == Id);
        }

        public IEnumerable<PrecentReport> GetAll()
        {
            return this._precentReportRepository.GetAll();
        }

        public PrecentReport UpdatePrecent(PrecentReport newItem)
        {
            var result = this._precentReportRepository.GetSingle(s => s.Id == newItem.Id);
            result.Precent = newItem.Precent;
            this._precentReportRepository.Update(result);
            this._precentReportRepository.Commit();
            return newItem;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VG.Common.Constant;
using VG.Data.Repository;
using VG.Model.ResponseModel;

namespace VG.Service.Service
{
    public interface IDashboardService
    {
        public DashboardResponseModel ShowDashboard();
        public DashboardResponseModel ShowDashBoardAboutShareAndExchange();
        public List<SelectedResponseModel> GetTop10(int Status);

    }
    public class DashboardService : IDashboardService
    {
        public IPostRepository _postRepository;
        public IExchangeDetailRepository _exchangeDetailRepository;
        public IAccountRepository _accountRepository;
        public IVegetableDescriptionRepository _vegetableDescriptionRepository;
        public DashboardService(IPostRepository postRepository, IExchangeDetailRepository exchangeDetailRepository, IAccountRepository accountRepository, IVegetableDescriptionRepository vegetableDescriptionRepository)
        {
            _postRepository = postRepository;
            _exchangeDetailRepository = exchangeDetailRepository;
            _accountRepository = accountRepository;
            _vegetableDescriptionRepository = vegetableDescriptionRepository;
        }

        public List<SelectedResponseModel> GetTop10(int Status)
        {
            List<SelectedResponseModel> response = new List<SelectedResponseModel>();
            switch (Status)
            {
                case 1:
                    var share = this._postRepository.GetAll().GroupBy(s => s.AccountId).Select(s => new
                    {
                        Id = s.Key,
                        Count = s.Count()
                    }).ToList();
                    foreach (var item in share)
                    {
                        var account = this._accountRepository.GetSingle(s => s.Id == item.Id, new string[] { "Members" });
                        response.Add(new SelectedResponseModel
                        {
                            Id = account.PhoneNumber + "-" + account.Members.FirstOrDefault().FullName,
                            Text = item.Count.ToString()
                        });
                    }
                    break;
                case 2:
                    var vegCommon = this._vegetableDescriptionRepository.GetMulti(s => s.VegetableCompositionId == "1" && (s.AccountId == "" || s.AccountId == null) && s.Vegetables.Count > 0, new string[] { "Vegetables" })
                        .Select(s => new SelectedResponseModel
                        {
                            Id = s.VegContent,
                            Text = s.Vegetables.Count().ToString()
                        });
                    var veg = this._vegetableDescriptionRepository.GetMulti(s => s.VegetableCompositionId == "1" && (s.AccountId != "" && s.AccountId != null)).GroupBy(s => s.VegContent).Select(s => new SelectedResponseModel
                    {
                        Id = s.Key,
                        Text = s.Count().ToString()
                    }).ToList();
                    response = vegCommon.Union(veg).GroupBy(s => s.Id).Select(s => new SelectedResponseModel
                    {
                        Id = s.Key,
                        Text = s.Sum(q => int.Parse(q.Text)).ToString()
                    }).ToList();
                    break;
                case 3:
                    break;
                default:
                    break;
            }
            return response.OrderByDescending(s => int.Parse(s.Text)).Take(10).ToList();
        }

        public DashboardResponseModel ShowDashboard()
        {
            DashboardResponseModel dashboardResponseModel = new DashboardResponseModel();
            List<int[]> series = new List<int[]>();
            dashboardResponseModel.labels = new string[] { "0 - 2", "2 - 4", "4 - 6", "6 - 8", "8 - 10", "10 - 12", "12 - 14", "14 - 16", "16 - 18", "18 - 20", "20 - 22", "22 - 24" };
            var share = this._postRepository.GetAll();
            var exchange = this._exchangeDetailRepository.GetAll();
            int count02 = 0;
            int count24 = 0;
            int count46 = 0;
            int count68 = 0;
            int count810 = 0;
            int count1012 = 0;
            int count1214 = 0;
            int count1416 = 0;
            int count1618 = 0;
            int count1820 = 0;
            int count2022 = 0;
            int count2224 = 0;
            foreach (var item in share)
            {
                if (item.DateShare.Hour >= 0 && item.DateShare.Hour < 2)
                {
                    count02++;
                }
                else if (item.DateShare.Hour >= 2 && item.DateShare.Hour < 4)
                {
                    count24++;
                }
                else if (item.DateShare.Hour >= 4 && item.DateShare.Hour < 6)
                {
                    count46++;
                }
                else if (item.DateShare.Hour >= 6 && item.DateShare.Hour < 8)
                {
                    count68++;
                }
                else if (item.DateShare.Hour >= 8 && item.DateShare.Hour < 10)
                {
                    count810++;
                }
                else if (item.DateShare.Hour >= 10 && item.DateShare.Hour < 12)
                {
                    count1012++;
                }
                else if (item.DateShare.Hour >= 12 && item.DateShare.Hour < 14)
                {
                    count1214++;
                }
                else if (item.DateShare.Hour >= 14 && item.DateShare.Hour < 16)
                {
                    count1416++;
                }
                else if (item.DateShare.Hour >= 16 && item.DateShare.Hour < 18)
                {
                    count1618++;
                }
                else if (item.DateShare.Hour >= 18 && item.DateShare.Hour < 20)
                {
                    count1820++;
                }
                else if (item.DateShare.Hour >= 20 && item.DateShare.Hour < 22)
                {
                    count2022++;
                }
                else if (item.DateShare.Hour >= 22 && item.DateShare.Hour <= 24)
                {
                    count2224++;
                }
            }
            series.Add(new int[] { count02, count24, count46, count68, count810, count1012, count1214, count1416, count1618, count1820, count2022, count2224 });
            foreach (var item in exchange)
            {
                if (item.DateExchange.Hour >= 0 && item.DateExchange.Hour < 2)
                {
                    count02++;
                }
                else if (item.DateExchange.Hour >= 2 && item.DateExchange.Hour < 4)
                {
                    count24++;
                }
                else if (item.DateExchange.Hour >= 4 && item.DateExchange.Hour < 6)
                {
                    count46++;
                }
                else if (item.DateExchange.Hour >= 6 && item.DateExchange.Hour < 8)
                {
                    count68++;
                }
                else if (item.DateExchange.Hour >= 8 && item.DateExchange.Hour < 10)
                {
                    count810++;
                }
                else if (item.DateExchange.Hour >= 10 && item.DateExchange.Hour < 12)
                {
                    count1012++;
                }
                else if (item.DateExchange.Hour >= 12 && item.DateExchange.Hour < 14)
                {
                    count1214++;
                }
                else if (item.DateExchange.Hour >= 14 && item.DateExchange.Hour < 16)
                {
                    count1416++;
                }
                else if (item.DateExchange.Hour >= 16 && item.DateExchange.Hour < 18)
                {
                    count1618++;
                }
                else if (item.DateExchange.Hour >= 18 && item.DateExchange.Hour < 20)
                {
                    count1820++;
                }
                else if (item.DateExchange.Hour >= 20 && item.DateExchange.Hour < 22)
                {
                    count2022++;
                }
                else if (item.DateExchange.Hour >= 22 && item.DateExchange.Hour <= 24)
                {
                    count2224++;
                }
            }
            series.Add(new int[] { count02, count24, count46, count68, count810, count1012, count1214, count1416, count1618, count1820, count2022, count2224 });
            dashboardResponseModel.series = series;
            return dashboardResponseModel;
        }

        public DashboardResponseModel ShowDashBoardAboutShareAndExchange()
        {
            DashboardResponseModel dashboardResponseModel = new DashboardResponseModel();
            List<int[]> series = new List<int[]>();
            dashboardResponseModel.labels = new string[] { "Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec" };
            var share = this._postRepository.GetMulti(s => s.Type == (int)EnumStatusShare.Share);
            var exchange = this._exchangeDetailRepository.GetMulti(s => s.Status == (int)EnumStatusShare.Exchange);
            int countJan = 0;
            int countFeb = 0;
            int countMar = 0;
            int countApr = 0;
            int countMay = 0;
            int countJun = 0;
            int countJul = 0;
            int countAug = 0;
            int countSep = 0;
            int countOct = 0;
            int countNov = 0;
            int countDec = 0;
            foreach (var item in share)
            {
                if (item.DateShare.Month == 1)
                {
                    countJan++;
                }
                else if (item.DateShare.Month == 2)
                {
                    countFeb++;
                }
                else if (item.DateShare.Month == 3)
                {
                    countMar++;
                }
                else if (item.DateShare.Month == 4)
                {
                    countApr++;
                }
                else if (item.DateShare.Month == 5)
                {
                    countMay++;
                }
                else if (item.DateShare.Month == 6)
                {
                    countJun++;
                }
                else if (item.DateShare.Month == 7)
                {
                    countJul++;
                }
                else if (item.DateShare.Month == 8)
                {
                    countAug++;
                }
                else if (item.DateShare.Month == 9)
                {
                    countSep++;
                }
                else if (item.DateShare.Month == 10)
                {
                    countOct++;
                }
                else if (item.DateShare.Month == 11)
                {
                    countNov++;
                }
                else if (item.DateShare.Month == 12)
                {
                    countDec++;
                }
            }
            series.Add(new int[] { countJan, countFeb, countMar, countApr, countMay, countJun, countJul, countAug, countSep, countOct, countNov, countDec });
            foreach (var item in exchange)
            {
                if (item.DateExchange.Month == 1)
                {
                    countJan++;
                }
                else if (item.DateExchange.Month == 2)
                {
                    countFeb++;
                }
                else if (item.DateExchange.Month == 3)
                {
                    countMar++;
                }
                else if (item.DateExchange.Month == 4)
                {
                    countApr++;
                }
                else if (item.DateExchange.Month == 5)
                {
                    countMay++;
                }
                else if (item.DateExchange.Month == 6)
                {
                    countJun++;
                }
                else if (item.DateExchange.Month == 7)
                {
                    countJul++;
                }
                else if (item.DateExchange.Month == 8)
                {
                    countAug++;
                }
                else if (item.DateExchange.Month == 9)
                {
                    countSep++;
                }
                else if (item.DateExchange.Month == 10)
                {
                    countOct++;
                }
                else if (item.DateExchange.Month == 11)
                {
                    countNov++;
                }
                else if (item.DateExchange.Month == 12)
                {
                    countDec++;
                }
            }
            series.Add(new int[] { countJan, countFeb, countMar, countApr, countMay, countJun, countJul, countAug, countSep, countOct, countNov, countDec });
            dashboardResponseModel.series = series;
            return dashboardResponseModel;
        }
    }
}

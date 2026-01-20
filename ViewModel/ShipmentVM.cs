using LiveCharts;
using LiveCharts.Wpf;
using Page_Navigation_App.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace Page_Navigation_App.ViewModel
{
    public class ShipmentVM : INotifyPropertyChanged
    {
        public SeriesCollection RevenueByDaySeries { get; set; }
        public SeriesCollection ThongKeTheoQuy { get; set; }

        public List<string> Days { get; set; }
        public string[] Quarters { get; set; }

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
                LoadAll();
            }
        }

        private int _totalOrders;
        public int TotalOrders
        {
            get => _totalOrders;
            set { _totalOrders = value; OnPropertyChanged(); }
        }

        private double _revenueByDate;
        public double RevenueByDate
        {
            get => _revenueByDate;
            set { _revenueByDate = value; OnPropertyChanged(); }
        }

        private double _revenueByMonth;
        public double RevenueByMonth
        {
            get => _revenueByMonth;
            set { _revenueByMonth = value; OnPropertyChanged(); }
        }

        public Func<double, string> YFormatter { get; set; }
        public ICommand LoadAllStatisticCommand { get; set; }

        public ShipmentVM()
        {
            Days = new List<string>();
            Quarters = new[] { "Quý 1", "Quý 2", "Quý 3", "Quý 4" };

            YFormatter = v => v.ToString("N0") + " đ";

            RevenueByDaySeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Doanh thu",
                    Values = new ChartValues<double>(),
                    Fill = Brushes.DodgerBlue
                }
            };

            ThongKeTheoQuy = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Doanh thu theo quý",
                    Values = new ChartValues<double>(),
                    StrokeThickness = 3,
                    PointGeometry = DefaultGeometries.Circle
                }
            };

            LoadAllStatisticCommand = new RelayCommand(_ => LoadAll());
            LoadAll();
        }

        private void LoadAll()
        {
            LoadSummary();
            LoadRevenueByWeek();
            LoadRevenueByQuarter();
        }

        private void LoadSummary()
        {
            string sql = @"
                SELECT 
                    COUNT(*) AS TotalOrders,
                    SUM(ThanhTien) AS RevenueDay
                FROM HOADON
                WHERE CAST(NgayLapHD AS DATE) = @Ngay";

            DataTable dt = DBConnection.ExecuteQuery(
                sql,
                new System.Data.SqlClient.SqlParameter("@Ngay", SelectedDate.Date)
            );

            if (dt.Rows.Count > 0)
            {
                TotalOrders = Convert.ToInt32(dt.Rows[0]["TotalOrders"]);
                RevenueByDate = dt.Rows[0]["RevenueDay"] == DBNull.Value ? 0 : Convert.ToDouble(dt.Rows[0]["RevenueDay"]);
            }

            string sqlMonth = @"
                SELECT SUM(ThanhTien)
                FROM HOADON
                WHERE MONTH(NgayLapHD) = @Month AND YEAR(NgayLapHD) = @Year";

            object monthRevenue = DBConnection.ExecuteScalar(
                sqlMonth,
                new System.Data.SqlClient.SqlParameter("@Month", SelectedDate.Month),
                new System.Data.SqlClient.SqlParameter("@Year", SelectedDate.Year)
            );

            RevenueByMonth = monthRevenue == DBNull.Value ? 0 : Convert.ToDouble(monthRevenue);
        }

        private void LoadRevenueByWeek()
        {
            int diff = (7 + (SelectedDate.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeek = SelectedDate.Date.AddDays(-diff);
            DateTime endOfWeek = startOfWeek.AddDays(7).AddTicks(-1);

            string sql = @"
                SELECT 
                    DATENAME(WEEKDAY, NgayLapHD) AS Thu,
                    SUM(ThanhTien) AS TongTien,
                    DATEPART(WEEKDAY, NgayLapHD) AS ThuSo
                FROM HOADON
                WHERE NgayLapHD BETWEEN @FromDate AND @ToDate
                GROUP BY DATENAME(WEEKDAY, NgayLapHD), DATEPART(WEEKDAY, NgayLapHD)
                ORDER BY ThuSo";

            DataTable dt = DBConnection.ExecuteQuery(
                sql,
                new System.Data.SqlClient.SqlParameter("@FromDate", startOfWeek),
                new System.Data.SqlClient.SqlParameter("@ToDate", endOfWeek)
            );

            Days.Clear();
            RevenueByDaySeries[0].Values.Clear();

            foreach (DataRow row in dt.Rows)
            {
                Days.Add(row["Thu"].ToString());
                RevenueByDaySeries[0].Values.Add(Convert.ToDouble(row["TongTien"]));
            }

            OnPropertyChanged(nameof(Days));
        }

        private void LoadRevenueByQuarter()
        {
            ThongKeTheoQuy[0].Values.Clear();

            double[] revenueByQuarter = new double[4];

            string sql = @"
                SELECT 
                    DATEPART(QUARTER, NgayLapHD) AS Quy,
                    SUM(ThanhTien) AS TongTien
                FROM HOADON
                WHERE YEAR(NgayLapHD) = @Year
                GROUP BY DATEPART(QUARTER, NgayLapHD)";

            DataTable dt = DBConnection.ExecuteQuery(
                sql,
                new System.Data.SqlClient.SqlParameter("@Year", SelectedDate.Year)
            );

            foreach (DataRow row in dt.Rows)
            {
                int quarter = Convert.ToInt32(row["Quy"]);
                double total = row["TongTien"] == DBNull.Value ? 0 : Convert.ToDouble(row["TongTien"]);
                revenueByQuarter[quarter - 1] = total;
            }

            for (int i = 0; i < 4; i++)
            {
                ThongKeTheoQuy[0].Values.Add(
          revenueByQuarter[i] == 0 ? 0.0001 : revenueByQuarter[i]
      );
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        public RelayCommand(Action<object> execute) => _execute = execute;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute(parameter);
        public event EventHandler CanExecuteChanged;
    }
}

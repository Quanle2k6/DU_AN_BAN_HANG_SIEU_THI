using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace Page_Navigation_App.ViewModel
{
    public class ShipmentVM : INotifyPropertyChanged
    {
       
        private SeriesCollection _revenueByDaySeries;
        public SeriesCollection RevenueByDaySeries
        {
            get => _revenueByDaySeries;
            set { _revenueByDaySeries = value; OnPropertyChanged(); }
        }
        public List<string> Days { get; set; }

       
        private SeriesCollection _thongKeTheoQuy;
        public SeriesCollection ThongKeTheoQuy
        {
            get => _thongKeTheoQuy;
            set { _thongKeTheoQuy = value; OnPropertyChanged(); }
        }
        public string[] Quarters { get; set; }

       
        public Func<double, string> YFormatter { get; set; }

        public ICommand LoadWeeklyCommand { get; set; }
        public ICommand LoadQuarterlyCommand { get; set; }

        public ShipmentVM()
        {
           
            Days = new List<string> { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN" };
            Quarters = new[] { "Quý 1", "Quý 2", "Quý 3", "Quý 4" };
            YFormatter = value => value.ToString("N0") + " tr";

           
            LoadInitialData();

          
            LoadWeeklyCommand = new RelayCommand(obj => UpdateWeeklyData());
            LoadQuarterlyCommand = new RelayCommand(obj => UpdateQuarterlyData());
        }

        private void LoadInitialData()
        {
            RevenueByDaySeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Doanh thu",
                    Values = new ChartValues<double> { 5, 7, 6, 9, 10, 8, 4 },
                    Fill = Brushes.DodgerBlue
                }
            };

            ThongKeTheoQuy = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Doanh thu Quý",
                    Values = new ChartValues<double> { 120, 180, 150, 250 },
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10,
                    StrokeThickness = 3
                }
            };
        }

        private void UpdateWeeklyData()
        {
            RevenueByDaySeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Doanh thu mới",
                    Values = new ChartValues<double> { 8, 10, 9, 12, 14, 11, 7 },
                    Fill = Brushes.OrangeRed
                }
            };
        }

        private void UpdateQuarterlyData()
        {
            
            ThongKeTheoQuy = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Doanh thu thực tế",
                    Values = new ChartValues<double> { 200, 350, 300, 500 },
                    PointGeometry = DefaultGeometries.Square,
                    Stroke = Brushes.MediumPurple
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
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
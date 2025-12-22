using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using RideBoard.Widget.Models;
using RideBoard.Widget.Services;

namespace RideBoard.Widget.ViewModels
{
    public class WidgetViewModel : INotifyPropertyChanged
    {
        private readonly StravaService _service = new StravaService();
        private readonly DispatcherTimer _uiTimer = new DispatcherTimer();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        // 0 = Daily View, 1 = Yearly View
        private int _currentPageIndex = 0;

        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand RefreshCommand { get; }

        private Visibility _offlineVisibility = Visibility.Collapsed;
        public Visibility OfflineVisibility { get => _offlineVisibility; set { _offlineVisibility = value; OnPropertyChanged(); } }

        // --- Page 1: Daily/Week ---
        private Visibility _pageDailyVisibility = Visibility.Visible;
        public Visibility PageDailyVisibility { get => _pageDailyVisibility; set { _pageDailyVisibility = value; OnPropertyChanged(); } }

        private string _todayDistance = "- km";
        public string TodayDistance { get => _todayDistance; set { _todayDistance = value; OnPropertyChanged(); } }
        private string _todayTime = "-:--:--";
        public string TodayTime { get => _todayTime; set { _todayTime = value; OnPropertyChanged(); } }
        private string _todayElev = "";
        public string TodayElev { get => _todayElev; set { _todayElev = value; OnPropertyChanged(); } }

        private string _lastDistance = "- km";
        public string LastDistance { get => _lastDistance; set { _lastDistance = value; OnPropertyChanged(); } }
        private string _lastPower = "";
        public string LastPower { get => _lastPower; set { _lastPower = value; OnPropertyChanged(); } }
        private string _lastHr = "";
        public string LastHr { get => _lastHr; set { _lastHr = value; OnPropertyChanged(); } }
        private string _lastElev = "";
        public string LastElev { get => _lastElev; set { _lastElev = value; OnPropertyChanged(); } }
        private string _lastTime = "";
        public string LastTime { get => _lastTime; set { _lastTime = value; OnPropertyChanged(); } }

        private string _weekDistance = "- km";
        public string WeekDistance { get => _weekDistance; set { _weekDistance = value; OnPropertyChanged(); } }
        private string _weekTime = "-:--";
        public string WeekTime { get => _weekTime; set { _weekTime = value; OnPropertyChanged(); } }
        private string _weekElev = "";
        public string WeekElev { get => _weekElev; set { _weekElev = value; OnPropertyChanged(); } }

        // --- Page 2: Yearly ---
        private Visibility _pageYearlyVisibility = Visibility.Collapsed;
        public Visibility PageYearlyVisibility { get => _pageYearlyVisibility; set { _pageYearlyVisibility = value; OnPropertyChanged(); } }

        private string _yearRange = "";
        public string YearRange { get => _yearRange; set { _yearRange = value; OnPropertyChanged(); } }
        
        private string _yearTotalDistance = "- km";
        public string YearTotalDistance { get => _yearTotalDistance; set { _yearTotalDistance = value; OnPropertyChanged(); } }

        private string _yearTotalElev = "- m";
        public string YearTotalElev { get => _yearTotalElev; set { _yearTotalElev = value; OnPropertyChanged(); } }

        private string _yearTotalTime = "-h -m";
        public string YearTotalTime { get => _yearTotalTime; set { _yearTotalTime = value; OnPropertyChanged(); } }

        private string _footerText = "";
        public string FooterText { get => _footerText; set { _footerText = value; OnPropertyChanged(); } }

        public bool IsAutoStartEnabled
        {
            get => StartupManager.IsEnabled;
            set
            {
                StartupManager.Set(value);
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public WidgetViewModel()
        {
            NextPageCommand = new RelayCommand(_ => SwitchPage(1));
            PrevPageCommand = new RelayCommand(_ => SwitchPage(-1));
            LoginCommand = new RelayCommand(_ => OpenLoginUrl());
            RefreshCommand = new RelayCommand(async _ => await RefreshAsync(true));
        }

        public void Start()
        {
            _uiTimer.Interval = TimeSpan.FromSeconds(45);
            _uiTimer.Tick += async (_, __) => await RefreshAsync();
            _uiTimer.Start();
            _ = RefreshAsync();
        }

        private void OpenLoginUrl()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "http://127.0.0.1:8787/login",
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Failed to open login page: " + ex.Message);
            }
        }

        private void SwitchPage(int direction)
        {
            _currentPageIndex = (_currentPageIndex + direction) % 2;
            if (_currentPageIndex < 0) _currentPageIndex = 1;

            if (_currentPageIndex == 0)
            {
                PageDailyVisibility = Visibility.Visible;
                PageYearlyVisibility = Visibility.Collapsed;
            }
            else
            {
                PageDailyVisibility = Visibility.Collapsed;
                PageYearlyVisibility = Visibility.Visible;
            }
        }

        private async Task RefreshAsync(bool force = false)
        {
            if (force) FooterText = "Refreshing...";
            var (payload, online) = await _service.GetDataAsync(_cts.Token, force);
            OfflineVisibility = online ? Visibility.Collapsed : Visibility.Visible;
            if (payload != null)
            {
                Apply(payload);
            }
            
            string timeStr = payload?.UpdatedAt ?? DateTime.Now.ToString("HH:mm");
            string status = payload?.Status ?? "";
            
            if (!online)
            {
                FooterText = $"离线 ({timeStr})";
            }
            else if (status.Contains("offline") || status.Contains("cached"))
            {
                 // Server is running but maybe token expired or not logged in
                 FooterText = $"Cached ({timeStr})";
            }
            else
            {
                FooterText = $"更新时间：{timeStr}";
            }
        }

        private static string FormatHms(string? src)
        {
            if (string.IsNullOrWhiteSpace(src)) return "000:00:00";
            var s = src.Trim().ToLowerInvariant();
            // Already contains ':' → normalize to HH:MM:SS
            if (s.Contains(":"))
            {
                var parts = s.Split(':');
                int h = 0, m = 0, sec = 0;
                _ = int.TryParse(parts.Length > 0 ? parts[0] : "0", out h);
                _ = int.TryParse(parts.Length > 1 ? parts[1] : "0", out m);
                _ = int.TryParse(parts.Length > 2 ? parts[2] : "0", out sec);
                return $"{h:000}:{m:00}:{sec:00}";
            }
            // Parse patterns like "173h 6m" or "12h" "45m" "30s"
            int hh = 0, mm = 0, ss = 0;
            try
            {
                var hMatch = System.Text.RegularExpressions.Regex.Match(s, "(\\d+)\\s*h");
                var mMatch = System.Text.RegularExpressions.Regex.Match(s, "(\\d+)\\s*m");
                var sMatch = System.Text.RegularExpressions.Regex.Match(s, "(\\d+)\\s*s");
                if (hMatch.Success) hh = int.Parse(hMatch.Groups[1].Value);
                if (mMatch.Success) mm = int.Parse(mMatch.Groups[1].Value);
                if (sMatch.Success) ss = int.Parse(sMatch.Groups[1].Value);
            }
            catch { }
            return $"{hh:000}:{mm:00}:{ss:00}";
        }

        private void Apply(StravaPayload p)
        {
            if (p.Today != null)
            {
                TodayDistance = p.Today.DistanceKm.HasValue ? $"{p.Today.DistanceKm:000.0}" : "000.0";
                TodayTime = FormatHms(p.Today.Time);
                TodayElev = p.Today.ElevationM.HasValue ? $"{p.Today.ElevationM}" : "0";
            }
            if (p.Last != null)
            {
                LastDistance = p.Last.DistanceKm.HasValue ? $"{p.Last.DistanceKm:000.0}" : "000.0";
                LastPower = p.Last.AvgPower.HasValue ? $"{p.Last.AvgPower}" : "-";
                LastHr = p.Last.AvgHr.HasValue ? $"{p.Last.AvgHr}" : "-";
                LastElev = p.Last.ElevM.HasValue ? $"{p.Last.ElevM}" : "0";
                LastTime = FormatHms(p.Last.Time);
            }
            if (p.Week != null)
            {
                WeekDistance = p.Week.DistanceKm.HasValue ? $"{p.Week.DistanceKm:000.0}" : "000.0";
                WeekTime = FormatHms(p.Week.Time);
                WeekElev = p.Week.ElevM.HasValue ? $"{p.Week.ElevM}" : "0";
            }

            if (p.Year != null)
            {
                YearRange = p.Year.Range ?? "";
                YearTotalDistance = p.Year.DistanceKm.HasValue ? $"{p.Year.DistanceKm:N0}" : "0";
                YearTotalElev = p.Year.ElevM.HasValue ? $"{p.Year.ElevM:N0}" : "0";
                YearTotalTime = FormatHms(p.Year.Time);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

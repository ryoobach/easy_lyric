using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace EasyLyric
{
    public partial class MainWindow : Window
    {
        private readonly string _historyFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EasyLyric", "broadcast-history.txt");

        public MainWindow()
        {
            InitializeComponent();
            LoadHistory();
        }

        private void LoadHistory()
        {
            try
            {
                if (!File.Exists(_historyFile)) return;
                foreach (string address in File.ReadAllLines(_historyFile).Where(x => !string.IsNullOrWhiteSpace(x)).Take(30))
                    AddressCombo.Items.Add(address.Trim());
            }
            catch
            {
                StatusText.Text = "주소 기록을 불러오지 못했습니다. 새 주소는 계속 입력할 수 있습니다.";
            }
        }

        private void SaveHistory(string address)
        {
            try
            {
                string folder = Path.GetDirectoryName(_historyFile);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                var entries = AddressCombo.Items.Cast<object>()
                    .Select(x => x == null ? string.Empty : x.ToString())
                    .Where(x => !string.IsNullOrWhiteSpace(x) && !string.Equals(x, address, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                entries.Insert(0, address);
                File.WriteAllLines(_historyFile, entries.Take(30));
                AddressCombo.Items.Clear();
                foreach (string item in entries.Take(30)) AddressCombo.Items.Add(item);
            }
            catch
            {
                StatusText.Text = "주소 기록을 저장하지 못했습니다.";
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string address = AddressCombo.Text == null ? string.Empty : AddressCombo.Text.Trim();
            Uri parsed;
            if (!Uri.TryCreate(address, UriKind.Absolute, out parsed) ||
                (parsed.Scheme != Uri.UriSchemeHttp && parsed.Scheme != Uri.UriSchemeHttps))
            {
                StatusText.Text = "올바른 http 또는 https 방송 주소를 입력해 주세요.";
                return;
            }

            SaveHistory(address);
            StatusText.Text = "주소 형식 확인 완료 · 실제 방송 수신 기능은 다음 개발 단계에서 연결합니다.";
        }
    }
}

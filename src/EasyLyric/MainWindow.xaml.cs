using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace EasyLyric
{
    public partial class MainWindow : Window
    {
        private readonly string _historyFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EasyLyric", "broadcast-history.txt");

        private FixedDocument _currentDocument;

        // A4 at 96 DPI.
        private const double PageWidth = 793.7;
        private const double PageHeight = 1122.5;

        public MainWindow()
        {
            InitializeComponent();
            LoadHistory();
            BuildScoreDocument();
        }

        private void LoadHistory()
        {
            try
            {
                if (!File.Exists(_historyFile)) return;
                foreach (string address in File.ReadAllLines(_historyFile).Where(x => !string.IsNullOrWhiteSpace(x)).Take(30))
                    AddressCombo.Items.Add(address.Trim());
            }
            catch { StatusText.Text = "주소 기록을 불러오지 못했습니다."; }
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
            catch { StatusText.Text = "주소 기록을 저장하지 못했습니다."; }
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
            StatusText.Text = "주소 형식 확인 완료 · 방송 수신/곡 인식 기능은 아직 연결되지 않았습니다.";
        }

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            BuildScoreDocument();
        }

        private void BuildScoreDocument()
        {
            var document = new FixedDocument();
            document.DocumentPaginator.PageSize = new Size(PageWidth, PageHeight);

            string[] measures = (ScoreInput == null ? string.Empty : ScoreInput.Text)
                .Replace("\r", string.Empty).Split('\n')
                .Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

            const int measuresOnFirstPage = 27;
            const int measuresOnLaterPage = 38;
            int index = 0;
            int pageNumber = 0;

            do
            {
                bool firstPage = pageNumber == 0;
                int capacity = firstPage ? measuresOnFirstPage : measuresOnLaterPage;
                var page = new FixedPage
                {
                    Width = PageWidth,
                    Height = PageHeight,
                    Background = Brushes.White
                };

                double left = 58;
                double top = 52;
                if (firstPage)
                {
                    AddText(page, SafeText(TitleInput.Text), 30, FontWeights.Bold, Brushes.Black,
                        left, top, PageWidth - 2 * left, 48, TextAlignment.Center);
                    top += 58;
                    AddText(page, "작사 · 작곡: " + SafeText(ComposerInput.Text), 13, FontWeights.Normal,
                        Brushes.Black, left, top, 360, 24, TextAlignment.Left);
                    AddText(page, "가수: " + SafeText(SingerInput.Text), 13, FontWeights.Normal,
                        Brushes.Black, PageWidth - left - 300, top, 300, 24, TextAlignment.Right);
                    top += 42;
                    var rule = new Line { X1 = left, Y1 = top, X2 = PageWidth - left, Y2 = top,
                        Stroke = Brushes.LightGray, StrokeThickness = 1 };
                    page.Children.Add(rule);
                    top += 22;
                }

                int count = Math.Min(capacity, measures.Length - index);
                if (measures.Length == 0) count = 0;
                for (int i = 0; i < count; i++)
                {
                    // Each input line is treated as one complete measure; it is never split across pages.
                    AddText(page, measures[index++], 18, FontWeights.SemiBold, Brushes.Black,
                        left + 8, top, PageWidth - 2 * left - 16, 25, TextAlignment.Left,
                        new Typeface("Consolas"));
                    top += 29;
                }

                AddText(page, "Ryoo Jaesang & 최피디", 12, FontWeights.SemiBold, Brushes.Black,
                    left, PageHeight - 54, 360, 22, TextAlignment.Left);
                AddText(page, (pageNumber + 1).ToString(), 10, FontWeights.Normal, Brushes.Gray,
                    PageWidth - left - 40, PageHeight - 54, 40, 22, TextAlignment.Right);

                page.Measure(new Size(PageWidth, PageHeight));
                page.Arrange(new Rect(0, 0, PageWidth, PageHeight));
                page.UpdateLayout();

                var content = new PageContent();
                ((IAddChild)content).AddChild(page);
                document.Pages.Add(content);
                pageNumber++;
            }
            while (index < measures.Length);

            _currentDocument = document;
            ScoreViewer.Document = document;
        }

        private static string SafeText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "" : value.Trim();
        }

        private static void AddText(FixedPage page, string text, double size, FontWeight weight,
            Brush brush, double x, double y, double width, double height,
            TextAlignment alignment, Typeface typeface = null)
        {
            var block = new TextBlock
            {
                Text = text,
                FontSize = size,
                FontWeight = weight,
                Foreground = brush,
                TextAlignment = alignment,
                TextWrapping = TextWrapping.NoWrap,
                FontFamily = typeface == null ? new FontFamily("Malgun Gothic") : typeface.FontFamily
            };
            if (typeface != null) block.FontFamily = typeface.FontFamily;
            FixedPage.SetLeft(block, x);
            FixedPage.SetTop(block, y);
            block.Width = width;
            block.Height = height;
            page.Children.Add(block);
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            BuildScoreDocument();
            var dialog = new System.Windows.Controls.PrintDialog();
            if (dialog.ShowDialog() == true && _currentDocument != null)
                dialog.PrintDocument(_currentDocument.DocumentPaginator, "Easy Lyric A4 악보");
        }
    }
}
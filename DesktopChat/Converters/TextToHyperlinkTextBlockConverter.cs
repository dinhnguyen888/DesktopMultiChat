using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;
using static System.Net.WebRequestMethods;

namespace DesktopChat.Converters
{
    public class TextToHyperlinkTextBlockConverter : IValueConverter
    {
        // Regex pattern để nhận diện URL 
        private static readonly string UrlPattern =
              @"(?:https?://|www\.)[^\s]+?(?=[\s]|$)";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;

            var textBlock = new TextBlock
            {
                TextWrapping = System.Windows.TextWrapping.Wrap
            };

            if (string.IsNullOrEmpty(text))
                return textBlock;

            // Tách văn bản thành các phần là URL và không phải URL
            var matches = Regex.Matches(text, UrlPattern);

            if (matches.Count == 0)
            {
                // Nếu không có URL, thêm toàn bộ văn bản vào TextBlock
                textBlock.Inlines.Add(new Run(text));
                return textBlock;
            }

            int lastIndex = 0;

            foreach (Match match in matches)
            {
                // Thêm văn bản thường trước URL (nếu có)
                if (match.Index > lastIndex)
                {
                    string normalText = text.Substring(lastIndex, match.Index - lastIndex);
                    textBlock.Inlines.Add(new Run(normalText));
                }

                // Tạo và thêm Hyperlink
                string url = match.Value;
                var hyperlink = new Hyperlink(new Run(url))
                {
                    NavigateUri = new Uri(EnsureHttpPrefix(url))
                };

                hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
                textBlock.Inlines.Add(hyperlink);

                lastIndex = match.Index + match.Length;
            }

            // Thêm phần văn bản còn lại sau URL cuối cùng (nếu có)
            if (lastIndex < text.Length)
            {
                string normalText = text.Substring(lastIndex);
                textBlock.Inlines.Add(new Run(normalText));
            }

            return textBlock;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Chuyển đổi ngược không được hỗ trợ cho TextToHyperlinkTextBlockConverter");
        }

        // Đảm bảo URL có tiền tố http:// hoặc https://
        private static string EnsureHttpPrefix(string url)
        {
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return "http://" + url;
            }
            return url;
        }

        // Xử lý sự kiện khi click vào Hyperlink
        private static void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = e.Uri.AbsoluteUri,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể mở URL: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            e.Handled = true;
        }
    }
}

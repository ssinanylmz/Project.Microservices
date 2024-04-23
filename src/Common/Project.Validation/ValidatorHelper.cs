using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Project.Extensions;

namespace Project.Validation
{
    public static class ValidatorHelper
    {
        public static bool IsValidEmail(string value)
        {
            if (value.IsNullOrWhitespace()) return false;

            const string pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

            return Regex.Match(value, pattern, RegexOptions.IgnoreCase).Success;
        }

        public static bool IsValidUrl(string value)
        {
            return Uri.TryCreate(value, UriKind.Absolute, out _);
        }

        public static bool IsValidIp(string value)
        {
            return IPAddress.TryParse(value, out var unused) &&
                   (unused.AddressFamily != AddressFamily.InterNetwork || value.Count(c => c == '.') == 3);
        }

        public static bool IsValidDateTime(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            var isDateTime = DateTime.TryParse(value, out _);

            return isDateTime;
        }

        public static bool IsValidPhone(string value)
        {
            value = ValidatorHelper.PhoneReplaceFormat(value) ?? value;

            if (value.IsNullOrWhitespace()) return false;

            const string pattern = @"^(5(\d{9}))$";

            return Regex.Match(value, pattern, RegexOptions.IgnoreCase).Success;
        }

        public static string? PhoneReplaceFormat(string phone)
        {
            if (phone.IsNullOrWhitespace()) return null;

            return phone[0] == '0' ? phone.Substring(1) : phone;
        }

        public static bool IsGeneralValidPhone(string value)
        {
            value = ValidatorHelper.PhoneReplaceFormat(value) ?? value;

            if (value.IsNullOrWhitespace()) return false;

            const string pattern = @"^((\d{10}))$";

            return Regex.Match(value, pattern, RegexOptions.IgnoreCase).Success;
        }

        public static bool IsValidFullName(string value)
        {
            if (value.IsNullOrWhitespace()) return false;

            const string pattern = @"^(?!.*[<>\""%;&+()|=!\\\d+]).*$";

            return Regex.Match(value, pattern, RegexOptions.IgnorePatternWhitespace).Success;
        }

        public static bool IsValidCustomerMessage(string value)
        {
            if (value.IsNullOrWhitespace()) return false;

            const string pattern = @"^[^<>\""%;&+()|=$\\+]+$";

            return Regex.Match(value, pattern, RegexOptions.IgnorePatternWhitespace).Success;
        }

        public static bool IsValidCustomerQuestion(string value)
        {
            if (value.IsNullOrWhitespace()) return false;

            const string pattern = @"^[^<>\""%;&+()|=$\\+]+$";

            return Regex.Match(value, pattern, RegexOptions.IgnorePatternWhitespace).Success;
        }
    }
}

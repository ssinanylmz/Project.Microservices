using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Project.Extensions
{
    public static class StringExtensions
    {
        public static string UppercaseFirstCharOfWord(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Length > 1
                ? $"{char.ToUpper(value[0])}{value.Substring(1).ToLower()}"
                : char.ToUpper(value[0]).ToString();
        }

        public static string UppercaseFirstCharOfWords(this string value)
        {
            var array = value.ToLower().ToCharArray();

            if (array.Length >= 1 && char.IsLower(array[0])) array[0] = char.ToUpper(array[0]);

            for (var i = 1; i < array.Length; i++)
            {
                if (array[i - 1] != ' ') continue;
                if (char.IsLower(array[i]))
                    array[i] = char.ToUpper(array[i]);
            }

            return new string(array);
        }

        public static bool IsNullOrWhitespace(this string value)
        {
            return value == null || value.Trim().Length == 0;
        }

        public static string SanitizeJson(this string value)
        {
            var sb = new StringBuilder(value);

            sb.Replace(@"\\", @"\").Replace(@"\\\\", @"\\");

            return sb.ToString();
        }

        public static T StringToEnum<T>(this string value, bool ignoreCase = true)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value, ignoreCase);
            }
            catch
            {
                return (T)Enum.Parse(typeof(T), "Unknown", ignoreCase);
            }
        }

        public static int GetDeterministicHashCode(this string value)
        {
            unchecked
            {
                var hash1 = (5381 << 16) + 5381;
                var hash2 = hash1;

                var input = value.ToLower();

                for (var i = 0; i < input.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ input[i];
                    if (i == input.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ input[i + 1];
                }

                var result = hash1 + (hash2 * 1566083941);

                if (result < 0)
                    result *= -1;

                return result;
            }
        }

        public static Guid ToGuid(this string value)
        {
            Guid.TryParse(value, out var guid);
            return guid;
        }

        public static string RemoveLastChar(this string value)
        {
            if (!value.IsNullOrWhitespace())
                value = value.Remove(value.Length - 1, 1);

            return value;
        }

        public static bool IsGuid(this string value)
        {
            return Guid.TryParse(value, out _);
        }

        public static bool IsAlphaNumeric(this string value)
        {
            return value.All(char.IsLetterOrDigit);
        }

        public static string GetDomain(this string value)
        {
            try
            {
                var uri = new Uri($"mailto:{value}");

                return uri.Host;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetMaskedEmail(this string value)
        {
            try
            {
                var pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)";

                return Regex.Replace(value, pattern, p => new string('*', p.Length + 1));
            }
            catch
            {
                return value;
            }
        }

        public static string GetRootDomain(this string value)
        {
            try
            {
                if (!value.Contains(Uri.SchemeDelimiter))
                    value = string.Concat(Uri.UriSchemeHttp, Uri.SchemeDelimiter, value);

                var host = new Uri(value).Host;

                return host.Substring(host.LastIndexOf('.', host.LastIndexOf('.') - 1) + 1);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetFormattedSql(this string[] value)
        {
            try
            {
                if (value != null && value.Length > 0)
                    return "'" + string.Join("','", value) + "'";

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string FormatForSearch(this string main)
        {
            if (string.IsNullOrEmpty(main))
            {
                return main;
            }

            main = main.Replace("%", " ").Replace("&", " ").Replace("!", " ").Replace("?", " ").Replace(",", " ")
                .Replace(@".", " ").Replace(":", " ").Replace(";", " ").Replace("/", " ").Replace(@"\", " ")
                .Replace(@"(", " ").Replace(@")", " ").Replace(@")", " ").Replace("*", " ").Replace(@"""", " ").FormatForSearch(@"'");

            var sb = new StringBuilder();
            foreach (var m in main.Split(' '))
            {
                if (string.IsNullOrEmpty(m) || (m.Length < 2 && main.Length < 5) ||
                    (m.Length == 2 && m.IsNumeric() && main.Length < 5))
                    continue;

                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(" ");
                sb.Append($"{m}");
            }

            return sb.ToString();
        }

        public static string FormatForSearch(this string main, string delimiter)
        {

            if (string.IsNullOrEmpty(main) || !main.Contains(delimiter))
            {
                return main;
            }


            var sb = new StringBuilder();
            var result = new StringBuilder();
            sb.Append(main.Trim());

            try
            {
                var keywordIndexOfValue = sb.ToString().IndexOf(delimiter, StringComparison.Ordinal);

                result.Append(sb.ToString().IndexOf(delimiter, StringComparison.Ordinal) > 0 ? sb.ToString().GetStringLeft(keywordIndexOfValue) : sb.ToString());
                sb = new StringBuilder(sb.ToString().GetStringRight(keywordIndexOfValue));

                if (sb.ToString().IndexOf(" ", StringComparison.Ordinal) > 0)
                {
                    result.Append("^ ");
                    result.Append(sb.ToString().GetStringRight(sb.ToString().IndexOf(" ", StringComparison.Ordinal)));
                }
            }
            catch (Exception)
            {

            }
            return result.ToString();
        }

        public static string GetStringRight(this string main, int length)
        {
            if (string.IsNullOrEmpty(main))
                return main;

            if (main.Trim().Length <= length)
                return main.Trim();

            return main.Trim().Substring(length, (main.Length - length));

        }

        public static string GetStringLeft(this string main, int length)
        {
            if (string.IsNullOrEmpty(main))
                return main;

            if (main.Trim().Length <= length)
                return main.Trim();

            return main.Trim().Substring(0, length);

        }

        public static string RemoveExtraBlank(this string main)
        {
            if (string.IsNullOrEmpty(main))
                return main;

            try
            {
                main = main.Trim();

                var loopState = true;
                while (loopState)
                {
                    if (main.IndexOf("  ") > 0)
                        main = main.Replace("  ", " ");
                    else
                        loopState = false;
                }
            }
            catch (Exception)
            {
                return main;
            }

            return main;
        }

        public static List<string> TurkhisChracterReplace(this string keyword)
        {

            #region Constant & Validation
            if (string.IsNullOrEmpty(keyword))
                return new List<string>();


            char[] ArrayOfTurkhisChracter = { 'ğ', 'ü', 'ş', 'i', 'ö', 'ç' };
            char[] ArrayOfGlobalChracter = { 'g', 'u', 's', 'ı', 'o', 'c' };

            char[] ArrayOfSpesificChracterTr = { 'i' };
            char[] ArrayOfSpesificChracterGl = { 'ı' };

            Dictionary<char, char> CollectionOfChracterSet = new Dictionary<char, char>();
            char[] keywordCharSet;

            List<int> baseTermCharacterOrder = new List<int>();
            List<int> refChracterInbaseTerm = new List<int>();

            List<string> newTerm = new List<string>();

            #endregion

            #region Method
            for (int i = 0; i < ArrayOfTurkhisChracter.Length; i++)
            {
                CollectionOfChracterSet.Add(ArrayOfTurkhisChracter[i], ArrayOfGlobalChracter[i]);
            }

            keywordCharSet = keyword.ToCharArray();
            for (int j = 0; j < keywordCharSet.Length; j++)
            {
                for (int i = 0; i < ArrayOfTurkhisChracter.Length; i++)
                {
                    var chracter = ArrayOfTurkhisChracter[i];

                    if (keywordCharSet[j] == chracter)
                    {
                        baseTermCharacterOrder.Add(j);
                        refChracterInbaseTerm.Add(i);
                    }
                }
            }


            if (refChracterInbaseTerm.Count > 0)
            {
                for (int i = 0; i < refChracterInbaseTerm.Count; i++)
                {
                    if (!string.IsNullOrEmpty(CollectionOfChracterSet[ArrayOfTurkhisChracter[refChracterInbaseTerm[i]]].ToString()))
                    {
                        keywordCharSet[baseTermCharacterOrder[i]] = CollectionOfChracterSet[ArrayOfTurkhisChracter[refChracterInbaseTerm[i]]];
                    }
                }

                newTerm.Add(new string(keywordCharSet));
            }





            var temp_keywordCharSet = keywordCharSet;
            bool changeState = false;
            for (int i = 0; i < temp_keywordCharSet.Length; i++)
            {
                if (temp_keywordCharSet[i] == 'i')
                {
                    temp_keywordCharSet[i] = 'ı';
                    changeState = true;
                }
            }
            if (changeState)
            {

                newTerm.Add(new string(temp_keywordCharSet));
            }

            var temp__keywordCharSet = keywordCharSet;
            bool _changeState = false;
            for (int i = 0; i < temp_keywordCharSet.Length; i++)
            {
                if (temp_keywordCharSet[i] == 'ı')
                {
                    temp_keywordCharSet[i] = 'i';
                    _changeState = true;
                }
            }
            if (_changeState)
            {

                newTerm.Add(new string(temp_keywordCharSet));
            }
            #endregion

            #region Return
            return newTerm;
            #endregion
        }

        public static Int32 ToInt32(this string input)
        {
            int intValue;
            if (Int32.TryParse(input, out intValue))
                return intValue;
            else
                return -1;
        }

        public static string GetCreditCardNumber(this string cardNumber)
        {
            return string.IsNullOrEmpty(cardNumber) ? "" : (cardNumber.Length > 14 ? $"{cardNumber.Substring(0, 8)}********" : cardNumber);
        }

        /// <summary>
        ///     Converts a string that has been encoded for transmission in a URL into a decoded string.
        /// </summary>
        /// <param name="str">The string to decode.</param>
        /// <returns>A decoded string.</returns>
        public static String UrlDecode(this String str)
        {
            return HttpUtility.UrlDecode(str);
        }

        /// <summary>
        ///     Converts a URL-encoded string into a decoded string, using the specified encoding object.
        /// </summary>
        /// <param name="str">The string to decode.</param>
        /// <param name="e">The  that specifies the decoding scheme.</param>
        /// <returns>A decoded string.</returns>
        public static String UrlDecode(this String str, Encoding e)
        {
            return HttpUtility.UrlDecode(str, e);
        }

        public static bool IsBetween<T>(this T item, T start, T end)
        {
            if (item == null)
                return false;

            return Comparer<T>.Default.Compare(item, start) >= 0
                    && Comparer<T>.Default.Compare(item, end) <= 0;
        }

        public static string StockImageMiddleSize(this string value)
        {
            var sb = new StringBuilder(value);

            sb.Replace(@".jpg", @"-orta.jpg").Replace(@".jpeg", @"-orta.jpeg");

            return sb.ToString();
        }

        public static string StockImageSmallSize(this string value)
        {
            var sb = new StringBuilder(value);

            sb.Replace(@".jpg", @"-kucuk.jpg").Replace(@".jpeg", @"-kucuk.jpeg");

            return sb.ToString();
        }

        public static string HtmlCodeToTurkish(this string value)
        {
            return WebUtility.HtmlDecode(value);
            //.Replace("&#199;", "Ç").Replace("&#xC7;", "Ç")
            //.Replace("&#231;", "ç").Replace("&#xE7;", "ç")
            //.Replace("&#286;", "Ğ").Replace("&#x11E;", "Ğ")
            //.Replace("&#287;", "ğ").Replace("&#x11F;", "ğ")
            //.Replace("&#304;", "İ").Replace("&#x130;", "İ")
            //.Replace("&#305;", "ı").Replace("&#x131;", "ı")
            //.Replace("&#214;", "Ö").Replace("&#xD6;", "Ö")
            //.Replace("&#246;", "ö").Replace("&#xF6;", "ö")
            //.Replace("&#350;", "Ş").Replace("&#x15E;", "Ş")
            //.Replace("&#351;", "ş").Replace("&#x15F;", "ş")
            //.Replace("&#220;", "Ü").Replace("&#xDC;", "Ü")
            //.Replace("&#252;", "ü").Replace("&#xFC;", "ü")
            //.Replace("&#38;", "&").Replace("&amp;", "&")
            //.Replace("&#32;", " ").Replace("&#x20;", " ")
            //.Replace("&#33;", "!").Replace("&#x21;", "!")
            //.Replace("&#34;", "\"").Replace("&quot;", "\"")
            //.Replace("&#35;", "#").Replace("&#x23;", "#")
            //.Replace("&#36;", "$").Replace("&#x24;", "$")
            //.Replace("&#37;", "%").Replace("&#x25;", "%")
            //.Replace("&#39;", "'").Replace("&#x27;", "'")
            //.Replace("&#40;", "(").Replace("&#x28;", "(")
            //.Replace("&#41;", ")").Replace("&#x29;", ")")
            //.Replace("&#42;", "*").Replace("&#x2a;", "*")
            //.Replace("&#43;", "+").Replace("&#x2B;", "+")
            //.Replace("&#44;", ",").Replace("&#x2c;", ",")
            //.Replace("&#45;", "-").Replace("&#x2d;", "-")
            //.Replace("&#46;", ".").Replace("&#x2e;", ".")
            //.Replace("&#47;", "/").Replace("&#x2f;", "/")
            //.Replace("&#48;", "0").Replace("&#x30;", "0")
            //.Replace("&#49;", "1").Replace("&#x31;", "1")
            //.Replace("&#50;", "2").Replace("&#x32;", "2")
            //.Replace("&#51;", "3").Replace("&#x33;", "3")
            //.Replace("&#52;", "4").Replace("&#x34;", "4")
            //.Replace("&#53;", "5").Replace("&#x35;", "5")
            //.Replace("&#54;", "6").Replace("&#x36;", "6")
            //.Replace("&#55;", "7").Replace("&#x37;", "7")
            //.Replace("&#56;", "8").Replace("&#x38;", "8")
            //.Replace("&#57;", "9").Replace("&#x39;", "9")
            //.Replace("&#58;", ":").Replace("&#x3a;", ":")
            //.Replace("&#59;", ";").Replace("&#x3b;", ";")
            //.Replace("&#60;", "<").Replace("&lt;", "<")
            //.Replace("&#61;", "=").Replace("&#x3d;", "=")
            //.Replace("&#62;", ">").Replace("&gt;", ">")
            //.Replace("&#63;", "?").Replace("&#x3f;", "?")
            //.Replace("&#64;", "@").Replace("&#x40;", "@")
            //.Replace("&#91;", "[").Replace("&#x5b;", "[")
            //.Replace("&#92;", "\\").Replace("&#x5c;", "\\")
            //.Replace("&#93;", "]").Replace("&#x5d;", "]")
            //.Replace("&#95;", "_").Replace("&#x5f;", "_")
            //.Replace("&#96;", "`").Replace("&#x60;", "`")
            //.Replace("&#123;", "{").Replace("&#x7b;", "{")
            //.Replace("&#124;", "|").Replace("&#x7c;", "|")
            //.Replace("&#125;", "}").Replace("&#x7d;", "}")
            //.Replace("&#126;", "~").Replace("&#x7e;", "~");
        }
        public static List<string> StringSplitToList(this string main)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(main))
            {
                return list;
            }

            main = main.Replace("%", " ").Replace("&", " ").Replace("!", " ").Replace("?", " ").Replace(",", " ")
                .Replace(@".", " ").Replace(":", " ").Replace(";", " ").Replace("/", " ").Replace(@"\", " ")
                .Replace(@"(", " ").Replace(@")", " ").Replace(@")", " ").Replace("*", " ").Replace(@"""", " ").FormatForSearch(@"'");

            foreach (var m in main.Split(' '))
            {
                if (!string.IsNullOrEmpty(m))
                    list.Add($"{m}");
            }

            return list;
        }

        public static string GenerateSlug(this string value)
        {
            var slug = value.RemoveAccent().ToLower();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", " ").Trim();
            slug = slug.Substring(0, slug.Length <= 45 ? slug.Length : 45).Trim();
            slug = Regex.Replace(slug, @"\s", "-");
            return slug.ToLower();
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public static string TakingAfterCharacter(this string value, char key)
        {
            var index = value.LastIndexOf(key);

            return value.Substring(index + 1);
        }
    }
}

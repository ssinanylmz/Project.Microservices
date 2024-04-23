using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Project.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNumericAndGreaterThenZero(this object value)
        {
            try
            {
                return Convert.ToDouble(value) > 0;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsNumericAndGreaterOrEqualZero(this object value)
        {
            try
            {
                return Convert.ToDouble(value) >= 0;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsNumeric(this object value)
        {
            try
            {
                return value is sbyte
                       || value is byte
                       || value is short
                       || value is ushort
                       || value is int
                       || value is uint
                       || value is long
                       || value is ulong
                       || value is float
                       || value is double
                       || value is decimal;
            }
            catch
            {
                return false;
            }
        }

        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static int GetMaxLength<T>(this T obj, string propertyName) where T : class
        {
            var attribute = (StringLengthAttribute)obj?.GetType().GetProperty(propertyName)!
                .GetCustomAttribute(typeof(StringLengthAttribute), false)!;
            if (attribute != null)
                return attribute.MaximumLength;
            return -1;
        }

        public static bool IsRequired<T>(this T obj, string propertyName) where T : class
        {
            var attribute = (StringLengthAttribute)obj?.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)!
                .GetCustomAttribute(typeof(RequiredAttribute), false)!;
            return attribute != null;
        }

        public static T? DeepCopy<T>(this T self)
        {
            var serialized = JsonConvert.SerializeObject(self);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}

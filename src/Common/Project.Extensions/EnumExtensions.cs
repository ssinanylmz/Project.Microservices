using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace Project.Extensions
{
    public static class EnumExtensions
    {
        public static List<string> GetEnumNames<T>()
        {
            return typeof(T).GetFields()
                .Select(info => info.Name)
                .Distinct()
                .ToList();
        }

        public static List<T?> GetEnumValues<T>() where T : new()
        {
            var valueType = new T();
            return typeof(T).GetFields()
                .Select(fieldInfo => (T?)fieldInfo.GetValue(valueType))
                .Distinct()
                .ToList();
        }

        public static string? GetDescription<T>(this T enumerationValue) where T : struct
        {
            var type = enumerationValue.GetType();

            if (!type.IsEnum)
                throw new ArgumentException("EnumerationValue must be of Enum type", nameof(enumerationValue));

            var memberInfo = type.GetMember(enumerationValue.ToString() ?? string.Empty);
            if (memberInfo.Length <= 0) return enumerationValue.ToString();
            var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attrs.Length > 0 ? ((DescriptionAttribute)attrs[0]).Description : enumerationValue.ToString();
        }

        public static string GetEnumDescription(this Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false)!;

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
        }

        public static bool TryParseWithMemberName<TEnum>(this string value, out TEnum result) where TEnum : struct
        {
            result = default;

            if (string.IsNullOrEmpty(value))
                return false;

            Type enumType = typeof(TEnum);

            foreach (string name in Enum.GetNames(typeof(TEnum)))
            {
                if (name.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    result = Enum.Parse<TEnum>(name);
                    return true;
                }

                EnumMemberAttribute? memberAttribute 
                    = enumType.GetField(name)?.GetCustomAttribute(typeof(EnumMemberAttribute)) as EnumMemberAttribute;

                if (memberAttribute is null)
                    continue;

                if (memberAttribute.Value!.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    result = Enum.Parse<TEnum>(name);
                    return true;
                }
            }
            return false;
        }
        public static TEnum? GetEnumValueOrDefault<TEnum>(this string value) where TEnum : struct
        {
            if (TryParseWithMemberName(value, out TEnum result))
                return result;

            return default;
        }

        public static string GetEnumValueMustBeBetweenErrorMessage(this Enum enumerationValue)
        {
            var type = enumerationValue?.GetType();

            if (!type!.IsEnum)
                throw new ArgumentException("EnumerationValue must be of Enum type", nameof(enumerationValue));

            var enumValueList = Enum.GetValues(type);

            return string.Format("{0} value must be between {1} and {2}", nameof(enumerationValue),
              (int)enumValueList?.GetValue(0)!,
              (int)enumValueList?.GetValue(Enum.GetValues(type).Length - 1)!);
        }

        public static string? GetEnumKeyName(this Enum enumerationValue, int value)
        {
            var type = enumerationValue?.GetType();

            if (type!.IsEnum)
                return Enum.GetName(type, value);

            return String.Empty;
        }
        public static string? GetEnumKeyName<TEnum>(this TEnum enumerationValue, int value) where TEnum : Enum
        {
            var type = enumerationValue?.GetType();

            if (type!.IsEnum)
                return Enum.GetName(type, value);

            return String.Empty;
        }
    }
}

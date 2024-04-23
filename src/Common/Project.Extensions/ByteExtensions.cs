namespace Project.Extensions
{
    public static class ByteExtensions
    {
        public static bool ByteToBool(this byte value)
        {
            return value == 1;
        }

        public static T ByteToEnum<T>(this byte value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value.ToString(), ignoreCase);
        }
        public static int ByteToInt(this bool value)
        {
            return value ? 1 : 0;
        }
    }
}

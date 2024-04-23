namespace Project.Shared.Helper
{
    public static class SD
    {
        public static string AuthBaseUri { get; set; }
        public static string DiscountBaseUri { get; set; }
        public static string AccountBaseUri { get; set; }
        public static string BasketBaseUri { get; set; }
        public static string InfosetBaseUri { get; set; }
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}

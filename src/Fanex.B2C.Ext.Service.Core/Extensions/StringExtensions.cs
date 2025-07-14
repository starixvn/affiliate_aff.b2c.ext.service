namespace Fanex.B2C.Ext.Service.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool MatchWith(this string value, string strToMatch)
        {
            return string.Equals(value, strToMatch, StringComparison.OrdinalIgnoreCase);
        }
    }
}

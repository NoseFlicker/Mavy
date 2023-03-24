namespace svchost.extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Ignores lower and UPPER case characters when comparing a string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string str, string content) => str.ToLower() == content;
    }
}
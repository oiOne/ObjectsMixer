using System.Text.RegularExpressions;

namespace ObjectsMixer.Tests.Services
{
    public static class StrHelpers
    {
        public static bool HasSingleQuote(this string str)
        {
            return (str.StartsWith("\'") && str.EndsWith("\'"));
        }

        public static string NoQuotation(this string quotedLine)
        {
            return (!quotedLine.StartsWith("\'") || !quotedLine.EndsWith("\'"))
                ? quotedLine
                : Regex.Match(quotedLine, @"'(.*?)'").Groups[1].Value;
        }
    }
}

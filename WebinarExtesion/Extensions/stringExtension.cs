using System.Text.RegularExpressions;

namespace WebinarExtension.servext.Extensions
{
    internal static class StringExtension
    {
        public static string PhoneNumberDigits(this string phoneNumber)
        {
            return Regex.Replace(phoneNumber, "[^0-9 _]", "").Remove(0, 1);
        }
    }
}
using System.Text.RegularExpressions;

namespace renamerIdee
{
    public static class Helpers
    {
        public static int? ExtractFirstNumber(string input)
        {
            var m = Regex.Match(input, @"\d+");
            if (m.Success && int.TryParse(m.Value, out int val))
                return val;
            return null;
        }
    }
}

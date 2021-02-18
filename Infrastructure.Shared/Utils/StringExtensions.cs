namespace Infrastructure.Shared.Utils
{
    static public class StringExtensions
    {
        static public bool IsNullOrEmpty(this string value) =>
            string.IsNullOrEmpty(value);

        static public bool IsNotNullOrEmpty(this string value) =>
            !string.IsNullOrEmpty(value);
    }
}

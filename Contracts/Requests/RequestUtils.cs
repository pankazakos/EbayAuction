namespace contracts.Requests
{
    internal static class RequestUtils
    {
        public static void EnsureStringContent(string fieldName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"{fieldName} cannot be empty or whitespace");
            }
        }

        public static void EnsurePositiveNumber(string fieldName, int value)
        {
            if (value <= 0)
            {
                throw new ArgumentException($"{fieldName} must be greater than 0");
            }
        }

        public static void EnsurePositiveNumber(string fieldName, decimal value)
        {
            if (value <= 0)
            {
                throw new ArgumentException($"{fieldName} must be greater than 0");
            }
        }

        public static void EnsureNonNegativeNumber(string fieldName, int value)
        {
            if (value < 0)
            {
                throw new ArgumentException($"{fieldName} must be greater than 0");
            }
        }
    }
}

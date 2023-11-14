using System.Globalization;

namespace contracts.Requests.Item
{
    public class PublishItemRequest : IAppRequest
    {
        public DateTime Expiration { get; init; }
        public void Validate()
        {
            const string format = "yyyy-MM-dd HH:mm";

            var dtInput = Expiration.ToString(CultureInfo.InvariantCulture);

            if (!DateTime.TryParseExact(dtInput, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtExpiration))
            {
                throw new ArgumentException("DateTime format is not correct. Correct format is: \"yyyy-MM-dd HH:mm\" ");
            }

            var now = DateTime.Now.ToString(format);

            _ = DateTime.TryParse(now, out var dtNow);

            if (dtExpiration <= dtNow)
            {
                throw new ArgumentException("Ends datetime cannot be set to a datetime earlier than the current datetime");
            }
        }
    }
}

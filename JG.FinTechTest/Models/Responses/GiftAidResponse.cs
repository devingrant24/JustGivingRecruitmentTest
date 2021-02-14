using Newtonsoft.Json;

namespace JG.FinTechTest.Models.Responses
{
    [JsonObject]
    public class GiftAidResponse
    {
        public double DonationAmount { get; set; }
        public double GiftAidAmount { get; set; }
    }
}

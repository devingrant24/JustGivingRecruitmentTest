using Newtonsoft.Json;

namespace JG.FinTechTest.Models.Responses
{
    [JsonObject]
    public class GiftAidDeclarationResponse
    {
        public int Id { get; set; }
        public double GiftAidAmount { get; set; }
    }
}

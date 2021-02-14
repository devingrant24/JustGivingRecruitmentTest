using Newtonsoft.Json;
using System.Collections.Generic;

namespace JG.FinTechTest.Tests.Controllers.Models
{
    public class GiftAidDeclarationErrorResponse
    {
        [JsonProperty("name")]
        public IEnumerable<string> NameErrors { get; set; }
        [JsonProperty("postCode")]
        public IEnumerable<string> PostCodeErrors { get; set; }
        [JsonProperty("donationAmount")]
        public IEnumerable<string> DonationAmountErrors { get; set; }
    }
}

using Newtonsoft.Json;
using System.Collections.Generic;

namespace JG.FinTechTest.Tests.Controllers.Models
{
    [JsonObject]
    public class GiftAidErrorResponse
    {
        [JsonProperty("amount")]
        public IEnumerable<string> AmountErrors { get; set; }
    }
}

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace JG.FinTechTest.Models.Requests
{
    [JsonObject]
    public class GiftAidDeclarationRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string PostCode { get; set; } // In an actual implementation, I would add a custom validator to ensure this is a valid postcode, based on the region we're operating in
        [Required, Range(2.0, 100000.0)]
        public double DonationAmount { get; set; }
    }
}

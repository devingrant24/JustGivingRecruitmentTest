using JG.FinTechTest.Helpers.Interfaces;
using JG.FinTechTest.Models;
using Microsoft.Extensions.Options;

namespace JG.FinTechTest.Helpers
{
    public class GiftAidCalculator : IGiftAidCalculator
    {
        private readonly double _taxRate;
        private const double DefaultTaxRate = 0.2;

        public GiftAidCalculator(IOptions<GiftAidOptions> options)
        {
            _taxRate = options?.Value?.TaxRate ?? DefaultTaxRate;
        }

        public double CalculateGiftAid(double donationAmount)
        {
            return donationAmount * (_taxRate / (1 - _taxRate));
        }
    }
}

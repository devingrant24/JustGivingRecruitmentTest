using JG.FinTechTest.Handlers.Interfaces;
using JG.FinTechTest.Helpers.Interfaces;
using JG.FinTechTest.Models.Responses;

namespace JG.FinTechTest.Handlers
{
    public class GiftAidHandler : IGiftAidHandler
    {
        private readonly IGiftAidCalculator _calculator;

        public GiftAidHandler(IGiftAidCalculator calculator)
        {
            _calculator = calculator;
        }

        public GiftAidResponse CalculateGiftAid(double donationAmount)
        {
            var giftAidAmount = _calculator.CalculateGiftAid(donationAmount);

            return new GiftAidResponse
            {
                DonationAmount = donationAmount,
                GiftAidAmount = giftAidAmount
            };
        }
    }
}

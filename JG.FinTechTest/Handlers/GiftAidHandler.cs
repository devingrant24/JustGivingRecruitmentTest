using JG.FinTechTest.Handlers.Interfaces;
using JG.FinTechTest.Helpers.Interfaces;
using JG.FinTechTest.Models.Requests;
using JG.FinTechTest.Models.Responses;
using JG.FinTechTest.Storage.Interfaces;

namespace JG.FinTechTest.Handlers
{
    public class GiftAidHandler : IGiftAidHandler
    {
        private readonly IGiftAidCalculator _calculator;
        private readonly IGiftAidDeclarationRepository _repository;

        public GiftAidHandler(IGiftAidCalculator calculator, IGiftAidDeclarationRepository repository)
        {
            _calculator = calculator;
            _repository = repository;
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

        public GiftAidDeclarationResponse CreateGiftAidDeclaration(GiftAidDeclarationRequest request)
        {
            var id = _repository.CreateGiftAidDeclaration(request.Name, request.PostCode, request.DonationAmount);
            var giftAidAmount = _calculator.CalculateGiftAid(request.DonationAmount);

            return new GiftAidDeclarationResponse
            {
                Id = id,
                GiftAidAmount = giftAidAmount
            };
        }
    }
}

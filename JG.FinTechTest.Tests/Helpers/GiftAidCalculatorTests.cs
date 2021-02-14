using JG.FinTechTest.Helpers;
using JG.FinTechTest.Helpers.Interfaces;
using JG.FinTechTest.Models.Options;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace JG.FinTechTest.Tests.Helpers
{
    public class GiftAidCalculatorTests
    {
        private readonly Mock<IOptions<GiftAidOptions>> _options;

        private IGiftAidCalculator _calculator;

        public GiftAidCalculatorTests()
        {
            _options = new Mock<IOptions<GiftAidOptions>>();
            _options.SetupGet(o => o.Value).Returns(new GiftAidOptions { TaxRate = 0.25 });

            _calculator = new GiftAidCalculator(_options.Object);
        }

        [Fact]
        public void Constructor_RetrievesTaxRateFromOptions()
        {
            _options.VerifyGet(o => o.Value, Times.Once);
        }

        [Theory]
        [InlineData(0.0, 0.0)]
        [InlineData(100.0, 100.0*(1.0/3.0))]
        public void CalculateGiftAid_ReturnsCorrectCalculation_UsingTaxRateFromOptions(double donationAmount, double expectedGiftAid)
        {
            var giftAid = _calculator.CalculateGiftAid(donationAmount);
            Assert.Equal(expectedGiftAid, giftAid);
        }

        [Theory]
        [InlineData(0.0, 0.0)]
        [InlineData(100.0, 25.0)]
        public void CalculateGiftAid_NoTaxRateInOptions_ReturnsCorrectCalculation_UsingDefaultTaxRate(double donationAmount, double expectedGiftAid)
        {
            _options.SetupGet(o => o.Value).Returns(new GiftAidOptions());
            _calculator = new GiftAidCalculator(_options.Object);

            var giftAid = _calculator.CalculateGiftAid(donationAmount);
            Assert.Equal(expectedGiftAid, giftAid);
        }
    }
}

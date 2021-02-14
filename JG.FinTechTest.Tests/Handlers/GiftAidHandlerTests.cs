using JG.FinTechTest.Handlers;
using JG.FinTechTest.Handlers.Interfaces;
using JG.FinTechTest.Helpers.Interfaces;
using JG.FinTechTest.Models.Responses;
using Moq;
using Xunit;

namespace JG.FinTechTest.Tests.Handlers
{
    public class GiftAidHandlerTests
    {
        private readonly Mock<IGiftAidCalculator> _calculator;

        private readonly IGiftAidHandler _handler;

        public GiftAidHandlerTests()
        {
            _calculator = new Mock<IGiftAidCalculator>();
            _calculator.Setup(c => c.CalculateGiftAid(It.IsAny<double>())).Returns(20.0);

            _handler = new GiftAidHandler(_calculator.Object);
        }

        [Fact]
        public void CalculateGiftAid_CallsCalculatorCalculateGiftAid_WithAmountProvided()
        {
            _handler.CalculateGiftAid(25.0);

            _calculator.Verify(c => c.CalculateGiftAid(25.0), Times.Once);
        }

        [Fact]
        public void CalculateGiftAid_ReturnsGiftAidResponse()
        {
            var response = _handler.CalculateGiftAid(25.0);
            Assert.NotNull(response);
            Assert.IsType<GiftAidResponse>(response);
        }

        [Fact]
        public void CalculateGiftAid_ReturnsDonationAmountProvided()
        {
            var response = _handler.CalculateGiftAid(25.0);
            Assert.Equal(25.0, response.DonationAmount);
        }

        [Fact]
        public void CalculateGiftAid_ReturnsGiftAidAmount_FromCalculator()
        {
            var response = _handler.CalculateGiftAid(25.0);
            Assert.Equal(20.0, response.GiftAidAmount);
        }
    }
}

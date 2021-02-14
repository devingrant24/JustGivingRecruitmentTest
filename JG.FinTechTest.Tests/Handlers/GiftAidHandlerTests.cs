using JG.FinTechTest.Handlers;
using JG.FinTechTest.Handlers.Interfaces;
using JG.FinTechTest.Helpers.Interfaces;
using JG.FinTechTest.Models.Requests;
using JG.FinTechTest.Models.Responses;
using JG.FinTechTest.Storage.Interfaces;
using Moq;
using Xunit;

namespace JG.FinTechTest.Tests.Handlers
{
    public class GiftAidHandlerTests
    {
        private readonly Mock<IGiftAidCalculator> _calculator;
        private readonly Mock<IGiftAidDeclarationRepository> _repository;

        private readonly IGiftAidHandler _handler;

        public GiftAidHandlerTests()
        {
            _calculator = new Mock<IGiftAidCalculator>();
            _calculator.Setup(c => c.CalculateGiftAid(It.IsAny<double>())).Returns(20.0);

            _repository = new Mock<IGiftAidDeclarationRepository>();
            _repository.Setup(r => r.CreateGiftAidDeclaration(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()))
                .Returns(0);

            _handler = new GiftAidHandler(_calculator.Object, _repository.Object);
        }

        #region CalculateGiftAid

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

        #endregion

        #region CreateGiftAidDeclaration

        [Fact]
        public void CreateGiftAidDeclaration_CallsRepositoryCreateGiftAIdDeclaration_WithRequestProperties()
        {
            var request = new GiftAidDeclarationRequest { Name = "Name", PostCode = "PostCode", DonationAmount = 100.0 };

            _handler.CreateGiftAidDeclaration(request);

            _repository.Verify(r => r.CreateGiftAidDeclaration(request.Name, request.PostCode, request.DonationAmount), Times.Once);
        }

        [Fact]
        public void CreateGiftAidDeclaration_CallsCalculatorCalculateGiftAid_WithDonationFromRequest()
        {
            var request = new GiftAidDeclarationRequest { DonationAmount = 100.0 };

            _handler.CreateGiftAidDeclaration(request);

            _calculator.Verify(c => c.CalculateGiftAid(request.DonationAmount), Times.Once);
        }

        [Fact]
        public void CreateGiftAidDeclaration_ReturnsGiftAidDeclarationResponse()
        {
            var response = _handler.CreateGiftAidDeclaration(new GiftAidDeclarationRequest());
            Assert.NotNull(response);
            Assert.IsType<GiftAidDeclarationResponse>(response);
        }

        [Fact]
        public void CreateGiftAidDeclaration_ReturnsId_FromRepository()
        {
            const int expectedId = 24;
            _repository.Setup(r => r.CreateGiftAidDeclaration(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()))
                .Returns(expectedId);

            var response = _handler.CreateGiftAidDeclaration(new GiftAidDeclarationRequest());
            Assert.Equal(expectedId, response.Id);
        }

        [Fact]
        public void CreateGiftAidDeclaration_ReturnsGiftAidAmount_FromCalculator()
        {
            const double expectedAmount = 200.0;
            _calculator.Setup(c => c.CalculateGiftAid(It.IsAny<double>())).Returns(expectedAmount);

            var response = _handler.CreateGiftAidDeclaration(new GiftAidDeclarationRequest());
            Assert.Equal(expectedAmount, response.GiftAidAmount);
        }

        #endregion
    }
}

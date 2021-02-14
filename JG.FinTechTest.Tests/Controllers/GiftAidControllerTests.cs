using JG.FinTechTest.Controllers;
using JG.FinTechTest.Helpers.Interfaces;
using JG.FinTechTest.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace JG.FinTechTest.Tests.Controllers
{
    public class GiftAidControllerTests
    {
        private readonly Mock<IGiftAidCalculator> _calculator;

        private readonly GiftAidController _controller;

        public GiftAidControllerTests()
        {
            _calculator = new Mock<IGiftAidCalculator>();
            _calculator.Setup(c => c.CalculateGiftAid(It.IsAny<double>())).Returns(20.0);

            _controller = new GiftAidController(_calculator.Object);
        }

        [Fact]
        public void Class_HasCorrectRouteAttribute()
        {
            var attribute = typeof(GiftAidController).GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault();
            Assert.NotNull(attribute);

            var routeAttribute = (RouteAttribute)Convert.ChangeType(attribute, typeof(RouteAttribute));
            Assert.Equal("api/giftaid", routeAttribute.Template);
        }

        [Fact]
        public void Class_HasApiControllerAttribute()
        {
            var attribute = typeof(GiftAidController).GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void CalculateGiftAid_AmountParameter_HasRequiredAttribute()
        {
            var parameter = typeof(GiftAidController)
                .GetMethod(nameof(GiftAidController.CalculateGiftAid))
                .GetParameters()
                .SingleOrDefault();
            Assert.NotNull(parameter);

            var requiredAttribute = parameter.CustomAttributes.SingleOrDefault(a => a.AttributeType == typeof(RequiredAttribute));
            Assert.NotNull(requiredAttribute);
        }

        [Fact]
        public void CalculateGiftAid_CallsCalculatorCalculateGiftAid_WithAmountProvided()
        {
            _controller.CalculateGiftAid(25.0);

            _calculator.Verify(c => c.CalculateGiftAid(25.0), Times.Once);
        }

        [Fact]
        public void CalculateGiftAid_ReturnsOkObjectResult()
        {
            var response = _controller.CalculateGiftAid(25.0);
            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(200, ((OkObjectResult)response.Result).StatusCode);
        }

        [Fact]
        public void CalculateGiftAid_ReturnsGiftAidResponse()
        {
            var response = _controller.CalculateGiftAid(25.0);
            Assert.NotNull(response);
            Assert.NotNull(((OkObjectResult)response.Result).Value);
            Assert.IsType<GiftAidResponse>(((OkObjectResult)response.Result).Value);
        }

        [Fact]
        public void CalculateGiftAid_ReturnsDonationAmountProvided()
        {
            var response = _controller.CalculateGiftAid(25.0);
            Assert.Equal(25.0, ((GiftAidResponse)((OkObjectResult)response.Result).Value).DonationAmount);
        }

        [Fact]
        public void CalculateGiftAid_ReturnsGiftAidAmount_FromCalculator()
        {
            var response = _controller.CalculateGiftAid(25.0);
            Assert.Equal(20.0, ((GiftAidResponse)((OkObjectResult)response.Result).Value).GiftAidAmount);
        }
    }
}

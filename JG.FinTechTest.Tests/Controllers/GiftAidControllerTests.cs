using JG.FinTechTest.Controllers;
using JG.FinTechTest.Handlers.Interfaces;
using JG.FinTechTest.Models.Requests;
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
        private readonly Mock<IGiftAidHandler> _handler;

        private readonly GiftAidController _controller;

        public GiftAidControllerTests()
        {
            _handler = new Mock<IGiftAidHandler>();
            _handler.Setup(c => c.CalculateGiftAid(It.IsAny<double>())).Returns(new GiftAidResponse());
            _handler.Setup(c => c.CreateGiftAidDeclaration(It.IsAny<GiftAidDeclarationRequest>())).Returns(new GiftAidDeclarationResponse());

            _controller = new GiftAidController(_handler.Object);
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

        #region CalculateGiftAid

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
        public void CalculateGiftAid_AmountParameter_HasCorrectRangeAttribute()
        {
            var parameter = typeof(GiftAidController)
                .GetMethod(nameof(GiftAidController.CalculateGiftAid))
                .GetParameters()
                .SingleOrDefault();
            Assert.NotNull(parameter);

            var rangeAttribute = parameter.CustomAttributes.SingleOrDefault(a => a.AttributeType == typeof(RangeAttribute));
            Assert.NotNull(rangeAttribute);

            Assert.Equal(2, rangeAttribute.ConstructorArguments.Count);
            Assert.Equal(2.0, rangeAttribute.ConstructorArguments[0].Value);
            Assert.Equal(100000.0, rangeAttribute.ConstructorArguments[1].Value);
        }

        [Fact]
        public void CalculateGiftAid_CallsHandlerCalculateGiftAid_WithAmountProvided()
        {
            _controller.CalculateGiftAid(25.0);

            _handler.Verify(c => c.CalculateGiftAid(25.0), Times.Once);
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
        public void CalculateGiftAid_ReturnsGiftAidResponseFromHandler()
        {
            var expectedResponse = new GiftAidResponse { DonationAmount = 100.0, GiftAidAmount = 25.0 };
            _handler.Setup(h => h.CalculateGiftAid(It.IsAny<double>())).Returns(expectedResponse);

            var response = _controller.CalculateGiftAid(25.0);
            Assert.Same(expectedResponse, (GiftAidResponse)((OkObjectResult)response.Result).Value);
        }

        #endregion

        #region CreateGiftAidDeclaration

        [Fact]
        public void CreateGiftAidDeclaration_CallsHandlerCreateGiftAidDeclaration_WithRequestProvided()
        {
            var request = new GiftAidDeclarationRequest { Name = "Name", PostCode = "PostCode", DonationAmount = 200.0 };

            _controller.CreateGiftAidDeclaration(request);

            _handler.Verify(c => c.CreateGiftAidDeclaration(request), Times.Once);
        }

        [Fact]
        public void CreateGiftAidDeclaration_ReturnsOkObjectResult()
        {
            var response = _controller.CreateGiftAidDeclaration(new GiftAidDeclarationRequest { Name = "Name", PostCode = "PostCode", DonationAmount = 200.0 });
            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(200, ((OkObjectResult)response.Result).StatusCode);
        }

        [Fact]
        public void CreateGiftAidDeclaration_ReturnsGiftAidDeclarationResponse()
        {
            var response = _controller.CreateGiftAidDeclaration(new GiftAidDeclarationRequest { Name = "Name", PostCode = "PostCode", DonationAmount = 200.0 });
            Assert.NotNull(response);
            Assert.NotNull(((OkObjectResult)response.Result).Value);
            Assert.IsType<GiftAidDeclarationResponse>(((OkObjectResult)response.Result).Value);
        }

        [Fact]
        public void CreateGiftAidDeclaration_ReturnsGiftAidDeclarationResponseFromHandler()
        {
            var expectedResponse = new GiftAidDeclarationResponse { Id = 24, GiftAidAmount = 20.0 };
            _handler.Setup(h => h.CreateGiftAidDeclaration(It.IsAny<GiftAidDeclarationRequest>())).Returns(expectedResponse);

            var response = _controller.CreateGiftAidDeclaration(new GiftAidDeclarationRequest { Name = "Name", PostCode = "PostCode", DonationAmount = 200.0 });
            Assert.Same(expectedResponse, (GiftAidDeclarationResponse)((OkObjectResult)response.Result).Value);

        }

        #endregion
    }
}

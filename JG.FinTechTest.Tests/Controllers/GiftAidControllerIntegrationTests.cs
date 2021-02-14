using JG.FinTechTest.Models.Responses;
using JG.FinTechTest.Tests.Controllers.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace JG.FinTechTest.Tests.Controllers
{
    public class GiftAidControllerIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        private const string GiftAidControllerPath = "/api/giftaid";

        public GiftAidControllerIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        private async Task<HttpResponseMessage> SendRequest(double? donationAmount = null)
        {
            var uriString = GiftAidControllerPath;
            if (donationAmount != null) uriString += $"?amount={donationAmount}";

            var response = await _client.GetAsync(uriString);

            return response;
        }

        [Fact]
        public async Task CalculateGiftAid_AmountProvided_Returns200()
        {
            var response = await SendRequest(100.0);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CalculateGiftAid_AmountProvided_ReturnsAmountInGiftAidResponse()
        {
            const double donationAmount = 100.0;

            var response = await SendRequest(donationAmount);
            var giftAidResponse = JsonConvert.DeserializeObject<GiftAidResponse>(await response.Content.ReadAsStringAsync());

            Assert.Equal(donationAmount, giftAidResponse.DonationAmount);
        }

        [Fact]
        public async Task CalculateGiftAid_AmountProvided_ReturnsExpectedGiftAidAmount()
        {
            const double donationAmount = 100.0;
            const double taxRate = 0.2;
            var expectedGiftAid = donationAmount * (taxRate / (1 - taxRate));

            var response = await SendRequest(donationAmount);
            var giftAidResponse = JsonConvert.DeserializeObject<GiftAidResponse>(await response.Content.ReadAsStringAsync());

            Assert.Equal(expectedGiftAid, giftAidResponse.GiftAidAmount);
        }

        [Fact]
        public async Task CalculateGiftAid_NoAmountProvided_Returns400()
        {
            var response = await SendRequest();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CalculateGiftAid_NoAmountProvided_ReturnsCorrectValidationError()
        {
            var response = await SendRequest();
            var errorResponse = JsonConvert.DeserializeObject<GiftAidErrorResponse>(await response.Content.ReadAsStringAsync());

            var validationError = Assert.Single(errorResponse.AmountErrors);
            Assert.Equal("The amount field is required.", validationError);
        }

        [Theory]
        [InlineData(1.0)]
        [InlineData(100001.0)]
        public async Task CalculateGiftAid_AmountProvidedOutsideOfRange_Returns400(double donationAmount)
        {
            var response = await SendRequest(donationAmount);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData(1.0)]
        [InlineData(100001.0)]
        public async Task CalculateGiftAid_AmountProvidedOutsideOfRange_ReturnsCorrectValidationError(double donationAmount)
        {
            var response = await SendRequest(donationAmount);
            var errorResponse = JsonConvert.DeserializeObject<GiftAidErrorResponse>(await response.Content.ReadAsStringAsync());

            var validationError = Assert.Single(errorResponse.AmountErrors);
            Assert.Equal("The field amount must be between 2 and 100000.", validationError);
        }
    }
}

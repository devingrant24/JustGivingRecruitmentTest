using JG.FinTechTest.Models.Options;
using JG.FinTechTest.Models.Requests;
using JG.FinTechTest.Models.Responses;
using JG.FinTechTest.Models.Storage;
using JG.FinTechTest.Tests.Controllers.Models;
using LiteDB;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace JG.FinTechTest.Tests.Controllers
{
    public class GiftAidControllerIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        private const string GiftAidControllerPath = "/api/giftaid";
        private const string TestDatabase = "Declarations-Integration-Test.db";

        private bool _cleanUpRequired;

        public GiftAidControllerIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbConfigSection = new Mock<IConfigurationSection>();
                    var declarationsDbSection = new Mock<IConfigurationSection>();
                    declarationsDbSection.SetupGet(s => s.Value).Returns(TestDatabase);
                    dbConfigSection.Setup(s => s.GetSection(It.IsAny<string>())).Returns(declarationsDbSection.Object);

                    services.Configure<StorageOptions>(dbConfigSection.Object); // Ensures tests will use a separate db to any other operations
                });
            });

            _client = _factory.CreateClient();

            _cleanUpRequired = false;
        }

        private async Task<HttpResponseMessage> SendGiftAidRequest(double? donationAmount = null)
        {
            var uriString = GiftAidControllerPath;
            if (donationAmount != null) uriString += $"?amount={donationAmount}";

            var response = await _client.GetAsync(uriString);

            return response;
        }

        private async Task<HttpResponseMessage> SendDeclarationRequest(GiftAidDeclarationRequest request)
        {
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.Default, MediaTypeNames.Application.Json);

            var response = await _client.PostAsync(GiftAidControllerPath, content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                _cleanUpRequired = true;
            }

            return response;
        }

        private static GiftAidDeclarationRequest GenerateDeclarationRequest(string name = "Name", string postCode = "PostCode", double? donation = 100.0)
        {
            return new GiftAidDeclarationRequest
            {
                Name = name,
                PostCode = postCode,
                DonationAmount = donation ?? 0.0
            };
        }

        #region CalculateGiftAid

        [Fact]
        public async Task CalculateGiftAid_AmountProvided_Returns200()
        {
            var response = await SendGiftAidRequest(100.0);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CalculateGiftAid_AmountProvided_ReturnsAmountInGiftAidResponse()
        {
            const double donationAmount = 100.0;

            var response = await SendGiftAidRequest(donationAmount);
            var giftAidResponse = JsonConvert.DeserializeObject<GiftAidResponse>(await response.Content.ReadAsStringAsync());

            Assert.Equal(donationAmount, giftAidResponse.DonationAmount);
        }

        [Fact]
        public async Task CalculateGiftAid_AmountProvided_ReturnsExpectedGiftAidAmount()
        {
            const double donationAmount = 100.0;
            const double taxRate = 0.2;
            var expectedGiftAid = donationAmount * (taxRate / (1 - taxRate));

            var response = await SendGiftAidRequest(donationAmount);
            var giftAidResponse = JsonConvert.DeserializeObject<GiftAidResponse>(await response.Content.ReadAsStringAsync());

            Assert.Equal(expectedGiftAid, giftAidResponse.GiftAidAmount);
        }

        [Fact]
        public async Task CalculateGiftAid_NoAmountProvided_Returns400()
        {
            var response = await SendGiftAidRequest();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CalculateGiftAid_NoAmountProvided_ReturnsCorrectValidationError()
        {
            var response = await SendGiftAidRequest();
            var errorResponse = JsonConvert.DeserializeObject<GiftAidErrorResponse>(await response.Content.ReadAsStringAsync());

            var validationError = Assert.Single(errorResponse.AmountErrors);
            Assert.Equal("The amount field is required.", validationError);
        }

        [Theory]
        [InlineData(1.0)]
        [InlineData(100001.0)]
        public async Task CalculateGiftAid_AmountProvidedOutsideOfRange_Returns400(double donationAmount)
        {
            var response = await SendGiftAidRequest(donationAmount);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData(1.0)]
        [InlineData(100001.0)]
        public async Task CalculateGiftAid_AmountProvidedOutsideOfRange_ReturnsCorrectValidationError(double donationAmount)
        {
            var response = await SendGiftAidRequest(donationAmount);
            var errorResponse = JsonConvert.DeserializeObject<GiftAidErrorResponse>(await response.Content.ReadAsStringAsync());

            var validationError = Assert.Single(errorResponse.AmountErrors);
            Assert.Equal("The field amount must be between 2 and 100000.", validationError);
        }

        #endregion

        #region CreateGiftAidDeclaration

        [Fact]
        public async Task CreateGiftAidDeclaration_ValidRequest_Returns200()
        {
            var response = await SendDeclarationRequest(GenerateDeclarationRequest());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateGiftAidDeclaration_ValidRequest_ReturnsGiftAidAmount()
        {
            const double donationAmount = 100.0;
            const double taxRate = 0.2;
            var expectedGiftAid = donationAmount * (taxRate / (1 - taxRate));

            var response = await SendDeclarationRequest(GenerateDeclarationRequest(donation: donationAmount));
            var declarationResponse = JsonConvert.DeserializeObject<GiftAidDeclarationResponse>(await response.Content.ReadAsStringAsync());

            Assert.Equal(expectedGiftAid, declarationResponse.GiftAidAmount);
        }

        [Fact]
        public async Task CreateGiftAidDeclaration_ValidRequest_ReturnsId()
        {
            var response = await SendDeclarationRequest(GenerateDeclarationRequest());
            var declarationResponse = JsonConvert.DeserializeObject<GiftAidDeclarationResponse>(await response.Content.ReadAsStringAsync());

            Assert.NotEqual(0, declarationResponse.Id);
        }

        [Fact]
        public async Task CreateGiftAidDeclaration_ValidRequest_ReturnAutoIncrementingId()
        {
            var response = await SendDeclarationRequest(GenerateDeclarationRequest());
            var declarationResponse = JsonConvert.DeserializeObject<GiftAidDeclarationResponse>(await response.Content.ReadAsStringAsync());
            var startingId = declarationResponse.Id;

            for (var i = 1; i <= 5; i++)
            {
                response = await SendDeclarationRequest(GenerateDeclarationRequest());
                declarationResponse = JsonConvert.DeserializeObject<GiftAidDeclarationResponse>(await response.Content.ReadAsStringAsync());

                Assert.Equal(startingId + i, declarationResponse.Id);
            }
        }

        [Theory]
        [InlineData(null, "PostCode", 100.0)]
        [InlineData("Name", null, 100.0)]
        [InlineData("Name", "PostCode", 1.0)]
        [InlineData("Name", "PostCode", 100001.0)]
        public async Task CreateGiftAidDeclaration_InvalidRequest_Returns400(string name, string postCode, double donation)
        {
            var response = await SendDeclarationRequest(GenerateDeclarationRequest(name, postCode, donation));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateGiftAidDeclaration_NullName_ReturnsCorrectValidationError()
        {
            var response = await SendDeclarationRequest(GenerateDeclarationRequest(null));
            var errorResponse = JsonConvert.DeserializeObject<GiftAidDeclarationErrorResponse>(await response.Content.ReadAsStringAsync());

            var validationError = Assert.Single(errorResponse.NameErrors);
            Assert.Equal("The Name field is required.", validationError);
        }

        [Fact]
        public async Task CreateGiftAidDeclaration_NullPostCode_ReturnsCorrectValidationError()
        {
            var response = await SendDeclarationRequest(GenerateDeclarationRequest(postCode: null));
            var errorResponse = JsonConvert.DeserializeObject<GiftAidDeclarationErrorResponse>(await response.Content.ReadAsStringAsync());

            var validationError = Assert.Single(errorResponse.PostCodeErrors);
            Assert.Equal("The PostCode field is required.", validationError);
        }

        [Theory]
        [InlineData(1.0)]
        [InlineData(100001.0)]
        public async Task CreateGiftAidDeclaration_DonationOutOfRange_ReturnsCorrectValidationError(double donation)
        {
            var response = await SendDeclarationRequest(GenerateDeclarationRequest(donation: donation));
            var errorResponse = JsonConvert.DeserializeObject<GiftAidDeclarationErrorResponse>(await response.Content.ReadAsStringAsync());

            var validationError = Assert.Single(errorResponse.DonationAmountErrors);
            Assert.Equal("The field DonationAmount must be between 2 and 100000.", validationError);
        }

        public void Dispose()
        {
            if (_cleanUpRequired)
            {
                using (var db = new LiteDatabase(TestDatabase))
                {
                    var declarations = db.GetCollection<GiftAidDeclaration>();

                    declarations.DeleteAll();
                }
            }
        }

        #endregion
    }
}

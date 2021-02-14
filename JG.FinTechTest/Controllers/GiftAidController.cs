using JG.FinTechTest.Helpers.Interfaces;
using JG.FinTechTest.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace JG.FinTechTest.Controllers
{
    [Route("api/giftaid")]
    [ApiController]
    public class GiftAidController : ControllerBase
    {
        private readonly IGiftAidCalculator _calculator;

        public GiftAidController(IGiftAidCalculator calculator)
        {
            _calculator = calculator;
        }

        [HttpGet]
        public ActionResult<GiftAidResponse> CalculateGiftAid([FromQuery, Required] double amount)
        {
            var giftAidAmount = _calculator.CalculateGiftAid(amount);

            var response = new GiftAidResponse
            {
                DonationAmount = amount,
                GiftAidAmount = giftAidAmount
            };

            return Ok(response);
        }
    }
}

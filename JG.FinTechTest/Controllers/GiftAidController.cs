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

        /// <summary>
        /// Get the amount of gift aid reclaimable for donation amount
        /// </summary>
        /// <param name="amount">The amount of the donation, must be between 2.0 and 100000.0 (inclusive)</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<GiftAidResponse> CalculateGiftAid([FromQuery, Required, Range(2.0, 100000.0)] double amount)
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

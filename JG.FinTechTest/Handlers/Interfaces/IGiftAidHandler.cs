using JG.FinTechTest.Models.Requests;
using JG.FinTechTest.Models.Responses;

namespace JG.FinTechTest.Handlers.Interfaces
{
    public interface IGiftAidHandler
    {
        GiftAidResponse CalculateGiftAid(double donationAmount);
        GiftAidDeclarationResponse CreateGiftAidDeclaration(GiftAidDeclarationRequest request);
    }
}

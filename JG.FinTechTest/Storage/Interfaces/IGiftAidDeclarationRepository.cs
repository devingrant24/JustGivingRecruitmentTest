namespace JG.FinTechTest.Storage.Interfaces
{
    public interface IGiftAidDeclarationRepository
    {
        int CreateGiftAidDeclaration(string name, string postCode, double donationAmount);
    }
}

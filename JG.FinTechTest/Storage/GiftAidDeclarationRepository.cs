using JG.FinTechTest.Models.Options;
using JG.FinTechTest.Models.Storage;
using JG.FinTechTest.Storage.Interfaces;
using LiteDB;
using Microsoft.Extensions.Options;

namespace JG.FinTechTest.Storage
{
    public class GiftAidDeclarationRepository : IGiftAidDeclarationRepository
    {
        private readonly string _databaseConnectionString;

        public GiftAidDeclarationRepository(IOptions<StorageOptions> options)
        {
            _databaseConnectionString = options.Value?.GiftAidDeclarationsDatabase;
        }

        public int CreateGiftAidDeclaration(string name, string postCode, double donationAmount)
        {
            using (var db = new LiteDatabase(_databaseConnectionString))
            {
                var declarations = db.GetCollection<GiftAidDeclaration>();

                var declaration = new GiftAidDeclaration
                {
                    Name = name,
                    PostCode = postCode,
                    DonationAmount = donationAmount
                };

                declarations.Insert(declaration); // LiteDB auto-increments the ID property of the object, resulting in a unique identifier.

                return declaration.Id;
            }
        }
    }
}

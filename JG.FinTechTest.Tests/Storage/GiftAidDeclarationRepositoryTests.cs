using JG.FinTechTest.Models.Options;
using JG.FinTechTest.Models.Storage;
using JG.FinTechTest.Storage;
using JG.FinTechTest.Storage.Interfaces;
using LiteDB;
using Microsoft.Extensions.Options;
using Moq;
using System;
using Xunit;

namespace JG.FinTechTest.Tests.Storage
{
    public class GiftAidDeclarationRepositoryTests : IDisposable
    {
        private readonly IGiftAidDeclarationRepository _repository;

        private const string TestDatabase = "Declarations-Test.db";

        public GiftAidDeclarationRepositoryTests()
        {
            var options = new Mock<IOptions<StorageOptions>>();
            options.SetupGet(o => o.Value).Returns(new StorageOptions { GiftAidDeclarationsDatabase = TestDatabase });

            CleanUpDatabase();

            _repository = new GiftAidDeclarationRepository(options.Object);
        }

        private void CleanUpDatabase()
        {
            using (var db = new LiteDatabase(TestDatabase))
            {
                var declarations = db.GetCollection<GiftAidDeclaration>();

                declarations.DeleteAll();
            }
        }

        private GiftAidDeclaration GetDeclarationFromDatabase(int id)
        {
            using (var db = new LiteDatabase(TestDatabase))
            {
                var declarations = db.GetCollection<GiftAidDeclaration>();

                return declarations.FindById(id);
            }
        }

        [Fact]
        public void CreateGiftAidDeclaration_StoresDeclarationInDatabase()
        {
            var id = _repository.CreateGiftAidDeclaration("Name", "PostCode", 20);

            var declaration = GetDeclarationFromDatabase(id);

            Assert.NotNull(declaration);
            Assert.Equal("Name", declaration.Name);
            Assert.Equal("PostCode", declaration.PostCode);
            Assert.Equal(20, declaration.DonationAmount);
        }

        [Fact]
        public void CreateGiftAidDeclaration_ReturnsAutoIncrementedId()
        {
            for (var i = 1; i <= 5; i++)
            {
                var id = _repository.CreateGiftAidDeclaration("Name_" + i, "PostCode", 20);
                Assert.Equal(i, id);

                var declaration = GetDeclarationFromDatabase(id);
                Assert.NotNull(declaration);
                Assert.Equal("Name_" + i, declaration.Name);
            }
        }

        public void Dispose()
        {
            CleanUpDatabase();
        }
    }
}

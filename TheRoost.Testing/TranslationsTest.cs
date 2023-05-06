using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using TheRoost.API;
using static System.Net.Mime.MediaTypeNames;
using TheRoost.API.Models.Json;
using System.Resources.NetStandard;

namespace TheRoost.Testing
{
    public class TranslationsTest
    {
        public TranslationsTest()
        {
        }

        [Fact]
        public async Task TestWritingToResourceFile()
        {
            //Arrange

            //Act
            var writer = new ResXResourceWriter(@".\TestFile.resx");
            writer.AddResource("Test key", "text value");
            writer.Generate();
            writer.Close();
            //Assert
            Assert.True(System.IO.File.Exists(@".\TestFile.resx"));

        }
    }
}

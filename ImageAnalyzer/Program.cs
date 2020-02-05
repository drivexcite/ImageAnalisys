using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;

namespace ImageAnalyzer
{
    class ComputerVisionConfiguration
    {
        public string Key { get; set; }
        public string Endpoint { get; set; }
    }

    class Program
    {
        private static async Task<ImageAnalysis> GetImageAnalysisAsync(ComputerVisionConfiguration configuration, Stream imageStream, List<VisualFeatureTypes> features = null)
        {
            using var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(configuration.Key)) { Endpoint = configuration.Endpoint };

            return await client.AnalyzeImageInStreamAsync(imageStream, features ?? new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Categories,
                VisualFeatureTypes.Description,
                VisualFeatureTypes.Tags,
                VisualFeatureTypes.Adult
            });
        }

        private static async Task<ImageAnalysis> GetImageAnalysis(ComputerVisionConfiguration configuration, string imageUrl, List<VisualFeatureTypes> features = null)
        {
            using var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(configuration.Key)) { Endpoint = configuration.Endpoint };

            return await client.AnalyzeImageAsync(imageUrl, features ?? new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Categories,
                VisualFeatureTypes.Description,
                VisualFeatureTypes.Tags,
                VisualFeatureTypes.Adult
            });
        }

        private static async Task<int> PersistImageAsync(string hwid, string filePath, ImageAnalysis imageAnalysis, SqlConnection connection)
        {
            var commandText = @"
 insert into [Image](ContentId, StorageLocation, [Description], AltText, Localization, Width, Height, [Format], ImageAnalysis, Xmp)
    values (@ContentId, @StorageLocation, @Description, @AltText, @Localization, @Width, @Height, @Format, @ImageAnalysis, @Xmp);
";
            var parameters = new
            {
                ContentId = hwid,
                StorageLocation = filePath,
                Description = imageAnalysis.Description?.Captions.FirstOrDefault()?.Text,
                AltText = imageAnalysis.Description?.Captions.FirstOrDefault()?.Text,
                Localization = "en-us",
                Width = imageAnalysis.Metadata.Width,
                Height = imageAnalysis.Metadata.Height,
                Format = imageAnalysis.Metadata.Format,
                ImageAnalysis = JsonConvert.SerializeObject(imageAnalysis),
                Xmp = null as string,
            };

            return await connection.ExecuteAsync(commandText, parameters);
        }

        static async Task Main(string[] args)
        {
            var filePath = @"Images\operatingroom.jpg";
            using var imageStream = File.OpenRead(filePath);
            var configuration = new ComputerVisionConfiguration { Key = "", Endpoint = "https://westus.api.cognitive.microsoft.com" };

            var analysis = await GetImageAnalysisAsync(configuration, imageStream);
            using var connection = new SqlConnection("Server=.;Database=Content;Trusted_Connection=True;");

            await PersistImageAsync("img0001", filePath, analysis, connection);            
        }
    }    
}

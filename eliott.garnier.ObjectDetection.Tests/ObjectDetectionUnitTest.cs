using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace eliott.garnier.ObjectDetection.Tests;

public class ObjectDetectionUnitTest
{
    [Fact]
    public async Task ObjectShouldBeDetectedCorrectly()
    {
        var executingPath = GetExecutingPath();
        var imageScenesData = new List<byte[]>();
        foreach (var imagePath in Directory.EnumerateFiles(Path.Combine(executingPath,
                     "Scenes")))
        {
            var imageBytes = await File.ReadAllBytesAsync(imagePath);
            imageScenesData.Add(imageBytes);
        }
        var detectObjectInScenesResults = await new
            ObjectDetection().DetectObjectInScenesAsync(imageScenesData);

        Assert.Equal("[{\"Dimensions\":{\"X\":178.44273,\"Y\":23.75328,\"Height\":306.39752,\"Width\":187.14682},\"Label\":\"chair\",\"Confidence\":0.7166497}," +
                     "{\"Dimensions\":{\"X\":262.6985,\"Y\":98.53662,\"Height\":343.36365,\"Width\":154.71152},\"Label\":\"person\",\"Confidence\":0.6447992}," +
                     "{\"Dimensions\":{\"X\":20.381744,\"Y\":88.846695,\"Height\":309.85242,\"Width\":185.04446},\"Label\":\"chair\",\"Confidence\":0.48622882}," +
                     "{\"Dimensions\":{\"X\":196.32056,\"Y\":9.081772,\"Height\":35.9929,\"Width\":28.329626},\"Label\":\"pottedplant\",\"Confidence\":0.46411657}]",JsonSerializer.Serialize(detectObjectInScenesResults[0].Box));

        Assert.Equal("[{\"Dimensions\":{\"X\":156.34952,\"Y\":150.91817,\"Height\":109.367516,\"Width\":110.41458},\"Label\":\"motorbike\",\"Confidence\":0.34336326}]",JsonSerializer.Serialize(detectObjectInScenesResults[1].Box));
    }
    private static string GetExecutingPath()
    {
        var executingAssemblyPath = Assembly.GetExecutingAssembly().Location;
        var executingPath = Path.GetDirectoryName(executingAssemblyPath);
        return executingPath;
    }
}
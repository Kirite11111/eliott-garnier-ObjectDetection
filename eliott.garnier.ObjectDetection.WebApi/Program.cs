using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// POST /ObjectDetection
app.MapPost("/ObjectDetection", async ([FromForm] IFormFileCollection files) =>
{
    if (files.Count < 1)
    {
        return Results.BadRequest("Aucune image n'a été envoyée.");
    }

    try
    {
        var processedImages = new List<(string FileName, byte[] ImageData)>();

        foreach (var file in files)
        {
            using var sceneSourceStream = file.OpenReadStream();
            using var sceneMemoryStream = new MemoryStream();
            await sceneSourceStream.CopyToAsync(sceneMemoryStream);
            var imageSceneData = sceneMemoryStream.ToArray();

            // Détection d'objets
            var objectDetection = new eliott.garnier.ObjectDetection.ObjectDetection();
            var detectionResults = await objectDetection.DetectObjectInScenesAsync(new List<byte[]> { imageSceneData });

            var result = detectionResults.FirstOrDefault();
            if (result == null || result.Box == null || !result.Box.Any())
            {
                return Results.BadRequest($"Aucune détection trouvée dans l'image : {file.FileName}");
            }

            // Annoter l'image
            using var image = Image.FromStream(new MemoryStream(imageSceneData));
            using var graphics = Graphics.FromImage(image);

            foreach (var box in result.Box)
            {
                var rect = new RectangleF(box.Dimensions.X, box.Dimensions.Y, box.Dimensions.Width, box.Dimensions.Height);
                graphics.DrawRectangle(Pens.Red, Rectangle.Round(rect));
                graphics.DrawString(box.Label, new Font("Arial", 12), Brushes.Red, box.Dimensions.X, box.Dimensions.Y);
            }

            // Convertir l'image annotée en tableau de bytes
            using var outputMemoryStream = new MemoryStream();
            image.Save(outputMemoryStream, ImageFormat.Jpeg);
            var imageData = outputMemoryStream.ToArray();

            // Ajouter l'image traitée à la liste
            processedImages.Add((file.FileName, imageData));
        }

        // Si une seule image, la retourner directement
        if (processedImages.Count == 1)
        {
            return Results.File(processedImages[0].ImageData, "image/jpeg");
        }

        // Si plusieurs images, créer un ZIP
        using var zipMemoryStream = new MemoryStream();
        using (var archive = new System.IO.Compression.ZipArchive(zipMemoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
        {
            foreach (var (fileName, imageData) in processedImages)
            {
                var entry = archive.CreateEntry(fileName, System.IO.Compression.CompressionLevel.Fastest);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(imageData, 0, imageData.Length);
            }
        }

        return Results.File(zipMemoryStream.ToArray(), "application/zip", fileDownloadName: "ProcessedImages.zip");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Erreur lors du traitement des images : {ex.Message}");
    }
}).DisableAntiforgery();


app.Run();


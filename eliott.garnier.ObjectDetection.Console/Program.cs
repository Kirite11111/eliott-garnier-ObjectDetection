using System.Text.Json;

namespace eliott.garnier.ObjectDetection.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                System.Console.WriteLine("Veuillez fournir le chemin complet du répertoire contenant les images de scène.");
                return;
            }

            string directoryPath = args[0];

            if (!Directory.Exists(directoryPath))
            {
                System.Console.WriteLine($"Le répertoire spécifié n'existe pas : {directoryPath}");
                return;
            }

            // Charger toutes les images du répertoire
            var imageFiles = Directory.GetFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(file => file.EndsWith(".jpg") || file.EndsWith(".png"))
                .ToList();

            if (!imageFiles.Any())
            {
                System.Console.WriteLine("Aucune image trouvée dans le répertoire spécifié.");
                return;
            }

            var images = new List<byte[]>();

            foreach (var filePath in imageFiles)
            {
                images.Add(File.ReadAllBytes(filePath));
            }

            // Effectuer la détection d'objets
            var objectDetection = new ObjectDetection();
            IList<ObjectDetectionResult> detectObjectInScenesResults = await objectDetection.DetectObjectInScenesAsync(images);

            // Imprimer les résultats au format JSON
            foreach (var objectDetectionResult in detectObjectInScenesResults)
            {
                System.Console.WriteLine($"Box: {JsonSerializer.Serialize(objectDetectionResult.Box)}");
            }
        }
    }
}

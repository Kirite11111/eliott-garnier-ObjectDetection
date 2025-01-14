using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ObjectDetection;

namespace eliott.garnier.ObjectDetection
{
    public class ObjectDetection
    {
        public async Task<IList<ObjectDetectionResult>> DetectObjectInScenesAsync(IList<byte[]> imagesSceneData)
        {
            // Initialise l'instance Yolo
            var tinyYolo = new Yolo();

            // Traite les images en parallèle
            var detectionTasks = imagesSceneData.Select(imageData =>
                Task.Run(() =>
                {
                    // Appelle la méthode de détection pour chaque image
                    var boxes = tinyYolo.Detect(imageData);

                    // Retourne le résultat pour cette image
                    return new ObjectDetectionResult
                    {
                        ImageData = imageData,
                        Box = boxes.Boxes
                    };
                })
            ).ToList();

            // Attend la fin de toutes les tâches
            var results = await Task.WhenAll(detectionTasks);

            return results.ToList();
        }
    }
}
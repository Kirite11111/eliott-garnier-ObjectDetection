using Microsoft.ML;

namespace eliott.garnier.ObjectDetection;

public record ObjectDetectionResult
{
    public byte[] ImageData { get; set; }
    public IList<BoundingBox> Box { get; set; } 
} 

public class Dimensions
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Height { get; set; }
    public float Width { get; set; }
}

public class BoundingBox
{
    public Dimensions Dimensions { get; set; }
    public string Label { get; set; }
    public float Confidence { get; set; }
}

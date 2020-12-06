using Microsoft.ML.Data;

namespace DetectSpam.SupportModel
{
    public class InputModel
    {
        [LoadColumn(0)]
        public string Label { get; set; }
        [LoadColumn(1)]
        public string Message { get; set; }
    }
}
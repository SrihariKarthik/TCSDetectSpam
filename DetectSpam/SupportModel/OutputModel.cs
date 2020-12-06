using Microsoft.ML.Data;

namespace DetectSpam.SupportModel
{
    public class OutputModel
    {
        [ColumnName("PredictedLabel")]
        public string isSpam { get; set; }
    }
}

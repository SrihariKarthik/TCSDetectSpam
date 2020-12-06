using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML;
using DetectSpam.SupportModel;
using System.IO;

namespace DetectSpam
{
    public static class Util
    {
        public static void Process(string sFilePath, string sComment)
        {
            DisplayInput(sComment);
            MLContext mlContext = new MLContext();
            var data = mlContext.Data.LoadFromTextFile<InputModel>(path: sFilePath, hasHeader: true, separatorChar: '\t');
            var dataPipeLine = mlContext.Transforms.Conversion.MapValueToKey("Label", "Label")
                                          .Append(mlContext.Transforms.Text.FeaturizeText("FeaturesText", new Microsoft.ML.Transforms.Text.TextFeaturizingEstimator.Options
                                          {
                                              WordFeatureExtractor = new Microsoft.ML.Transforms.Text.WordBagEstimator.Options { NgramLength = 2, UseAllLengths = true },
                                              CharFeatureExtractor = new Microsoft.ML.Transforms.Text.WordBagEstimator.Options { NgramLength = 3, UseAllLengths = false },
                                              Norm = Microsoft.ML.Transforms.Text.TextFeaturizingEstimator.NormFunction.L2,
                                          }, "Message"))
                                          .Append(mlContext.Transforms.CopyColumns("Features", "FeaturesText"))
                                          .AppendCacheCheckpoint(mlContext);

            var trainer = mlContext.MulticlassClassification.Trainers.OneVersusAll(mlContext.BinaryClassification.Trainers.AveragedPerceptron(labelColumnName: "Label", numberOfIterations: 10, featureColumnName: "Features"), labelColumnName: "Label")
                                      .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
            var trainPipeline = dataPipeLine.Append(trainer);

            Console.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============");
            var crossValidationResults = mlContext.MulticlassClassification.CrossValidate(data: data, estimator: trainPipeline, numberOfFolds: 5);

            var model = trainPipeline.Fit(data);

            var oPredictor = mlContext.Model.CreatePredictionEngine<InputModel, OutputModel>(model);

            Console.WriteLine("===============Output ===================");
            ClassifyMessage(oPredictor, "Its Awesome! Most appreciated!");
            ClassifyMessage(oPredictor, "You are a Winner! You deserved a wonderful vacation for free!");
            ClassifyMessage(oPredictor, "Let's connect this week!!");
            ClassifyMessage(oPredictor, "Free Free Free. Call us in +001 12345600342. We would deliver instantly!");
            Console.WriteLine("-----------------------------------------");
            ClassifyMessage(oPredictor, sComment);
            Console.WriteLine("-------------------------------------------------");
        }
        public static void ClassifyMessage(PredictionEngine<InputModel, OutputModel> predictor, string message)
        {
            var input = new InputModel { Message = message };
            var prediction = predictor.Predict(input);

            Console.WriteLine("The message '{0}' is {1}", input.Message, prediction.isSpam == "spam" ? "====SPAM" : "====NOT SPAM");
        }

        public static string GetDataPath(string sBinPath, string sDataFolderName)
        {
            DirectoryInfo info =  Directory.GetParent(sBinPath).Parent.Parent;
            string sPath = info.ToString() + @"\" +sDataFolderName;
            return sPath;
        }

        private static void DisplayInput(string sComment)
        {
            Console.WriteLine("");
            Console.WriteLine("===============Input ===================");
            Console.WriteLine("Its Awesome! Most appreciated!");
            Console.WriteLine("You are a Winner! You deserved a wonderful vacation for free!");
            Console.WriteLine("Let's connect this week!!");
            Console.WriteLine("Free Free Free. Call us in +001 12345600342. We would deliver instantly!");
            Console.WriteLine("");
            Console.WriteLine("-----------------User Added Input---------------");
            Console.WriteLine(sComment);
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("");
        }
    }
}

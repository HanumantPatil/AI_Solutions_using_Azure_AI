
using Microsoft.ML;
using Microsoft.ML.Data;

namespace GenONNX
{
    class Program
    {
        public static string path = "./Models/";
        public class HealthData
        {
            public float Freq { get; set; }
            public float Factor { get; set; }
        }
        public class Prediction
        {
            [ColumnName("Score")]
            public float Factor { get; set; }
        }
        static void Main(string[] args)
        {
            MLContext mLContext = new MLContext();
            // 1a. Create training data
            HealthData[] healthDatas = new HealthData[]
            {
                new HealthData(){ Freq=1F, Factor=0.5f},
                new HealthData(){ Freq=2F, Factor=2.3f},
                new HealthData(){ Freq=3F, Factor=3.5f},
                new HealthData(){ Freq=4F, Factor=5.5f},
                new HealthData(){ Freq=5F, Factor=7.5f},
                new HealthData(){ Freq=6F, Factor=9.5f},
                new HealthData(){ Freq=7F, Factor=11.5f},
            };

            // 1b. Import training data
            IDataView trainingData = mLContext.Data.LoadFromEnumerable<HealthData>(healthDatas);

            // 2. Specify data preparation and model training pipeline
            var pipeline = mLContext.Transforms.Concatenate("Features", new string[] { "Freq" })
                .Append(mLContext.Regression.Trainers.Sdca(labelColumnName: "Factor", maximumNumberOfIterations: 100));

            // 3. Train the model
            var model = pipeline.Fit(trainingData);

            // 4. Make a prediction
            var hdPredictionInput = new HealthData() { Freq = 2.5F };
            var hdPredictionOutput = mLContext.Model.CreatePredictionEngine<HealthData, Prediction>(model).Predict(hdPredictionInput);

            Console.WriteLine($"For Freq: {hdPredictionInput.Freq}, predicted Factor: {hdPredictionOutput.Factor}");
            // **************** save the model for future use ****************
            mLContext.Model.Save(model, trainingData.Schema, path + "HealthModel.zip");

            using (var fileStream = new FileStream(path + "HealthModel.onnx", FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                mLContext.Model.ConvertToOnnx(model, trainingData, fileStream);
            }
        }

    }
}

using SharpNeat.Experiments.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpNeat.Experiments.Clustering
{
    public class MapClusteringDataset : IMapClusteringDataset
    {

        public List<List<double>> InputSamples { get; private set; }

        /// <summary>
        /// Number of inputs values. Corresponds to the number of
        /// input layers to create.
        /// </summary>
        public int InputCount { get; private set; }

        /// <summary>
        /// Number of rows in the sample. Corresponds to height of
        /// the input layers matrices.
        /// </summary>
        public int RowCount { get; private set; }

        /// <summary>
        /// Number of rows in the sample. Corresponds to width of
        /// the input layers matrices.
        /// </summary>
        public int ColumnCount { get; private set; }

        /// <summary>
        /// Creates a dataset for clustering problems given by files with the following form:
        /// 
        /// M
        /// input1sample1value1;input1sample1value2;...;input1sample1valueN
        /// input1sample2value1;input1sample2value2;...;input1sample2valueN
        /// ...
        /// input1sampleMvalue1;input1sampleMvalue2;...;input1sampleMvalueN
        /// input2sample1value1;input2sample1value2;...;input2sample1valueN
        /// ...
        /// 
        /// After parsing, InputSamples will yield a list of the rows, with an
        /// additionnal column at the end of each row containing the input id.
        /// </summary>
        public MapClusteringDataset()
        {
        }

        private string loaded = "";

        private double parseToDouble(string str)
        {
            var value = double.Parse(str, System.Globalization.NumberFormatInfo.InvariantInfo);
            if (value < 0.0) // NODATA = -9999
                return 0.0;
            return value;
        }

        public void LoadFromFile(string filename)
        {
            // If already loaded, does nothing
            Console.WriteLine("Loading " + filename + "...");
            if (filename == loaded) // FIXME
            {
                Console.WriteLine("Already loaded.");
                return;
            }

            int nbRowsPerMatrix;
            using (StreamReader rdr = new StreamReader(filename))
            {
                nbRowsPerMatrix = int.Parse(rdr.ReadLine().Trim());
            }

            int currentRowId = 0;

            // Parse the data
            var data = from line in EasyCSV.FromFile(filename).Skip(1)
                       select new
                       {
                           Inputs = line.Select(x => parseToDouble(x))
                                        .Concat(new double[] { currentRowId++ / nbRowsPerMatrix })
                                        .ToList()
                       };
            InputSamples = new List<List<double>>();
            foreach (var entry in data)
            {
                InputSamples.Add(entry.Inputs);
            }

            InputCount = currentRowId / nbRowsPerMatrix;
            RowCount = nbRowsPerMatrix;
            ColumnCount = data.First().Inputs.Count() - 1;

            Console.WriteLine("data.Count = " + data.Count());
            Console.WriteLine("InputCount = " + InputCount);
            Console.WriteLine("RowCount = " + RowCount);
            Console.WriteLine("ColumnCount = " + ColumnCount);
        }

        static double[, ,] samplesMatrix = null;

        /// <summary>
        /// Convenience method that builds the hole input matrix.
        /// The dimensions are InputCount x RowCount x ColumnCount
        /// </summary>
        public double[, ,] GetSamplesMatrix()
        {
            if (samplesMatrix == null)
            {
                samplesMatrix = new double[InputCount, RowCount, ColumnCount];
                for (var row = 0; row < RowCount * InputCount; row++)
                {
                    var input = Convert.ToInt32(InputSamples[row].Last());
                    for (var col = 0; col < ColumnCount; col++)
                    {
                        var actualRowInMatrix = row % RowCount;
                        samplesMatrix[input, actualRowInMatrix, col] = InputSamples[row][col];
                    }
                }
            }
            return samplesMatrix;
        }
    }
}

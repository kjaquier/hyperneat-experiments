using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using SharpNeat.Experiments.Common;

namespace SharpNeat.Experiments.Classification
{
    /// <summary>
    /// Generic dataset parser for classification problems with a fixed number of inputs and classes.
    /// </summary>
    class ClassificationDataset : IClassificationDataset
    {

        public List<List<double>> InputSamples { get; private set; }

        public List<List<double>> OutputSamples { get; private set; }

        public int InputCount { get; private set; }

        public int OutputCount { get; private set; }

        /// <summary>
        /// Creates a dataset for classification problems given by files with the following form:
        /// input1;input2;...;inputN;output1;output2;...;outputM
        /// Files must not have any header line.
        /// </summary>
        /// <param name="nbInputs">Number of input columns</param>
        /// <param name="nbOutputs">Number of output columns. If less than the difference between 
        /// the number of columns and the number of inputs, the excessive outputs will be ignored from
        /// the end.</param>
        public ClassificationDataset(int nbInputs, int nbOutputs)
        {
            InputCount = nbInputs;
            OutputCount = nbOutputs;
        }

        private string loaded = "";

        public void LoadFromFile(string filename)
        {
            // If already loaded, does nothing
            Console.WriteLine("Loading " + filename + "...");
            if (filename == loaded) // FIXME
            {
                Console.WriteLine("Already loaded.");
                return;
            }

            var data = from line in EasyCSV.FromFile(filename)
                       select new
                       {
                           Inputs = line.Slice(0, InputCount)
                                        .Select(x => double.Parse(x, System.Globalization.NumberFormatInfo.InvariantInfo))
                                        .ToList(),
                           Outputs = line.Slice(InputCount, Math.Min(line.Count, InputCount + OutputCount))
                                         .Select(x => double.Parse(x, System.Globalization.NumberFormatInfo.InvariantInfo))
                                         .ToList()
                       };
            InputSamples = new List<List<double>>();
            OutputSamples = new List<List<double>>();
            foreach (var entry in data)
            {
                InputSamples.Add(entry.Inputs);
                OutputSamples.Add(entry.Outputs);
            }
            Console.WriteLine("InputCount = " + InputCount);
            Console.WriteLine("OutputCount = " + OutputCount);
            Console.WriteLine("data.Count = " + data.Count());
        }
    }
}

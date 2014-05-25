using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using SharpNeat.Experiments.Common;

namespace SharpNeat.Experiments.Clustering
{
    class ClusteringDataset : IClusteringDataset
    {
        public List<List<double>> InputSamples { get; private set; }

        public int InputCount { get; private set; }

        /// <summary>
        /// Creates a dataset for clustering problems given by files with the following form:
        /// input1;input2;...;inputN
        /// Files must not have any header line.
        /// </summary>
        /// <param name="nbInputs">Number of input columns</param>
        public ClusteringDataset(int nbInputs)
        {
            InputCount = nbInputs;
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
                                        .ToList()
                       };
            InputSamples = new List<List<double>>();
            foreach (var entry in data)
            {
                InputSamples.Add(entry.Inputs);
            }
            Console.WriteLine("InputCount = " + InputCount);
            Console.WriteLine("data.Count = " + data.Count());
        }
    }
}

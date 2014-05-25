using System;
using System.Collections.Generic;

namespace SharpNeat.Experiments.Common
{
    public interface IDataset
    {
        int InputCount { get; }
        List<List<double>> InputSamples { get; }
        void LoadFromFile(string filename);
    }
}

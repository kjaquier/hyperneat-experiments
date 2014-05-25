using SharpNeat.Experiments.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNeat.Experiments.Classification
{
    public interface IClassificationDataset : IDataset
    {
        List<List<double>> OutputSamples { get; }

        int OutputCount { get; }
    }
}

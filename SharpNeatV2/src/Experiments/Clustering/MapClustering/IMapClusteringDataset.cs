using System;
namespace SharpNeat.Experiments.Clustering
{
    public interface IMapClusteringDataset : IClusteringDataset
    {
        int ColumnCount { get; }
        int RowCount { get; }

        double[, ,] GetSamplesMatrix();
    }
}

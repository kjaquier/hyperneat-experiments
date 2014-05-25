
using SharpNeat.Experiments.Common;

namespace SharpNeat.Experiments.Clustering.Colombia
{
    /// <summary>
    /// Visualizing similar areas in Colombia with climatic data
    /// from http://www.worldclim.org/
    /// </summary>
    class WindowColombiaExperimentHyperNeat : WindowMapClusteringExperimentHyperNeat
    {
        const int NbInputs = 2;
        const int NbClusters = 2;

        static readonly bool[,] Filter = { { true } };

        public WindowColombiaExperimentHyperNeat()
            : base(NbClusters, Filter)
        {
        }

        protected override string DatasetFileName
        {
            get { return @"Datasets\tmean_10x8_2var_norm.txt"; }
        }

        protected override Genotype Genotype
        {
            get { return Genotype.Neat; }
        }

        protected override Phenotype Phenotype
        {
            get { return Phenotype.HyperNeat; }
        }

        protected override IMapClusteringDataset CreateDataset()
        {
            return new MapClusteringDataset();
        }
    }
}

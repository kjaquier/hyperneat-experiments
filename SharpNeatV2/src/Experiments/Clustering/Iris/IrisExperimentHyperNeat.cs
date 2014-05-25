using SharpNeat.Experiments.Common;
using SharpNeat.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNeat.Experiments.Clustering.Iris
{
    /// <summary>
    /// Simple clustering of iris data.
    /// </summary>
    class IrisExperimentHyperNeat : ClusteringExperimentHyperNeat
    {
        const int NbInputs = 4;
        const int NbClusters = 3;

        public IrisExperimentHyperNeat()
            : base(NbClusters)
        {
        }

        protected override string DatasetFileName
        {
            get { return @"Datasets\iris.data"; }
        }

        protected override Genotype Genotype
        {
            get { return Genotype.Neat; }
        }

        protected override Phenotype Phenotype
        {
            get { return Phenotype.HyperNeat; }
        }

        protected override IClusteringDataset CreateDataset()
        {
            return new ClusteringDataset(NbInputs);
        }
    }
}

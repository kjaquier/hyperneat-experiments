using SharpNeat.Experiments.Common;
using SharpNeat.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNeat.Experiments.Classification.Iris
{
    /// <summary>
    /// Experiment for the classification of data from the iris dataset
    /// </summary>
    class IrisExperimentHyperNeat : ClassificationExperimentHyperNeat
    {
        protected override string DatasetFileName
        {
            get { return @"Datasets\iris.data"; }
        }

        protected override IEnumerable<double> EvaluationWeights
        {
            get
            {
                return new double[] { 
                    1.0, // accuracy
                    1.0, // sensitivity
                    1.0, // specificity
                    0.2  // rmse
                };
            }
        }

        protected override Genotype Genotype
        {
            get { return Genotype.Neat; }
        }

        protected override Phenotype Phenotype
        {
            get { return Phenotype.HyperNeat; }
        }

        protected override int HiddenNodesCount
        {
            get
            {
                return 4;
            }
        }

        protected override IClassificationDataset CreateDataset()
        {
            return new ClassificationDataset(4, 3);
        }
    }
}

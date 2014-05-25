using SharpNeat.Experiments.Common;
using SharpNeat.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNeat.Experiments.Classification.Voices
{
    /// <summary>
    /// Voices recognition experiment. The goal is to discriminate between male and female voices.
    /// 
    /// Raw data is from : http://www.utdallas.edu/~assmann/KIDVOW/
    /// </summary>
    class VoicesExperimentHyperNeat : ClassificationExperimentHyperNeat
    {
        protected override string DatasetFileName
        {
            get { return @"Datasets\dataset_natural_adults.csv"; }
        }

        protected override IEnumerable<double> EvaluationWeights
        {
            get
            {
                return new double[] { 
                    0.0, // accuracy
                    1.0, // sensitivity
                    1.0, // specificity
                    0.5, // rmse
                };
            }
        }

        protected override Phenotype Phenotype
        {
            get { return Phenotype.HyperNeat; }
        }

        protected override IClassificationDataset CreateDataset()
        {
            return new ClassificationDataset(26, 2);
        }
    }
}

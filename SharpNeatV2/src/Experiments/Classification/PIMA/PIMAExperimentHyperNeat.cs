using SharpNeat.Experiments.Common;
using SharpNeat.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNeat.Experiments.Classification.PIMA
{
    /// <summary>
    /// Classification expriment on the PIMA dataset.
    /// 
    /// Dataset is from : http://archive.ics.uci.edu/ml/datasets/Pima+Indians+Diabetes
    /// </summary>
    class PIMAExperimentHyperNeat : ClassificationExperimentHyperNeat
    {
        protected override string DatasetFileName
        {
            get { return @"Datasets\PIMA.csv"; }
        }

        protected override IEnumerable<double> EvaluationWeights
        {
            get
            {
                return new double[] { 
                    0.3, // accuracy
                    1.0, // sensitivity
                    0.9, // specificity
                    0.2  // rmse
                };
            }
        }

        protected override Phenotype Phenotype
        {
            get { return Phenotype.HyperNeat; }
        }

        protected override IClassificationDataset CreateDataset()
        {
            return new ClassificationDataset(8, 1);
        }
    }
}

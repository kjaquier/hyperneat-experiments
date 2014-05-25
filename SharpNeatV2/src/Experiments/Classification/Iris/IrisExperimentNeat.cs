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
    class IrisExperimentNeat : IrisExperimentHyperNeat
    {
        protected override Phenotype Phenotype
        {
            get { return Phenotype.Neat; }
        }
    }
}

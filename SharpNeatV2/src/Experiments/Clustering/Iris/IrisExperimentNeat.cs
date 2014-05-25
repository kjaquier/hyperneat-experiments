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
    class IrisExperimentNeat : IrisExperimentHyperNeat
    {
        protected override Phenotype Phenotype
        {
            get { return Phenotype.Neat; }
        }
    }
}

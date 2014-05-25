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
    class PIMAExperimentNeat : PIMAExperimentHyperNeat
    {
        protected override Phenotype Phenotype
        {
            get { return Phenotype.Neat; }
        }
    }
}

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
    class VoicesExperimentNeat : VoicesExperimentHyperNeat
    {
        protected override Phenotype Phenotype
        {
            get { return Phenotype.Neat; }
        }
    }
}

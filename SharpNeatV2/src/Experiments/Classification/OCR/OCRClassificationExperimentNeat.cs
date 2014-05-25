using SharpNeat.Experiments.Common;
using SharpNeat.Core;
using SharpNeat.Decoders.Neat;
using SharpNeat.Experiments.Classification.OCR;
using SharpNeat.Genomes.Neat;
using SharpNeat.Network;
using SharpNeat.Phenomes;
using SharpNeat.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SharpNeat.Experiments.Classification.OCR
{
    /// <summary>
    /// Experiment for the classification of handwritten digits
    /// Dataset is from : http://archive.ics.uci.edu/ml/datasets/Semeion+Handwritten+Digit
    /// </summary>
    class OCRClassificationExperimentNeat : OCRClassificationExperimentHyperNeat
    {
        protected override Phenotype Phenotype
        {
            get { return Phenotype.Neat; }
        }

    }
}

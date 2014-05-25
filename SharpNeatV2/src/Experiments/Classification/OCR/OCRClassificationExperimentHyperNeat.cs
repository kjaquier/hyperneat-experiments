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
    class OCRClassificationExperimentHyperNeat : ClassificationExperimentHyperNeat
    {
        private const int NbInputs = 16 * 16;
        private const int NbOutputs = 10;

        protected override string DatasetFileName
        {
            get { return @"Datasets\semeion.samples.data"; }
        }

        protected override IEnumerable<double> EvaluationWeights
        {
            get
            {
                return new double[] { 
                    0.5, // accuracy
                    1.0, // sensitivity
                    1.0, // specificity
                    0.0  // rmse
                };
            }
        }

        protected override IClassificationDataset CreateDataset()
        {
            return new ClassificationDataset(NbInputs, NbOutputs);
        }

        public override Domains.AbstractDomainView CreateDomainView()
        {
            return new OCRClassificationView(CreateGenomeDecoder(), new ClassificationDataset(NbInputs, NbOutputs), _evaluator);
        }

        protected override int DimensionCount
        {
            get { return 2; }
        }

        protected override Phenotype Phenotype
        {
            get { return Phenotype.HyperNeat; }
        }

        protected override Genotype Genotype
        {
            get { return Genotype.Neat; }
        }

        protected override double[] SetInputNodePosition(int nodeIndex)
        {
            int n = 16;
            int m = 16;
            int i = nodeIndex / m;
            int j = nodeIndex % m;
            return new double[] { (double)i / n, (double)j / m };
        }

        protected override double[] SetOutputNodePosition(int nodeIndex)
        {
            var x = (double)nodeIndex / NbOutputs;
            return new double[] { x, x };
        }

    }
}

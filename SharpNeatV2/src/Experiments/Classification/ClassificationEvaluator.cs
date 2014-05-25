
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using System.Diagnostics;
using SharpNeat.Utility;
using SharpNeat.Experiments.Common;

namespace SharpNeat.Experiments.Classification
{
    public class ClassificationEvaluator : IViewableEvaluator
    {
        #region Instance Fields

        // Evaluator state.
        ulong _evalCount;
        protected IClassificationDataset dataset;
        private IEnumerable<double> _weights;
        protected Phenotype phenotype;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct evaluator with the provided task arguments/variables.
        /// IMPORTANT : only for use with Test !
        /// </summary>
        public ClassificationEvaluator(IClassificationDataset dataset, Phenotype phenotype)
        {
            this.dataset = dataset;
            this._weights = null;
            this.phenotype = phenotype;
        }

        public ClassificationEvaluator(IClassificationDataset dataset, Phenotype phenotype, IEnumerable<double> weights)
        {
            this.dataset = dataset;
            this.phenotype = phenotype;
            this._weights = weights;
        }

        #endregion

        #region IPhenomeEvaluator<IBlackBox> Members

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the total number of evaluations that have been performed.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        /// <summary>
        /// Evaluate the provided IBlackBox.
        /// </summary>
        public FitnessInfo Evaluate(IBlackBox box)
        {
            int nbSamples = dataset.InputSamples.Count();
            var results = new ResultType[nbSamples][];
            var squaredErrors = new double[nbSamples][];

            // Evaluate each samples of the dataset
            for (var i = 0; i < nbSamples; i++)
            {
                var inputs = dataset.InputSamples[i];
                var expected = dataset.OutputSamples[i];
                var outputs = new double[dataset.OutputCount];

                activate(box, dataset.InputSamples[i], outputs);
                results[i] = outputs.Zip(expected, (o, e) => getResultType(o, e)).ToArray();
                squaredErrors[i] = outputs.Zip(expected, (o, e) => Math.Pow(e - o, 2.0)).ToArray();
            }

            // Compute per-column sums
            var TPs = new int[dataset.OutputCount];
            var TNs = new int[dataset.OutputCount];
            var FPs = new int[dataset.OutputCount];
            var FNs = new int[dataset.OutputCount];
            var sumSquaredErrors = new double[dataset.OutputCount];
            for (var i = 0; i < dataset.OutputCount; i++)
            {
                for (var j = 0; j < nbSamples; j++)
                {
                    TPs[i] += (results[j][i] == ResultType.TP) ? 1 : 0;
                    TNs[i] += (results[j][i] == ResultType.TN) ? 1 : 0;
                    FPs[i] += (results[j][i] == ResultType.FP) ? 1 : 0;
                    FNs[i] += (results[j][i] == ResultType.FN) ? 1 : 0;
                    sumSquaredErrors[i] += squaredErrors[j][i];
                }
            }

            // Compute fitness measures
            var TP = TPs.Mean();
            var TN = TNs.Mean();
            var FP = FPs.Mean();
            var FN = FNs.Mean();
            var RMSE = sumSquaredErrors.Select(x => Math.Pow(2.0, -Math.Sqrt(x))).Mean();

            // Compute final fitness value
            var fitness = new double[4];
            var weights = _weights.ToList();
            Debug.Assert(weights.Count == 4, "weights must correspond to { accuracy, sensitivity, specificity, rmse }");

            // accuracy
            fitness[0] = (TP + TN) / (TP + TN + FP + FN);
            // sensitivity 
            fitness[1] = (TP > 0) ? TP / (TP + FN) : 0;
            // specificity 
            fitness[2] = (TN > 0) ? TN / (TN + FP) : 0;
            // rmse 
            fitness[3] = RMSE;

            var score = fitness.Zip(weights, (f, w) => f * w).Sum() / weights.Sum();

            _evalCount++;

            return new FitnessInfo(score, fitness[0]);
        }

        /// <summary>
        /// Interpret a result as a true positive, false positive, true negative or false negative.
        /// </summary>
        private ResultType getResultType(double output, double expected)
        {
            var binOutput = binarize(output);
            var binExpected = binarize(expected);
            bool isCorrect = binOutput == binExpected;
            if (binOutput == 1)
            {
                return isCorrect ? ResultType.TP : ResultType.FP;
            }
            else
            {
                return isCorrect ? ResultType.TN : ResultType.FN;
            }
        }

        private int binarize(double value)
        {
            return value >= 0.5 ? 1 : 0;
        }

        private double ratioOfCorrectOutputForValue(int value, IList<int> outputs, IList<int> expected)
        {
            int nbCorrectValue = 0;
            int nbEqualsValue = 0;
            for (int i = 0; i < outputs.Count(); i++)
            {
                nbCorrectValue += (outputs[i] == value && expected[i] == value ? 1 : 0);
                nbEqualsValue += (outputs[i] == value ? 1 : 0);
            }
            return nbEqualsValue > 0 ? (double)nbCorrectValue / nbEqualsValue : 0.0;
        }

        public void Test(SharpNeat.Phenomes.IBlackBox box, IList<double> inputs, OutputProcessor fun)
        {
            var actualOutputs = new double[dataset.OutputCount];
            activate(box, inputs, actualOutputs);
            for (int i = 0; i < dataset.OutputCount; i++)
            {
                fun(i, actualOutputs[i]);
            }
        }

        private double[] activate(IBlackBox box, IList<double> inputs, double[] outputs)
        {
            box.ResetState();

            // Give inputs to the network
            var nodeId = 0;
            foreach (var input in inputs)
            {
                box.InputSignalArray[nodeId++] = input;
            }

            // Activate the network and get outputs back
            box.Activate();
            box.OutputSignalArray.CopyTo(outputs, 0);

            // Normalize outputs when using NEAT
            if (phenotype == Phenotype.Neat)
            {
                NormalizeOutputs(outputs);
            }
            else
            {
                Debug.Assert(outputs.All(x => x >= 0.0 && x <= 1.0));
            }

            return outputs;
        }

        protected void NormalizeOutputs(double[] outputs)
        {
            for (var i = 0; i < outputs.Count(); i++)
            {
                outputs[i] = (outputs[i] + 1.0) / 2.0;
            }
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        public void Reset()
        {
        }

        #endregion

        #region InnerClasses

        public enum ResultType
        {
            TP, TN, FP, FN
        }

        #endregion
    }
}


using SharpNeat.Experiments.Common;
using SharpNeat.Core;
using SharpNeat.Experiments.Clustering;
using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace SharpNeat.Experiments.Classification
{
    public class ClusteringEvaluator : IViewableEvaluator
    {
        #region Instance Fields

        // Evaluator state.
        ulong _evalCount;
        IClusteringDataset dataset;
        Phenotype phenotype;
        int nbClusters;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct evaluator with the provided task arguments/variables.
        /// </summary>
        public ClusteringEvaluator(IClusteringDataset dataset, int nbClusters, Phenotype phenotype)
        {
            this.dataset = dataset;
            this.nbClusters = nbClusters;
            this.phenotype = phenotype;
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
            var outputs = new double[nbSamples][];
            var selectedClusters = new int[nbSamples];
            var centers = new double[nbClusters][];
            for (var i = 0; i < nbClusters; i++)
            {
                centers[i] = new double[dataset.InputCount];
            }

            // Evaluate each samples of the dataset
            for (var i = 0; i < nbSamples; i++)
            {
                var inputs = dataset.InputSamples[i];
                outputs[i] = new double[nbClusters];

                activate(box, inputs, outputs[i]);
                selectedClusters[i] = outputs[i].MaxIndex();

                for (var j = 0; j < dataset.InputCount; j++)
                {
                    centers[selectedClusters[i]][j] += inputs[j];
                }
            }

            // Compute center of each cluster
            for (var i = 0; i < nbClusters; i++)
            {
                for (var j = 0; j < dataset.InputCount; j++)
                {
                    centers[i][j] /= nbSamples;
                }
            }

            // Compute inter and intra cluster distances
            var distancesIntra = new double[nbClusters];
            var distancesInter = 0.0;
            for (var i = 0; i < nbSamples; i++)
            {
                var cluster = selectedClusters[i];
                distancesIntra[cluster] += distance(dataset.InputSamples[i], centers[cluster]);
            }
            for (var i = 0; i < nbClusters - 1; i++)
            {
                for (var j = i; j < nbClusters; j++)
                {
                    distancesInter += distance(centers[i], centers[j]);
                }
            }
            double intra = distancesIntra.Mean();
            double inter = distancesInter / nbClusters;

            _evalCount++;

            return new FitnessInfo(inter / intra, intra);
        }

        private double distance(IList<double> sample1, IList<double> sample2)
        {
            // Simple euclidian distance
            return Math.Sqrt(sample1.Zip(sample2, (s1, s2) => Math.Pow(s1 - s2, 2.0)).Sum());
        }

        public void Test(SharpNeat.Phenomes.IBlackBox box, IList<double> inputs, OutputProcessor fun)
        {
            var actualOutputs = new double[nbClusters];
            activate(box, inputs, actualOutputs);
            for (int i = 0; i < nbClusters; i++)
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
                for (var i = 0; i < outputs.Count(); i++)
                {
                    outputs[i] = (outputs[i] + 1.0) / 2.0;
                }
            }
            else
            {
                Debug.Assert(outputs.All(x => x >= 0.0 && x <= 1.0));
            }

            return outputs;
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        public void Reset()
        {
        }

        #endregion

        #region InnerClasses

        #endregion
    }
}

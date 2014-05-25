
using SharpNeat.Core;
using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SharpNeat.Experiments.Common;

namespace SharpNeat.Experiments.Clustering
{
    public class WindowMapClusteringEvaluator : IViewableEvaluator
    {
        #region Instance Fields

        // Evaluator state.
        ulong _evalCount;
        Phenotype phenotype;
        int nbClusters;
        bool[,] filter;
        double[, ,] samples;
        private int f, f2, t, n, m;
        private int nbInputsNN, nbOutputsNN;
        private int nbInputs;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct evaluator with the provided task arguments/variables.
        /// </summary>
        public WindowMapClusteringEvaluator(IMapClusteringDataset dataset, int nbClusters, Phenotype phenotype, bool[,] filter)
        {
            this.nbClusters = nbClusters;
            this.phenotype = phenotype;
            this.filter = filter;

            Debug.Assert(filter.GetLength(0) % 2 != 0 && filter.GetLength(1) % 2 != 0);

            // Build input layers (samples matrices)
            nbInputs = dataset.InputCount;
            samples = dataset.GetSamplesMatrix();

            // Extract useful values
            f = filter.GetLength(0); // filter width
            f2 = filter.Length; // f^2
            t = (f - 1) / 2; // filter thickness
            nbInputsNN = nbInputs * f2;
            nbOutputsNN = nbClusters * f2;
            n = samples.GetLength(1); // layers width
            m = samples.GetLength(2); // layers height
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

        private double[][] center;

        /// <summary>
        /// Evaluate the provided IBlackBox.
        /// </summary>
        public FitnessInfo Evaluate(IBlackBox box)
        {
            var outputs = new double[n, m][];
            var results = new double[n, m][];
            var clusterSize = new double[nbClusters];
            var selectedClusters = new int[n, m];
            center = new double[nbClusters][];
            var clusterMin = new double[nbClusters];
            var clusterMax = new double[nbClusters];
            for (var i = 0; i < nbClusters; i++)
            {
                center[i] = new double[nbInputs];
                clusterMin[i] = double.PositiveInfinity;
            }

            // Evaluate each samples of the dataset (where filter doesn't overflow)
            int nbSamples = 0;
            for (var i = t; i < n - t; i++)
            {
                for (var j = t; j < m - t; j++)
                {
                    // Get inputs with filter and activate the network
                    var inputs = GetInputs(i, j).ToList();
                    outputs[i, j] = new double[nbOutputsNN];
                    activate(box, inputs, outputs[i, j]);
                    results[i, j] = GetOutputValuePerCluster(outputs[i, j]) as double[];

                    // Update cluster metrics
                    for (int cluster = 0; cluster < nbClusters; cluster++)
                    {
                        for (var k = 0; k < nbInputs; k++)
                        {
                            double membership = results[i, j][cluster];
                            if (membership > 0.2)
                            {
                                if (membership < clusterMin[cluster])
                                    clusterMin[cluster] = membership;
                                if (membership > clusterMax[cluster])
                                    clusterMax[cluster] = membership;
                            }
                            else
                            {
                                membership = 0.0;
                            }

                            center[cluster][k] += membership * inputs[k];
                            clusterSize[cluster] += membership;
                            results[i, j][cluster] = membership;
                        }
                    }

                    nbSamples++;
                }
            }

            // Compute the center of each cluster
            for (var i = 0; i < nbClusters; i++)
            {
                for (var j = 0; j < nbInputs; j++)
                {
                    center[i][j] /= Math.Max(1.0, clusterSize[i]);
                }
            }

            // Compute inter and intra cluster distances
            var distancesIntra = new double[nbClusters];
            var distancesInter = 0.0;
            for (var i = t; i < n - t; i++) // avoids samples that are not evaluated
            {
                for (var j = t; j < m - t; j++)
                {
                    for (int cluster = 0; cluster < nbClusters; cluster++)
                    {
                        var weightedPoint = getSampleValues(i, j).Select(x => x * results[i, j][cluster]).ToArray();
                        distancesIntra[cluster] += distance(weightedPoint, center[cluster]);
                    }
                }
            }
            for (var i = 0; i < nbClusters; i++)
            {
                for (var j = i + 1; j < nbClusters; j++)
                {
                    distancesInter += distance(center[i], center[j]);
                }
            }

            double intra = distancesIntra.Mean();
            double inter = distancesInter / (nbClusters * (nbClusters - 1) / 2.0);
            double minSize = clusterSize.Min();
            double activity = minSize > 0 ? clusterMax.Zip(clusterMin, (cmax, cmin) => cmax - cmin).Mean() : 0;

            _evalCount++;

            return new FitnessInfo(activity + inter / (1 + intra), activity);
        }

        /// <summary>
        /// Get a vector of all input values for a specific sample.
        /// </summary>
        /// <param name="x">X coord. of sample position</param>
        /// <param name="y">Y coord. of sample position</param>
        /// <returns></returns>
        private IList<double> getSampleValues(int x, int y)
        {
            var values = new double[nbInputs];
            for (var i = 0; i < nbInputs; i++)
            {
                values[i] = samples[i, x, y];
            }
            return values;
        }

        /// <summary>
        /// Iterate through the positions of the area delimited by the filter around a specific sample,
        /// and yield the corresponding input values.
        /// </summary>
        /// <param name="x">X coord. of sample position</param>
        /// <param name="y">Y coord. of sample position</param>
        /// <returns></returns>
        public IEnumerable<double> GetInputs(int x, int y)
        {
            for (int i = 0; i < nbInputs; i++)
            {
                for (int j = x - t, fj = 0; j <= x + t; j++, fj++)
                {
                    for (int k = y - t, fk = 0; k <= y + t; k++, fk++)
                    {
                        if (filter[fj, fk])
                        {
                            if (i >= 0 && j >= 0 && k >= 0 && j < n && k < m)
                            {
                                yield return samples[i, j, k];
                            }
                            else
                            {
                                yield return 0;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Extract the activation levels for each cluster from the output matrices, considering
        /// only the sample that we're actually interested in.
        /// </summary>
        /// <param name="outputVector">Flat output vector as given by the IBlackBox.Activate() method.</param>
        /// <returns></returns>
        public IList<double> GetOutputValuePerCluster(double[] outputVector)
        {
            if (phenotype == Phenotype.Neat)
            {
                return outputVector;
            }
            else
            {
                var result = new double[nbClusters];
                for (var cluster = 0; cluster < nbClusters; cluster++)
                {
                    result[cluster] = outputVector[cluster * f2 + t * f + t]; // We're only considering the value at the center
                }
                return result;
            }
        }

        private double distance(IList<double> sample1, IList<double> sample2)
        {
            // Simple euclidian distance
            return Math.Sqrt(sample1.Zip(sample2, (s1, s2) => Math.Pow(s1 - s2, 2.0)).Sum());
        }

        public void Test(SharpNeat.Phenomes.IBlackBox box, IList<double> inputs, OutputProcessor fun)
        {
            var outputs = new double[nbOutputsNN];
            activate(box, inputs, outputs);
            var clustersOutputs = GetOutputValuePerCluster(outputs);
            for (int i = 0; i < nbClusters; i++)
            {
                fun(i, clustersOutputs[i]);
            }
        }

        private double[] activate(IBlackBox box, IEnumerable<double> inputs, double[] outputs)
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
            return outputs;
        }

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        public void Reset()
        {
        }

        #endregion
    }
}

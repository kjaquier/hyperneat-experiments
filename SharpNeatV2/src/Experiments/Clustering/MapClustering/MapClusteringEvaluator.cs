
using SharpNeat.Experiments.Common;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SharpNeat.Experiments.Clustering
{
    public class MapClusteringEvaluator : IViewableEvaluator
    {
        #region Instance Fields

        // Evaluator state.
        ulong _evalCount;
        Phenotype phenotype;
        int nbClusters;
        double[, ,] samples;
        private int n, m;
        private int nbInputsNN, nbOutputsNN;
        private int nbInputs;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct evaluator with the provided task arguments/variables.
        /// </summary>
        public MapClusteringEvaluator(IMapClusteringDataset dataset, int nbClusters, Phenotype phenotype)
        {
            this.nbClusters = nbClusters;
            this.phenotype = phenotype;

            // Build input layers (samples matrices)
            nbInputs = dataset.InputCount;
            samples = dataset.GetSamplesMatrix();

            // Extract useful values
            n = samples.GetLength(1); // layers width
            m = samples.GetLength(2); // layers height
            nbInputsNN = samples.Length;
            nbOutputsNN = nbClusters * n * m;
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
            double[] outputs;
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

            // Get the outputs from the neural network
            outputs = new double[nbOutputsNN];
            activate(box, GetAllInputs(), outputs);

            // Evaluate each samples of the dataset
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < m; j++)
                {
                    var inputs = GetInputs(i, j).ToList();

                    results[i, j] = GetOutputValuePerCluster(outputs, i, j) as double[];

                    // Update cluster metrics
                    for (int cluster = 0; cluster < nbClusters; cluster++)
                    {
                        for (var k = 0; k < nbInputs; k++)
                        {
                            // Apply threshold on the output (so that networks cannot output values like 0.0001
                            // just to create variation)
                            double membership = results[i, j][cluster];
                            if (membership < 0.2)
                            {
                                membership = 0.0;
                            }

                            if (membership < clusterMin[cluster])
                                clusterMin[cluster] = membership;
                            if (membership > clusterMax[cluster])
                                clusterMax[cluster] = membership;

                            center[cluster][k] += membership * inputs[k];
                            clusterSize[cluster] += membership;
                            results[i, j][cluster] = membership;
                        }
                    }
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

            // Compute intra-cluster distances
            var distancesIntra = new double[nbClusters];
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < m; j++)
                {
                    for (int cluster = 0; cluster < nbClusters; cluster++)
                    {
                        var weightedPoint = getSampleValues(i, j).Select(x => x * results[i, j][cluster]).ToArray();
                        distancesIntra[cluster] += distance(weightedPoint, center[cluster]);
                    }
                }
            }
            for (int cluster = 0; cluster < nbClusters; cluster++)
            {
                distancesIntra[cluster] /= n * m;
            }


            // Compute inter-cluster distances
            var distancesInter = 0.0;
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
        /// Returns the input vector for the sample at position (x,y)
        /// </summary>
        public IEnumerable<double> GetInputs(int x, int y)
        {
            for (int i = 0; i < samples.GetLength(0); i++)
            {
                yield return samples[i, x, y];
            }
        }

        /// <summary>
        /// Get all elements from the input matrix
        /// </summary>
        public IEnumerable<double> GetAllInputs()
        {
            for (int i = 0; i < samples.GetLength(0); i++)
            {
                for (int j = 0; j < samples.GetLength(1); j++)
                {
                    for (int k = 0; k < samples.GetLength(2); k++)
                    {
                        yield return samples[i, j, k];
                    }
                }
            }
        }

        /// <summary>
        /// Extract the activation levels for each cluster from the output matrix, considering
        /// only the sample that we're actually interested in.
        /// </summary>
        /// <param name="outputVector">"Flat" output vector as given by the IBlackBox.Activate() method.</param>
        /// <param name="x">X coordinate of the sample</param>
        /// <param name="y">Y coordinate of the sample</param>
        /// <returns>The vector containing the activation level of each cluster for this sample</returns>
        public IList<double> GetOutputValuePerCluster(double[] outputVector, int x, int y)
        {
            if (phenotype == Phenotype.Neat)
            {
                return outputVector;
            }
            else
            {
                var result = new double[nbClusters];
                for (int cluster = 0; cluster < nbClusters; cluster++)
                {
                    var idx = cluster * n * m + x * m + y;
                    result[cluster] = outputVector[idx];
                }
                return result;
            }
        }

        /// <summary>
        /// Computes the distance between two points in the sample space
        /// </summary>
        private double distance(IList<double> sample1, IList<double> sample2)
        {
            // Euclidian distance
            return Math.Sqrt(sample1.Zip(sample2, (s1, s2) => Math.Pow(s1 - s2, 2.0)).Sum());

            // Manhattan distance
            //return sample1.Zip(sample2, (s1, s2) => Math.Abs(s1 - s2)).Sum();
        }

        /// <summary>
        /// Convenien
        /// </summary>
        /// <param name="box"></param>
        /// <param name="inputs"></param>
        /// <param name="fun"></param>
        public void Test(SharpNeat.Phenomes.IBlackBox box, IList<double> inputs, OutputProcessor fun)
        {
            var outputs = new double[nbOutputsNN];
            activate(box, inputs, outputs);
            for (int i = 0; i < nbClusters; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < m; k++)
                    {
                        fun(i, GetOutputValuePerCluster(outputs, j, k)[i]);
                    }
                }
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

            // Normalize outputs when necessary
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

    }
}

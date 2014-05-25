
using SharpNeat.Experiments.Common;
using SharpNeat.Core;
using SharpNeat.Decoders.HyperNeat;
using SharpNeat.Domains;
using SharpNeat.Experiments.Classification;
using SharpNeat.Genomes.Neat;
using SharpNeat.Network;
using SharpNeat.Phenomes;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SharpNeat.Experiments.Clustering
{
    /// <summary>
    /// Soft clustering experiment with data given as a set of input vectors with 
    /// x and y coordinates.
    /// </summary>
    public abstract class MapClusteringExperimentHyperNeat : QuickExperimentHyperNeat
    {
        private int nbClusters;
        private IMapClusteringDataset _dataset;
        private int nbInputs;
        private int n, m;
        private double[, ,] samples;

        protected MapClusteringExperimentHyperNeat(int nbClusters)
        {
            this.nbClusters = nbClusters;
        }

        public override void Initialize(string name, XmlElement xmlConfig)
        {
            base.Initialize(name, xmlConfig);

            _dataset = CreateDataset();
            _dataset.LoadFromFile(DatasetFileName);

            samples = _dataset.GetSamplesMatrix();

            nbInputs = _dataset.InputCount;
            n = samples.GetLength(1);
            m = samples.GetLength(2);
        }

        protected override int DimensionCount
        {
            get
            {
                return 2;
            }
        }

        protected override IViewableEvaluator CreateEvaluator()
        {
            return new MapClusteringEvaluator(_dataset, nbClusters, Phenotype);
        }

        /// <summary>
        /// Gets the number of inputs required by the network/black-box that the underlying problem domain is based on.
        /// </summary>
        public override int InputCount
        {
            get
            {
                if (Phenotype == Phenotype.HyperNeat)
                    return 2 * (DimensionCount + 1) + (_lengthCppnInput ? 1 : 0);
                else
                    return samples.Length;
            }
        }

        /// <summary>
        /// Gets the number of outputs required by the network/black-box that the underlying problem domain is based on.
        /// </summary>
        public override int OutputCount
        {
            get
            {
                if (Phenotype == Phenotype.HyperNeat)
                    return nbClusters;
                else
                    return nbClusters * n * m;
            }
        }

        /// <summary>
        /// Create a genome decoder for the experiment.
        /// </summary>
        protected override IGenomeDecoder<NeatGenome, IBlackBox> CreateHyperNeatGenomeDecoder()
        {
            // Create HyperNEAT network substrate.

            uint nodeId = 1;

            //-- Create input layer nodes.
            var inputLayer = new SubstrateNodeSet(samples.Length);
            for (int v = 0; v < samples.GetLength(0); v++) // variables
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        var x = 2 * (double)i / n - 1;
                        var y = 2 * (double)j / m - 1;
                        var z = -(double)v / samples.GetLength(0);
                        inputLayer.NodeList.Add(new SubstrateNode(nodeId++, new double[] { x, y, z }));
                    }
                }
            }

            //-- Create output layer nodes.
            var outputLayers = new SubstrateNodeSet[nbClusters];
            for (var c = 0; c < nbClusters; c++) // clusters
            {
                outputLayers[c] = new SubstrateNodeSet(n * m);

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        var x = 2 * (double)i / n - 1;
                        var y = 2 * (double)j / m - 1;
                        var z = c / nbClusters;
                        outputLayers[c].NodeList.Add(new SubstrateNode(nodeId++, new double[] { x, y, z }));
                    }
                }
            }

            // Connect up layers.
            List<SubstrateNodeSet> nodeSetList = new List<SubstrateNodeSet>(1 + nbClusters);
            nodeSetList.Add(inputLayer);
            foreach (var layer in outputLayers)
            {
                nodeSetList.Add(layer);
            }


            List<NodeSetMapping> nodeSetMappingList = new List<NodeSetMapping>(nbClusters);
            for (int i = 0; i < nbClusters; i++)
            {
                var inputLayerIdx = 0;
                var outputLayerIdx = 1 + i;
                nodeSetMappingList.Add(NodeSetMapping.Create(inputLayerIdx, outputLayerIdx, (double?)null));
            }

            // Construct substrate.
            Substrate substrate = new Substrate(nodeSetList,
                DefaultActivationFunctionLibrary.CreateLibraryNeat(SteepenedSigmoid.__DefaultInstance),
                0, 0.2, 5,
                nodeSetMappingList);

            // Create genome decoder. Decodes to a neural network packaged with an activation scheme that defines a fixed number of activations per evaluation.
            IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder = new HyperNeatDecoder(substrate, _activationSchemeCppn, _activationScheme, _lengthCppnInput);
            return genomeDecoder;

        }

        public override AbstractDomainView CreateDomainView()
        {
            //return new MapClusteringScatterPlotView(CreateGenomeDecoder(), _dataset, CreateEvaluator() as MapClusteringEvaluator, nbClusters);
            return new MapClusteringView(CreateGenomeDecoder(), _dataset, CreateEvaluator() as MapClusteringEvaluator, nbClusters);
        }

        protected abstract IMapClusteringDataset CreateDataset();
    }
}

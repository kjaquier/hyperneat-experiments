
using SharpNeat.Core;
using SharpNeat.Decoders.HyperNeat;
using SharpNeat.Domains;
using SharpNeat.Experiments.Common;
using SharpNeat.Genomes.Neat;
using SharpNeat.Network;
using SharpNeat.Phenomes;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SharpNeat.Experiments.Clustering
{
    public abstract class WindowMapClusteringExperimentHyperNeat : QuickExperimentHyperNeat
    {
        private int nbClusters;
        private IMapClusteringDataset _dataset;
        private bool[,] filter;
        private int nbInputs;
        private int n, m, f, f2, t;

        protected WindowMapClusteringExperimentHyperNeat(int nbClusters, bool[,] filter)
        {
            this.nbClusters = nbClusters;
            this.filter = filter;
        }

        public override void Initialize(string name, XmlElement xmlConfig)
        {
            base.Initialize(name, xmlConfig);

            _dataset = CreateDataset();
            _dataset.LoadFromFile(DatasetFileName);

            nbInputs = _dataset.InputCount;
            n = _dataset.InputSamples.Count(sampleRow => sampleRow.Last() == 0);
            m = _dataset.InputSamples.First().Count();
            f = filter.GetLength(0);
            f2 = filter.Length; // f^2
            t = (f - 1) / 2; // filter thickness
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
            return new WindowMapClusteringEvaluator(_dataset, nbClusters, Phenotype, filter);
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
                    return nbInputs * f2;
            }
        }

        /// <summary>
        /// Gets the number of outputs required by the network/black-box that the underlying problem domain is based on.
        /// </summary>
        public override int OutputCount
        {
            get
            {
                return nbClusters;
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
            var inputLayer = new SubstrateNodeSet(nbInputs * f2);
            for (var i = 0; i < nbInputs; i++)
            {
                for (var j = 0; j < f; j++)
                {
                    for (var k = 0; k < f; k++)
                    {
                        if (filter[j, k])
                        {
                            var x = (double)j / f - 0.5;
                            var y = (double)k / f - 0.5;
                            inputLayer.NodeList.Add(new SubstrateNode(nodeId++, new double[] { x, y, -1 - i }));
                        }
                    }
                }
            }

            //-- Create hidden layer nodes.
            var hiddenLayers = new SubstrateNodeSet[nbClusters];
            for (var i = 0; i < nbClusters; i++)
            {
                hiddenLayers[i] = new SubstrateNodeSet(f2);

                for (var j = 0; j < f; j++)
                {
                    for (var k = 0; k < f; k++)
                    {
                        if (filter[j, k])
                        {
                            var x = (double)j / f - 0.5;
                            var y = (double)k / f - 0.5;
                            hiddenLayers[i].NodeList.Add(new SubstrateNode(nodeId++, new double[] { x, y, 0 }));
                        }
                    }
                }
            }

            //-- Create output layer nodes.
            var outputLayers = new SubstrateNodeSet[nbClusters];
            for (var i = 0; i < nbClusters; i++)
            {
                outputLayers[i] = new SubstrateNodeSet(f2);

                for (var j = 0; j < f; j++)
                {
                    for (var k = 0; k < f; k++)
                    {
                        if (filter[j, k])
                        {
                            var x = (double)j / f - 0.5;
                            var y = (double)j / f - 0.5;
                            outputLayers[i].NodeList.Add(new SubstrateNode(nodeId++, new double[] { x, y, +1 + i }));
                        }
                    }
                }
            }

            // Connect up layers.
            List<SubstrateNodeSet> nodeSetList = new List<SubstrateNodeSet>(1 + 2 * nbClusters);
            nodeSetList.Add(inputLayer);
            foreach (var layer in hiddenLayers)
            {
                nodeSetList.Add(layer);
            }
            foreach (var layer in outputLayers)
            {
                nodeSetList.Add(layer);
            }


            List<NodeSetMapping> nodeSetMappingList = new List<NodeSetMapping>(2 * nbClusters);
            for (int i = 0; i < nbClusters; i++)
            {
                var inputLayerIdx = 0;
                var hiddenLayerIdx = 1 + i;
                var outputLayerIdx = 1 + nbClusters + i;
                nodeSetMappingList.Add(NodeSetMapping.Create(inputLayerIdx, hiddenLayerIdx, (double?)null));
                nodeSetMappingList.Add(NodeSetMapping.Create(hiddenLayerIdx, outputLayerIdx, (double?)null));
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
            return new WindowMapClusteringView(CreateGenomeDecoder(), _dataset, CreateEvaluator() as WindowMapClusteringEvaluator, nbClusters);
        }

        protected abstract IMapClusteringDataset CreateDataset();
    }
}

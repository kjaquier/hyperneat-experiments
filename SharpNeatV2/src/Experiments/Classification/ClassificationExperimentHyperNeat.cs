
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using log4net;
using SharpNeat.Core;
using SharpNeat.Experiments.Common;
using SharpNeat.Decoders;
using SharpNeat.Decoders.HyperNeat;
using SharpNeat.DistanceMetrics;
using SharpNeat.Domains;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.Genomes.HyperNeat;
using SharpNeat.Genomes.Neat;
using SharpNeat.Network;
using SharpNeat.Phenomes;
using SharpNeat.SpeciationStrategies;
using SharpNeat.Decoders.Neat;
using System.Diagnostics;
using SharpNeat.Genomes.RbfNeat;

namespace SharpNeat.Experiments.Classification
{
    /// <summary>
    /// Generic classification experiment. Just needs a dataset with a specified number of
    /// inputs and classes.
    /// </summary>
    public abstract class ClassificationExperimentHyperNeat : QuickExperimentHyperNeat
    {
        protected IClassificationDataset _dataset;

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ClassificationExperimentHyperNeat()
        {
        }

        #endregion

        #region INeatExperiment

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
                    return _dataset.InputCount;
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
                    return 2;
                else
                    return _dataset.OutputCount;
            }
        }

        public override void Initialize(string name, XmlElement xmlConfig)
        {
            base.Initialize(name, xmlConfig);

            _dataset = CreateDataset();
            _dataset.LoadFromFile(DatasetFileName);
        }

        /*
         * Overridable settings
         */

        /// <summary>
        /// Weight of the different metrics used in the evaluation function.
        /// Corresponds to { accuracy, sensitivity, specificity, rmse }
        /// </summary>
        protected abstract IEnumerable<double> EvaluationWeights { get; }

        protected abstract IClassificationDataset CreateDataset();

        protected virtual double[] SetInputNodePosition(int nodeIndex)
        {
            return new double[] { (double)nodeIndex / InputCount };
        }

        protected virtual double[] SetHiddenNodePosition(int nodeIndex)
        {
            return new double[] { (double)nodeIndex / HiddenNodesCount };
        }

        protected virtual double[] SetOutputNodePosition(int nodeIndex)
        {
            return new double[] { (double)nodeIndex / _dataset.OutputCount };
        }

        /// <summary>
        /// Create a genome decoder for the experiment.
        /// </summary>
        protected override IGenomeDecoder<NeatGenome, IBlackBox> CreateHyperNeatGenomeDecoder()
        {
            // Create HyperNEAT network substrate.

            uint nodeId = 1;

            //-- Create input layer nodes.
            SubstrateNodeSet inputLayer = new SubstrateNodeSet(_dataset.InputCount);
            for (var i = 0; i < _dataset.InputCount; i++)
            {
                inputLayer.NodeList.Add(new SubstrateNode(nodeId++, SetInputNodePosition(i).Extend(-1)));
            }

            //-- Create output layer nodes.
            SubstrateNodeSet outputLayer = new SubstrateNodeSet(_dataset.OutputCount);
            for (var i = 0; i < _dataset.OutputCount; i++)
            {
                outputLayer.NodeList.Add(new SubstrateNode(nodeId++, SetOutputNodePosition(i).Extend(+1)));
            }

            //-- Create hidden layer nodes.
            SubstrateNodeSet hiddenLayer = null;
            if (HiddenNodesCount > 0)
            {
                hiddenLayer = new SubstrateNodeSet(HiddenNodesCount);
                for (var i = 0; i < HiddenNodesCount; i++)
                {
                    hiddenLayer.NodeList.Add(new SubstrateNode(nodeId++, SetHiddenNodePosition(i).Extend(0)));
                }
            }

            // Connect up layers.
            List<SubstrateNodeSet> nodeSetList = new List<SubstrateNodeSet>(2 + (HiddenNodesCount > 0 ? 1 : 0));
            nodeSetList.Add(inputLayer);
            nodeSetList.Add(outputLayer);
            if (HiddenNodesCount > 0)
            {
                nodeSetList.Add(hiddenLayer);
            }

            List<NodeSetMapping> nodeSetMappingList = new List<NodeSetMapping>(1 + (HiddenNodesCount > 0 ? 1 : 0));
            if (HiddenNodesCount > 0)
            {
                nodeSetMappingList.Add(NodeSetMapping.Create(0, 2, (double?)null));  // Input -> Hidden.
                nodeSetMappingList.Add(NodeSetMapping.Create(2, 1, (double?)null));  // Hidden-> Output.
                nodeSetMappingList.Add(NodeSetMapping.Create(0, 1, (double?)null));  // Input -> Output.
            }
            else
            {
                nodeSetMappingList.Add(NodeSetMapping.Create(0, 1, (double?)null));  // Input -> Output.
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

        protected override IViewableEvaluator CreateEvaluator()
        {
            return new ClassificationEvaluator(_dataset, Phenotype, EvaluationWeights);
        }

        #endregion

    }
}

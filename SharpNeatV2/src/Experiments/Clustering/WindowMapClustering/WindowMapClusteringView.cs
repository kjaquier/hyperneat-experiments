using SharpNeat.Core;
using SharpNeat.Domains;
using SharpNeat.Experiments.Common;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SharpNeat.Experiments.Clustering
{
    public partial class WindowMapClusteringView : AbstractDomainView
    {
        private IGenomeDecoder<NeatGenome, IBlackBox> decoder;
        private int nbClusters;
        private IMapClusteringDataset dataset = null;
        private WindowMapClusteringEvaluator evaluator;
        private double[, ,] samples;
        private double[, ,] outputs;

        private int currentClusterIdx = 0;
        private int currentInputIdx = 0;

        private double maxValue;

        public WindowMapClusteringView()
        {
            InitializeComponent();
        }

        public WindowMapClusteringView(IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder,
                                IMapClusteringDataset dataset,
                                WindowMapClusteringEvaluator evaluator,
                                int nbClusters)
            : this()
        {
            this.dataset = dataset;
            this.nbClusters = nbClusters;
            this.decoder = genomeDecoder;
            this.evaluator = evaluator;

            samples = dataset.GetSamplesMatrix();
            outputs = new double[nbClusters, samples.GetLength(1), samples.GetLength(2)];

            inputView.LabelName = "Input";
            outputView.LabelName = "Output";

            inputView.SetDimensions(samples.GetLength(0), samples.GetLength(1), samples.GetLength(2));
            outputView.SetDimensions(nbClusters, samples.GetLength(1), samples.GetLength(2));

            maxValue = 0.0;
            for (var i = 0; i < samples.GetLength(0); i++)
            {
                for (var j = 0; j < samples.GetLength(1); j++)
                {
                    for (var k = 0; k < samples.GetLength(2); k++)
                    {
                        var value = samples[i, j, k];
                        if (value > maxValue) maxValue = value;
                    }
                }
            }
            maxValue *= 2; // margin

            inputView.OnClusterChanged += (id) =>
            {
                currentInputIdx = id;
                RefreshInput();
            };
            outputView.OnClusterChanged += (id) =>
            {
                currentClusterIdx = id;
                RefreshOutput();
            };

            RefreshInput();
        }

        public override Size WindowSize
        {
            get
            {
                return new Size(700, 580);
            }
        }

        public override Size MinimumSize
        {
            get
            {
                return new Size(700, 500);
            }
        }

        public override void RefreshView(object genome)
        {
            if (genome == null) return;

            var box = decoder.Decode(genome as NeatGenome);

            for (var i = 0; i < samples.GetLength(1); i++)
            {
                for (var j = 0; j < samples.GetLength(2); j++)
                {
                    evaluator.Test(box, evaluator.GetInputs(i, j).ToArray(), (idx, output) =>
                    {
                        outputs[idx, i, j] = output;
                    });
                }
            }

            RefreshOutput();
        }

        private Brush valueToColor(double value, double minValue, double maxValue)
        {
            var n = normalize(value, minValue, maxValue);
            var c = Convert.ToInt32(n * 255);
            return new SolidBrush(Color.FromArgb(c, c, c));
        }

        private void RefreshInput()
        {
            inputView.Preview.Update((x, y) => valueToColor(samples[currentInputIdx, x, y], 0, maxValue));
        }

        private void RefreshOutput()
        {
            outputView.Preview.Update((x, y) => valueToColor(outputs[currentClusterIdx, x, y], 0, 1));
        }

        private double normalize(double value, double lowerBound, double upperBound)
        {
            return (value - lowerBound) / (upperBound - lowerBound);
        }
    }
}

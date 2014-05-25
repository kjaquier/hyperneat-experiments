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
    public partial class MapClusteringView : AbstractDomainView
    {
        private IGenomeDecoder<NeatGenome, IBlackBox> decoder;
        private int nbClusters;
        private IMapClusteringDataset dataset = null;
        private MapClusteringEvaluator evaluator;
        private double[, ,] samples;
        private double[, ,] outputs;

        private int currentClusterIdx = 0;
        private int currentInputIdx = 0;

        private double maxValue, minValue;

        private MapClusteringScatterPlotView scatterPlotView = null;

        public MapClusteringView()
        {
            InitializeComponent();
        }

        public MapClusteringView(IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder,
                                IMapClusteringDataset dataset,
                                MapClusteringEvaluator evaluator,
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
            minValue = double.PositiveInfinity;
            for (var i = 0; i < samples.GetLength(0); i++)
            {
                for (var j = 0; j < samples.GetLength(1); j++)
                {
                    for (var k = 0; k < samples.GetLength(2); k++)
                    {
                        var value = samples[i, j, k];
                        if (value > maxValue) maxValue = value;
                        if (value < minValue) minValue = value;
                    }
                }
            }
            // Add some margin
            maxValue *= 2;
            minValue *= 2;

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

            if (scatterPlotView != null)
            {
                scatterPlotView.RefreshView(genome);
            }

            var box = decoder.Decode(genome as NeatGenome);

            var samples = dataset.GetSamplesMatrix();
            var n = samples.GetLength(1);
            var m = samples.GetLength(2);

            int c = 0, i = 0, j = 0;
            evaluator.Test(box, evaluator.GetAllInputs().ToArray(), (idx, output) =>
            {
                outputs[c, i, j] = output;

                j++;
                if (j == m) i++;
                if (i == n) c++;
                c %= nbClusters;
                i %= n;
                j %= m;
            });


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
            inputView.Preview.Update((x, y) => valueToColor(samples[currentInputIdx, x, y], minValue, maxValue));
        }

        private void RefreshOutput()
        {
            outputView.Preview.Update((x, y) => valueToColor(outputs[currentClusterIdx, x, y], 0, 1));
        }

        private double normalize(double value, double lowerBound, double upperBound)
        {
            return (value - lowerBound) / (upperBound - lowerBound);
        }

        private void btnShowScatterPlot_Click(object sender, EventArgs e)
        {
            if (scatterPlotView == null)
            {
                scatterPlotView = new MapClusteringScatterPlotView(decoder, dataset, evaluator, nbClusters);
                scatterPlotView.FormClosing += (s, args) => scatterPlotView = null;
                scatterPlotView.Show();
            }
        }

    }
}

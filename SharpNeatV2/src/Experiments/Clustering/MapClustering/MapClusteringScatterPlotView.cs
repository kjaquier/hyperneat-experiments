using SharpNeat.Core;
using SharpNeat.Domains;
using SharpNeat.Experiments.Clustering;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SharpNeat.Experiments.Common
{
    public partial class MapClusteringScatterPlotView : Form
    {
        private IGenomeDecoder<NeatGenome, IBlackBox> decoder;
        private int nbClusters;
        private int nbInputs = 1;
        private IMapClusteringDataset dataset = null;
        private double[, ,] samples;
        private double[, ,] outputs;
        private double maxValue;
        private MapClusteringEvaluator evaluator;
        private int currentDimensionIdx = 0;

        private string _name = "";
        public string LabelName
        {
            get { return _name; }
            set
            {
                _name = value;
                updateLabel();
            }
        }

        public MapClusteringScatterPlotView()
        {
            InitializeComponent();
            LabelName = "";
        }

        public MapClusteringScatterPlotView(IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder, IMapClusteringDataset dataset, MapClusteringEvaluator evaluator, int nbClusters)
            : this()
        {
            Initialize(genomeDecoder, dataset, evaluator, nbClusters);
        }

        public void Initialize(IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder, IMapClusteringDataset dataset, MapClusteringEvaluator evaluator, int nbClusters)
        {
            this.dataset = dataset;
            this.nbClusters = nbClusters;
            this.decoder = genomeDecoder;
            this.evaluator = evaluator;

            samples = dataset.GetSamplesMatrix();
            outputs = new double[nbClusters, samples.GetLength(1), samples.GetLength(2)];
            this.nbInputs = samples.GetLength(0);

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
        }

        public void RefreshView(object genome)
        {
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

            refreshPlot();
        }

        private void refreshPlot()
        {

            plotChart.Series.Clear();

            // Add series for sample values
            Series serie;
            serie = plotChart.Series.Add("No cluster");
            serie.ChartType = SeriesChartType.Point;
            serie.Color = Color.Gray;
            for (var cluster = 0; cluster < nbClusters; cluster++)
            {
                serie = plotChart.Series.Add("Cluster #" + cluster);
                serie.ChartType = SeriesChartType.Point;
            }

            double xmin = double.PositiveInfinity;
            double xmax = 0;
            double ymin = double.PositiveInfinity;
            double ymax = 0;

            for (var i = 0; i < samples.GetLength(1); i++)
            {
                for (var j = 0; j < samples.GetLength(2); j++)
                {
                    var inputs = evaluator.GetInputs(i, j).ToArray();

                    var x = inputs[currentDimensionIdx];
                    var y = inputs[(currentDimensionIdx + 1) % nbInputs];

                    int selectedCluster = 0;
                    double maxActivation = 0.0;
                    for (int cluster = 0; cluster < nbClusters; cluster++)
                    {
                        if (outputs[cluster, i, j] > maxActivation)
                        {
                            selectedCluster = cluster;
                            maxActivation = outputs[cluster, i, j];
                        }
                    }

                    if (x < xmin) xmin = x;
                    if (x > xmax) xmax = x;
                    if (y < ymin) ymin = y;
                    if (y > ymax) ymax = y;

                    plotChart.Series[maxActivation > 0 ? selectedCluster + 1 : 0].Points.AddXY(x, y);
                }
            }

            plotChart.ChartAreas[0].AxisX.Minimum = xmin;
            plotChart.ChartAreas[0].AxisX.Maximum = xmax;
            plotChart.ChartAreas[0].AxisY.Minimum = ymin;
            plotChart.ChartAreas[0].AxisY.Maximum = xmax;

        }

        private void btnNextDimension_Click(object sender, System.EventArgs e)
        {
            onDimensionChange(+1);
        }

        private void btnPrevDimension_Click(object sender, System.EventArgs e)
        {
            onDimensionChange(-1);
        }

        private void onDimensionChange(int incr)
        {
            incrementIdx(incr);

            updateLabel();

            refreshPlot();
        }

        private void updateLabel()
        {
            lblDimension.Text = LabelName + " " + currentDimensionIdx + " / " + ((currentDimensionIdx + 1) % nbInputs);
        }

        private void incrementIdx(int incr)
        {
            currentDimensionIdx += incr;
            if (currentDimensionIdx == nbInputs)
            {
                currentDimensionIdx = 0;
            }
            if (currentDimensionIdx < 0)
            {
                currentDimensionIdx = nbInputs - 1;
            }
        }
    }
}

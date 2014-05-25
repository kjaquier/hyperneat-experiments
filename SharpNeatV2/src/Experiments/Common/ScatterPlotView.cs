using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using SharpNeat.Core;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using SharpNeat.Domains;
using SharpNeat.Experiments;
using SharpNeat.Experiments.Clustering;

namespace SharpNeat.Experiments.Common
{
    public partial class ScatterPlotView : AbstractDomainView //Form
    {
        private IGenomeDecoder<NeatGenome, IBlackBox> decoder;
        private IBlackBox currentBox;
        private int nbClusters;
        private IClusteringDataset dataset = null;

        public ScatterPlotView()
        {
            InitializeComponent();
        }

        public ScatterPlotView(IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder, IClusteringDataset dataset, int nbClusters)
            : this()
        {
            this.dataset = dataset;
            this.nbClusters = nbClusters;
            this.decoder = genomeDecoder;
        }

        public override void RefreshView(object genome)
        {
            currentBox = decoder.Decode(genome as NeatGenome);

            plotChart.Series.Clear();
            for (var cluster = 0; cluster < nbClusters; cluster++)
            {
                var serie = plotChart.Series.Add("Cluster #" + cluster);
                serie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            }

            var outputs = new double[nbClusters];

            double xmin = double.PositiveInfinity;
            double xmax = 0;
            double ymin = double.PositiveInfinity;
            double ymax = 0;

            for (var i = 0; i < dataset.InputSamples.Count(); i++)
            {
                for (var j = 0; j < dataset.InputCount; j++)
                {
                    currentBox.InputSignalArray[j] = dataset.InputSamples[i][j];
                }
                currentBox.Activate();
                currentBox.OutputSignalArray.CopyTo(outputs, 0);

                var x = dataset.InputSamples[i][0];
                var y = dataset.InputSamples[i][1];
                var cluster = outputs.MaxIndex();

                if (x < xmin) xmin = x;
                if (x > xmax) xmax = x;
                if (y < ymin) ymin = y;
                if (y > ymax) ymax = y;

                plotChart.Series[cluster].Points.AddXY(x, y);
            }

            plotChart.ChartAreas[0].AxisX.Minimum = xmin;
            plotChart.ChartAreas[0].AxisX.Maximum = xmax;
            plotChart.ChartAreas[0].AxisY.Minimum = ymin;
            plotChart.ChartAreas[0].AxisY.Maximum = xmax;

        }
    }
}

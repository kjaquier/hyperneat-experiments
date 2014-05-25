using SharpNeat.Core;
using SharpNeat.Domains;
using SharpNeat.Experiments.Common;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpNeat.Experiments.Classification.OCR
{
    public partial class OCRClassificationView : AbstractDomainView
    {
        private IGenomeDecoder<NeatGenome, IBlackBox> decoder;
        private IClassificationDataset dataset = null;
        private IBlackBox currentBox;
        private int currentSampleId = 0;
        private int sampleCount;
        private Random rng = new Random();
        private IViewableEvaluator evaluator = null;

        public OCRClassificationView()
        {
            InitializeComponent();

            txtOutput.Text = "";
        }

        public OCRClassificationView(IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder, IClassificationDataset dataset, IViewableEvaluator evaluator)
            : this()
        {
            this.decoder = genomeDecoder;
            this.dataset = dataset;
            this.evaluator = evaluator;

            dataset.LoadFromFile(@"Datasets\semeion.samples.data");
            sampleCount = dataset.InputSamples.Count();
        }

        public override Size WindowSize
        {
            get { return new Size(420, 460); }
        }

        public override void RefreshView(object genome)
        {
            currentBox = decoder.Decode(genome as NeatGenome);
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            var pixelsSamples = dataset.InputSamples.Select(sample => sample.ToMatrix(16)).ToList();
            int n = pixelsSamples.First().Count();
            int m = pixelsSamples.First().First().Count();
            picbox.SetResolution(n, m);
            var currentLetter = pixelsSamples[currentSampleId];

            picbox.Update((x, y) => currentLetter[x][y] > 0 ? Brushes.Black : Brushes.White);

            lblLetter.Text = "" + (char)(dataset.OutputSamples[currentSampleId].IndexOf(1) + '0');
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            incrementSampleId(+1);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            incrementSampleId(-1);
        }

        private void incrementSampleId(int incr)
        {
            currentSampleId += incr;
            if (currentSampleId == sampleCount)
            {
                currentSampleId = 0;
            }
            if (currentSampleId < 0)
            {
                currentSampleId = sampleCount - 1;
            }
            UpdatePreview();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            testSample();
        }

        private void btnRandom_Click(object sender, EventArgs e)
        {
            setRandomSample();
        }

        private void setRandomSample()
        {
            currentSampleId = rng.Next(sampleCount);
            UpdatePreview();
        }

        private void testSample()
        {
            var inputs = dataset.InputSamples.ToList()[currentSampleId];
            var outputs = dataset.OutputSamples.ToList()[currentSampleId].ToArray();
            var sb = new StringBuilder();
            evaluator.Test(currentBox, inputs, (i, o) => sb.AppendLine(string.Format("{0} : {1:N3}\n", i, o)));
            txtOutput.Text = sb.ToString();
        }

        private void OCRClassificationView_KeyDown(object sender, KeyEventArgs e) // FIXME
        {
            Console.WriteLine(e.KeyCode);
            bool handled = true;
            switch (e.KeyCode)
            {
                case Keys.Right:
                    incrementSampleId(+1);
                    break;
                case Keys.Left:
                    incrementSampleId(-1);
                    break;
                case Keys.Enter:
                    testSample();
                    break;
                case Keys.Space:
                    setRandomSample();
                    break;
                default:
                    handled = false;
                    break;
            }
            e.Handled = handled;
        }

    }
}

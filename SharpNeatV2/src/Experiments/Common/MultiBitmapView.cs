using System;
using System.Windows.Forms;

namespace SharpNeat.Experiments.Common
{
    public partial class MultiBitmapView : UserControl
    {
        private int currentClusterIdx = 0;
        private int nbClusters = 0;

        private string _name;

        public MultiBitmapView()
        {
            InitializeComponent();

            LabelName = "";
        }

        public BitmapView Preview
        {
            get { return previewBox; }
        }

        public string LabelName
        {
            get { return _name; }
            set
            {
                _name = value;
                updateLabel();
            }
        }

        public void SetDimensions(int nbClusters, int pixelsX, int pixelsY)
        {
            previewBox.SetResolution(pixelsX, pixelsY);
            this.nbClusters = nbClusters;
        }

        public delegate void OnClusterChangedHandler(int newClusterIdx);
        public event OnClusterChangedHandler OnClusterChanged;

        private void btnNextCluster_Click(object sender, EventArgs e)
        {
            onClusterChange(+1);
        }

        private void btnPrevCluster_Click(object sender, EventArgs e)
        {
            onClusterChange(-1);
        }

        private void onClusterChange(int incr)
        {
            incrementIdx(incr);

            updateLabel();

            OnClusterChanged(currentClusterIdx);
        }

        private void updateLabel()
        {
            lblCluster.Text = LabelName + " " + currentClusterIdx;
        }

        private void incrementIdx(int incr)
        {
            currentClusterIdx += incr;
            if (currentClusterIdx == nbClusters)
            {
                currentClusterIdx = 0;
            }
            if (currentClusterIdx < 0)
            {
                currentClusterIdx = nbClusters - 1;
            }
        }

        private void MultiBitmapView_Resize(object sender, EventArgs e)
        {
            Preview.Update();
        }
    }
}

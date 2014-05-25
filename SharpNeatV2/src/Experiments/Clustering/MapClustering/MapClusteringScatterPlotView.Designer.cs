namespace SharpNeat.Experiments.Common
{
    partial class MapClusteringScatterPlotView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPrevDimension = new System.Windows.Forms.Button();
            this.btnNextDimension = new System.Windows.Forms.Button();
            this.lblDimension = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.plotChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.plotChart)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(486, 420);
            this.panel1.TabIndex = 1;
            // 
            // btnPrevDimension
            // 
            this.btnPrevDimension.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrevDimension.Location = new System.Drawing.Point(3, 3);
            this.btnPrevDimension.Name = "btnPrevDimension";
            this.btnPrevDimension.Size = new System.Drawing.Size(54, 43);
            this.btnPrevDimension.TabIndex = 2;
            this.btnPrevDimension.Text = "<";
            this.btnPrevDimension.UseVisualStyleBackColor = true;
            this.btnPrevDimension.Click += new System.EventHandler(this.btnPrevDimension_Click);
            // 
            // btnNextDimension
            // 
            this.btnNextDimension.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNextDimension.Location = new System.Drawing.Point(63, 3);
            this.btnNextDimension.Name = "btnNextDimension";
            this.btnNextDimension.Size = new System.Drawing.Size(54, 43);
            this.btnNextDimension.TabIndex = 3;
            this.btnNextDimension.Text = ">";
            this.btnNextDimension.UseVisualStyleBackColor = true;
            this.btnNextDimension.Click += new System.EventHandler(this.btnNextDimension_Click);
            // 
            // lblDimension
            // 
            this.lblDimension.AutoSize = true;
            this.lblDimension.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDimension.Location = new System.Drawing.Point(123, 10);
            this.lblDimension.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.lblDimension.Name = "lblDimension";
            this.lblDimension.Size = new System.Drawing.Size(21, 23);
            this.lblDimension.TabIndex = 4;
            this.lblDimension.Text = "0";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.btnPrevDimension);
            this.flowLayoutPanel2.Controls.Add(this.btnNextDimension);
            this.flowLayoutPanel2.Controls.Add(this.lblDimension);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(486, 50);
            this.flowLayoutPanel2.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.flowLayoutPanel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.plotChart);
            this.splitContainer1.Size = new System.Drawing.Size(486, 420);
            this.splitContainer1.TabIndex = 6;
            // 
            // plotChart
            // 
            chartArea3.Name = "ChartArea1";
            this.plotChart.ChartAreas.Add(chartArea3);
            this.plotChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend3.Name = "Legend1";
            this.plotChart.Legends.Add(legend3);
            this.plotChart.Location = new System.Drawing.Point(0, 0);
            this.plotChart.Name = "plotChart";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.plotChart.Series.Add(series3);
            this.plotChart.Size = new System.Drawing.Size(486, 366);
            this.plotChart.TabIndex = 0;
            this.plotChart.Text = "chart1";
            // 
            // MapClusteringScatterPlotView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 420);
            this.Controls.Add(this.panel1);
            this.Name = "MapClusteringScatterPlotView";
            this.Text = "ScatterPlotView";
            this.panel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.plotChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button btnPrevDimension;
        private System.Windows.Forms.Button btnNextDimension;
        private System.Windows.Forms.Label lblDimension;
        private System.Windows.Forms.DataVisualization.Charting.Chart plotChart;
    }
}
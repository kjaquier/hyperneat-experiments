﻿namespace SharpNeat.Experiments.Clustering
{
    partial class WindowMapClusteringView
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.inputView = new SharpNeat.Experiments.Common.MultiBitmapView();
            this.outputView = new SharpNeat.Experiments.Common.MultiBitmapView();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.99999F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.00001F));
            this.tableLayoutPanel1.Controls.Add(this.inputView, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.outputView, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(966, 461);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // inputView
            // 
            this.inputView.AutoSize = true;
            this.inputView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.inputView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputView.LabelName = "";
            this.inputView.Location = new System.Drawing.Point(3, 3);
            this.inputView.MinimumSize = new System.Drawing.Size(300, 350);
            this.inputView.Name = "inputView";
            this.inputView.Padding = new System.Windows.Forms.Padding(10);
            this.inputView.Size = new System.Drawing.Size(476, 405);
            this.inputView.TabIndex = 0;
            // 
            // outputView
            // 
            this.outputView.AutoSize = true;
            this.outputView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.outputView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputView.LabelName = "";
            this.outputView.Location = new System.Drawing.Point(485, 3);
            this.outputView.MinimumSize = new System.Drawing.Size(300, 350);
            this.outputView.Name = "outputView";
            this.outputView.Padding = new System.Windows.Forms.Padding(10);
            this.outputView.Size = new System.Drawing.Size(478, 405);
            this.outputView.TabIndex = 1;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 414);
            this.flowLayoutPanel1.MinimumSize = new System.Drawing.Size(0, 50);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(476, 50);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // WindowMapClusteringView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(966, 461);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "WindowMapClusteringView";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private SharpNeat.Experiments.Common.MultiBitmapView inputView;
        private SharpNeat.Experiments.Common.MultiBitmapView outputView;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;

    }
}
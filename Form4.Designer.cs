namespace AracMuayeneOtomasyonu
{
    partial class Form4
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chartSonuc = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartKullaniciArac = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnGeriDon = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chartSonuc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartKullaniciArac)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chartSonuc
            // 
            chartArea1.Name = "ChartArea1";
            this.chartSonuc.ChartAreas.Add(chartArea1);
            this.chartSonuc.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chartSonuc.Legends.Add(legend1);
            this.chartSonuc.Location = new System.Drawing.Point(0, 0);
            this.chartSonuc.Name = "chartSonuc";
            this.chartSonuc.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series1.IsValueShownAsLabel = true;
            series1.Label = "Sonuçlar";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartSonuc.Series.Add(series1);
            this.chartSonuc.Size = new System.Drawing.Size(1032, 689);
            this.chartSonuc.TabIndex = 3;
            this.chartSonuc.Text = "chart1";
            // 
            // chartKullaniciArac
            // 
            chartArea2.Name = "ChartArea1";
            this.chartKullaniciArac.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chartKullaniciArac.Legends.Add(legend2);
            this.chartKullaniciArac.Location = new System.Drawing.Point(1248, 166);
            this.chartKullaniciArac.Name = "chartKullaniciArac";
            this.chartKullaniciArac.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Pastel;
            series2.ChartArea = "ChartArea1";
            series2.IsValueShownAsLabel = true;
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chartKullaniciArac.Series.Add(series2);
            this.chartKullaniciArac.Size = new System.Drawing.Size(957, 689);
            this.chartKullaniciArac.TabIndex = 4;
            this.chartKullaniciArac.Text = "chart1";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2399, 100);
            this.panel1.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(410, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(373, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Detaylı Analiz Raporları";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel2.Controls.Add(this.chartSonuc);
            this.panel2.Location = new System.Drawing.Point(83, 166);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1032, 689);
            this.panel2.TabIndex = 6;
            // 
            // btnGeriDon
            // 
            this.btnGeriDon.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnGeriDon.Location = new System.Drawing.Point(1062, 896);
            this.btnGeriDon.Name = "btnGeriDon";
            this.btnGeriDon.Size = new System.Drawing.Size(274, 111);
            this.btnGeriDon.TabIndex = 7;
            this.btnGeriDon.Text = "Geri Dön";
            this.btnGeriDon.UseVisualStyleBackColor = false;
            this.btnGeriDon.Click += new System.EventHandler(this.btnGeriDon_Click);
            // 
            // Form4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(2399, 1067);
            this.Controls.Add(this.btnGeriDon);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chartKullaniciArac);
            this.Name = "Form4";
            this.Text = "Analiz Ekranı";
            ((System.ComponentModel.ISupportInitialize)(this.chartSonuc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartKullaniciArac)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSonuc;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartKullaniciArac;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnGeriDon;
    }
}
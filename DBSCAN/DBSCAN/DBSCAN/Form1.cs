using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DBSCAN
{
    public partial class Form1 : Form
    {
        string dosya_yolu;

        HamVeriler veriler;
        DBSCANKumeleme kumele;
        DosyaIslemleri di;
        List<Dictionary<int, List<double>>> kumelenmisVeriler;
        Dictionary<int, List<double>> outlier;
        public Form1()
        {
            InitializeComponent();
            label3.Visible = false;
            label4.Visible = false;
            comboBox1.Visible = false;
            comboBox2.Visible = false;
            btnGuncelle.Visible = false;
            btnYaz.Visible = false;
        }

        private void btnDosyaSec_Click(object sender, EventArgs e)
        {
            if (dosyaYolu())
            {
                di = new DosyaIslemleri(dosya_yolu);
                veriler = di.verileriDöndür();
                kumele = new DBSCANKumeleme(veriler);
                kumelenmisVeriler = kumele.kumele(Convert.ToDouble(textBox1.Text), Convert.ToInt32(textBox2.Text));
                outlier = kumele.outlier();
                label3.Visible = true;
                label4.Visible = true;
                comboBox1.Visible = true;
                comboBox2.Visible = true;
                btnGuncelle.Visible = true;
                btnYaz.Visible = true;
                cbDoldur();
                chartDoldur();
            }
            else
                MessageBox.Show("Dosya alırken hata oluştu.");
        }

        public bool dosyaYolu()
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "txt,csv|*.csv;*.txt", ValidateNames = true })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    dosya_yolu = ofd.FileName;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        void cbDoldur()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();

            foreach (var item in veriler.baslik)
            {
                comboBox1.Items.Add(item);
                comboBox2.Items.Add(item);
            }

            if (veriler.baslik.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 1;
            }
        }

        void chartDoldur()
        {
            int index = 0;
            chart1.Series.Clear();
            foreach (var dict in kumelenmisVeriler)
            {
                chart1.Series.Add("K" + index.ToString());
                chart1.Series[index].ChartType = SeriesChartType.Point;
                foreach (var item in dict)
                {
                    chart1.Series[index].Points.Add(new DataPoint(veriler.veriler[item.Key][comboBox1.SelectedIndex], veriler.veriler[item.Key][comboBox2.SelectedIndex]));
                    //chart1.Series[index].Points.Add(new DataPoint(item.Value[comboBox1.SelectedIndex], item.Value[comboBox2.SelectedIndex]));
                }
                index++;

            }

            chart1.Series.Add("Outlier");
            chart1.Series[index].ChartType = SeriesChartType.Point;
            foreach (var item in outlier)
            {
                chart1.Series[index].Points.Add(new DataPoint(veriler.veriler[item.Key][comboBox1.SelectedIndex], veriler.veriler[item.Key][comboBox2.SelectedIndex]));
                //chart1.Series[index].Points.Add(new DataPoint(item.Value[comboBox1.SelectedIndex], item.Value[comboBox2.SelectedIndex]));
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            kumelenmisVeriler = kumele.kumele(Convert.ToDouble(textBox1.Text), Convert.ToInt32(textBox2.Text));
            outlier = kumele.outlier();
            chartDoldur();
        }

        private void chart1_MouseDown(object sender, MouseEventArgs e)
        {
            HitTestResult result = chart1.HitTest(e.X, e.Y);
            if (result.ChartElementType == ChartElementType.DataPoint)
            {
                this.chart1.GetToolTipText += this.chart1_GetToolTipText;
            }
        }

        private void chart1_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            switch (e.HitTestResult.ChartElementType)
            {
                case ChartElementType.DataPoint:
                    var dataPoint = e.HitTestResult.Series.Points[e.HitTestResult.PointIndex];
                    e.Text = string.Format("No: {0}\nX: {1}\nY: {2}", indexBul(dataPoint.XValue, dataPoint.YValues[0]), dataPoint.XValue, dataPoint.YValues[0]);
                    break;
            }
        }

        int indexBul(double x, double y)
        {
            foreach (var item in veriler.veriler)
            {
                if (item.Value[comboBox1.SelectedIndex] == x && item.Value[comboBox2.SelectedIndex] == y)
                    return item.Key;
            }
            return -1;
        }

        private void btnYaz_Click(object sender, EventArgs e)
        {
            DialogResult dr;
            if (di.TXTYaz(false,veriler.baslik, kumelenmisVeriler, outlier, Convert.ToDouble(textBox1.Text), Convert.ToInt32(textBox2.Text)))
                MessageBox.Show("Kurallar dosyaya yazdırıldı.");
            else
            {
                dr = MessageBox.Show("Aynı isimde sonuç dosyası var, üzerine yazdırmak ister misiniz ?", "Dikkat", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                    if (di.TXTYaz(true, veriler.baslik, kumelenmisVeriler, outlier, Convert.ToDouble(textBox1.Text), Convert.ToInt32(textBox2.Text)))
                        MessageBox.Show("Kurallar dosyaya yazdırıldı.");
                    else
                        MessageBox.Show("Bilinmeyen Hata");
            }
        }
    }
}

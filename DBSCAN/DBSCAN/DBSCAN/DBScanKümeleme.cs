using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSCAN
{
    class DBSCANKumeleme
    {
        HamVeriler veriler, normalize;
        List<double> max, min;
        public DBSCANKumeleme(HamVeriler veriler)
        {
            this.veriler = veriler;
        }

        public List<Dictionary<int, List<double>>> kumele(double eps, int minPointNumber)
        {
            //verileri normalize ediyorum
            normalize = normalizasyon();
            List<Dictionary<int, List<double>>> kumelenmis = new List<Dictionary<int, List<double>>>();
            int sayac = 0;
            bool flag;
            //veri sayısı minPointNumberden küçükse çalışmasını engelliyorum
            if (normalize.veriler.Count > minPointNumber)
                flag = true;
            else
                flag = false;

            while (flag)
            {
                Dictionary<int, List<double>> temp = new Dictionary<int, List<double>>();
                temp.Add(normalize.veriler.Keys.ElementAt(sayac), normalize.veriler.Values.ElementAt(sayac));
                normalize.veriler.Remove(normalize.veriler.Keys.ElementAt(sayac));
                //tüm verileri gezerek eps koşuluna uyanları kümeliyorum
                for (int i = 0; i < temp.Count; i++)
                {
                    for (int j = 0; j < normalize.veriler.Count; j++)
                    {
                        if (sayac != j && oklid(temp.Values.ElementAt(i), normalize.veriler.Values.ElementAt(j)) <= eps)
                        {
                            temp.Add(normalize.veriler.Keys.ElementAt(j), normalize.veriler.Values.ElementAt(j));
                            normalize.veriler.Remove(normalize.veriler.Keys.ElementAt(j));
                        }
                    }
                }
                //kümelediğim veriler minPointNumberden büyükse küme listesine ekliyorum
                if (temp.Count > minPointNumber)
                {
                    kumelenmis.Add(temp);
                    sayac = 0;
                }
                else
                {//kümelediğim verileri tekrar eski yerlerine ekliyorum
                    foreach (var item in temp)
                    {
                        normalize.veriler.Add(item.Key,item.Value);
                    }
                    sayac++;
                    if (sayac == normalize.veriler.Count || normalize.veriler.Count == 0)
                    {
                        break;
                    }
                }
            }
            return kumelenmis;
        }

        public Dictionary<int, List<double>> outlier()
        {
            //kalan verileri outlier olarak ayarlıyorum
            Dictionary<int, List<double>> outlier = new Dictionary<int, List<double>>();
            if (normalize.veriler.Count > 0)
            {
                foreach (var item in normalize.veriler)
                {
                    outlier.Add(item.Key,item.Value);
                }
            }
            return outlier;
        }

        HamVeriler normalizasyon()
        {
            maxMin();
            HamVeriler temp = new HamVeriler();
            temp.veriler = new Dictionary<int, List<double>>();
            temp.baslik = veriler.baslik;
            for (int i = 0; i < veriler.veriler.Count; i++)
            {
                List<double> temp2 = new List<double>();

                for (int j = 0; j < veriler.baslik.Count; j++)
                {
                    temp2.Add((veriler.veriler.Values.ElementAt(i)[j] - min[j]) / (max[j] - min[j]));
                }
                temp.veriler.Add(veriler.veriler.Keys.ElementAt(i), temp2);
            }
            return temp;
        }

        void maxMin()
        {
            max = new List<double>();
            min = new List<double>();

            for (int i = 0; i < veriler.baslik.Count; i++)
            {
                double tmax = double.MinValue;
                double tmin = double.MaxValue;
                for (int j = 0; j < veriler.veriler.Count; j++)
                {
                    if (tmax < veriler.veriler.Values.ElementAt(j)[i])
                        tmax = veriler.veriler.Values.ElementAt(j)[i];

                    if (tmin > veriler.veriler.Values.ElementAt(j)[i])
                        tmin = veriler.veriler.Values.ElementAt(j)[i];
                }
                max.Add(tmax);
                min.Add(tmin);
            }
        }

        double oklid(List<double> x1, List<double> x2)
        {
            double sonuc = 0;
            for (int i = 0; i < x1.Count; i++)
            {
                sonuc += Math.Pow(x1[i] - x2[i], 2);
            }
            return Math.Sqrt(sonuc);
        }
    }
}

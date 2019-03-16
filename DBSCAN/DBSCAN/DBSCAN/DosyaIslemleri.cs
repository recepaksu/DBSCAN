using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSCAN
{
    class DosyaIslemleri
    {
        HamVeriler veriler = new HamVeriler();
        string dosya_yolu;

        public DosyaIslemleri(string _dosya_yolu)
        {
            dosya_yolu = _dosya_yolu;
            verileriIsle();
        }

        public HamVeriler verileriDöndür()
        {
            return veriler;
        }

        private void verileriIsle()
        {
            FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);

            string[] gelenVeri = sr.ReadToEnd().Replace("\r", string.Empty).Replace(" ", string.Empty).Split('\n');
            int satırSayısı = gelenVeri.Length, sütunSayısı = gelenVeri[0].Split(',').Length;
            veriler.baslik = new List<string>();
            veriler.veriler = new Dictionary<int, List<double>>();
            foreach (var item in gelenVeri[0].Split(','))
            {
                veriler.baslik.Add(item);
            }

            for (int i = 1; i < satırSayısı; i++)
            {
                List<double> veriTemp = new List<double>();
                string[] temp = gelenVeri[i].Split(',');
                if (temp.Length >= sütunSayısı)
                {
                    for (int j = 0; j < sütunSayısı; j++)
                    {
                        veriTemp.Add(Convert.ToDouble(temp[j]));
                    }
                    veriler.veriler.Add(i, veriTemp);
                }
            }
        }

        public bool TXTYaz(bool flag,List<string> baslik, List<Dictionary<int, List<double>>> kumelenmisVeriler, Dictionary<int, List<double>> outlier,double eps,int minPointNumber)
        {
            try
            {
                string[] seperator = { ".txt", ".csv" };
                string dosya_yolu2 = dosya_yolu.Split(seperator, System.StringSplitOptions.RemoveEmptyEntries)[0] + "_Sonuclar.txt";
                if (!File.Exists(dosya_yolu2) || flag)
                {
                    FileStream fs = new FileStream(dosya_yolu2, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    string baslikText = "";
                    foreach (var item in baslik)
                    {
                        baslikText += item + ";";
                    }
                    baslikText += "Epsilon: " + eps.ToString() + ";MinPointNumber: " + minPointNumber.ToString();
                    sw.WriteLine(baslikText);
                    Dictionary<int, int> kumeList = new Dictionary<int, int>();
                    int index = 1,sayac=0,kume;
                    foreach (var liste in kumelenmisVeriler)
                    {
                        sayac += liste.Count;
                    }
                    sayac += outlier.Count;
                    while(sayac>0)
                    {
                        kume = 1;
                        foreach (var liste in kumelenmisVeriler)
                        {                           
                            foreach (var item in liste)
                            {
                                if(item.Key == index)
                                {
                                    sw.WriteLine("Kayıt " + item.Key.ToString() + ":\t\t" + kume.ToString());
                                    index++;
                                    if (kumeList.Keys.Contains(kume))
                                        kumeList[kume]++;
                                    else
                                        kumeList.Add(kume, 1);
                                }
                            }
                            kume++;
                        }
                        foreach (var item in outlier)
                        {
                            if (item.Key == index)
                            {
                                sw.WriteLine("Kayıt " + item.Key.ToString() + ":\t\tOutlier");
                                index++;
                            }
                        }
                        sayac--;
                    }

                    foreach (var item in kumeList)
                    {
                        sw.WriteLine("Kume " + item.Key.ToString() + ":\t" + item.Value.ToString() + " kayıt");
                    }
                    sw.WriteLine("Outlier: " + outlier.Count.ToString());

                    sw.Flush();
                    sw.Close();
                    fs.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}

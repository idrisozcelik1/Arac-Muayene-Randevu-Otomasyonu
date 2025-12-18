using System;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;  //Chart kontrolü için gerekli kütüphane. Sınavda sakın unutma eklemeyi!

namespace AracMuayeneOtomasyonu
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            // Olayı elle bağlıyoruz (Tasarım hatası almamak için)
            this.Load += new EventHandler(Form4_Load);
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            SonucGrafiginiDoldur();        // 1. Grafik (Kaldı - Geçti - Gelmedi)
            KullaniciAracGrafiginiDoldur(); // 2. Grafik (Kullanıcıların Araç Sayıları)
        }

        // 1. Grafik: Muayene Sonuçları
        void SonucGrafiginiDoldur()
        {
            
            chartSonuc.Series[0].Points.Clear();
            chartSonuc.Series[0].Name = "Sonuclar";

            // #VALX : Dilimin Adı (Örn: Geçti)
            // #VALY : Dilimin Değeri (Örn: 5)
            // Örnek Görüntü: "Geçti (5)" şeklinde olacak.
            chartSonuc.Series[0].Label = "#VALX (#VALY)";
            chartSonuc.Series[0].LegendText = "#VALX"; // Sağdaki renk kutucuklarında da isim yazsın diye yaptık.

            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                try
                {
                    conn.Open();
                    // SQL: Sonuç sütununa göre grupla (Kaç Geçti, Kaç Kaldı?)
                    // Sadece muayenesi bitenleri (Durum='Tamamlandı') alıyoruz.
                    string sql = "SELECT Sonuc, COUNT(*) as Sayi FROM Appointments WHERE Durum='Tamamlandı' GROUP BY Sonuc";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                string sonucIsmi = dr["Sonuc"].ToString(); // Geçti, Kaldı vb.
                                int adet = Convert.ToInt32(dr["Sayi"]);
                                chartSonuc.Series[0].Points.AddXY(sonucIsmi, adet);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Grafik 1 Hatası: " + ex.Message);
                }
            }
        }

        // 2. Grafik: Kullanıcıların Araç Sayıları
        void KullaniciAracGrafiginiDoldur()
        {
            chartKullaniciArac.Series[0].Points.Clear();
            chartKullaniciArac.Series[0].Name = "AracSayilari";

            // ChartArea'ya erişim sağlıyoruz.
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea = chartKullaniciArac.ChartAreas[0];

            // 1. EKSEN ARALIĞI: "1" yaparak "Hiçbir ismi atlama, hepsini yaz" diyoruz.
            chartArea.AxisX.Interval = 1;

            // 2. YAZI AÇISI: İsimler uzunsa yan yana sığmaz. 
            // -90 derece yaparak yazıları DİK yazdırıyoruz ki hepsi sığsın.
            chartArea.AxisX.LabelStyle.Angle = -90;

            // (İsteğe bağlı) Yazı tipi boyutu sığmazsa otomatik küçülsün mü? Hayır, sabit kalsın.
            chartArea.AxisX.IsLabelAutoFit = false;
            
            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                try
                {
                    conn.Open();
                    
                    // 1. "FROM Users" dedik (Önce kullanıcıları al)
                    // 2. "LEFT JOIN Vehicles" dedik (Aracı varsa ekle, yoksa boş geçme)
                    // 3. "COUNT(V.Id)" dedik (Aracı yoksa NULL döner, Count bunu 0 sayar. Yıldız(*) koyarsak 1 sayabilir hatalı olur)

                    string sql = @"SELECT U.Username, COUNT(V.Id) as Sayi 
                           FROM Users U 
                           LEFT JOIN Vehicles V ON U.Id = V.OwnerId 
                           GROUP BY U.Username";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                string isim = dr["Username"].ToString();
                                int sayi = Convert.ToInt32(dr["Sayi"]);

                                chartKullaniciArac.Series[0].Points.AddXY(isim, sayi);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Grafik 2 Hatası: " + ex.Message);
                }
            }
        }

        private void btnGeriDon_Click(object sender, EventArgs e)
        {
            Form3 frmMain = new Form3();
            frmMain.Show();
            this.Close();
        }
    }
}
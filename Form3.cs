using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;  //Olmazsa Olmaz!!!!


namespace AracMuayeneOtomasyonu
{
    public partial class Form3 : Form
    {
        // Giriş yapan kullanıcının ID'sini burada tutacağız.
        int loggedInUserId = 0;
        int guncellenecekAracId = 0;

        public Form3()
        {
            InitializeComponent();
            // Olayı elle bağlıyoruz (Designer hatası almamak için manuel yazdık.)
            this.Load += new EventHandler(Form3_Load);

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // Form2'den gönderilen Kullanıcı ID'sini alıyoruz
            if (this.Tag != null)
            {
                loggedInUserId = Convert.ToInt32(this.Tag);
            }

            // Sayfa yüklendiğinde yapılacak işlemler burada koda hatırlatıyoruz.
            TumKullanicilariGetir();
            KendiAraclarimiComboboxaDoldur();
            RandevulariListele();
            SaatleriDoldur();
            SonucBekleyenleriGetir();
            FiltreListesiniDoldur();
            KusurListesiniDoldur(); 

           
            dtpTarih.Format = DateTimePickerFormat.Short;
            dtpTarih.MinDate = DateTime.Now; // Geçmişe randevu alınamaycağı için bu komutu ekliyoruz.
        }

        // Kullanacağımız tüm fonksiyonlar burada olacak.
        void SaatleriDoldur()
        {
            cmbSaat.Items.Clear();
            
            string[] saatler = { "09:00", "09:30", "10:00", "10:30", "11:00", "11:30",
                         "13:00", "13:30", "14:00", "14:30", "15:00", "15:30", "16:00", "16:30" };
            cmbSaat.Items.AddRange(saatler);
            cmbSaat.SelectedIndex = 0; // İlk saati seçili yap olarak gözükür.
        }
        void TumKullanicilariGetir()
        {
            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                conn.Open();
                string sql = "SELECT Id, Username, NameSurname FROM Users";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvKullanicilar.DataSource = dt;
                }
            }
        }

        void SeciliKullanicininAraclariniGetir(int userId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Vehicles WHERE OwnerId=@id";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvAraclar.DataSource = dt;
                    }
                }
            }
        }

        void KendiAraclarimiComboboxaDoldur()
        {
            // Randevu sekmesindeki ComboBox'a sadece giriş yapan kişinin araçları gelsin.
            cmbAraclar.Items.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                conn.Open();
                string sql = "SELECT Plaka FROM Vehicles WHERE OwnerId=@id";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", loggedInUserId);
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            cmbAraclar.Items.Add(dr["Plaka"].ToString());
                        }
                    }
                }
            }
        }

        void RandevulariListele()
        {
            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                conn.Open();
                // RandevuSaati'ni de çekiyoruz
                string sql = @"SELECT A.Id, V.Plaka, U.NameSurname, A.RandevuTarihi, A.RandevuSaati, A.Durum 
                       FROM Appointments A 
                       JOIN Vehicles V ON A.AracId = V.Id
                       JOIN Users U ON V.OwnerId = U.Id";

                using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvRandevular.DataSource = dt;
                }
            }
        }

        void KusurListesiniDoldur()
        {
            cmbAciklama.Items.Clear();

            // Senin verdiğin standart kusur listesi
            string[] kusurlar = {
                                    "Frenlerin dengesiz veya yetersiz çalışması",
                                    "Lastiklerin aşırı aşınmış, yarık veya diş derinliği yetersiz olması",
                                    "Far ayarlarının bozuk veya çalışmıyor olması",
                                    "Direksiyon sisteminde boşluk veya kritik arıza",
                                    "Emniyet kemerlerinin çalışmaması veya eksik olması",
                                    "Yoğun egzoz emisyonu (yüksek duman veya değerlerin aşılması)",
                                    "Şasi numarasının okunmaması veya tahrip olması",
                                    "Ön veya arka camın kırık olup görüşü engellemesi",
                                    "Fren hortumu, yakıt hortumu veya yağ kaçağı gibi tehlikeli kaçaklar",
                                    "Aracın alt takımında ciddi çürüme, kırık veya bağlantı kopukluğu",
                                    "Diğer (Hafif Kusur)",
                                    "Kusursuz" // Eğer kullanıcı Geçti olarak işaretlerse bu seçeneği kullanabilir. Kullanmazsa da sorun olmaz.
             };
     

            cmbAciklama.Items.AddRange(kusurlar);
        }

        // Tab Page 1: Araç Kayıt İşlemi

        private void btnAracKaydet_Click(object sender, EventArgs e)
        {
            // 1. Boş Alan Kontrolü Yapıyoruz.
            if (string.IsNullOrEmpty(txtPlaka.Text) || string.IsNullOrEmpty(txtMarka.Text) || string.IsNullOrEmpty(txtSasiNo.Text))
            {
                MessageBox.Show("Plaka, Marka ve Şasi No alanları zorunludur!");
                return;
            }

            // Araç Tipi Belirleme Yapıyoruz.
            string tip = "Otomobil";
            if (rbTicari.Checked) tip = "Ticari";
            if (rbMotosiklet.Checked) tip = "Motosiklet";

            // Yakıt Tipi Seçimi Yapıyoruz.
            string yakit = cmbYakit.Text;

            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Vehicles (Plaka, SasiNo, Marka, Model, AracTipi, YakitTipi, OwnerId) 
                               VALUES (@plaka, @sasi, @marka, @model, @tip, @yakit, @owner)";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@plaka", txtPlaka.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@sasi", txtSasiNo.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@marka", txtMarka.Text);
                    cmd.Parameters.AddWithValue("@model", txtModel.Text);
                    cmd.Parameters.AddWithValue("@tip", tip);
                    cmd.Parameters.AddWithValue("@yakit", yakit);
                    cmd.Parameters.AddWithValue("@owner", loggedInUserId); // Aracı giriş yapan kullanıcıya kaydediyoruz.

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Araç başarıyla kaydedildi!");

                    KendiAraclarimiComboboxaDoldur();
                    
                    txtPlaka.Clear(); txtMarka.Clear(); txtModel.Clear(); txtSasiNo.Clear();
                }
            }
        }

        // Kullanıcı Listesinden (Sol Alt) Biri Seçilince veya Butona Basınca
        private void btnListele_Click(object sender, EventArgs e)
        {
            if (dgvKullanicilar.SelectedRows.Count > 0)
            {
                // Seçili satırdaki ID'yi al (Hücre 0 genelde ID'dir)
                int secilenUserId = Convert.ToInt32(dgvKullanicilar.SelectedRows[0].Cells["Id"].Value);
                SeciliKullanicininAraclariniGetir(secilenUserId);
            }
            else
            {
                MessageBox.Show("Lütfen listeden bir kullanıcı seçin.");
            }
        }

        // Tab Page 2: Randevu Yönetimi

        private void btnRandevuOlustur_Click(object sender, EventArgs e)
        {
            if (cmbAraclar.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen araç seçin.");
                return;
            }

            string secilenPlaka = cmbAraclar.Text;
            DateTime secilenTarih = dtpTarih.Value.Date;
            string secilenSaat = cmbSaat.Text; 

            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                conn.Open();

                // KURAL 1: Aynı gün ve aynı saate başka randevu var mı? (Çakışma Kontrolü)
                string saatKontrol = "SELECT Count(*) FROM Appointments WHERE RandevuTarihi=@tarih AND RandevuSaati=@saat";
                using (SQLiteCommand cmdSaat = new SQLiteCommand(saatKontrol, conn))
                {
                    cmdSaat.Parameters.AddWithValue("@tarih", secilenTarih);
                    cmdSaat.Parameters.AddWithValue("@saat", secilenSaat);
                    int cakisma = Convert.ToInt32(cmdSaat.ExecuteScalar());
                    if (cakisma > 0)
                    {
                        MessageBox.Show("Seçilen tarih ve saatte zaten dolu! Lütfen başka saat seçin.", "Dolu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // KURAL 2: Günde Max 3 Randevu Alınabilir.
                string gunKontrol = "SELECT Count(*) FROM Appointments WHERE RandevuTarihi = @tarih";
                using (SQLiteCommand cmdGun = new SQLiteCommand(gunKontrol, conn))
                {
                    cmdGun.Parameters.AddWithValue("@tarih", secilenTarih);
                    int sayi = Convert.ToInt32(cmdGun.ExecuteScalar());
                    if (sayi >= 3)
                    {
                        MessageBox.Show("Bu tarihteki randevu kotası dolmuştur (Max 3).", "Uyarı");
                        return;
                    }
                }

                // Araç ID bul
                int aracId = 0;
                string idBulSql = "SELECT Id FROM Vehicles WHERE Plaka=@plaka";
                using (SQLiteCommand cmdId = new SQLiteCommand(idBulSql, conn))
                {
                    cmdId.Parameters.AddWithValue("@plaka", secilenPlaka);
                    object result = cmdId.ExecuteScalar();
                    if (result != null) aracId = Convert.ToInt32(result);
                }

                string kayitSql = "INSERT INTO Appointments (AracId, RandevuTarihi, RandevuSaati, Durum) VALUES (@aracId, @tarih, @saat, 'Aktif')";
                using (SQLiteCommand cmdKayit = new SQLiteCommand(kayitSql, conn))
                {
                    cmdKayit.Parameters.AddWithValue("@aracId", aracId);
                    cmdKayit.Parameters.AddWithValue("@tarih", secilenTarih);
                    cmdKayit.Parameters.AddWithValue("@saat", secilenSaat);
                    cmdKayit.ExecuteNonQuery();

                    MessageBox.Show("Randevu oluşturuldu!");
                    RandevulariListele();
                }
            }
        }

        private void btnRandevuListele_Click(object sender, EventArgs e)
        {
            RandevulariListele();

            MessageBox.Show("Randevu listesi güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnRandevuIptal_Click(object sender, EventArgs e)
        {
            if (dgvRandevular.SelectedRows.Count > 0)
            {
                int randevuId = Convert.ToInt32(dgvRandevular.SelectedRows[0].Cells["Id"].Value);

                using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
                {
                    conn.Open();
                    string sql = "DELETE FROM Appointments WHERE Id=@id";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", randevuId);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Randevu iptal edildi.");
                RandevulariListele();
            }
            else
            {
                MessageBox.Show("İptal edilecek randevuyu listeden seçin.");
            }
        }

        // Analiz sayfasına gitmek için gerekli buton kodları
        private void btnAnalizGit_Click(object sender, EventArgs e)
        {
            Form4 frmAnaliz = new Form4();
            frmAnaliz.Show();
            this.Hide();
        }

        private void btnRandevuGuncelle_Click(object sender, EventArgs e)
        {
            // 1. Listeden bir şey seçili mi?
            if (dgvRandevular.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen güncellemek istediğiniz randevuyu listeden seçin.", "Seçim Yapın");
                return;
            }

            // 2. Seçili ID'yi al
            int randevuId = Convert.ToInt32(dgvRandevular.SelectedRows[0].Cells["Id"].Value);

            // 3. Yeni verileri al
            DateTime yeniTarih = dtpTarih.Value.Date;
            string yeniSaat = cmbSaat.Text;

            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                conn.Open();

                string sql = "UPDATE Appointments SET RandevuTarihi=@tarih, RandevuSaati=@saat WHERE Id=@id";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@tarih", yeniTarih);
                    cmd.Parameters.AddWithValue("@saat", yeniSaat);
                    cmd.Parameters.AddWithValue("@id", randevuId);

                    int etki = cmd.ExecuteNonQuery();

                    if (etki > 0)
                    {
                        MessageBox.Show("Randevu başarıyla güncellendi!");
                        RandevulariListele(); 
                    }
                    else
                    {
                        MessageBox.Show("Güncelleme başarısız oldu.");
                    }
                }
            }
        }

        private void btnSonucKaydet_Click(object sender, EventArgs e)
        {
            // 1. Listeden seçim kontrolü
            if (dgvSonucBekleyenler.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen listeden bir randevu seçiniz.");
                return;
            }

            // 2. Sonuç seçimi kontrolü
            if (cmbSonuc.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen sonuç (Geçti/Kaldı) seçiniz.");
                return;
            }

            int randevuId = Convert.ToInt32(dgvSonucBekleyenler.SelectedRows[0].Cells["Id"].Value);
            string sonuc = cmbSonuc.Text;

            
            string aciklama = cmbAciklama.Text;

            // "Kaldı" seçildiyse ve kusur seçilmediyse uyarı ver
            if (sonuc == "Kaldı" && (string.IsNullOrEmpty(aciklama) || aciklama == "Kusursuz"))
            {
                MessageBox.Show("Araç kaldıysa listeden geçerli bir kusur nedeni seçmelisiniz!", "Eksik Bilgi");
                return;
            }

            // Eğer "Geçti" seçildiyse ve açıklama boşsa otomatik "Kusursuz" yazdıralım.
            if (sonuc == "Geçti" && string.IsNullOrEmpty(aciklama))
            {
                aciklama = "Kusursuz";
            }

            
            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                conn.Open();
                string sql = "UPDATE Appointments SET Durum='Tamamlandı', Sonuc=@sonuc, Aciklama=@aciklama WHERE Id=@id";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@sonuc", sonuc);
                    cmd.Parameters.AddWithValue("@aciklama", aciklama);
                    cmd.Parameters.AddWithValue("@id", randevuId);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Muayene sonucu kaydedildi.");

                    SonucBekleyenleriGetir();
                    cmbSonuc.SelectedIndex = -1;
                    cmbAciklama.SelectedIndex = -1; 
                }
            }
        }

        void GecmisMuayeneleriGetir(string arananPlaka = "")
        {
            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                try
                {
                    conn.Open();

                   
                    string sql = @"SELECT 
                            U.NameSurname AS 'Araç Sahibi',
                            U.Telefon AS 'Telefon',
                            V.Plaka AS 'Plaka',
                            V.Marka || ' ' || V.Model AS 'Araç Bilgisi',
                            A.RandevuTarihi AS 'Muayene Tarihi',
                            A.Sonuc AS 'Sonuç',
                            A.Aciklama AS 'Kusur/Açıklama'
                           FROM Appointments A 
                           JOIN Vehicles V ON A.AracId = V.Id
                           JOIN Users U ON V.OwnerId = U.Id
                           WHERE A.Durum = 'Tamamlandı'";

                    // Eğer arama kutusuna bir şey yazıldıysa filtrele
                    if (!string.IsNullOrEmpty(arananPlaka))
                    {
                        sql += " AND V.Plaka LIKE @plaka";
                    }

                    // Sıralama: En son yapılan işlem en üstte görünsün
                    sql += " ORDER BY A.RandevuTarihi DESC";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        if (!string.IsNullOrEmpty(arananPlaka))
                        {
                            cmd.Parameters.AddWithValue("@plaka", "%" + arananPlaka + "%");
                        }

                        using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dgvGecmisIslemler.DataSource = dt;

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Geçmiş getirilirken hata: " + ex.Message);
                }
            }
        }

      
        void SonucBekleyenleriGetir()
        {
            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                try
                {
                    conn.Open();
                    // Kullanıcının Telefonunu (U.Telefon) ve Adını da çekiyoruz
                    string sql = @"SELECT A.Id, V.Plaka, U.NameSurname, U.Telefon, A.RandevuTarihi, A.RandevuSaati 
                           FROM Appointments A 
                           JOIN Vehicles V ON A.AracId = V.Id
                           JOIN Users U ON V.OwnerId = U.Id
                           WHERE A.Durum = 'Aktif'"; // Sadece henüz sonucu girilmemişleri getiriyoruz.

                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvSonucBekleyenler.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Liste getirilirken hata oluştu: " + ex.Message);
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2)
            {
                SonucBekleyenleriGetir(); // Listeyi veritabanından tekrar çekiyoruz.
            }

           
            if (tabControl1.SelectedIndex == 1)
            {
                RandevulariListele();
                KendiAraclarimiComboboxaDoldur(); // Yeni araç eklediysen o da gelsin
            }

            if (tabControl1.SelectedIndex == 3)
            {
                if (tabControl1.SelectedIndex == 3)
                {
                    RaporPlakalariniDoldur(); // Önce plakaları doldur 
                    GecmisMuayeneleriGetir(); // Sonra tüm listeyi getir
                }
            }
        }

        private void btnRaporYenile_Click(object sender, EventArgs e)
        {
            string secilenPlaka = cmbRaporPlaka.Text;

            if (secilenPlaka == "Tümü" || string.IsNullOrEmpty(secilenPlaka))
            {
               
                GecmisMuayeneleriGetir("");
            }
            else
            {
               
                GecmisMuayeneleriGetir(secilenPlaka);
            }
        }

        private void btnAracSil_Click(object sender, EventArgs e)
        {
            // 1. Seçim Kontrolü
            if (dgvAraclar.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silinecek aracı listeden seçin.");
                return;
            }

            // Seçilen aracın ID ve Plaka bilgisini al
            int aracId = Convert.ToInt32(dgvAraclar.SelectedRows[0].Cells["Id"].Value);
            string plaka = dgvAraclar.SelectedRows[0].Cells["Plaka"].Value.ToString();

            // 2. Onay İsteme (Güvenlik)
            DialogResult cevap = MessageBox.Show(plaka + " plakalı aracı ve tüm randevularını silmek istiyor musunuz?",
                                                 "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (cevap == DialogResult.Yes)
            {
                using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
                {
                    conn.Open();
                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            
                            string sqlRandevuSil = "DELETE FROM Appointments WHERE AracId=@id";
                            using (SQLiteCommand cmd1 = new SQLiteCommand(sqlRandevuSil, conn))
                            {
                                cmd1.Parameters.AddWithValue("@id", aracId);
                                cmd1.ExecuteNonQuery();
                            }

                            
                            string sqlAracSil = "DELETE FROM Vehicles WHERE Id=@id";
                            using (SQLiteCommand cmd2 = new SQLiteCommand(sqlAracSil, conn))
                            {
                                cmd2.Parameters.AddWithValue("@id", aracId);
                                cmd2.ExecuteNonQuery();
                            }

                            
                            transaction.Commit();
                            MessageBox.Show("Araç ve geçmişi başarıyla silindi.");

                            
                            KendiAraclarimiComboboxaDoldur(); // Combobox güncellensin

                            
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // Hata olursa işlemi geri al
                            MessageBox.Show("Silme hatası: " + ex.Message);
                        }
                    }
                }
            }
        }

        void RaporPlakalariniDoldur()
        {
            cmbRaporPlaka.Items.Clear();
            cmbRaporPlaka.Items.Add("Tümü"); // En başa "Tümü" seçeneği koyalım

            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                try
                {
                    conn.Open();
                    
                    string sql = @"SELECT DISTINCT V.Plaka 
                           FROM Appointments A 
                           JOIN Vehicles V ON A.AracId = V.Id 
                           WHERE A.Durum = 'Tamamlandı'
                           ORDER BY V.Plaka ASC";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                cmbRaporPlaka.Items.Add(dr["Plaka"].ToString());
                            }
                        }
                    }
                }
                catch { }
            }
        }

        void FiltreListesiniDoldur()
        {
            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                try
                {
                    conn.Open();
                    // Tüm kullanıcıları çek (Adı ve ID'si lazım)
                    string sql = "SELECT Id, NameSurname FROM Users ORDER BY NameSurname ASC";

                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // ComboBox'a bağla
                        cmbSilinecekUye.DataSource = dt;
                        cmbSilinecekUye.DisplayMember = "NameSurname"; // Kutuda görünecek isim
                        cmbSilinecekUye.ValueMember = "Id";           // Arka plandaki kimlik no

                        // Başlangıçta kimse seçili olmasın
                        cmbSilinecekUye.SelectedIndex = -1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Üye listesi yüklenemedi: " + ex.Message);
                }
            }
        }

        void KullaniciyiFiltrele(int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                try
                {
                    conn.Open();
                    // Sadece ID'si eşleşen kullanıcıyı getir
                    string sql = "SELECT Id, Username, NameSurname FROM Users WHERE Id=@id";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dgvKullanicilar.DataSource = dt; // Tabloyu sadece bu kişiyle güncelle
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Filtreleme hatası: " + ex.Message);
                }
            }
        }

        private void btnKullaniciSil_Click(object sender, EventArgs e)
        {
            // 1. Seçim Kontrolü
            if (dgvKullanicilar.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silinecek kullanıcıyı seçin.");
                return;
            }

            // Seçilen kullanıcının ID ve Adını al
            int userId = Convert.ToInt32(dgvKullanicilar.SelectedRows[0].Cells["Id"].Value);
            string adSoyad = dgvKullanicilar.SelectedRows[0].Cells["NameSurname"].Value.ToString();

            // Kendini silmeye çalışırsa engelle (Opsiyonel güvenlik)
            if (userId == loggedInUserId)
            {
                MessageBox.Show("Kendi hesabınızı buradan silemezsiniz!", "Dur Yolcu");
                return;
            }

            // 2. Onay İsteme
            DialogResult cevap = MessageBox.Show(adSoyad + " isimli üyeyi SİLMEK üzeresiniz.\n\n" +
                                                 "Bu işlem şunları da silecektir:\n" +
                                                 "- Üyenin tüm araçları\n" +
                                                 "- Araçların tüm randevuları\n\n" +
                                                 "Devam edilsin mi?",
                                                 "Kritik Silme İşlemi", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);

            if (cevap == DialogResult.Yes)
            {
                using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
                {
                    conn.Open();
                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // A. Kullanıcının araçlarına ait RANDEVULARI sil
                            
                            string sqlRandevuSil = @"DELETE FROM Appointments 
                                             WHERE AracId IN (SELECT Id FROM Vehicles WHERE OwnerId = @uid)";
                            using (SQLiteCommand cmd1 = new SQLiteCommand(sqlRandevuSil, conn))
                            {
                                cmd1.Parameters.AddWithValue("@uid", userId);
                                cmd1.ExecuteNonQuery();
                            }

                            // B. Kullanıcının ARAÇLARINI sil
                            string sqlAracSil = "DELETE FROM Vehicles WHERE OwnerId = @uid";
                            using (SQLiteCommand cmd2 = new SQLiteCommand(sqlAracSil, conn))
                            {
                                cmd2.Parameters.AddWithValue("@uid", userId);
                                cmd2.ExecuteNonQuery();
                            }

                            // C. KULLANICIYI sil
                            string sqlUserSil = "DELETE FROM Users WHERE Id = @uid";
                            using (SQLiteCommand cmd3 = new SQLiteCommand(sqlUserSil, conn))
                            {
                                cmd3.Parameters.AddWithValue("@uid", userId);
                                cmd3.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            MessageBox.Show("Kullanıcı ve tüm verileri sistemden silindi.");

                            // Listeleri Yenile
                            TumKullanicilariGetir(); // Sol alttaki listeyi güncelle

                            // Sağdaki liste silinen kişiye aitse temizle
                            dgvAraclar.DataSource = null;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Silme işlemi başarısız: " + ex.Message);
                        }
                    }
                }
            }
        }

        private void cmbSilinecekUye_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Eğer geçerli bir seçim yapıldıysa
            if (cmbSilinecekUye.SelectedIndex != -1 && cmbSilinecekUye.SelectedValue != null)
            {
                try
                {
                    // Seçilen kişinin ID'sini al ve filtrele
                    if (int.TryParse(cmbSilinecekUye.SelectedValue.ToString(), out int secilenId))
                    {
                        KullaniciyiFiltrele(secilenId);
                    }
                }
                catch { }
            }
            else
            {
                // Kutu boşaltılırsa herkesi geri getir
                TumKullanicilariGetir();
            }
        }

        private void dgvAraclar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Başlıklara tıklarsa hata vermesin
            if (e.RowIndex < 0) return;

            // 1. Seçilen satırdaki bilgileri al
            DataGridViewRow row = dgvAraclar.Rows[e.RowIndex];

            // 2. Hafızaya ID'yi kaydet (Güncelleme butonu bunu kullanacak)
            guncellenecekAracId = Convert.ToInt32(row.Cells["Id"].Value);

            // 3. Bilgileri soldaki kutulara doldur
            txtPlaka.Text = row.Cells["Plaka"].Value.ToString();
            txtSasiNo.Text = row.Cells["SasiNo"].Value.ToString();
            txtMarka.Text = row.Cells["Marka"].Value.ToString();
            txtModel.Text = row.Cells["Model"].Value.ToString();

            // Araç Tipini Seç (RadioButton)
            string tip = row.Cells["AracTipi"].Value.ToString();
            if (tip == "Otomobil") rbOtomobil.Checked = true;
            else if (tip == "Ticari") rbTicari.Checked = true;
            else if (tip == "Motosiklet") rbMotosiklet.Checked = true;

            // Yakıt Tipini Seç (ComboBox)
            cmbYakit.Text = row.Cells["YakitTipi"].Value.ToString();
        }

        private void btnAracGuncelle_Click(object sender, EventArgs e)
        {
            // 1. Güvenlik Kontrolü: Bir araç seçilmiş mi? diye kontrol ediyoruz.
            if (guncellenecekAracId == 0)
            {
                MessageBox.Show("Lütfen önce listeden güncellenecek aracı seçin!");
                return;
            }

            // 2. Kutular boş mu kontrolü (Basitçe plaka kontrolü)
            if (txtPlaka.Text == "" || txtSasiNo.Text == "")
            {
                MessageBox.Show("Plaka ve Şasi numarası boş olamaz.");
                return;
            }

            // 3. Veritabanı Güncelleme İşlemi
            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = @"UPDATE Vehicles 
                           SET Plaka=@p1, SasiNo=@p2, Marka=@p3, Model=@p4, AracTipi=@p5, YakitTipi=@p6 
                           WHERE Id=@id";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        // Parametreleri kutulardan al
                        cmd.Parameters.AddWithValue("@p1", txtPlaka.Text);
                        cmd.Parameters.AddWithValue("@p2", txtSasiNo.Text);
                        cmd.Parameters.AddWithValue("@p3", txtMarka.Text);
                        cmd.Parameters.AddWithValue("@p4", txtModel.Text);

                        // Araç Tipi Belirleme
                        string aracTipi = "Otomobil";
                        if (rbTicari.Checked) aracTipi = "Ticari";
                        if (rbMotosiklet.Checked) aracTipi = "Motosiklet";
                        cmd.Parameters.AddWithValue("@p5", aracTipi);

                        cmd.Parameters.AddWithValue("@p6", cmbYakit.Text);

                        // Hangi araç güncellenecek? (Hafızadaki ID)
                        cmd.Parameters.AddWithValue("@id", guncellenecekAracId);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Araç bilgileri başarıyla güncellendi.");

                    // 4. Temizlik ve Yenileme
                    Temizle(); // Kutuları boşalt
                    guncellenecekAracId = 0; // Hafızayı sıfırla

                    // Eğer bir kullanıcı filtrelenmişse listeyi ona göre yenilememiz lazım
                    // Şimdilik genel listeyi yenileyelim, filtre varsa o bozulabilir ama veri güncel olur.
                    if (dgvAraclar.DataSource != null)
                    {
                        // Eğer sol alttan bir kişi seçiliyse onun araçlarını tekrar yükle
                        if (dgvKullanicilar.SelectedRows.Count > 0)
                        {
                            // Mevcut "Seçili Kişinin Araçlarını Listele" butonuna basılmış gibi yap
                            // Veya direkt o kodu çağır. En kolayı arayüzü resetlemektir:
                        }
                    }

                    // Listeyi yenilemek için basit bir yöntem:
                    // Eğer "Tüm Araçlar" veya "Filtreli Araçlar" açıksa yenileme mantığı karmaşık olabilir.
                    // En garantisi, sağdaki listeyi temizleyip kullanıcıyı tekrar seçmeye zorlamaktır:
                    dgvAraclar.DataSource = null;
                    MessageBox.Show("Listeyi görmek için lütfen kullanıcıyı tekrar seçiniz.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Güncelleme Hatası: " + ex.Message);
                }
            }
        }

        // Kutuları temizleyen küçük bir yardımcı metot (Eğer yoksa ekle)
        void Temizle()
        {
            txtPlaka.Clear();
            txtSasiNo.Clear();
            txtMarka.Clear();
            txtModel.Clear();
            rbOtomobil.Checked = true;
            cmbYakit.SelectedIndex = -1;
        }
    }
}
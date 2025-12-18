using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;


namespace AracMuayeneOtomasyonu
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtLoginUser.Text) || string.IsNullOrEmpty(txtLoginPass.Text))
            {
                MessageBox.Show("Kullanıcı adı ve şifre boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                try
                {
                    conn.Open();
                    // Kullanıcıyı sorguluyoruz.
                    string sql = "SELECT * FROM Users WHERE Username=@usr AND Password=@pass";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@usr", txtLoginUser.Text.Trim());
                        cmd.Parameters.AddWithValue("@pass", txtLoginPass.Text.Trim());

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Giriş Başarılı!
                               
                                int userId = Convert.ToInt32(reader["Id"]); //Araç kaydedilirken kullanıcı ID'sine ihtiyacımız olacak.
                                string nameSurname = reader["NameSurname"].ToString();

                                MessageBox.Show($"Hoşgeldiniz Sayın {nameSurname}, giriş başarılı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                Form3 frmMain = new Form3();
                                frmMain.Tag = userId; // Kullanıcı ID'sini Form3'e "Tag" özelliği ile taşıyoruz (Pratik Yöntem).
                                frmMain.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Kullanıcı adı veya şifre hatalı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bağlantı hatası: " + ex.Message);
                }
            }
        }
        // Kayıt formunu açma butonu.
        private void btnOpenRegister_Click(object sender, EventArgs e)
        {
            Form1 frmRegister = new Form1();
            frmRegister.Show();
            this.Hide();
        }

        // Form kapandığında uygulama tamamen kapansın
        
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}

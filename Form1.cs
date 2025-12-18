using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace AracMuayeneOtomasyonu
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Eğer kayıt olurken boş alan bırakırsak uyarı mesajı alıyoruz.
            if (string.IsNullOrEmpty(txtRegUser.Text) ||
                string.IsNullOrEmpty(txtRegPass.Text) ||
                string.IsNullOrEmpty(txtRegName.Text) ||
                !mskRegTelefon.MaskCompleted) // Telefon tam girildi mi?
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!", "Uyarı");
                return;
            }

            using (SQLiteConnection conn = new SQLiteConnection(SqlHelper.connectionString))
            {
                try
                {
                    conn.Open();

                    // Eğer girdiğimiz kullanıcı adı zaten varsa uyarı veriyoruz.
                    string checkUser = "SELECT Count(*) FROM Users WHERE Username=@usr";
                    using (SQLiteCommand checkCmd = new SQLiteCommand(checkUser, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@usr", txtRegUser.Text.Trim());
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show("Bu kullanıcı adı alınmış.", "Hata");
                            return;
                        }
                    }

                    // Kayıt işlemi yapıyoruz.
                    string sql = "INSERT INTO Users (Username, Password, NameSurname, Telefon) VALUES (@usr, @pass, @name, @tel)";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@usr", txtRegUser.Text.Trim()); // Trim() girilen değerin başındaki ve sonundaki gereksiz boşlukları siler.
                        cmd.Parameters.AddWithValue("@pass", txtRegPass.Text.Trim());
                        cmd.Parameters.AddWithValue("@name", txtRegName.Text.Trim());
                        cmd.Parameters.AddWithValue("@tel", mskRegTelefon.Text); 

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Başarıyla Üye Oldunuz! Giriş yapabilirsiniz.");

                    // Önceki alanları temizleyip giriş formuna geçiyoruz.
                    txtRegUser.Clear(); txtRegPass.Clear(); txtRegName.Clear(); mskRegTelefon.Clear();
                    Form2 frmLogin = new Form2();
                    frmLogin.Show();
                    this.Hide();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

  
        private void btnGoToLogin_Click(object sender, EventArgs e)
        {
            Form2 frmLogin = new Form2();
            frmLogin.Show();
            this.Hide();
        }

    }
  }


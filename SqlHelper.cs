using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace AracMuayeneOtomasyonu
{
    public class SqlHelper
    {
        // Muayene database dosya adı
        private static string dbName = "MuayeneDB.db";
        // Bağlantı cümlemiz (Connection String)
        public static string connectionString = "Data Source=MuayeneDB.db;Version=3;";

        // Bu metot proje ilk çalıştığında veritabanı dosyasını ve tabloları oluşturacak
        public static void CreateDatabase()
        {
            if (!File.Exists(dbName))
            {
                SQLiteConnection.CreateFile(dbName);

                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    // Kullanıcılar Tablosu 1.Tablo
                    string sqlUsers = @"CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT UNIQUE,
                    Password TEXT,
                    NameSurname TEXT,
                    Telefon TEXT 
                    )";

                    // Araçlar Tablosu 2.Tablo
                    string sqlVehicles = @"CREATE TABLE IF NOT EXISTS Vehicles (
                       Id INTEGER PRIMARY KEY AUTOINCREMENT,
                       Plaka TEXT,
                       SasiNo TEXT,
                       Marka TEXT,
                       Model TEXT,
                       AracTipi TEXT,
                       YakitTipi TEXT,
                       OwnerId INTEGER,
                       FOREIGN KEY(OwnerId) REFERENCES Users(Id)
                       )";

                    // Randevular Tablosu 3.Tablo
                    string sqlAppointments = @"CREATE TABLE IF NOT EXISTS Appointments (
                           Id INTEGER PRIMARY KEY AUTOINCREMENT,
                           AracId INTEGER,
                           RandevuTarihi DATETIME,
                           RandevuSaati TEXT,
                           Durum TEXT,
                           Sonuc TEXT,
                           Aciklama TEXT,
                           FOREIGN KEY(AracId) REFERENCES Vehicles(Id)
                           )";

                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = sqlUsers;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = sqlVehicles;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = sqlAppointments;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
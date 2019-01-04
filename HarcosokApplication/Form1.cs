using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace HarcosokApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CreateTables();
            AdatKiir();



        }


        public void CreateTables()
        {



            using (var conn = new MySqlConnection(
                "Server=localhost;Database=cs_harcosok;Uid=root;Password=;"))
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = @"CREATE TABLE IF NOT EXISTS harcosok(
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                nev VARCHAR(128) NOT NULL,
                letrehozas DATE NOT NULL
                );";
                var command2 = conn.CreateCommand();
                command2.CommandText = @"CREATE TABLE IF NOT EXISTS kepessegek(
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                nev VARCHAR(128) NOT NULL,
                leiras VARCHAR(128) NOT NULL,
                FOREIGN KEY (harcos_id) REFERENCES harcosok(id)
                );";

            }

        }
        void AdatKiir()
        {
            using (var conn = new MySqlConnection(
                  "Server=localhost;Database=cs_harcosok;Uid=root;Password=;"))
            {
                conn.Open();
                harcosokListBox.Items.Clear();
                string formatString = "{0} létrehozás:{1}";

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
            SELECT * FROM harcosok";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    harcosokListBox.Items.Add(string.Format(formatString, reader["nev"], reader["letrehozas"]));
                    hasznaloComboBox.Items.Add(string.Format("{0}", reader["nev"]));
                }
            }
        }

        private void letrehozButton_Click(object sender, EventArgs e)
        {

            string nev = harcosNevetextBox.Text;
            DateTime letrehozas = DateTime.Now;
            if (nev != null)
            {
                using (var conn = new MySqlConnection(
                    "Server=localhost;Database=cs_harcosok;Uid=root;Password=;"))
                {
                    conn.Open();

                    var ellenorzes = conn.CreateCommand();
                    ellenorzes.CommandText = "SELECT COUNT(*) FROM harcosok WHERE nev = @nev";
                    ellenorzes.Parameters.AddWithValue("@nev", nev);
                    var darab = (long)ellenorzes.ExecuteScalar();
                    if (darab != 0)
                    {
                        MessageBox.Show("A usernev mar letezik");
                        return;
                    }

                    var command = conn.CreateCommand();
                    command.CommandText = "INSERT INTO harcosok (nev, letrehozas) VALUES (@nev, @letrehozas)";
                    command.Parameters.AddWithValue("@nev", nev);
                    command.Parameters.AddWithValue("@letrehozas", letrehozas);
                    int erintettSorok = command.ExecuteNonQuery();
                }
            }
            else
            {
                MessageBox.Show("Nem adtál meg nevet!");
            }
            AdatKiir();

        }

        private void hozzaadButton_Click(object sender, EventArgs e)
        {

            string nev = kepessegNeveTextBox.Text;
            string leiras = leirasTextBox.Text;
            string harcosid = harcosNumericUpDown1.Value.ToString();

            if (nev != null || leiras != null)
            {
                using (var conn = new MySqlConnection(
                    "Server=localhost;Database=cs_harcosok;Uid=root;Password=;"))
                {
                    conn.Open();

                    var ellenorzes = conn.CreateCommand();
                    ellenorzes.CommandText = "SELECT COUNT(*) FROM kepessegek WHERE nev = @nev";
                    ellenorzes.Parameters.AddWithValue("@nev", nev);
                    var darab = (long)ellenorzes.ExecuteScalar();
                    if (darab != 0)
                    {
                        MessageBox.Show("Ez a képesség már létezik");
                        return;
                    }

                    var command = conn.CreateCommand();
                    command.CommandText = "INSERT INTO kepessegek (nev, leiras,harcos_id) VALUES (@nev,@leiras,@harcos_id)";
                    command.Parameters.AddWithValue("@nev", nev);
                    command.Parameters.AddWithValue("@leiras", leiras);
                    command.Parameters.AddWithValue("@harcos_id", harcosid);
                    int erintettSorok = command.ExecuteNonQuery();
                }
            }
            else
            {
                MessageBox.Show("Nem adtál meg minden adatot!");
            }
            AdatKiir();
        }

        private void harcosokListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            kepessegekListBox.Items.Clear();
            kepessegLeirasaTextBox.Clear();
            string nev = harcosokListBox.SelectedItem.ToString();
            string[] asd = nev.Split(' ');

            using (var conn = new MySqlConnection(
                     "Server=localhost;Database=cs_harcosok;Uid=root;Password=;"))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                SELECT kepessegek.nev
                FROM kepessegek
                INNER JOIN harcosok ON kepessegek.harcos_id = harcosok.id
                WHERE harcosok.nev = @nev";
                cmd.Parameters.AddWithValue("@nev", asd[0]);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    kepessegekListBox.Items.Add(string.Format("{0}", reader["nev"]));
                }


            }
        }

        private void kepessegekListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            kepessegLeirasaTextBox.Clear();
            string nev = kepessegekListBox.SelectedItem.ToString();


            using (var conn = new MySqlConnection(
                     "Server=localhost;Database=cs_harcosok;Uid=root;Password=;"))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                SELECT leiras
                FROM kepessegek
                WHERE nev = @nev";
                cmd.Parameters.AddWithValue("@nev", nev);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    kepessegLeirasaTextBox.Text = string.Format("{0}", reader["leiras"]);

                }


            }
        }

        private void torolButton_Click(object sender, EventArgs e)
        {
            
          
            

            if (kepessegekListBox.SelectedItem.ToString() != null)
            {
                using (var conn = new MySqlConnection(
                             "Server=localhost;Database=cs_harcosok;Uid=root;Password=;"))
                {
                    conn.Open();

                    
                    var command = conn.CreateCommand();
                    command.CommandText = "DELETE FROM kepessegek WHERE nev = @nev";

                    command.Parameters.AddWithValue("@nev", kepessegekListBox.SelectedItem.ToString());

                    int erintettSorok = command.ExecuteNonQuery();
                }
            }
            else
            {
                MessageBox.Show("Nem választottál képességet!");
            }
            AdatKiir();
        }

      
     
    }
    
}

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace adatbazisAlapok
{
    class Program
    {
        static void Main(string[] args)
        {

            using (var conn = new SQLiteConnection("Data Source=mydb.db"))//fájlkiterjesztésnek akár sqlite, vagy sqlite3|:memory:; -> memóriában tárolja az adatbázist, tesztelésre jó
            {
                //system.data.sqlite paranccsal töltsük le
                conn.Open();
                var command = conn.CreateCommand();//var helyett SQLiteCommand is lehet
                /* a @ jel jelzi, hogy nem ér véget abban a sorban a parancs*/ command.CommandText = @"CREATE TABLE IF NOT EXISTS macskak (
                id INTEGER PRIMARY KEY AUTOINCREMENT, 
                nev VARCHAR(1000) NOT NULL,
                meret INTEGER NOT NULL)";
                command.ExecuteNonQuery();

                var beszurCmd = conn.CreateCommand();
                beszurCmd.CommandText = @"INSERT INTO macskak(nev,meret) VALUES (@nev,@meret)";
                beszurCmd.Parameters.AddWithValue("@nev", "Kormos");
                beszurCmd.Parameters.AddWithValue("@meret", 41);
                beszurCmd.ExecuteNonQuery();

                /*var beszurCmd = conn.CreateCommand();
                beszurCmd.CommandText= @"INSERT INTO macskak (nev, meret) VALUES ('Tigris',45), ('Cirmi',20),('Pici',120)";
                beszurCmd.ExecuteNonQuery();*/

                var osszegCmd = conn.CreateCommand();
                osszegCmd.CommandText = @"SELECT COUNT(*) FROM macskak";
                long db=(long)osszegCmd.ExecuteScalar();

                Console.WriteLine("Darab: "+db);

                Console.WriteLine("Mekkora macska kell?");
                string userMeretStr = Console.ReadLine();
                int userMeret;
                if (!int.TryParse(userMeretStr, out userMeret))
                {
                    Console.WriteLine("Érvénytelen méret");
                    return;
                }

                //' OR 1=1; --    az összes adatot visszaadja a táblából
                // SQL injection
                //mysql real escape string



                var lekerdezesCmd = conn.CreateCommand();
                lekerdezesCmd.CommandText = @"
                SELECT id,nev,meret
                FROM macskak
                WHERE meret>=@meret";
                lekerdezesCmd.Parameters.AddWithValue("@meret",userMeret);
                using (var reader = lekerdezesCmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string nev = reader.GetString(1);
                        int meret = reader.GetInt32(2);
                        Console.WriteLine("{0}, {1}cm ({2})",nev,meret,id);
                    }
                }








                Console.ReadLine();


                



            }





        }
    }
}

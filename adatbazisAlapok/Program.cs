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
            //adatbázis létrehozása
            using (var conn = new SQLiteConnection("Data Source=mydb.db"))//fájlkiterjesztésnek akár sqlite, vagy sqlite3|:memory: -> memóriában tárolja az adatbázist, tesztelésre jó
            {
                //system.data.sqlite paranccsal töltsük le
                //tábla létrehozása
                conn.Open();
                var command = conn.CreateCommand();//var helyett SQLiteCommand is lehet
                /* a @ jel jelzi, hogy nem ér véget abban a sorban a parancs*/ command.CommandText = @"CREATE TABLE IF NOT EXISTS macskak (
                id INTEGER PRIMARY KEY AUTOINCREMENT, 
                nev VARCHAR(1000) NOT NULL,
                meret INTEGER NOT NULL)";
                command.ExecuteNonQuery();
                
                /*tábla feltöltése
                var beszurCmd = conn.CreateCommand();
                beszurCmd.CommandText = @"INSERT INTO macskak(nev,meret) VALUES (@nev,@meret)";
                beszurCmd.Parameters.AddWithValue("@nev", "Kormos");
                beszurCmd.Parameters.AddWithValue("@meret", 41);
                beszurCmd.ExecuteNonQuery();

                var beszurCmd = conn.CreateCommand();
                beszurCmd.CommandText= @"INSERT INTO macskak (nev, meret) VALUES ('Tigris',45), ('Cirmi',20),('Pici',120)";
                beszurCmd.ExecuteNonQuery();*/

                //hány rekord van?
                var osszegCmd = conn.CreateCommand();
                osszegCmd.CommandText = @"SELECT COUNT(*) FROM macskak";
                long db=(long)osszegCmd.ExecuteScalar();

                Console.WriteLine("Darab: "+db);

                //nagyobb rekordok adatai
                Console.WriteLine("Mekkora macska kell?");
                string userMeretStr = Console.ReadLine();
                int userMeret;
                if (!int.TryParse(userMeretStr, out userMeret))
                {
                    Console.WriteLine("Érvénytelen méret");
                    return;
                }
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

                //' OR 1=1; --    az összes adatot visszaadja a táblából
                // SQL injection
                //mysql real escape string

                //egy id-hez tartozó sor kiírása
                Console.WriteLine("Melyik id?");
                string userIdStr = Console.ReadLine();
                int userId;
                if (!int.TryParse(userIdStr, out userId))
                {
                    Console.WriteLine("Érvénytelen id");
                    return;
                }

                var lekerdezIdAlapjan = conn.CreateCommand();
                lekerdezIdAlapjan.CommandText = @"SELECT id, nev,meret FROM macskak WHERE id=@id";
                lekerdezIdAlapjan.Parameters.AddWithValue("@id", userId);
                using (var reader = lekerdezIdAlapjan.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string nev = reader.GetString(1);
                        int meret = reader.GetInt32(2);
                        Console.WriteLine("{0}, {1}cm ({2})", nev, meret, id);
                    }
                }

                //egy névhez tartozó sor kiírása
                Console.WriteLine("Melyik név?");
                string userNevStr = Console.ReadLine();

                var lekerdezNevAlapjan = conn.CreateCommand();
                lekerdezNevAlapjan.CommandText = @"SELECT id, nev,meret FROM macskak WHERE nev=@nev";
                lekerdezNevAlapjan.Parameters.AddWithValue("@nev", userNevStr);
                using (var reader = lekerdezNevAlapjan.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string nev = reader.GetString(1);
                        int meret = reader.GetInt32(2);
                        Console.WriteLine("{0}, {1}cm ({2})", nev, meret, id);
                    }
                }

                //kisebb méretűekből hány van?
                Console.WriteLine("Mekkora macska kell?");
                string userMeretSt = Console.ReadLine();
                int userMeretKis;
                if (!int.TryParse(userMeretSt, out userMeretKis))
                {
                    Console.WriteLine("Érvénytelen méret");
                    return;
                }
                var lekerdezKisebbMeret = conn.CreateCommand();
                lekerdezKisebbMeret.CommandText = @"SELECT Count(nev) FROM macskak WHERE meret<=@meret";
                lekerdezKisebbMeret.Parameters.AddWithValue("@meret", userMeretKis);
                long kisebbdb = (long)lekerdezKisebbMeret.ExecuteScalar();
                Console.WriteLine(kisebbdb);

                //egy id-jú elem átnevezése
                Console.WriteLine("Melyik azonosítójú macska kell?");
                string userAzonStr = Console.ReadLine();
                int userAzon;
                if (!int.TryParse(userAzonStr, out userAzon))
                {
                    Console.WriteLine("Érvénytelen id");
                    return;
                }
                Console.WriteLine("Mi legyen a macska neve?");
                string userUjNev = Console.ReadLine();
                var UjNevIdRa = conn.CreateCommand();
                UjNevIdRa.CommandText = @"UPDATE macskak SET nev=@nev WHERE id=@id";
                UjNevIdRa.Parameters.AddWithValue("@id", userAzon);
                UjNevIdRa.Parameters.AddWithValue("@nev", userUjNev);
                UjNevIdRa.ExecuteNonQuery();
                Console.WriteLine("OK");

                Console.ReadLine();

            }

        }
    }
}

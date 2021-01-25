using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo_Injection_SQL
{
    class Program
    {
        static void Main(string[] args)
        {
            string email = "zaza@outlook.be";
            string pwd = "Test1234=";
            string pwd_NoResult = "qkjbdfqkjbskqjscn=";
            string pwd_Injection1 = "Super Pwd' OR 1=1; --";
            string pwd_Injection2 = "test'; USE [Master]; DROP Database [Demo_Injection_SQL]; --";

            using (SqlConnection connection = new SqlConnection(@"Server=DESKTOP-CE6MM13\SQLEXPRESS;Database=Demo_Injection_SQL;Trusted_Connection=True;"))
            {
                
                using(SqlCommand command = connection.CreateCommand())
                {

                    command.CommandType = System.Data.CommandType.Text;

                    // Bad request !!!! Injection possible
                    //command.CommandText = $"SELECT [ID], [Email], [Role] FROM Utilisateur WHERE Email LIKE '{email}' AND Pwd LIKE '{pwd}' ;";


                    command.CommandText = $"SELECT [ID], [Email], [Role] FROM Utilisateur WHERE Email LIKE @param1 AND Pwd LIKE @param2 ;";

                    SqlParameter paramEmail = new SqlParameter("param1", email);
                    command.Parameters.Add(paramEmail);

                    command.Parameters.AddWithValue("param2", pwd);


                    connection.Open();

                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            Guid recupGuid = new Guid(reader["ID"].ToString());
                            string recupEmail = reader["Email"].ToString();
                            string recupRole = reader["Role"].ToString();

                            Console.WriteLine($"{recupGuid} {recupEmail} {recupRole}");
                        }
                    }
                }
            }

            Console.ReadLine();
        }
    }
}

using System;
using Microsoft.Data.SqlClient;

namespace _03.MinionNames
{
    class Program
    {
        private const string serverConnection = "Server=.\\SQLEXPRESS;Integrated Security=true;Database=MinionsDB";

        static void Main(string[] args)
        {

            using (var connection = new SqlConnection(serverConnection))
            {
                connection.Open();

                string id = Console.ReadLine();

                string query = $"SELECT Name FROM Villains WHERE Id = {id}";
                SqlCommand sqlCommand = new SqlCommand(query, connection);

                string name = (string)sqlCommand.ExecuteScalar();

                if (name is null)
                {
                    Console.WriteLine($"No villain with ID {id} exists in the database.");
                }
                else
                {
                    Console.WriteLine($"Villian: {name}");
                }

                query = $@"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                 m.Name, 
                                 m.Age
                          FROM MinionsVillains AS mv
                                 JOIN Minions As m ON mv.MinionId = m.Id
                          WHERE mv.VillainId = {id}
                          ORDER BY m.Name";

                sqlCommand = new SqlCommand(query, connection);
                var result = sqlCommand.ExecuteReader();

                if (result is null)
                {
                    Console.WriteLine("(no minions)");
                }
                else
                {
                    int count = 1;
                    while (result.Read())
                    {
                        Console.WriteLine($"{count}. {result["Name"]} {result["Age"]}");
                        count++;
                    }
                }
            }
        }
    }
}

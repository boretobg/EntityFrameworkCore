using System;
using Microsoft.Data.SqlClient;

namespace _02.VillainNames
{
    class Program
    {
        static void Main(string[] args)
        {
            string connection = "Server=.\\SQLEXPRESS;Integrated Security=true;Database=MinionsDB";

            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                string query = @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount 
                                 FROM Villains AS v
                                 JOIN MinionsVillains AS mv ON v.Id = mv.VillainId
                                 GROUP BY v.Id, v.Name
                                 HAVING COUNT(mv.VillainId) > 3
                                 ORDER BY COUNT(mv.VillainId)";

               using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            Console.WriteLine($"{sqlDataReader["Name"]} - {sqlDataReader["MinionsCount"]}");
                        }
                    }
                }
            }
        }
    }
}

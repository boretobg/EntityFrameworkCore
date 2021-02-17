using System;
using Microsoft.Data.SqlClient;

namespace _04.AddMinion
{
    class Program
    {
        private const string serverConnection = "Server=.\\SQLEXPRESS;Integrated Security=true;Database=MinionsDB";

        static void Main(string[] args)
        {
            string[] minionInput = Console.ReadLine().Split();
            string[] villianInput = Console.ReadLine().Split();

            string minionName = minionInput[1];
            string minionAge = minionInput[2];
            string minionTown = minionInput[3];

            string villianName = villianInput[1];

            using (var connection = new SqlConnection(serverConnection))
            {
                connection.Open();

                string getTownIdQuery = $"SELECT Id FROM Towns WHERE Name = {minionTown}";

                SqlCommand sqlCommand = new SqlCommand(getTownIdQuery, connection);

                var townId = sqlCommand.ExecuteScalar();

                Console.WriteLine(townId);
            }
        }
    }
}

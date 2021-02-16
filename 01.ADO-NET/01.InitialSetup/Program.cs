using System;
using Microsoft.Data.SqlClient;

namespace _01.InitialSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            string connection = "Server=.\\SQLEXPRESS;Integrated Security=true;Database=MinionsDB";

            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
            }
        }
    }
}

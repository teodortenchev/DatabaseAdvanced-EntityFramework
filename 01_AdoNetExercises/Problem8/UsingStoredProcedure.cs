using AdoNet_Exercises;
using System;
using System.Data.SqlClient;

namespace Problem8
{
    class UsingStoredProcedure
    {
        static void Main(string[] args)
        {
            int minionId = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string uspGetOlderProc = "EXEC usp_GetOlder @id";

                using (SqlCommand command = new SqlCommand(uspGetOlderProc, connection))
                {
                    command.Parameters.AddWithValue("@id", minionId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        string name = (string)reader[0];
                        int age = (int)reader[1];

                        Console.WriteLine($"{name} - {age}");
                    }
                }

                
            }
        }
    }
}

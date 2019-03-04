using AdoNet_Exercises;
using System;
using System.Data.SqlClient;

namespace Problem4
{
    class AddMinions
    {
        static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string[] minionInfo = Console.ReadLine().Split();
                string[] villain = Console.ReadLine().Split();

                string minionName = minionInfo[1];
                int minionAge = int.Parse(minionInfo[2]);
                string minionTown = minionInfo[3];
                string villainName = villain[1];


                CheckTownInDB(connection, minionTown);



            }
        }

        private static void CheckTownInDB(SqlConnection connection, string minionTown)
        {
            string checkTownExists = "SELECT Id FROM Towns WHERE Name = @Name";

            using (SqlCommand command = new SqlCommand(checkTownExists, connection))
            {
                command.Parameters.AddWithValue("@Name", minionTown);

                int? id = (int?)command.ExecuteScalar();

                if (id == null)
                {
                    AddTown(connection, minionTown);
                }
            }
        }

        private static void AddTown(SqlConnection connection, string townName)
        {
            string insertTown = "INSERT INTO Towns (Name) VALUES (@townName)";

            using (SqlCommand command = new SqlCommand(insertTown, connection))
            {
                command.Parameters.AddWithValue("@townName", townName);

                int rowsAffected = (int)command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Town {townName} was added to the database.");
                }
                else
                {
                    throw new InvalidOperationException($"Something went wrong while attempting to add {townName} to the Database");
                }

            }
        }
    }
}

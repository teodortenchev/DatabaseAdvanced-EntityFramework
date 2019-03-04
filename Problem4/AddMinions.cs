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

                string checkTownExists = "SELECT Id FROM Towns WHERE Name = @Name";

                using (SqlCommand command = new SqlCommand(checkTownExists, connection))
                {
                    command.Parameters.AddWithValue("@Name", minionTown);

                    int? id = (int?)command.ExecuteScalar();

                    if(id == null)
                    {
                        AddTown(connection, minionTown);
                    }
                }

            }
        }

        private static void AddTown(SqlConnection connection, string townName)
        {
            throw new NotImplementedException();
        }
    }
}

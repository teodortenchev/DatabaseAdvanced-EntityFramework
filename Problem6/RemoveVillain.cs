using AdoNet_Exercises;
using System;
using System.Data.SqlClient;

namespace Problem6
{
    class RemoveVillain
    {
        static void Main(string[] args)
        {
            int villainId = int.Parse(Console.ReadLine());
            string villainName = "";
            int minionsReleased = 0;

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                villainName = GetVillainNameById(connection, villainId);

                if (villainName == null)
                {
                    Console.WriteLine("No such villain was found.");
                }
                else
                {
                    minionsReleased = ReleaseMinions(connection, villainId);

                    DeleteVillain(connection, villainId, villainName);

                    Console.WriteLine($"{minionsReleased} were released.");
                }
            }
        }

        private static void DeleteVillain(SqlConnection connection, int villainId, string villainName)
        {
            string deleteVillain = @"DELETE FROM Villains WHERE Id = @villainId";

            using (SqlCommand command = new SqlCommand(deleteVillain, connection))
            {
                command.Parameters.AddWithValue("@villainId", villainId);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException("Deleting villain did not succeed");
                }

                Console.WriteLine($"{villainName} was deleted.");
            }
        }

        private static int ReleaseMinions(SqlConnection connection, int villainId)
        {
            string deleteVillainMinions = @"DELETE FROM MinionsVillains WHERE VillainId = @villainId";

            using (SqlCommand command = new SqlCommand(deleteVillainMinions, connection))
            {
                command.Parameters.AddWithValue("@villainId", villainId);

                return command.ExecuteNonQuery();
            }

        }

        private static string GetVillainNameById(SqlConnection connection, int villainId)
        {
            string selectVillain = @"SELECT Name FROM Villains WHERE Id = @villainId";

            using (SqlCommand command = new SqlCommand(selectVillain, connection))
            {
                command.Parameters.AddWithValue("@villainId", villainId);
                return (string)command.ExecuteScalar();
            }
        }
    }
}

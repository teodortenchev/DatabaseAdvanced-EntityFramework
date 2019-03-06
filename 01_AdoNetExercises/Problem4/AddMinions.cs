using AdoNet_Exercises;
using System;
using System.Data.SqlClient;

namespace Problem4
{
    class AddMinions
    {
        static void Main(string[] args)
        {
            string[] minionInfo = Console.ReadLine().Split();
            string[] villain = Console.ReadLine().Split();

            string minionName = minionInfo[1];
            int minionAge = int.Parse(minionInfo[2]);
            string minionTown = minionInfo[3];
            string villainName = villain[1];

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                int? townId = GetTownByName(connection, minionTown);

                if (townId == null)
                {
                    AddTown(connection, minionTown);
                }

                townId = GetTownByName(connection, minionTown);

                AddMinion(connection, minionName, minionAge, (int)townId);

                int? villainId = GetVillainByName(connection, villainName);

                if (villainId == null)
                {
                    AddVillain(connection, villainName);
                }

                villainId = GetVillainByName(connection, villainName);

                int minionId = GetMinionByName(connection, minionName);

                AssignMinionToVillain(connection, minionId, villainId, minionName, villainName);
            }
        }


        private static void AssignMinionToVillain(SqlConnection connection, int minionId, int? villainId, string minionName, string villainName)
        {
            string assignMinionVillain = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@minionId, @villainId)";

            using (SqlCommand command = new SqlCommand(assignMinionVillain, connection))
            {
                command.Parameters.AddWithValue("@villainId", villainId);
                command.Parameters.AddWithValue("@minionId", minionId);

                int rowsAffected = 0;

                try
                {
                    rowsAffected = (int)command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    Console.WriteLine($"{minionName} is already a slave of {villainName}");
                }
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
                }
            }
        }

        private static int GetMinionByName(SqlConnection connection, string minionName)
        {
            string minionIdQuery = "SELECT Id FROM Minions WHERE Name = @Name";

            using (SqlCommand command = new SqlCommand(minionIdQuery, connection))
            {
                command.Parameters.AddWithValue("@Name", minionName);
                int? minionID = (int?)command.ExecuteScalar();

                if (minionID == null)
                {
                    throw new InvalidOperationException("Could not find minion. Looks like he ran away...");
                }

                return (int)minionID;
            }
        }

        private static void AddMinion(SqlConnection connection, string minionName, int minionAge, int townId)
        {
            string insertMinion = "INSERT INTO Minions (Name, Age, TownId) VALUES (@name, @age, @townId)";

            using (SqlCommand command = new SqlCommand(insertMinion, connection))
            {
                command.Parameters.AddWithValue("@name", minionName);
                command.Parameters.AddWithValue("@age", minionAge);
                command.Parameters.AddWithValue("@townId", townId);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected <= 0)
                {
                    throw new InvalidOperationException("Something went wrong while attempting to add minion to DB");
                }

            }

        }

        private static int? GetTownByName(SqlConnection connection, string minionTown)
        {
            string checkTownExists = "SELECT Id FROM Towns WHERE Name = @Name";

            using (SqlCommand command = new SqlCommand(checkTownExists, connection))
            {
                command.Parameters.AddWithValue("@Name", minionTown);

                return (int?)command.ExecuteScalar();
            }


        }

        private static int? GetVillainByName(SqlConnection connection, string villainName)
        {
            string checkVillainExists = "SELECT Id FROM Villains WHERE Name = @Name";

            using (SqlCommand command = new SqlCommand(checkVillainExists, connection))
            {
                command.Parameters.AddWithValue("@Name", villainName);

                return (int?)command.ExecuteScalar();
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
                    throw new InvalidOperationException($"Something went wrong while attempting to add town {townName} to the Database");
                }

            }
        }

        private static void AddVillain(SqlConnection connection, string villainName)
        {
            string insertVillain = "INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";

            using (SqlCommand command = new SqlCommand(insertVillain, connection))
            {
                command.Parameters.AddWithValue("@villainName", villainName);

                int rowsAffected = (int)command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Villain {villainName} was added to the database.");
                }
                else
                {
                    throw new InvalidOperationException($"Something went wrong while attempting to add villain {villainName} to the Database");
                }

            }
        }
    }
}

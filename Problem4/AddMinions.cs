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

                //Check if town exists in DB and add it if not
                CheckTownInDB(connection, minionTown);

                //Check if villain exists in DB and add if not
                CheckVillainInDB(connection, villainName);

                //Add minion to DB.
                AddMinionToDB(connection, minionName, minionAge, minionTown, villainName);

                //

            }
        }

        private static void AddMinionToDB(SqlConnection connection, string minionName, int minionAge, string minionTown, string villainName)
        {
            string insertMinion = "INSERT INTO Minions (Name, Age, TownId) VALUES (@name, @age, @townId)";

            using (SqlCommand command = new SqlCommand(insertMinion, connection))
            {
                int townID = CheckTownInDB(connection, minionTown);

                command.Parameters.AddWithValue("@name", minionName);
                command.Parameters.AddWithValue("@age", minionAge);
                command.Parameters.AddWithValue("@townId", townID);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Successfully added {minionName} to be a servant of {villainName}");
                }

            }

            Console.WriteLine("Something went wrong while attempting to add minion to DB");
        }

        private static int CheckTownInDB(SqlConnection connection, string minionTown)
        {
            string checkTownExists = "SELECT Id FROM Towns WHERE Name = @Name";
            int townIdResult = 0;

            using (SqlCommand command = new SqlCommand(checkTownExists, connection))
            {
                command.Parameters.AddWithValue("@Name", minionTown);

                int? id = (int?)command.ExecuteScalar();

                if (id == null)
                {
                    AddTown(connection, minionTown);
                }

                townIdResult = (int)command.ExecuteScalar();

            }

            return townIdResult;
        }

        private static int CheckVillainInDB(SqlConnection connection, string villainName)
        {
            string checkVillainExists = "SELECT Id FROM Villains WHERE Name = @Name";
            int villainIdResult = 0;

            using (SqlCommand command = new SqlCommand(checkVillainExists, connection))
            {
                command.Parameters.AddWithValue("@Name", villainName);

                int? id = (int?)command.ExecuteScalar();

                if (id == null)
                {
                    AddVillain(connection, villainName);
                }

                villainIdResult = (int)command.ExecuteScalar();

            }

            return villainIdResult;
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

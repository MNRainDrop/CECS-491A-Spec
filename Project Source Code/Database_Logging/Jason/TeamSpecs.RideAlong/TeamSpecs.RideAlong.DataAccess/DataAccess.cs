using System.Data.SqlClient;

namespace TeamSpecs.RideAlong.DataAccess
{
    public class DataAccess
    {
        private string connectionString = @"Server=LAPTOP-6OLKME95;Database=RideAlongDB;";
        private string adminString = "User Id=RideAlongAdmin;Password=admin;";
        private string readString = "User Id=RideAlongRead;Password=readme;";
        private string writeString = "User Id=RideAlongWrite;Password=writeme;";

        public void write(string querry)
        {
            SqlConnection writeConnection = new SqlConnection(connectionString+writeString);
            SqlCommand command;

            writeConnection.Open();
            command = writeConnection.CreateCommand();
            command.CommandText = querry;
            command.ExecuteNonQuery();
            writeConnection.Close();
        }

        public void read(string querry)
        {
            SqlConnection readConnection = new SqlConnection(connectionString+readString);
            SqlCommand command;

            readConnection.Open();
            command = readConnection.CreateCommand();
            command.CommandText = querry;
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("{0} {1} {2}", reader["logID"].ToString(), reader["logTime"].ToString(), reader["logMessage"].ToString());
            }
            readConnection.Close();

        }

        public void admin(string querry)
        {
            SqlConnection adminConnection = new SqlConnection(connectionString+adminString);
            SqlCommand command;

            adminConnection.Open();
            command = adminConnection.CreateCommand();
            command.CommandText = querry;
            command.ExecuteNonQuery();
            adminConnection.Close();
        }
    }
}
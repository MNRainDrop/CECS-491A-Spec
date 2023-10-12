using System;
using System.Data.SqlClient;
using TeamSpecs.Ridealong.DataAccess;
using TeamSpecs.Ridealong.Logging;

namespace TeamSpecs.RideAlong.ConsoleApp
{
    public class ConsoleApp
    {
        static void Main(string[] args)
        {
            Logging logs = new Logging();

            bool invalid = true;
            string message = "";
            Console.WriteLine("What message do you want to send (maximum 50 characters)");
            while (invalid)
            {
                message += Console.ReadLine();
                if (message.Length < 100 && message != "")
                {
                    invalid = false;
                }
                else
                {
                    Console.WriteLine("Invalid argument");
                    message = "";
                }
            }
            logs.writeToDataStore(message);

            DataAccess data = new DataAccess();
            data.read("SELECT * FROM LoggingTestTable;");
        }
    }
}
namespace Specs.RideAlong.ConsoleApp
{
    public class ConsoleApp
    {
        static void Main(string[] args)
        {
            Logging.Logging logs = new Logging.Logging();

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

            DataAccess.DataAccess data = new DataAccess.DataAccess();
            data.read("SELECT * FROM LoggingTestTable;");
        }
    }
}
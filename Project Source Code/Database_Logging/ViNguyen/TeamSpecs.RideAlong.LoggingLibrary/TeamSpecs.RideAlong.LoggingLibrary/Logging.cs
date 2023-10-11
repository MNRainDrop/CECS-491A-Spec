﻿using System;
using TeamSpecs.RideAlong.DataAccess;

namespace TeamSpecs.RideAlong.Logging
{
    public class Logging
    {
        private DataAccess.DataAccess dataAccess;
        public Logging()
        {
            dataAccess = new DataAccess.DataAccess();
        }

        public void writeToDataStore(string message)
        {
            string querry = @"Insert Into LoggingTestTable(logMessage, logTime) Values('" + message + "', GETDATE());";
            dataAccess.write(querry);
            Console.WriteLine("Querry written to database");
        }
    }
}
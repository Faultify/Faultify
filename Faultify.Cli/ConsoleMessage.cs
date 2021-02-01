using System;

namespace Faultify.Cli
{
    internal static class ConsoleMessage
    {
        private static readonly string _logo = @"    
             ______            ____  _ ____     
            / ____/___ ___  __/ / /_(_) __/_  __
           / /_  / __ `/ / / / / __/ / /_/ / / / 
          / __/ / /_/ / /_/ / / /_/ / __/ /_/ / 
         / _/    \__,_/\__,_/_/\__/_/_/  \__,/ 
                                       /____/  
        ";

        public static void PrintLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(_logo);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintSettings(Settings settings)
        {
            var settingsString =
                "\n" +
                $"| Mutation Level: {settings.MutationLevel} \t\t \n" +
                $"| Test Runners: {settings.Parallel} \t\t \n" +
                $"| Report Path: {settings.ReportPath} \t\t \n" +
                $"| Report Type: {settings.ReportType} \t\t \n" +
                $"| Test Project Path: {settings.TestProjectPath} \t\t \n" +
                "\n";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(settingsString);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
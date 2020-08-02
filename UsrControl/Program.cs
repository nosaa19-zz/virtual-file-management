using System;
using UsrControl.Service;

namespace UsrControl
{
    class Program
    {
        static void Main(string[] args)
        {
            String command;
            MainController mainController = new MainController();

            do {
                Console.Write("# ");
                command = Console.ReadLine();
                mainController.CommandProcess(command);
                Console.WriteLine();
                //Console.Write("press Enter to Continue ...");
                //Console.ReadLine();
                //Console.Clear();
            }
            while (command.ToLower() != "exit"); 
        }
    }
}

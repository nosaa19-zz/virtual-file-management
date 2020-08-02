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
            Console.WriteLine("==========__======__==__============__===");
            Console.WriteLine("\\  /\\  / |__ |   |   |  |  /\\  /\\  |__  ");
            Console.WriteLine(" \\/  \\/  |__ |__ |__ |__| /  \\/  \\ |__  ");
            Console.WriteLine("=========================================");
            Console.WriteLine("--------VIRTUAL-FILE-MANAGEMENT----------");
            Console.WriteLine("=========================================");
            do {
                Console.Write("# ");
                command = Console.ReadLine();
                mainController.CommandProcess(command);
                Console.WriteLine();
            }
            while (command.ToLower() != "exit"); 
        }
    }
}

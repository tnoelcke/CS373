using System;

namespace HW1
{
    class Program
    {
        static void Main(string[] args)
        {
            bool quit = false;
            string input;
            while(!quit){
                Console.Write("$ ");
                input = Console.ReadLine();
                if(input.ToLower().StartsWith("q")){
                    quit = true;
                } else {
                    processInput(input);
                }
            }

        }

        // Handles choosing what function to run
        static void processInput(string input){
            string[] split = input.Split(' ');
            switch(split[0]){
                case "help":
                    printCommands();
                    break;
                default:
                    foreach(string item in split){
                        Console.Write("$ " + item + " ");
                    }
                    Console.Write("is not a valid command. Type help for a list of valid commands.");
                    Console.WriteLine();
                    break;
            }
        }

        static void printCommands(){
            Console.WriteLine("Below is a valid list of commands that can be run in the program:");
            Console.WriteLine("p -                      Lists out all the processes");
            Console.WriteLine("p <processId>            Lists out all of the running threads with in a process");
            Console.WriteLine("pm <processId>           Lists out all the modules loaded in a process");
            Console.WriteLine("page <processId>         show all executable pages in a process");
            Console.WriteLine("mr <processId> <#bytes>  read #bytes form a processes memory starting at the lowest address (out put in hex)");
            Console.WriteLine("q                        quit");            
        }
    }
}

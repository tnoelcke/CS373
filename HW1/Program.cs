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
            string split = input.split(" ");
            if()
        }
    }
}

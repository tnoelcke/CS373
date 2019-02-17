using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace HW1
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead);

        public enum AllocationProtect : uint 
        { 
            PAGE_EXECUTE = 0x00000010, 
            PAGE_EXECUTE_READ = 0x00000020, 
            PAGE_EXECUTE_READWRITE = 0x00000040, 
            PAGE_EXECUTE_WRITECOPY = 0x00000080, 
            PAGE_NOACCESS = 0x00000001, 
            PAGE_READONLY = 0x00000002, 
            PAGE_READWRITE = 0x00000004, 
            PAGE_WRITECOPY = 0x00000008, 
            PAGE_GUARD = 0x00000100, 
            PAGE_NOCACHE = 0x00000200, 
            PAGE_WRITECOMBINE = 0x00000400 
        }

        private const long MAX_ADDRESS = 0x7fffffff;

        [StructLayout(LayoutKind.Sequential)] 
        public struct MEMORY_BASIC_INFORMATION 
        { 
            public IntPtr BaseAddress; 
            public IntPtr AllocationBase; 
            public uint AllocationProtect; 
            public IntPtr RegionSize; 
            public uint State; 
            public uint Protect; 
            public uint Type; 
        }

        [DllImport("kernel32.dll")]
        static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);


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
                case "p":
                    handleProcess(split);
                    break;
                case "pm":
                    handleModules(split);
                    break;
                case "page":
                    handlePages(split);
                    break;
                case "mr":
                    handleMemoryRead(split);
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
            Console.WriteLine("mr <processId>           read #bytes form a processes memory starting at the lowest address (out put in hex)");
            Console.WriteLine("q                        quit");            
        }

        static void handleProcess(string[] args){
            if(args.Length == 1){
                listProcesses();
            } else {
                int processId = 0;
                if(Int32.TryParse(args[1], out processId)){
                    listThreads(processId);
                } else {
                    Console.WriteLine("p command requires a valid process id, " + args[1] + "is not a valid process id");
                }
            }
        }

        static void listProcesses(){
            Process[] allProcesses = Process.GetProcesses();
            Console.WriteLine("-------------------------------------------------------------------------------------------------");
            Console.WriteLine("| Process Name\t|\tProcessId|");
            foreach(Process p in allProcesses){
                Console.WriteLine("| "+ p.ProcessName + "\t|\t" +  p.Id + " |");
            }
            Console.WriteLine("-------------------------------------------------------------------------------------------------");
        }

        static void listThreads(int processId){
            try {
                Process myProcess = Process.GetProcessById(processId);
                Console.WriteLine("-------------------------------------------------------------------------------------------------");
                Console.WriteLine("| Id \t|\t Priority Level \t|\t Current priority |");
                foreach(ProcessThread t in myProcess.Threads) {
                    Console.WriteLine("| " + t.Id + "\t|\t\t" + t.PriorityLevel + "\t|\t\t" + t.CurrentPriority + " |");
                }
                Console.WriteLine("-------------------------------------------------------------------------------------------------");
            } catch(Exception e) {
                Console.WriteLine("Could Not find Process Id " + processId);
                Console.WriteLine("Error: " + e.ToString());
            }

        }

        static void handleModules(string[] args){
            if(args.Length <= 1){
                Console.WriteLine("The pm command requires an event Id");
                Console.WriteLine("pm <processId>");
            } else {
                int processId = 0;
                if(Int32.TryParse(args[1], out processId)){
                    listModules(processId);
                } else {
                    Console.WriteLine("pm command requires a valid process id, " + args[1] + "is not a valid process id");
                }
            }
        }

        static void listModules(int processId){
            try {
                Process myProcess = Process.GetProcessById(processId);
                Console.WriteLine("-------------------------------------------------------------------------------------------------");
                Console.WriteLine("| Modules Name \t|\t Memory Size \t|\t File Name |");
                foreach(ProcessModule m in myProcess.Modules) {
                    Console.WriteLine("| " + m.ModuleName + "\t|\t\t" + m.ModuleMemorySize + "\t|\t\t" + m.FileName + " |");
                }
                Console.WriteLine("-------------------------------------------------------------------------------------------------");
            } catch(Exception e) {
                Console.WriteLine("Could Not find Process Id " + processId);
                Console.WriteLine("Error: " + e.ToString());
            }
        }

        static void handlePages(string[] args){
            if(args.Length <= 1){
                Console.WriteLine("The page command requires an event Id");
                Console.WriteLine("page <processId>");
            } else {
                int processId = 0;
                if(Int32.TryParse(args[1], out processId)){
                    listPages(processId);
                } else {
                    Console.WriteLine("page command requires a valid process id, " + args[1] + "is not a valid process id");
                }
            }
        }

        static void handleMemoryRead(string[] args){
            if(args.Length <= 1){
                Console.WriteLine("The mr command requires an event Id");
                Console.WriteLine("mr <processId>");
            } else {
                int processId = 0;
                if(Int32.TryParse(args[1], out processId)){
                    listModules(processId);
                } else {
                    Console.WriteLine("mr command requires a valid process id, " + args[1] + "is not a valid process id");
                }
            }
        }

        static void readMenu () {

        }

        static void listPages(int processId){
            long address = 0;
            Process currentProcess;
            try{
            currentProcess = Process.GetProcessById(processId);
            IntPtr hProcess = currentProcess.Handle;
            Console.WriteLine("-------------------------------------------------------------------------------------------------");
            Console.WriteLine("| Base Address \t|\t Region Size \t|\t State |");
            while(address < MAX_ADDRESS) 
            {
                MEMORY_BASIC_INFORMATION memInfo;
                int result = VirtualQueryEx(hProcess, (IntPtr) address, out memInfo, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                if(memInfo.AllocationProtect == (uint)AllocationProtect.PAGE_EXECUTE ||
                    memInfo.AllocationProtect == (uint)AllocationProtect.PAGE_EXECUTE_READ ||
                    memInfo.AllocationProtect == (uint)AllocationProtect.PAGE_EXECUTE_READWRITE ||
                    memInfo.AllocationProtect == (uint)AllocationProtect.PAGE_EXECUTE_WRITECOPY){
                        Console.WriteLine("| " + (uint)memInfo.BaseAddress + "\t|\t" + memInfo.RegionSize + "\t|\t" + memInfo.State + " |");
                }
                address = (long) memInfo.BaseAddress + (long)memInfo.RegionSize;
            }
            } catch (Exception e) {
                Console.WriteLine("Could Not find Process Id " + processId);
                Console.WriteLine("Error: " + e.ToString());
            }
            Console.WriteLine("-------------------------------------------------------------------------------------------------");
        }
    }
}

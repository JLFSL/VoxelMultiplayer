using System;
using System.IO;
using System.Diagnostics;
using System.Threading;

using SharpMonoInjector;

namespace VoxelMultiplayer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load our assembly and start VoxelTycoon.exe + inject
            Console.WriteLine("Starting server");
            if (Load("VoxelTycoon.exe", "VoxelMultiplayer.dll", "VoxelMultiplayer", "Core", "Load"))
            {
                Console.WriteLine("Starting client");
                if(Load("VoxelTycoon2.exe", "VoxelMultiplayer.dll", "VoxelMultiplayer", "Core", "Load"))
                {
                    // Close our console
                    Environment.Exit(0);
                }
            }
        }

        static bool Load(string executablePath, string assemblyPath, string @namespace, string className, string methodName)
        {
            byte[] assembly;

            try
            {
                // Read our assembly into byte array
                assembly = File.ReadAllBytes(assemblyPath);
                IntPtr remoteAssembly = IntPtr.Zero;

                try
                {
                    // Start the game process
                    Process process = Process.Start(executablePath);
                    // Wait for the game to actually open (https://devblogs.microsoft.com/oldnewthing/20100325-00/?p=14493)
                    Thread.Sleep(1500);
                    // Inject our assembly into the game process
                    Injector injector = new Injector(process.Id);
                    remoteAssembly = injector.Inject(assembly, @namespace, className, methodName);

                    return true;
                }
                catch (InjectorException ie)
                {
                    Console.WriteLine("Failed to inject assembly: " + ie);
                    Console.ReadLine();

                    return false;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Failed to inject assembly (system error): " + e);
                    Console.ReadLine();

                    return false;
                }
            }
            catch
            {
                Console.WriteLine("Could not find " + assemblyPath);
                Console.ReadLine();

                return false;
            }
        }
    }
}

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
            Load("VoxelTycoon.exe", "VoxelMultiplayer.dll", "VoxelMultiplayer", "Core", "Load");
        }

        static void Load(string executablePath, string assemblyPath, string @namespace, string className, string methodName)
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
                    process.WaitForInputIdle();
                    // Inject our assembly into the game process
                    Injector injector = new Injector(process.Id);
                    remoteAssembly = injector.Inject(assembly, @namespace, className, methodName);
                    // Close our console
                    Environment.Exit(0);
                }
                catch (InjectorException ie)
                {
                    Console.WriteLine("Failed to inject assembly: " + ie);
                    Console.ReadLine();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Failed to inject assembly (system error): " + e);
                    Console.ReadLine();
                }
            }
            catch
            {
                Console.WriteLine("Could not find " + assemblyPath);
                Console.ReadLine();
            }
        }
    }
}

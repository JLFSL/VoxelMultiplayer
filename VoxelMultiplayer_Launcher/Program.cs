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
            Load("VoxelMultiplayer.dll", "VoxelMultiplayer", "Core", "Load");
        }

        static void Load(string assemblyPath, string @namespace, string className, string methodName)
        {
            byte[] assembly;

            try
            {
                assembly = File.ReadAllBytes(assemblyPath);

                IntPtr remoteAssembly = IntPtr.Zero;

                try
                {
                    Process process = Process.Start("VoxelTycoon.exe");

                    Thread.Sleep(1000);

                    Injector injector = new Injector(process.Id);
                    remoteAssembly = injector.Inject(assembly, @namespace, className, methodName);

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

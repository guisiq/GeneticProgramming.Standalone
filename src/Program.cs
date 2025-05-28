using System;
using GeneticProgramming.Examples;

namespace GeneticProgramming.Standalone
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                BasicGPTest.RunBasicTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during test execution: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}

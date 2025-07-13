using System;
using System.IO;

namespace R2FibonacciApp
{
    internal class R2Fibonacci
    {

        static void Main()

        {

            // Generate the first 15 Fibonacci numbers
            // The generateFibonacciSequence method creates an array of the specified length
            int[] fibonacciSequence = generateFibonacciSequence(15);

            // Convert the array to a comma-separated string like 0,1,1,2,3,5,8,13,21,34,55,89,144,233,377
            string output = string.Join(",", fibonacciSequence);

            // Write to file
            // The program will create the file in the same directory as your executable.
            // If you need to specify a different path, you can modify the filePath variable.
            string filePath = "R2Fibonacci.txt";

            // Uses File.WriteAllText to create/overwrite "Fibonacci.txt"
            // Creates a file containing: 0,1,1,2,3,5,8,13,21,34,55,89,144,233,377
            File.WriteAllText(filePath, output);

            // Includes console output to confirm the operation
            Console.WriteLine($"Fibonacci sequence saved to {filePath}");
            Console.WriteLine($"Content: {output}");

        }


        // Generate the first 15 Fibonacci numbers
        // The generateFibonacciSequence method creates an array of the specified length
        static int[] generateFibonacciSequence(int count)
        {
            int[] sequence = new int[count];
            // If count <= 0, then we initialize the sequence to be an empty array of int data type
            if (count <= 0)
                sequence = Array.Empty<int>();

            // Sets the first two numbers (i.e. array index 0 and 1)
            if (count >= 1)
                sequence[0] = 0;

            if (count >= 2)
                sequence[1] = 1;

            // Generates subsequent numbers from i.e. array index 2 to 14 by summing the previous two numbers
            for (int i = 2; i < count; i++)
                sequence[i] = sequence[i - 2] + sequence[i - 1];

            // returns the generated first count=15 Fibonacci numbers
            return sequence;
        }

    }
}
using System;
using System.Collections.Generic;

namespace R1SumNumbersApp
{
    internal class R1SumNumbers
    {

        static void Main()

        {

            int sum = 0;

            // Prompts the user to enter an integer
            // Enter an integer: 20 (say user inputs the integer as 20)
            Console.Write("Enter an integer: ");

            // Validates the input is a positive integer
            if (int.TryParse(Console.ReadLine(), out int endNumber) && endNumber > 0)
            {
                List<int> divisibleNumbers = new List<int>();

                // The getSumofDivisibleNoswithinRange function does 2 tasks -
                // 1 - stores those numbers divisible by 3 or 5 into a 'divisibleNumbers' named list of int datatype
                // 2 - calculates the total/sum of those numbers divisible by 3 or 5 and returns it as a result in a 'sum' variable from this function call.
                sum = getSumofDivisibleNoswithinRange(endNumber, divisibleNumbers);

                if (divisibleNumbers.Count > 0)
                {
                    // Build the necessary output string
                    // Joins the numbers with "+" for display
                    string output = string.Join("+", divisibleNumbers);

                    // Outputs the result in the requested format                    
                    // 3+5+6+9+10+12+15+18+20 = 98
                    Console.WriteLine($"{output} = {sum}");
                }
                else
                    Console.WriteLine($"{sum}");
            }
            else
            {
                Console.WriteLine("Please enter a valid positive integer.");
            }

        }

        // The getSumofDivisibleNoswithinRange function does 2 tasks -
        // 1 - stores those numbers divisible by 3 or 5 into a 'divisibleNumbers' named list of int datatype
        // 2 - calculates the total/sum of those numbers divisible by 3 or 5 and returns it as a result in a 'sum' variable from this function call.
        static int getSumofDivisibleNoswithinRange(int endNumber, List<int> divisibleNumbers)
        {
            int sum = 0;

            // Iterates through numbers from 1 to the entered number
            for (int i = 1; i <= endNumber; i++)
            {

                // Checks if each number is divisible by 3 or 5
                if (isDivisibleByOr(i, 3, 5))
                {

                    // Stores qualifying numbers in a list
                    divisibleNumbers.Add(i);

                    // Calculates the sum of these divisible numbers
                    sum += i;
                }

            }

            return sum;
        }

        // Checks if each number is divisible by 3 or 5
        static bool isDivisibleByOr(int number, int a, int b)
        {
            bool flag = false;

            // Checks if each number is divisible by a or b
            if (number % a == 0 || number % b == 0)
                flag = true;

            return flag;
        }
    }
}



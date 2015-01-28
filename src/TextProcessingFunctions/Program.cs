using System;
using System.Diagnostics;
using System.Linq;
using TextProcessingFunctions.Core.TextProcesser;

namespace TextProcessingFunctions
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length != 1)
                Console.WriteLine("ERROR: Invalid command-line arguments: provide a single argument with the text file path");
            else
            {                
                try
                {
                    // Takes the text file path as a command-line input
                    string textFilePath = args[0];

                    ITextProcesser tokenizer = new TextFileProcesser(textFilePath);

                    int option;

                    do
                    {
                        Console.WriteLine("Press 1 to tokenize");
                        Console.WriteLine("Press 2 to calculate word frequencies");
                        Console.WriteLine("Press 3 to calculate 2-gram frequencies");
                        Console.WriteLine("Press 4 to calculate palindrome frequencies");
                        Console.WriteLine("Press 0 to exit");

                        int.TryParse(Console.ReadLine(), out option);

                        if (option == 1)
                            _MeasureTime(() => _PrintTokens(tokenizer));
                        else if (option == 2)
                            _MeasureTime(() => _PrintWordFrequencies(tokenizer));
                        else if (option == 3)
                            _MeasureTime(() => _PrintTwoGramFrequencies(tokenizer));
                        else if (option == 4)
                            _MeasureTime(() => _PrintPalindromeFrequencies(tokenizer));

                        Console.WriteLine();

                    } while (option != 0);
                }                
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: {0}", ex.Message);
                }
            }
        }        

        private static void _MeasureTime(Action action)
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();

            action();

            watch.Stop();

            Console.WriteLine();
            Console.WriteLine("Elapsed time: {0}", watch.Elapsed);
        }

        private static void _PrintTokens(ITextProcesser tokenizer)
        {
            int count = 0;

            foreach (var token in tokenizer.Tokenize().Distinct())
            {
                Console.WriteLine(token);
                count++;
            }

            Console.WriteLine();
            Console.WriteLine("Total number of tokens: {0}", count);
        }

        private static void _PrintWordFrequencies(ITextProcesser tokenizer)
        {
            var wordFrequencies = tokenizer.ComputeWordFrequencies();

            foreach (var pair in wordFrequencies)
                Console.WriteLine(pair.Key + " - " + pair.Value);
        }

        private static void _PrintTwoGramFrequencies(ITextProcesser tokenizer)
        {
            var twoGramFrequencies = tokenizer.ComputeTwoGramFrequencies();

            foreach (var pair in twoGramFrequencies)
                Console.WriteLine(pair.Key + " - " + pair.Value);
        }

        private static void _PrintPalindromeFrequencies(ITextProcesser tokenizer)
        {
            var palindromeFrequencies = tokenizer.ComputePalindromeFrequencies();

            foreach (var pair in palindromeFrequencies)
                Console.WriteLine(pair.Key + " - " + pair.Value);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessingFunctions.Core.TextProcesser;

namespace TextProcessingFunctions
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length != 1)
                throw new ArgumentException("Invalid command-line arguments: provide a single argument with the text file path");

            // Takes the text file path as a command-line input
            string textFilePath = args[0];

            ITextProcesser tokenizer = new TextFileProcesser(textFilePath);

            _PrintWordFrequencies(tokenizer);
        }

        private static void _PrintTokens(ITextProcesser tokenizer)
        {
            var tokens = tokenizer.Tokenize();

            foreach (var token in tokens)
                Console.WriteLine(token);
        }

        private static void _PrintWordFrequencies(ITextProcesser tokenizer)
        {
            var wordFrequencies = tokenizer.ComputeWordFrequencies();

            foreach (var pair in wordFrequencies)
                Console.WriteLine(pair.Key + " - " + pair.Value);
        }
    }
}

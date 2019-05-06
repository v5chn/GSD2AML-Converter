﻿using System;

namespace Gsd2Aml.Cli
{
    internal class Util
    {
        private static string HelpText { get; } = $"{Environment.NewLine}GSD2AML Converter" +
                                                  $"{Environment.NewLine}" +
                                                  $"{Environment.NewLine}Converts a GSD-formatted file in an AML-formatted file." +
                                                  $"{Environment.NewLine}" +
                                                  $"{Environment.NewLine}Usage:" +
                                                  $"{Environment.NewLine}\tgsd2aml [-i | --input] <path-to-gsd-file> [options]" +
                                                  $"{Environment.NewLine}" +
                                                  $"{Environment.NewLine}Options:" +
                                                  $"{Environment.NewLine}\t-h, --help\t\tPrints this info and the converter's usage/options." +
                                                  $"{Environment.NewLine}\t-i, --input file\tThe path to the file which should be converted. Example: C:\\path\\to\\input\\file.xml" +
                                                  $"{Environment.NewLine}\t-o, --output file\tSets the path to the output file. Example: C:\\path\\to\\output\\file.amlx" +
                                                  $"{Environment.NewLine}\t\t\t\tIf nothing is specified default is: C:\\path\\to\\input\\file\\<timestamp>.amlx (OPTIONAL)" +
                                                  $"{Environment.NewLine}\t-s, --string\t\tPrints the generated AML XML file to stdout. No *.amlx file will be generated. (OPTIONAL)" +
                                                  $"{Environment.NewLine}Note:" +
                                                  $"{Environment.NewLine}\t--output and --string cannot be used together.";

        /// <summary>
        /// Prints the help text and exits the program.
        /// </summary>
        public static void PrintHelpText()
        {
            Console.WriteLine(HelpText);
            Environment.Exit(0);
        }
    }
}
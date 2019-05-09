﻿using Gsd2Aml.Lib.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace Gsd2Aml.Lib
{
    /// <summary>
    /// Compress an AML file and its resources to an .amlx package using the AML.Engine.
    /// </summary>
    public static class Compressor
    {
        private const string Gsd2AmlName = "GSD2AML";

        /// <summary>
        /// Creates a zip archive from a directory and the relevant GSD resources.
        /// </summary>
        /// <param name="amlFilePath">The path to the AML file which will be zipped.</param>
        /// <param name="destination">The directory you want to store the archive in including the name of the archive.amlx.</param>
        /// <param name="resources">An array of paths to the resources to be part of the .amlx package.</param>
        /// <param name="overwriteFile">A flag which indicates if the file should be overwritten if it exists.</param>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void Compress(string amlFilePath, string destination, string[] resources, bool overwriteFile = false)
        {
            if (string.IsNullOrEmpty(amlFilePath)) throw new ArgumentException("AML file path was null or empty.");
            if (string.IsNullOrEmpty(destination)) throw new ArgumentException("Destination file path was null or empty.");

            var tmpPath = CreateTmpDirectory(Gsd2AmlName);

            if (string.IsNullOrEmpty(tmpPath)) throw new IOException("Path to GSD2AML-Temp folder unexpectedly null or empty.");

            var amlFileName = Path.GetFileName(amlFilePath);
            if (string.IsNullOrEmpty(amlFileName)) throw new IOException("AML filename unexpectedly null or empty.");

            var amlFileTmpPath = Path.Combine(tmpPath, amlFileName);
            if (string.IsNullOrEmpty(amlFileTmpPath)) throw new IOException("Path to AML file in TMP directory unexpectedly null or empty.");

            try
            {
                foreach (var resource in resources)
                {
                    var fileName = Path.GetFileName(resource);
                    if (!string.IsNullOrEmpty(fileName)) CopyFile(resource, Path.Combine(tmpPath, fileName));
                    else throw new IOException($"Resource filename unexpectedly null or empty");
                }

                if (!string.IsNullOrEmpty(amlFileName)) CopyFile(amlFilePath, amlFileTmpPath);

                Zip(amlFileTmpPath, destination, resources, overwriteFile);

                Converter.Logger?.Log(LogLevel.Info, $"Successfully saved .amlx package {Path.GetFileName(destination)} to {destination}.");

                DeleteFolder(tmpPath);
            }
            catch (IOException e)
            {
                throw new IOException("Error while compressing the AML-file and the resources to the .amlx package.", e);
            }
        }

        /// <summary>
        /// Creates a zip archive from a directory.
        /// </summary>
        /// <param name="sourceAml">The directory you want to be zipped.</param>
        /// <param name="destination">The directory you want to store the zip archive in.</param>
        /// <param name="resources">An array of paths to the resources to be part of the .amlx package.</param>
        /// <param name="overwriteFile">A flag which indicates if the file should be overwritten if it exists.</param>
        /// <exception cref="IOException"></exception>
        private static void Zip(string sourceAml, string destination, IEnumerable<string> resources, bool overwriteFile)
        {
            if (overwriteFile)
            {
                File.Delete(destination);
            }
            else if (File.Exists(destination))
            {
                throw new IOException("Could not create the .amlx compressed file because the file should not be overwritten.");
            }

            try
            {
                using (var ac = new Aml.Engine.AmlObjects.AutomationMLContainer(destination))
                {
                    var root = ac.AddRoot(sourceAml, new Uri("/" + Path.GetFileName(sourceAml), UriKind.Relative));

                    foreach (var resource in resources)
                    {
                        var fileName = Path.GetFileName(resource);
                        if (string.IsNullOrEmpty(fileName)) continue;
                        var fileTmpPath = Path.Combine(Path.GetTempPath(), Gsd2AmlName, fileName);
                        var fileUri = new Uri("/" + fileName, UriKind.Relative);
                        ac.AddAnyContent(root, fileTmpPath, fileUri);

                    }

                    ac.Close();
                }
            }
            catch (IOException e)
            {
                throw new IOException("Could not create the .amlx compressed file.", e);
            }
        }

        /// <summary>
        /// Creates a new directory in the temporary folder of Windows.
        /// </summary>
        /// <param name="folderName">The name of the folder to be created.</param>
        /// <returns>The path to the directory as a string.</returns>
        /// <exception cref="IOException"></exception>
        private static string CreateTmpDirectory(string folderName)
        {
            try
            {
                var path = Path.Combine(Path.GetTempPath(), folderName);
                return CreateDirectory(path);
            }
            catch (IOException e)
            {
                throw new IOException("Could not create the the temporary directory which was created to compress the .amlx package.", e);
            }
        }

        /// <summary>
        /// Moves a file from one directory to another.
        /// </summary>
        /// <param name="source">The source path.</param>
        /// <param name="destination">The destination path.</param>
        /// <exception cref="IOException"></exception>
        private static void CopyFile(string source, string destination)
        {
            try
            {
                File.Copy(source, destination, true);
            }
            catch (IOException e)
            {
                throw new IOException("Could not copy a resource file to the temporary directory which was created to compress the .amlx package.", e);
            }
        }

        /// <summary>
        /// Deletes a folder if it exists.
        /// </summary>
        /// <param name="destination">The destination path.</param>
        /// <exception cref="IOException"></exception>
        private static void DeleteFolder(string destination)
        {
            try
            {
                if (Directory.Exists(destination))
                {
                    Directory.Delete(destination, true);
                }
            }
            catch (IOException e)
            {
                throw new IOException("Could not delete the temporary directory which was created to compress the .amlx package.", e);
            }
        }

        /// <summary>
        /// Creates a directory path.
        /// </summary>
        /// <param name="destination">The directory path.</param>
        /// <returns>Path to the created directory.</returns>
        /// <exception cref="IOException"></exception>
        private static string CreateDirectory(string destination)
        {
            var output = new DirectoryInfo(destination);

            if (string.IsNullOrEmpty(output.FullName) || output.Exists)
            {
                return output.Exists ? output.FullName : null;
            }

            try
            {
                return Directory.CreateDirectory(output.FullName).FullName;
            }
            catch (IOException e)
            {
                throw new IOException("Could not create destination output directory for creating the .amlx package.", e);
            }
        }
    }
}

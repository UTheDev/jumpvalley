﻿using System;
using System.Collections.Generic;
using Jumpvalley.IO;

namespace Jumpvalley.Testing
{
    /// <summary>
    /// Test class for making sure the <see cref="InfoFile"/> class works properly
    /// </summary>
    public partial class InfoFileTest
    {
        private InfoFile infoFile;
        private string name;

        public InfoFileTest(string infoFileText, string infoFileName = "test info file")
        {
            infoFile = new InfoFile(infoFileText);
            name = infoFileName;
        }

        public void PrintInfoFileData()
        {
            string printedInfoFileData = "";

            foreach (KeyValuePair<string, string> property in infoFile.Data)
            {
                printedInfoFileData += $"- The '{property.Key}' property has the value '{property.Value}'\n";
            }

            Console.WriteLine($"Test info file named '{name}' has this data:\n{printedInfoFileData}");
        }
    }
}

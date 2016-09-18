using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;

namespace ut_amc_automation
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                new CustomeFilebot().StartProcess(args);
            }
            else
            {
                MovieImporter.ImportMovies();
            }
        }
        
    }
}

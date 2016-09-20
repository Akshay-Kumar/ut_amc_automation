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
                if (args[0].ToLower().Contains("purge"))
                {
                    MovieImporter.PurgeMovies();
                    MovieImporter.ImportMovies();
                }
                else
                {
                    new CustomeFilebot().StartProcess(args);
                }
            }
            else
            {
                MovieImporter.ImportMovies();
            }
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logger;
using System.IO;
using System.Collections;
using SourceCodeDAL;
using System.Text.RegularExpressions;

namespace ut_amc_automation
{
    public class MovieImporter
    {
        static SourceDataAccessLayer dal = new SourceDataAccessLayer();
        static Dictionary<string, List<Movie>> movieDetails = new Dictionary<string, List<Movie>>();
        static List<string> recordsNotInserted = new List<string>();
        static int totalSize = 0;
        public static string[] GetMovieDetails(string movieDirectory)
        {
            string supportedExtensions = "*.jpg,*.gif,*.png,*.bmp,*.jpeg,*.ico,*.wmv,*.mp4,*.mkv,*.avi,*.flv,*.srt,*.nfo";
            List <string>fileList = new List<string>();
            foreach (string movieFile in Directory.GetFiles(movieDirectory, "*.*", SearchOption.AllDirectories).Where(s => supportedExtensions.Contains(Path.GetExtension(s).ToLower())))
            {
                fileList.Add(movieFile);
            }
            return fileList.ToArray<string>();
        }
        public static void PurgeMovies()
        {
            dal.PurgeMovies();
        }
        public static void ImportMovies()
        {
            string[] movieList = GetMovieDetails(@"G:\Media1\New Films");
            totalSize = movieList.Length;
            foreach (string movie in movieList)
            {
                int release_yr = 1900;
                DateTime insertDatetime = DateTime.Now;
                string directory = Path.GetDirectoryName(movie);
                string filepath = Path.GetFullPath(movie);
                string movietitle = Path.GetDirectoryName(movie);
                movietitle = movietitle.Substring(movietitle.LastIndexOf(@"\")+1);
                string resultString = Regex.Match(movietitle, @"\d{4}").Value;
                release_yr = Int32.Parse(resultString);
                List<Movie> movies;
                if (!movieDetails.TryGetValue(movietitle, out movies))
                {
                    movies = new List<Movie>();
                    movieDetails.Add(movietitle, movies);
                }
                Movie movieObj = new Movie();
                movieObj.Movie_title = movietitle;
                movieObj.Movie_directory = directory;
                movieObj.Release_year = release_yr;
                movieObj.Movie_file_name.Add(filepath);
                movieObj.InsertDate = insertDatetime;
                movies.Add(movieObj);
            }
            int rowInserted = 0;
            foreach (string movieT in movieDetails.Keys)
            {
                List<Movie> movies = movieDetails[movieT];
                try
                {
                    foreach (Movie movieObj in movies)
                    {
                        if (dal.InsertOrUpdateMovieData(movieObj))
                        {
                            ErrorLog.ErrorRoutine(string.Format("Inserted : {0}", movieT));
                            rowInserted++;
                        }
                        else
                        {
                            ErrorLog.ErrorRoutine(string.Format("Not inserted : {0}", movieT));
                            recordsNotInserted.Add(movieObj.Movie_title);
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    ErrorLog.ErrorRoutine(string.Format("Not inserted : {0}", movieT));
                    ErrorLog.ErrorRoutine(ex);
                }
            }
            if (rowInserted == totalSize)
            {
                ErrorLog.ErrorRoutine(string.Format("All {0} records are inserted.", rowInserted));
            }
            else
            {
                ErrorLog.ErrorRoutine(string.Format("{0} records are not inserted.", totalSize - rowInserted));
                ErrorLog.ErrorRoutine(string.Format("Following records are not inserted: "));
                int i = 1;
                foreach (string r in recordsNotInserted)
                {
                    ErrorLog.ErrorRoutine(string.Format("{0}. {1}", i++, r));
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SourceCodeDAL
{
    public class Movie
    {
        public Movie()
        {
            movie_file_name = new ArrayList();
        }
        string movie_title = string.Empty;

        public string Movie_title
        {
            get { return movie_title; }
            set { movie_title = value; }
        }
        int release_year = 1900;

        public int Release_year
        {
            get { return release_year; }
            set { release_year = value; }
        }
        string movie_directory = string.Empty;

        public string Movie_directory
        {
            get { return movie_directory; }
            set { movie_directory = value; }
        }
        ArrayList movie_file_name = null;

        public ArrayList Movie_file_name
        {
            get { return movie_file_name; }
            set { movie_file_name = value; }
        }

        private DateTime insertDate;

        public DateTime InsertDate
        {
            get { return insertDate; }
            set { insertDate = value; }
        }
    }
}

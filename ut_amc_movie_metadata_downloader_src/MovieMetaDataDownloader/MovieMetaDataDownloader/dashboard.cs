using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Collections;

namespace MovieMetaDataDownloader
{
    public partial class dashboard : Form
    {
        static string Torrent = @"C:\Users\AkshayKumar\Downloads\Torrents";
        static string yifyUrl = @"https://yts.ag/movie/";
        static SortedDictionary<string, string> torrentUrlList = new SortedDictionary<string, string>();
        // Read the file and build the movie list
        static SortedDictionary<string, string> movieBook = new SortedDictionary<string, string>();
        string filePath = @"C:\Users\AkshayKumar\Downloads\movie.txt";
        
        public dashboard()
        {
            InitializeComponent();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            ReadFromFile(filePath);
        }
        public void ReadFromFile(string filepath)
        {
            StreamReader reader = new StreamReader(filepath);
            using (reader)
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    string[] entry = line.Split(new char[] { '|' });
                    string name = entry[0].Trim();
                    string url = name;
                    string year = entry[1].Trim();
                    url = url.ToLower();
                    url = url.Replace(" ", "-");
                    url = url.Replace(":","");
                    url = url.Replace("&", "-");
                    url = url + '-' + year;
                    name = url;
                    url = yifyUrl + url;
                    movieBook.Add(name,url);
                }
            }
            //Fetch torrent urls
            foreach (string movieName in movieBook.Keys)
            {
                DownloadFile(movieName,movieBook[movieName]);
            }
            foreach (string mName in torrentUrlList.Keys)
            {
                DownloadTorrentFile(mName, torrentUrlList[mName]);
            }
        }
        public void DownloadFile(string name,string url)
        {
            if (url == null || url.Length == 0)
            {
                throw new ApplicationException("Specify the URI of the resource to retrieve.");
            }
            WebClient client = new WebClient();
            Stream data = null;
            StreamReader reader = null;
            // Add a user agent header in case the 
            // requested URI contains a query.

            //client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            try
            {
                Uri uri = new Uri(url.Trim());
                client.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                client.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                client.UseDefaultCredentials = true;
                data = client.OpenRead(uri);
                reader = new StreamReader(data);
                string s = reader.ReadToEnd();
                string torrentLink = ScarapMovieUrl(s);
                torrentUrlList.Add(name, torrentLink);
                data.Close();
                reader.Close();
            }
            catch (Exception ex){}
        }
        public static string ScarapMovieUrl(string content)
        {
            int length = 0, start = -1, end = -1;
            string torrent = string.Empty;
            int index = content.IndexOf("Available in:");
            index = content.IndexOf("<a href", index);
            index = content.IndexOf("https://", index);
            start = index;
            index = content.IndexOf(".torrent", index);
            end = index + ".torrent".Length;
            length = (end - start);
            torrent = content.Substring(start, length);
            if (torrent.StartsWith("http") && torrent.EndsWith(".torrent"))
            {
                torrent = torrent.Trim();
            }
            else
            {
                torrent = string.Empty;
            }
            return torrent;
        }
        public void DownloadTorrentFile(string movieName,string movieUrl)
        {
            string file = string.Empty;
            if (movieUrl == null || movieUrl.Length == 0)
            {
                throw new ApplicationException("Specify the URI of the resource to retrieve.");
            }
            WebClient client = new WebClient();            
            try
            {
                Uri uri = new Uri(movieUrl);
                client.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                client.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                client.DownloadFileAsync(uri, file = Path.Combine(Torrent,movieName.Replace(" ","_").Replace(":","").Replace("&","_").Trim()+".torrent"));
                while (client.IsBusy)
                {
                    movieList.Items.Add(file);
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (UriFormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

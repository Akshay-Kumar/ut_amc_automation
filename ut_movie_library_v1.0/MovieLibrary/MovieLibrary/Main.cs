using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SourceCodeDAL;
using ExcelSheet=Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using Logger;

namespace MovieLibrary
{
    public partial class Main : Form
    {
        SourceDataAccessLayer dal = null;
        public static DataView dvMovieNames = null;
        DataView dvMovie = new DataView();
        public Main()
        {
            InitializeComponent();
            dal = new SourceDataAccessLayer();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mvData.Rows.Clear();
        }
        private void GetMovieDataStructrure()
        {
            DataTable dtMovie = dal.GetAllMovieData();
            dtMovie.TableName = "Movies";
            DataTable dt = PopulateData(dtMovie);
            dvMovie.Table = dt;
            mvData.DataSource = dvMovie;
            FormatDataGridViewCells(mvData);
        }
        private DataTable PopulateData(DataTable dtTemp)
        {
            DataTable dtFormatData = new DataTable("MovieData");
            dtFormatData.Columns.Add("movie_title", typeof(string));
            dtFormatData.Columns.Add("movie_directory", typeof(string));
            dtFormatData.Columns.Add("release_year", typeof(string));
            dtFormatData.Columns.Add("insert_datetime", typeof(string));
            dtFormatData.Columns.Add("movie_file_path", typeof(Image));
            int index = 0;
            int max = dtTemp.Rows.Count;
            toolStripProgressBar2.Maximum = max;

            foreach (DataRow row in dtTemp.Rows)
            {
                string movie_title = row[0].ToString().Trim();  //ie. id
                string movie_directory = row[1].ToString().Trim(); //ie. name
                string release_year = row[2].ToString().Trim();
                string insert_date_time = DateTime.ParseExact(NVL(row[3].ToString().Trim(),DateTime.Now.ToString()), "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).ToString();
                 //create new object of System.Drawing.Image class
                string picPath = row[4].ToString().Trim(); //ie. image path
                Image newImage = null;
                if (picPath.EndsWith(".jpg") || picPath.EndsWith(".png")
                    || picPath.EndsWith(".jpeg") || picPath.EndsWith(".JPG")
                    || picPath.EndsWith(".JPEG") || picPath.EndsWith(".PNG")
                    || picPath.EndsWith(".ico") || picPath.EndsWith(".ICO"))
                {
                    newImage = ResizeImage(picPath, 78, 80, true);
                }
                else
                {
                    newImage = ResizeImage(AppDomain.CurrentDomain.BaseDirectory.Replace(@"bin\Debug", "") + "\\image\\dialog_warning.png", 78, 80, true);
                }
                dtFormatData.Rows.Add(movie_title, movie_directory, release_year, insert_date_time, newImage);
                IncrementProgressBar(index++);
            }
            return dtFormatData;
        }
        private string NVL(string text,string replacement)
        {
            return (string.IsNullOrEmpty(text) ? replacement : text); 
        }
        public void IncrementProgressBar(int increment)
        {
            toolStripProgressBar2.Increment(increment);
        }
        public static void RemoveEmptyCells(DataGridView dgv,int rowIndex, int columnIndex)
        {
            DataGridViewCellStyle dataGridViewCellStyle =
                              new DataGridViewCellStyle();
            dataGridViewCellStyle.NullValue = null;
            dataGridViewCellStyle.Tag = "BLANK";
            dgv.Rows[rowIndex].Cells[columnIndex].Style = dataGridViewCellStyle;
        }
        public static void FormatDataGridViewCells(DataGridView dgv)
        {
            dgv.RowsDefaultCellStyle.BackColor = Color.Bisque;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.None;

            dgv.DefaultCellStyle.SelectionBackColor = Color.Red;
            dgv.DefaultCellStyle.SelectionForeColor = Color.Yellow;

            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //dgv.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToResizeColumns = false;
        }

        private void loadDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetMovieDataStructrure();
        }
        
        public static Image ResizeImage(string file,
                                     int width,
                                     int height,
                                     bool onlyResizeIfWider)
        {
            using (Image image = Image.FromFile(file))
            {
                // Prevent using images internal thumbnail
                image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                image.RotateFlip(RotateFlipType.Rotate180FlipNone);

                if (onlyResizeIfWider == true)
                {
                    if (image.Width <= width)
                    {
                        width = image.Width;
                    }
                }

                int newHeight = image.Height * width / image.Width;
                if (newHeight > height)
                {
                    // Resize with height instead
                    width = image.Width * height / image.Height;
                    newHeight = height;
                }

                Image NewImage = image.GetThumbnailImage(width,
                                                         newHeight,
                                                         null,
                                                         IntPtr.Zero);

                return NewImage;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void mvData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                StartProcess(mvData[e.ColumnIndex,e.RowIndex].Value.ToString().Trim());
            }
        }
        private void StartProcess(string path)
        {
            path = System.IO.Path.GetFullPath(path);
            ProcessStartInfo StartInformation = new ProcessStartInfo();
            StartInformation.FileName = path;
            Process process = Process.Start(StartInformation);
        }

        /// <summary>
        /// SearchProgramByKeywords
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="dataView"></param>
        /// <param name="keyWord"></param>
        public void SearchMovieByKeywords(DataGridView dataGridView, DataView dataView, string keyWord)
        {
            string filter = "";
            string[] keyWords = null;
            string column = "";
            string AND = " AND ";
            string OR = " OR ";
            StringBuilder sb = new StringBuilder("");

            if (!string.IsNullOrEmpty(keyWord))
            {
                keyWords = keyWord.Split(' ');
            }
            try
            {
                if (!string.IsNullOrEmpty(keyWord) && keyWords != null)
                {
                    foreach (string word in keyWords)
                    {
                        if (filter.Length == 0)
                        {
                            filter += "(";
                            for (int i = 0; i < dataView.ToTable().Columns.Count; i++)
                            {
                                if (column.Length == 0)
                                {
                                    column = dataView.ToTable().Columns[i].ColumnName;
                                    if(!column.Contains("movie_file_path"))
                                    filter += "(" + column + " LIKE " + "'%" + word + "%')";
                                }
                                {
                                    column = dataView.ToTable().Columns[i].ColumnName;
                                    if (!column.Contains("movie_file_path"))
                                    filter += OR + "(" + column + " LIKE " + "'%" + word + "%')";
                                }
                            }
                            filter += ")";
                            column = "";
                        }
                        else
                        {
                            filter += AND + "(";
                            for (int i = 0; i < dataView.ToTable().Columns.Count; i++)
                            {
                                if (column.Length == 0)
                                {
                                    column = dataView.ToTable().Columns[i].ColumnName;
                                    filter += "(" + column + " LIKE " + "'%" + word + "%')";
                                }
                                else
                                {
                                    column = dataView.ToTable().Columns[i].ColumnName;
                                    filter += OR + "(" + column + " LIKE " + "'%" + word + "%')";
                                }
                            }
                            filter += ")";
                        }
                    }
                }
                dataView.RowFilter = filter;
                dvMovieNames = dataView;
                dataGridView.DataSource = dataView;
                FormatDataGridViewCells(dataGridView);
                filter = string.Empty;
            }
            catch (Exception exHandler)
            {
                ErrorLog.ErrorRoutine(exHandler);
            }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            SearchMovieByKeywords(mvData, dvMovie, toolStripTextBox1.Text.Trim());
        }
    }
}

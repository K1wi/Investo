using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using System.Data.OleDb;
using System.Data;


namespace Investo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 



    public partial class MainWindow : Window
    {
        public List<string> CSV_FILE_PATHS = new List<string>();
        private Investo.dbFinancial_DataDataSet dbFDataSet;
        private Investo.dbFinancial_DataDataSetTableAdapters.tblShareTableAdapter tblAdaptShare;
        private Investo.dbFinancial_DataDataSetTableAdapters.tblDataTableAdapter tblAdaptData;


        public MainWindow()
        {

            InitializeComponent();

            resetlstbox_Dropped();


        }



        private void ListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
        private Brush makeBrush(string sColorHex)
        {
            BrushConverter bc = new BrushConverter();
            Brush result = (Brush)bc.ConvertFrom(sColorHex);
            result.Freeze();
            return (result);
        }


        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            // Green  : #998FEC21
            Brush GreenBrush = makeBrush("#998FEC21");
            // Yellow : #99F4FF17
            Brush YellowBrush = makeBrush("#99F4FF17");
            // Orange : #99F7A938
            Brush OrangeBrush = makeBrush("#99F7A938");
            // Red    : #99F12020
            Brush RedBrush = makeBrush("#99F12020");

            List<DataTable> Tablelist = new List<DataTable>();

            DataTable dbShareTable = new DataTable();
            dbShareTable.Columns.Add("ID");
            dbShareTable.Columns.Add("Share_Code");
            dbShareTable.Columns.Add("Share_Name");
            dbShareTable.Columns.Add("Market_Code");
            dbShareTable.Columns.Add("Market_Country");

            DataTable csvDataTable = new DataTable();
            csvDataTable.Columns.Add("Date");
            csvDataTable.Columns.Add("Open");
            csvDataTable.Columns.Add("High");
            csvDataTable.Columns.Add("Low");
            csvDataTable.Columns.Add("Close");
            csvDataTable.Columns.Add("Volume");

            DataTable dbDataTable = new DataTable();
            dbDataTable.Columns.Add("ID");
            dbDataTable.Columns.Add("FK");
            dbDataTable.Columns.Add("Date");
            dbDataTable.Columns.Add("Open");
            dbDataTable.Columns.Add("High");
            dbDataTable.Columns.Add("Low");
            dbDataTable.Columns.Add("Close");
            dbDataTable.Columns.Add("Volume");


            TextBlock tmpTxtBlock;

            string sCode, dbStartDate, dbEndDate;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string[] fields = null;
            DateTime DatabaseEndDate = DateTime.MinValue, DatabaseStartDate = DateTime.MinValue, csvStartDate = DateTime.MinValue, csvEndDate = DateTime.MinValue;
            // lstbox_Dropped.Items.Add("?" + lstbox_Share_Code.Items[0].ToString() + "?");
            //For every file dropped on the listbox
            foreach (string fileName in files)
            {
                sCode = System.IO.Path.GetFileNameWithoutExtension(fileName);
                sCode = sCode.ToUpper();
                // if the file is a .csv file
                if (System.IO.Path.GetExtension(fileName) == ".csv")
                {
                    tmpTxtBlock = new TextBlock();
                    string[] cvsRows = System.IO.File.ReadAllLines(fileName);

                    tmpTxtBlock.Text = sCode + "\t - \t" + fileName;
                 //   int tal = 300;
                  //  tal -= fileName.Length;
                  //  tmpTxtBlock.Text += " " + tal.ToString();

                    // for each row in the current file
                    foreach (string cvsRow in cvsRows.Skip(1))
                    {
                        if (cvsRow.Contains(",,"))
                        {
                            Console.WriteLine("ERROR " + cvsRow + " has empty entries");
                            break;
                        }
                        fields = cvsRow.Split(',');
                        DataRow row = csvDataTable.NewRow();
                        row.ItemArray = fields;
                        csvDataTable.Rows.Add(row);
                    }// end for each row in current file

                    Tablelist.Add(csvDataTable);
                    CSV_FILE_PATHS.Add(fileName);

                    csvEndDate = DateTime.ParseExact(csvDataTable.Rows[0][0].ToString(), "dd-MMM-yy",
                        System.Globalization.CultureInfo.InvariantCulture);
                    csvStartDate = DateTime.ParseExact(csvDataTable.Rows[csvDataTable.Rows.Count - 1][0].ToString(), "dd-MMM-yy",
                        System.Globalization.CultureInfo.InvariantCulture);

                    dbShareTable = tblAdaptShare.GetDataByCode(sCode);
                    // if the share is in the database
                    if (dbShareTable.Rows.Count > 0)
                    {
                        if (dbShareTable.Rows.Count > 1)    // More than one share with same code (i.e JSE:FSR and Nasdaq:FSR)
                            foreach (DataRow r in dbShareTable.Rows)
                                Console.WriteLine("There is more than one with " + sCode + " namely: " + r[1]);

                        Console.WriteLine("The ID that is Found : " + Convert.ToInt32(dbShareTable.Rows[0][0]).ToString());

                        dbDataTable = tblAdaptData.GetDataByID(Convert.ToInt32(dbShareTable.Rows[0][0]));
                        // if there are data available for the current share
                        if (dbDataTable.Rows.Count > 0)
                        {
                            Console.WriteLine("Number of data rows for share " + sCode + " : " + dbDataTable.Rows.Count.ToString());
                            //  foreach (DataRow r in dbDataTable.Rows)
                            //    Console.WriteLine("Date : " + r[2] + "; Open : " + r[3] + "; Hihg : " + r[4] + "; Low : " + r[5] + "; Close : " + r[6] + "; Volume : " + r[7]);

                            dbStartDate = dbDataTable.Rows[0][2].ToString();
                            dbEndDate = dbDataTable.Rows[dbDataTable.Rows.Count - 1][2].ToString();

                            Console.WriteLine("dbStartDate : " + dbStartDate);
                            Console.WriteLine("dbEndDate : " + dbEndDate);


                            DatabaseStartDate = DateTime.Parse(dbDataTable.Rows[0][2].ToString());
                            DatabaseEndDate = DateTime.Parse(dbDataTable.Rows[dbDataTable.Rows.Count - 1][2].ToString());


                            //    if (Convert.ToDateTime(dbEndDate) <= Convert.ToDateTime("tmpStartDate"))
                            //    {
                            //    }
                            if (DatabaseEndDate <= csvEndDate)
                            {
                                if (DatabaseEndDate <= csvStartDate)
                                    tmpTxtBlock.Background = RedBrush;
                                else
                                    tmpTxtBlock.Background = YellowBrush;

                            }
                            else if (DatabaseStartDate >= csvStartDate)
                            {
                                if (DatabaseStartDate >= csvEndDate)
                                    tmpTxtBlock.Background = RedBrush;
                                else
                                    tmpTxtBlock.Background = YellowBrush;
                            }
                            else
                                tmpTxtBlock.Background = GreenBrush;
                        }
                        else // No data for the share in DB 
                        {
                            dbStartDate = "-1";
                            dbEndDate = "-1";
                            DatabaseStartDate = DateTime.MinValue;
                            DatabaseEndDate = DateTime.MinValue;
                            tmpTxtBlock.Background = YellowBrush;
                        }

                    }// end if share not in database
                    else // Share not in DB yet
                    {
                        tmpTxtBlock.Background = OrangeBrush;

                    }


                    tmpTxtBlock.FontWeight = FontWeights.Bold;
                    lstbox_Dropped.Items.Add(tmpTxtBlock);


                    /*   if (csvStartDate == DateTime.MinValue)
                           lstbox_Dropped.Items.Add("csv starting date : -");//+ dbStartDate);
                       else
                           lstbox_Dropped.Items.Add("csv starting date : " + csvStartDate.ToShortDateString());
                    */
                    if (csvStartDate == DateTime.MinValue || csvEndDate == DateTime.MinValue)
                        lstbox_Dropped.Items.Add(".csv Date range \t NO DATA FROM CSV");
                    else
                        lstbox_Dropped.Items.Add(".csv Date range \t " + csvStartDate.ToShortDateString() + " - " + csvEndDate.ToShortDateString());

                    /*   if (csvEndDate == DateTime.MinValue)
                           lstbox_Dropped.Items.Add("csv end date : -");//+ dbStartDate);
                       else
                           lstbox_Dropped.Items.Add("csv end date : " + csvEndDate.ToShortDateString());
                    */

                    if (DatabaseStartDate == DateTime.MinValue || DatabaseEndDate == DateTime.MinValue)
                        lstbox_Dropped.Items.Add("DB Date range \t No data in DB");
                    else
                        lstbox_Dropped.Items.Add("DB Date range \t " + DatabaseStartDate.ToShortDateString() + " - " + DatabaseEndDate.ToShortDateString());

                    dbDataTable.Clear();
                    dbShareTable.Clear();
                    DatabaseStartDate = DateTime.MinValue;
                    DatabaseEndDate = DateTime.MinValue;
                }// end if the file is .csv file


            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            DataTable dtCountry = new DataTable();
            DataTable dtShare = new DataTable();
            DataTable dtMarket = new DataTable();
            DataTable dtData = new DataTable();

            dtShare = tblAdaptShare.GetData();
            dtData = tblAdaptData.GetData();

            DataRow[] dr = dtCountry.Select("ID = 1");
            if (dr.Length > 0)
                foreach (DataRow row in dr)
                {
                    Console.WriteLine("Found the row with vales :\t" + row[0] + "\t" + row[1]);
                }


            Console.WriteLine(dtCountry.Columns[0] + "\t" + dtCountry.Columns[1]);


            foreach (DataRow row in dtCountry.Rows)
            {
                Console.WriteLine(row[0] + "\t" + row[1]);

            }
            Console.WriteLine(dtCountry.Rows.Count);


        }


        private void resetlstbox_Dropped()
        {
            lstbox_Dropped.Items.Clear();
            TextBlock s = new TextBlock();
            s.Text = "FILE\t   \tPATH";
            // s.Background = Brushes.Yellow;
            // s.Opacity = 0.9;

            s.FontWeight = FontWeights.UltraBold;
            lstbox_Dropped.Items.Add(s);


        }

        private void HomeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dbFDataSet = ((Investo.dbFinancial_DataDataSet)(this.FindResource("dbFinancial_DataDataSet")));

            /*    // Load data into the table tblCountry. You can modify this code as needed.
                tblAdaptCountry = new Investo.dbFinancial_DataDataSetTableAdapters.tblCountryTableAdapter();
                tblAdaptCountry.Fill(dbFDataSet.tblCountry);
                tblCountryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("tblCountryViewSource")));
                tblCountryViewSource.View.MoveCurrentToFirst();
    */

            // Load data into the table tblCountry. You can modify this code as needed.
            tblAdaptShare = new Investo.dbFinancial_DataDataSetTableAdapters.tblShareTableAdapter();
            tblAdaptShare.Fill(dbFDataSet.tblShare);


            // Load data into the table tblCountry. You can modify this code as needed.
            tblAdaptData = new Investo.dbFinancial_DataDataSetTableAdapters.tblDataTableAdapter();
            tblAdaptData.Fill(dbFDataSet.tblData);
            /*    // Load data into the table tblShares. You can modify this code as needed.
                tblAdaptShare = new Investo.dbFinancial_DataDataSetTableAdapters.tblShareTableAdapter();
                tblAdaptShare.Fill(dbFinancial_DataDataSet.tblShare);
                System.Windows.Data.CollectionViewSource tblShareViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("tblShareViewSource")));
                tblShareViewSource.View.MoveCurrentToFirst();
             * */
        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            resetlstbox_Dropped();
        }

        private void lstbox_Share_Code_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstbox_Dropped.Items.Add("?" + "?");
        }

        private void lstbox_Dropped_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstbox_Dropped.SelectedIndex = -1;
        }

     

       
       


    }

}

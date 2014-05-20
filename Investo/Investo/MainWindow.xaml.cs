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
        private Investo.dbFinancial_DataDataSetTableAdapters.tblCountryTableAdapter tblAdaptCountry;
        private Investo.dbFinancial_DataDataSetTableAdapters.tblMarketTableAdapter tblAdaptMarket;
        private Investo.dbFinancial_DataDataSetTableAdapters.tblShareTableAdapter tblAdaptShare;
        private Investo.dbFinancial_DataDataSetTableAdapters.tblDataTableAdapter tblAdaptData;
        private System.Windows.Data.CollectionViewSource tblCountryViewSource;


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
        private Brush makeBrush (string sColorHex)
        {
            BrushConverter bc = new BrushConverter();
            Brush result = (Brush)bc.ConvertFrom(sColorHex);
            result.Freeze();
            return(result);
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
            DataTable myTable = new DataTable();



            List<DataTable> Tablelist = new List<DataTable>();
            myTable.Columns.Add("Date");
            myTable.Columns.Add("Open");
            myTable.Columns.Add("High");
            myTable.Columns.Add("Low");
            myTable.Columns.Add("Close");
            myTable.Columns.Add("Volume");
           

            string sCode;
            int val;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
           // lstbox_Dropped.Items.Add("?" + lstbox_Share_Code.Items[0].ToString() + "?");
            foreach (string fileName in files)
            {
                sCode = System.IO.Path.GetFileNameWithoutExtension(fileName);
                sCode = sCode.ToUpper();
                if (System.IO.Path.GetExtension(fileName) == ".csv")
                {
                    string[] cvsRows = System.IO.File.ReadAllLines(fileName);
                   
                    lstbox_Dropped.Items.Add(cvsRows[0]);
                    lstbox_Dropped.Items.Add(cvsRows[1]);
                    lstbox_Dropped.Items.Add(cvsRows[2]);
                    string[] fields = null;
                    foreach (string cvsRow in cvsRows.Skip(1))
                    {
                        
                        fields = cvsRow.Split(',');
                        DataRow row = myTable.NewRow();
                        row.ItemArray = fields;
                        myTable.Rows.Add(row);
                    }
                    
                   
                    
                    CSV_FILE_PATHS.Add(fileName);
                   
                    

                    val = (int)tblAdaptShare.sqlCheckIfCodeExists(sCode);
                    TextBlock tmpTxtBlock = new TextBlock();
                    tmpTxtBlock.Text = sCode + "\t - \t" + fileName + "\t\t\t\t\t\t\t";

                    if (val == 0)
                        tmpTxtBlock.Background = RedBrush;
                    else
                    {
                        tmpTxtBlock.Background = GreenBrush;
                    }

                    //tmpTxtBlock.Opacity = 0.9;

                    tmpTxtBlock.FontWeight = FontWeights.Bold;

                    lstbox_Dropped.Items.Add(tmpTxtBlock);
                }
             

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            DataTable dtCountry = new DataTable();
            DataTable dtShare = new DataTable();
            DataTable dtMarket = new DataTable();
            DataTable dtData = new DataTable();

            dtCountry = tblAdaptCountry.GetData();
            dtShare = tblAdaptShare.GetData();
            dtMarket = tblAdaptMarket.GetData();
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

            // Load data into the table tblCountry. You can modify this code as needed.
            tblAdaptCountry = new Investo.dbFinancial_DataDataSetTableAdapters.tblCountryTableAdapter();
            tblAdaptCountry.Fill(dbFDataSet.tblCountry);
            tblCountryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("tblCountryViewSource")));
            tblCountryViewSource.View.MoveCurrentToFirst();

            // Load data into the table tblCountry. You can modify this code as needed.
            tblAdaptMarket = new Investo.dbFinancial_DataDataSetTableAdapters.tblMarketTableAdapter();
            tblAdaptMarket.Fill(dbFDataSet.tblMarket);


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
            lstbox_Dropped.Items.Add("?"  + "?");
        }

        private void lstbox_Dropped_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }


    }

}

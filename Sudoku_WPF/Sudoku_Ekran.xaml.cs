using Microsoft.SharePoint.Client;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sudoku_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DataTable dataTable_Sudoku = new DataTable();

        public MainWindow()
        {
            InitializeComponent();

            dataTable_Sudoku.Columns.Add("0");
            dataTable_Sudoku.Columns.Add("1");
            dataTable_Sudoku.Columns.Add("2");
            dataTable_Sudoku.Columns.Add("3");
            dataTable_Sudoku.Columns.Add("4");
            dataTable_Sudoku.Columns.Add("5");
            dataTable_Sudoku.Columns.Add("6");
            dataTable_Sudoku.Columns.Add("7");
            dataTable_Sudoku.Columns.Add("8");
            dataTable_Sudoku.Columns.Add("9");

            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "", "");
            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "", "");
            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "", "");
            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "", "");
            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "", "");
            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "", "");
            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "", "");
            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "", "");
            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "", "");
            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "", "");

            dataGrid_Sudoku.ItemsSource = dataTable_Sudoku.DefaultView;

        }


        private void Tabloyu_Hazirla()
        {
            //içeriği yatayda hizala
            var cellStyle = new Style(typeof(DataGridCell)) { Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) } };
            dataGrid_Sudoku.CellStyle = cellStyle;

            //hücre boyutlarını hizala ve yazı boyutu ata
            for (int i = 0; i < dataGrid_Sudoku.Items.Count ; i++)
            {
                for (int j = 0; j < dataGrid_Sudoku.Columns.Count; j++)
                {
                    DataGridCell cell = Hucreyi_Getir(i, j);
                    TextBlock tb = cell.Content as TextBlock;

                    cell.Width = 40;
                    cell.Height = 40;

                    cell.FontSize = 20;

                }
            }
        }

        private void Puzzle_Olustur()
        {
            dataTable_Sudoku.Rows[0][0] = 2;
            dataTable_Sudoku.Rows[0][1] = 1;
            dataTable_Sudoku.Rows[0][2] = 3;
            dataTable_Sudoku.Rows[0][3] = 4;
            dataTable_Sudoku.Rows[0][4] = 5;


        }

        private void Tamamla_Click(object sender, RoutedEventArgs e)
        {
     
            int sayac = 0;

            for (int i = 0; i < dataGrid_Sudoku.Items.Count ; i++)
            {
                for (int j = 0; j < dataGrid_Sudoku.Columns.Count; j++)
                {
                    DataGridCell cell = Hucreyi_Getir(i, j);
                    TextBlock tb = cell.Content as TextBlock;
                    
                    if (tb.Text != "")
                    {
                        sayac++;

                    }

                }
            }
            MessageBox.Show(sayac.ToString());
        }

        private DataGridCell Hucreyi_Getir(int satir, int sutun)
        {
            DataGridRow rowData = Hucreyi_Getir(satir);
            if (rowData != null)
            {
                DataGridCellsPresenter cellPresenter = GetVisualChild<DataGridCellsPresenter>(rowData);
                DataGridCell cell = (DataGridCell)cellPresenter.ItemContainerGenerator.ContainerFromIndex(sutun);
                if (cell == null)
                {
                    dataGrid_Sudoku.ScrollIntoView(rowData, dataGrid_Sudoku.Columns[sutun]);
                    cell = (DataGridCell)cellPresenter.ItemContainerGenerator.ContainerFromIndex(sutun);
                }
                return cell;
            }
            return null;
        }

        private DataGridRow Hucreyi_Getir(int index)
        {
            DataGridRow row = (DataGridRow)dataGrid_Sudoku.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                dataGrid_Sudoku.UpdateLayout();
                dataGrid_Sudoku.ScrollIntoView(dataGrid_Sudoku.Items[index]);
                row = (DataGridRow)dataGrid_Sudoku.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Tabloyu_Hazirla();

            Puzzle_Olustur();

        }
    }
}




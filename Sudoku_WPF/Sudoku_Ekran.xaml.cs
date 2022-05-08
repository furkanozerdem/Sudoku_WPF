using Microsoft.SharePoint.Client;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
using System.Windows.Threading;
using System.IO;



namespace Sudoku_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DataTable dataTable_Sudoku = new DataTable();
        bool puzzle_olusturuldu_mu = false;
        int son_cozulen_puzzle_id = 0;

        /// <summary>
        /// Sudoku tablosunda bu değerler dışında değer varsa tamamlanmayacak.
        /// </summary>
        List<string> gecerli_degerler = new List<string>()
        {
            "1","2","3","4","5","6","7","8","9"
        };


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

            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "");
            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "");
            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "");
            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "");
            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "");
            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "");
            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "");
            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "");
            dataTable_Sudoku.Rows.Add("", "", "", "", "", "", "", "", "");

            dataGrid_Sudoku.ItemsSource = dataTable_Sudoku.DefaultView;

            puzzle_id_bilgi.Visibility = Visibility.Collapsed;

        }

        private void Tabloyu_Hazirla()
        {
            //içeriği yatayda hizala
            var cellStyle = new Style(typeof(DataGridCell)) { Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) } };
            dataGrid_Sudoku.CellStyle = cellStyle;

            //hücre boyutlarını hizala ve yazı boyutu ata
            for (int i = 0; i < dataGrid_Sudoku.Items.Count; i++)
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

        /// <summary>
        /// Verilerin tabloya aktarılacağı fonksyion
        /// </summary>
        private void Puzzle_Olustur()
        {

            string puzzle_json_icerigi = Puzzle_Dosyasini_Oku();

            //dosyada veri yok
            if (puzzle_json_icerigi == "")
            {
                MessageBox.Show("Puzzle listesi boş. Lütfen listeyi doldurun.");
                puzzle_olusturuldu_mu = false;
                return;
            }

            //dosyada veriler mevcut
            else
            {

                List<Model_Puzzle> puzzle_listesi = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Model_Puzzle>>(puzzle_json_icerigi);

                Model_Puzzle goruntulenecek_puzzle = puzzle_listesi[son_cozulen_puzzle_id];

                List<List<int>> satir_listesi = new List<List<int>>();

                satir_listesi.Add(goruntulenecek_puzzle.satir_1);
                satir_listesi.Add(goruntulenecek_puzzle.satir_2);
                satir_listesi.Add(goruntulenecek_puzzle.satir_3);
                satir_listesi.Add(goruntulenecek_puzzle.satir_4);
                satir_listesi.Add(goruntulenecek_puzzle.satir_5);
                satir_listesi.Add(goruntulenecek_puzzle.satir_6);
                satir_listesi.Add(goruntulenecek_puzzle.satir_7);
                satir_listesi.Add(goruntulenecek_puzzle.satir_8);
                satir_listesi.Add(goruntulenecek_puzzle.satir_9);

                for (int i = 0; i < dataTable_Sudoku.Columns.Count; i++)
                {
                    for (int j = 0; j < dataTable_Sudoku.Rows.Count; j++)
                    {
                        //0 yazan ifadeler boş hücreleri temsil eder
                        if (satir_listesi[i][j] != 0)
                        {
                            dataTable_Sudoku.Rows[i][j] = satir_listesi[i][j];
                        }
                    }
                }

                puzzle_id_bilgi.Visibility = Visibility.Visible;
                puzzle_id_bilgi.Text = "Çözülen Puzzle Numarası : " + son_cozulen_puzzle_id;

                son_cozulen_puzzle_id++;
                puzzle_olusturuldu_mu = true;
            }
        }

        /// <summary>
        /// Proje dizininde bulunan json dosyasını okur, bu dosya puzzle listesini içerir.
        /// </summary>
        /// <returns></returns>
        public string Puzzle_Dosyasini_Oku()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string proje_yolu = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

            string yol = proje_yolu + @"\Puzzle_Listesi.json";

            if (System.IO.File.Exists(yol))
            {
                string all_text = System.IO.File.ReadAllText(yol);
                return all_text;
            }

            return "";
        }

        /// <summary>
        /// Oyunu bitirecek fonksiyon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tamamla_Click(object sender, RoutedEventArgs e)
        {
            string[,] doldurulan_tablo_degerleri = new string[9, 9];

            for (int i = 0; i < dataGrid_Sudoku.Items.Count; i++)
            {
                for (int j = 0; j < dataGrid_Sudoku.Columns.Count; j++)
                {
                    DataGridCell cell = Hucreyi_Getir(i, j);
                    TextBlock tb = cell.Content as TextBlock;

                    string hucre_icerigi_str = tb.Text;

                    //yanlışlıkla yazılan boşlukları sil
                    hucre_icerigi_str = hucre_icerigi_str.Replace(" ", "");

                    if (hucre_icerigi_str == "")
                    {
                        MessageBox.Show("Lütfen puzzle ı bitirdiğinize emin olun.");
                        return;
                    }

                    if (false == gecerli_degerler.Contains(hucre_icerigi_str))
                    {
                        MessageBox.Show("Lütfen geçersiz değer girmediğinize emin olun. (1-9 a kadar sayı girilmeli)");
                        return;
                    }
                    doldurulan_tablo_degerleri[i, j] = hucre_icerigi_str;
                }
            }

            bool cozum_dogru_mu = Puzzle_Dogrulugunu_Kontrol_Et(doldurulan_tablo_degerleri);

            //çözüm kontrol edilecek.
            if (true == cozum_dogru_mu)
            {
                MessageBox.Show("Tebrikler!");
            }
            else
            {
                MessageBox.Show("Çözüm yanlış. Lütfen devam ediniz.");
            }

        }

        private bool Puzzle_Dogrulugunu_Kontrol_Et(string[,] doldurulan_tablo_degerleri)
        {
            //satır
            for (int i = 0; i < doldurulan_tablo_degerleri.GetLength(0)-1; i++)
            {
                //sütun
                for (int j = 0; j < doldurulan_tablo_degerleri.GetLength(1)-1; j++)
                {
                    if (true == Satirda_Ayni_Deger_Var_Mi(doldurulan_tablo_degerleri, i, j+1))
                    {
                        return false;
                    }
                    if (true == Sutunda_Ayni_Deger_Var_Mi(doldurulan_tablo_degerleri, i, j+1))
                    {
                        return false;
                    }
                }
            }
            //çözüm doğru
            return true;
        }

        private bool Satirda_Ayni_Deger_Var_Mi(string[,] doldurulan_tablo_degerleri, int satir, int sutun)
        {
            string kontrol_edilecek_deger = doldurulan_tablo_degerleri[satir, sutun];
            for (int j = 0; j < 9; j++)
            {
                if(kontrol_edilecek_deger == doldurulan_tablo_degerleri[satir,j] && j != sutun)
                {
                    return true;
                }

            }
            return false;
        }

        private bool Sutunda_Ayni_Deger_Var_Mi(string[,] doldurulan_tablo_degerleri, int satir, int sutun)
        {
            string kontrol_edilecek_deger = doldurulan_tablo_degerleri[satir, sutun];
            for (int i = 0; i < 9; i++)
            {
                if(kontrol_edilecek_deger == doldurulan_tablo_degerleri[i, sutun] && i != satir)
                {
                    return true;
                }
            }
            return false;
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

            Basla.IsEnabled = true;

            Tamamla.IsEnabled = false;
        }

        /// <summary>
        /// Oyunu başlatacak fonksiyon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Basla_Click(object sender, RoutedEventArgs e)
        {
            Puzzle_Olustur();

            if (true == puzzle_olusturuldu_mu)
            {
                Sayac_Baslat();

                //Butonu pasifize et
                Basla.IsEnabled = false;

                //Tamamlama butonunu aktif et
                Tamamla.IsEnabled = true;
            }
        }

        private void Sayac_Baslat()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();

            //Başlama zamanı
            DateTime currentTime = DateTime.Now;
            dispatcherTimer.Tick += (sender, e) => { dispatcherTimer_Tick(sender, e, currentTime); };

            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e, DateTime baslama_zamani)
        {
            DateTime anlik_zaman = DateTime.Now;

            string saniye = (anlik_zaman - baslama_zamani).TotalSeconds.ToString("F0");

            TextBlock_Timer.Text = "Geçen Süre (sn) : " + saniye;
        }
    }
}




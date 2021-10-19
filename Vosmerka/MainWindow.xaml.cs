
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Vosmerka
{
    public partial class Product
    {

        public Uri ImagePreviewService
        {
            get
            {
                var imageName = System.IO.Path.Combine(Environment.CurrentDirectory, Image ?? "");
                return System.IO.File.Exists(imageName) ? new Uri(imageName) : new Uri("pack://application:,,,/products/picture.jpg");
            }
        }

        public string MinCostDiscount
        {
            get
            {
                return (MinCostForAgent.ToString("#.##"));
            }
        }



    }
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private IEnumerable<Product> _ProductLIst;

        public IEnumerable<Product> ProductList
        {
            get
            {
                var res = _ProductLIst;

                // у объекта "Все улицы" Id=0, т.к. он взят не из базы, а создан в приложении
                // если выбрана улица, то выбираем только объекты с такой улицей
                if (_StreetFilterValue > 0)
                    res = res.Where(ai => ai.ProductTypeID == _StreetFilterValue);

                if (SortAsc) res = res.OrderBy(ai => ai.MinCostDiscount);
                else res = res.OrderByDescending(ai => ai.MinCostDiscount);
                if (SearchFilter != "")
                    res = res.Where(ai => ai.ProductType.Title.IndexOf(SearchFilter, StringComparison.OrdinalIgnoreCase) >= 0);


                return res;


            }
            set
            {
                _ProductLIst = value;
            }
        }

        public List<ProductType> StreetsList { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            ProductList = Class1.DB.Product.ToList();
            StreetsList = Class1.DB.ProductType.ToList();
            StreetsList.Insert(0, new ProductType { Title = "Все типы материала" });
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private bool _SortAsc = true;
        public bool SortAsc
        {
            get
            {
                return _SortAsc;
            }
            set
            {
                _SortAsc = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProductList"));
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            SortAsc = (sender as RadioButton).Tag.ToString() == "1";
        }


        private int _StreetFilterValue = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public int StreetFilterValue
        {
            get
            {
                return _StreetFilterValue;
            }
            set
            {
                _StreetFilterValue = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProductList"));
                }
            }
        }

        private void StreetFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StreetFilterValue = (StreetFilter.SelectedItem as ProductType).ID;
        }
        private string _SearchFilter = "";
        public string SearchFilter
        {
            get
            {
                return _SearchFilter;
            }
            set
            {
                _SearchFilter = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProductList"));
                }
            }
        }
        public int SelectedRows
        {
            get
            {
                return ProductList.Count();
            }
        }

        public int TotalRows
        {
            get
            {
                return _ProductLIst.Count();
            }
        }
        private void Invalidate()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("ProductList"));
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedRows"));
                PropertyChanged(this, new PropertyChangedEventArgs("TotalRows"));
            }
        }

        private void SearchFilter_KeyUp(object sender, KeyEventArgs e)
        {
            SearchFilter = SearchFilterTextBox.Text;
        }

    }
}

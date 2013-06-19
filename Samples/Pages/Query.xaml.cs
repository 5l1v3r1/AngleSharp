﻿using Samples.ViewModels;
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

namespace Samples.Pages
{
    /// <summary>
    /// Interaction logic for Query.xaml
    /// </summary>
    public partial class Query : Page
    {
        QueryViewModel vm;

        public Query()
        {
            InitializeComponent();
            vm = new QueryViewModel();
            DataContext = vm;
        }

        private void UrlKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                vm.Load(Url.Text);
        }

        private void Button_Click(Object sender, RoutedEventArgs e)
        {
            vm.Load(Url.Text);
        }
    }
}

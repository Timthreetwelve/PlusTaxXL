// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System.Windows;
using System.Windows.Media;

namespace PlusTaxXL
{
    public partial class TitleWindow : Window
    {
        public TitleWindow()
        {
            InitializeComponent();

            double curZoom = UserSettings.Setting.GridZoom;
            TitleWindowGrid.LayoutTransform = new ScaleTransform(curZoom, curZoom);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region Using directives
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
#endregion

namespace PlusTaxXL
{
    public partial class MainWindow : Window
    {
        private readonly Tax tax = new Tax();

        public MainWindow()
        {
            UserSettings.Init(UserSettings.AppFolder, UserSettings.DefaultFilename, true);
            InitializeComponent();
            DataContext = tax;
            ReadSettings();
        }

        #region Read settings from config file
        private void ReadSettings()
        {
            // Settings change event
            UserSettings.Setting.PropertyChanged += UserSettingChanged;

            // Window position
            Top = UserSettings.Setting.WindowTop;
            Left = UserSettings.Setting.WindowLeft;
            Topmost = UserSettings.Setting.KeepOnTop;

            // Set data grid zoom level
            double curZoom = UserSettings.Setting.GridZoom;
            Grid1.LayoutTransform = new ScaleTransform(curZoom, curZoom);

            // Tax rate
            tax.TaxRate = UserSettings.Setting.TaxRate;
            ShowHideTaxRate(UserSettings.Setting.ShowTaxRate);

            // Show version in title bar
            WindowTitleVersion();
        }
        #endregion

        #region Menu Events
        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void EditTitle_Click(object sender, RoutedEventArgs e)
        {
            TitleWindow tw = new TitleWindow
            {
                Owner = this
            };
            tw.ShowDialog();
        }

        private void GridSmaller_Click(object sender, RoutedEventArgs e)
        {
            GridSmaller();
        }
        private void GridLarger_Click(object sender, RoutedEventArgs e)
        {
            GridLarger();
        }
        private void GridReset_Click(object sender, RoutedEventArgs e)
        {
            GridSizeReset();
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            About about = new About
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            _ = about.ShowDialog();
        }
        #endregion Menu Events

        #region Textbox Events
        private void TbxPreviewTextInp(object sender, TextCompositionEventArgs e)
        {
            // Only digits and '.'
            e.Handled = Regex.IsMatch(e.Text, "[^0-9.]+");
        }

        private void PreTaxTxtBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Disallow space
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }

            // If enter pressed when empty, update other text boxes
            if (e.Key == Key.Enter)
            {
                if (preTaxTxtBox.Text.Length == 0)
                {
                    totaltaxTxtBox.Text = "-";
                    totalTxtBox.Text = "-";
                }
                // Move focus to "fake" textbox
                tbxFake.Focus();
                e.Handled = true;
            }
        }

        // Move focus right back to pretax amount textbox
        private void TbxFake_GotFocus(object sender, RoutedEventArgs e)
        {
            preTaxTxtBox.Focus();
            preTaxTxtBox.CaretIndex = preTaxTxtBox.Text.Length;
        }

        // Move caret to right side
        private void TaxRateTxtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            taxRateTxtBox.CaretIndex = taxRateTxtBox.Text.Length;
        }
        #endregion Textbox Events

        #region Window Events
        // Set focus on Amount textbox
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            preTaxTxtBox.SelectAll();
            _ = preTaxTxtBox.Focus();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // Save settings on exit
            UserSettings.Setting.TaxRate = tax.TaxRate;
            UserSettings.Setting.WindowTop = Top;
            UserSettings.Setting.WindowLeft = Left;
            UserSettings.SaveSettings();
        }
        #endregion Window Events

        #region Keyboard Events
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.NumPad0 && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                GridSizeReset();
            }

            if (e.Key == Key.Add && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                GridLarger();
            }

            if (e.Key == Key.Subtract && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                GridSmaller();
            }
            if (e.Key == Key.F1)
            {
                About about = new About
                {
                    Owner = Application.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                _ = about.ShowDialog();
            }
            //Debug.WriteLine(e.Key);
        }
        #endregion Keyboard Events

        #region Mouse Wheel Events
        private void Grid1_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
                return;

            if (e.Delta > 0)
            {
                GridLarger();
            }
            else if (e.Delta < 0)
            {
                GridSmaller();
            }
        }
        #endregion Mouse Wheel Events

        #region Helper Methods
        public void WindowTitleVersion()
        {
            // Get the assembly version
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            // Remove the release (last) node
            string titleVer = version.ToString().Remove(version.ToString().LastIndexOf("."));

            // Set the windows title
            Title = $"Plus Tax XL - {titleVer}";
        }
        #endregion Helper Methods

        #region Grid Zoom
        private void GridSmaller()
        {
            double curZoom = UserSettings.Setting.GridZoom;
            if (curZoom > 0.75)
            {
                curZoom -= .05;
                UserSettings.Setting.GridZoom = Math.Round(curZoom, 2);
            }
            Grid1.LayoutTransform = new ScaleTransform(curZoom, curZoom);
        }
        private void GridLarger()
        {
            double curZoom = UserSettings.Setting.GridZoom;
            if (curZoom < 1.5)
            {
                curZoom += .05;
                UserSettings.Setting.GridZoom = Math.Round(curZoom, 2);
            }
            Grid1.LayoutTransform = new ScaleTransform(curZoom, curZoom);
        }
        private void GridSizeReset()
        {
            UserSettings.Setting.GridZoom = 1.0;
            Grid1.LayoutTransform = new ScaleTransform(1, 1);
        }
        #endregion Grid Zoom

        #region Setting change
        private void UserSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyInfo prop = sender.GetType().GetProperty(e.PropertyName);
            var newValue = prop?.GetValue(sender, null);
            switch (e.PropertyName)
            {
                case "ShowTaxRate":
                    {
                        ShowHideTaxRate((bool)newValue);
                        break;
                    }
                case "KeepOnTop":
                    {
                        Topmost = (bool)newValue;
                        break;
                    }
            }
            Debug.WriteLine($"***Setting change: {e.PropertyName} New Value: {newValue}");
        }
        #endregion Setting change

        #region Show or hide tax rate grid row
        private void ShowHideTaxRate(bool x)
        {
            GridLengthConverter glc = new GridLengthConverter();
            rowTaxRate.Height = x ? (GridLength)glc.ConvertFromString("45") :
                                    (GridLength)glc.ConvertFromString("0");
            taxRateTxtBox.IsEnabled = x;
        }
        #endregion Show or hide tax rate grid row
    }
}
// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region Using directives
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PlusTaxXL.Properties;
#endregion

namespace PlusTaxXL
{
    public partial class MainWindow : Window
    {
        private readonly Tax tax = new Tax();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = tax;
            ReadSettings();
        }

        #region Read settings from config file
        private void ReadSettings()
        {
            if (Settings.Default.SettingsUpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.SettingsUpgradeRequired = false;
                Settings.Default.Save();
                CleanUp.CleanupPrevSettings();
            }

            // Settings change event
            Settings.Default.SettingChanging += SettingChanging;

            Top = Settings.Default.WindowTop;
            Left = Settings.Default.WindowLeft;
            Topmost = Settings.Default.KeepOnTop;
            tax.TaxRate = Settings.Default.TaxRate;
            //if (!string.IsNullOrEmpty(Settings.Default.TitleString))
            //{
            //    tbTitle.Text = Settings.Default.TitleString;
            //}
            ShowHideTaxRate(Settings.Default.ShowTaxRate);
            WindowTitleVersion();
        }
        #endregion

        #region Menu Events
        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }

            if (e.Key == Key.Enter)
            {
                if (preTaxTxtBox.Text.Length == 0)
                {
                    totaltaxTxtBox.Text = "-";
                    totalTxtBox.Text = "-";
                }
                tbxFake.Focus();
                e.Handled = true;
            }
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
            Settings.Default.TaxRate = tax.TaxRate;
            Settings.Default.WindowTop = Top;
            Settings.Default.WindowLeft = Left;
            Settings.Default.Save();
        }
        #endregion Window Events

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

        #region Setting change
        private void SettingChanging(object sender, SettingChangingEventArgs e)
        {
            switch (e.SettingName)
            {
                case "ShowTaxRate":
                    {
                        ShowHideTaxRate((bool)e.NewValue);
                        break;
                    }
                case "KeepOnTop":
                    {
                        Topmost = (bool)e.NewValue;
                        break;
                    }
            }
            Debug.WriteLine($"Setting: {e.SettingName} New Value: {e.NewValue}");
        }
        #endregion Setting change

        #region Show or hide tax rate grid row
        private void ShowHideTaxRate(bool x)
        {
            GridLengthConverter glc = new GridLengthConverter();
            rowTaxRate.Height = x ? (GridLength)glc.ConvertFromString("75") :
                                    (GridLength)glc.ConvertFromString("0");
            taxRateTxtBox.IsEnabled = x;
        }
        #endregion Show or hide tax rate grid row

        private void TbxFake_GotFocus(object sender, RoutedEventArgs e)
        {
            preTaxTxtBox.Focus();
            preTaxTxtBox.CaretIndex = preTaxTxtBox.Text.Length;
        }

        private void TaxRateTxtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            taxRateTxtBox.CaretIndex = taxRateTxtBox.Text.Length;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            TitleWindow tw = new TitleWindow();
            tw.Owner = this;
            tw.ShowDialog();
        }
    }
}
// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region Using directives
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
#endregion

namespace PlusTaxXL
{
    public class Tax : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Private fields
        private decimal taxRate;
        private decimal preTaxAmount;
        #endregion

        #region Properties
        public decimal TaxRate
        {
            get => taxRate;
            set
            {
                if (value != taxRate)
                {
                    taxRate = value;
                    CalculateTax();
                }
            }
        }

        public decimal PreTaxAmount
        {
            get => preTaxAmount;
            set
            {
                if (value != preTaxAmount)
                {
                    preTaxAmount = value;
                    CalculateTax();
                }
            }
        }

        public decimal TotalTax { get; set; }

        public decimal TotalAmount { get; set; }

        #endregion

        #region Calculate
        public void CalculateTax()
        {
            // Do the math
            TotalTax = preTaxAmount * TaxRate / 100;
            TotalAmount = preTaxAmount + TotalTax;

            // Copy the total amount to the windows clipboard
            if (Properties.Settings.Default.CopyToClipboard)
            {
                Clipboard.SetText(TotalAmount.ToString());
            }

            // Notify of property change
            RaisePropertyChanged("PreTaxAmount");
            RaisePropertyChanged("TotalTax");
            RaisePropertyChanged("TotalAmount");
        }
        #endregion

        #region Error handling
        public string this[string name]
        {
            get
            {
                string result = null;

                switch (name)
                {
                    case "PreTaxAmount":
                        if (PreTaxAmount.ToString()?.Length == 0)
                        {
                            result = "Cannot be blank";
                            Debug.WriteLine(result);
                        }
                        break;
                    case "TaxRate":
                        if (TaxRate == 0)
                        {
                            result = "Amount cannot be zero";
                            Debug.WriteLine(result);
                        }
                        if (TaxRate.ToString()?.Length == 0)
                        {
                            result = "Cannot be blank";
                            Debug.WriteLine(result);
                        }
                        break;
                }

                return result;
            }
        }

        public string Error { get { return null; } }

        #endregion

        #region Property changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
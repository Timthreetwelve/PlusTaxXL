// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region Using directives
using System.ComponentModel;
using TKUtils;
#endregion Using directives

namespace PlusTaxXL
{
    public class UserSettings : SettingsManager<UserSettings>, INotifyPropertyChanged
    {
        #region Constructor
        public UserSettings()
        {
            // Set defaults
            GridZoom = 1;
            KeepOnTop = false;
            WindowLeft = 100;
            WindowTop = 100;
            ShowTaxRate = true;
            TaxRate = 8.25M;
            TitleString = "My Sales Tax Calculator";
        }
        #endregion Constructor

        #region Properties
        public bool CopyToClipboard { get; set; }

        public double GridZoom
        {
            get
            {
                if (gridZoom <= 0)
                {
                    gridZoom = 1;
                }
                return gridZoom;
            }
            set
            {
                gridZoom = value;
                OnPropertyChanged(nameof(GridZoom));
            }
        }

        public bool KeepOnTop
        {
            get => keepOnTop;
            set
            {
                keepOnTop = value;
                OnPropertyChanged(nameof(KeepOnTop));
            }
        }

        public bool ShowTaxRate
        {
            get => showTaxRate;
            set
            {
                showTaxRate = value;
                OnPropertyChanged(nameof(ShowTaxRate));
            }
        }

        public decimal TaxRate { get; set; }

        public string TitleString
        {
            get => titleString;
            set
            {
                titleString = value;
                OnPropertyChanged(nameof(TitleString));
            }
        }

        public double WindowHeight
        {
            get
            {
                if (windowHeight < 100)
                {
                    windowHeight = 100;
                }
                return windowHeight;
            }
            set => windowHeight = value;
        }

        public double WindowLeft
        {
            get
            {
                if (windowLeft < 0)
                {
                    windowLeft = 100;
                }
                return windowLeft;
            }
            set => windowLeft = value;
        }

        public double WindowTop
        {
            get
            {
                if (windowTop < 0)
                {
                    windowTop = 100;
                }
                return windowTop;
            }
            set => windowTop = value;
        }
        #endregion Properties

        #region Private backing fields
        private bool keepOnTop;
        private double gridZoom;
        private bool showTaxRate;
        private string titleString;
        private double windowHeight;
        private double windowLeft;
        private double windowTop;
        #endregion Private backing fields

        #region Handle property change event
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion Handle property change event
    }
}

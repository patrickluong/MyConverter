using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MyConverter.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MyConverter
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private const string Url = "https://openexchangerates.org/api/latest.json?app_id=";
        private HttpClient _client;

        ObservableCollection<string> _currencyCodes = new ObservableCollection<string>
        {
            "USD",
            "KRW",
            "JPY",     // Short list of currency codes that I am most likely to run into
            "EUR",
            "CNY",
            "CAD"
        };

        public int BaseCurrency { get; set; }
        public int TargetCurrency { get; set; }

        public string _baseValue = "0";
        public string BaseValue
        {
            get { return _baseValue; }
            set { _baseValue = value; }
        }

        public decimal _targetValue = 0;
        public decimal TargetValue
        {
            get { return _targetValue; }
            set
            {
                _targetValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TargetValue)));
            }
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            baseCurrency.ItemsSource = _currencyCodes;
            targetCurrency.ItemsSource = _currencyCodes;
            baseCurrency.SelectedIndex = 0;
            targetCurrency.SelectedIndex = 1;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            entry.TextChanged += Entry_TextChanged;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                DisplayAlert("No internet connection", "", "OK");
                connectionLabel.Opacity = 1;
                return;
            }

            _client = new HttpClient();
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                (sender as Entry).Text = 0.ToString();
                return;
            }

            if (e.NewTextValue.Length == 2)
            {
                if (e.NewTextValue.ElementAt(0) == '0')
                {
                    (sender as Entry).Text = e.NewTextValue.ElementAt(1).ToString();
                }
            }
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                connectionLabel.FadeTo(0).ContinueWith((result) => { });
            }
            else
            {
                connectionLabel.FadeTo(1).ContinueWith((result) => { });
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }

        async void OnConvert(object sender, System.EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("No internet connection", "", "OK");
                return;
            }

            var content = await _client.GetStringAsync(Url); // + app_id + "&symbols=USD,KRW,JPY,EUR,CNY,CAD"
            ApiResponse response = JsonConvert.DeserializeObject<ApiResponse>(content);

            decimal target = decimal.Parse(BaseValue);
            target *= (1 / response.Rates[_currencyCodes[BaseCurrency]]);
            target *= (response.Rates[_currencyCodes[TargetCurrency]]);

            TargetValue = Math.Round(target, 2);
        }
    }
}

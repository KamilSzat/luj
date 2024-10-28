using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Microsoft.Maui.Controls;

namespace luj
{
    public partial class MainPage : ContentPage
    {
        public class Currency
        {
            public string? table { get; set; }
            public string? currency { get; set; }
            public string? code { get; set; }
            public IList<Rate> rates { get; set; }
        }

        public class Rate
        {
            public string? no { get; set; }
            public string? effectiveDate { get; set; }
            public double? mid { get; set; }
        }

        public MainPage()
        {
            InitializeComponent();
            DateTime dzis = DateTime.Now;
            DateEntry.MaximumDate = dzis;
        }

        private async void OnFetchRatesClicked(object sender, EventArgs e)
        {
            RatesListView.ItemsSource = null;

            var currencyCodes = CurrencyCodesEntry.Text?.Split(',') ?? Array.Empty<string>();
            var date = DateEntry.Date.ToString("yyyy-MM-dd");

            var exchangeRates = new List<string>();
            foreach (var code in currencyCodes)
            {
                var rate = await FetchExchangeRateAsync(code.Trim().ToUpper(), date);
                if (rate != null)
                {
                    exchangeRates.Add($"{code.Trim().ToUpper()}: {rate}");
                }
                else
                {
                    exchangeRates.Add($"{code.Trim().ToUpper()}: Brak danych");
                }
            }

            RatesListView.ItemsSource = exchangeRates.Count > 0 ? exchangeRates : new List<string> { "Brak danych do wyświetlenia." };
        }

        private async Task<string?> FetchExchangeRateAsync(string currencyCode, string date)
        {
            try
            {
                string url = $"https://api.nbp.pl/api/exchangerates/rates/A/{currencyCode}/{date}/?format=json";
                string json;

                using (var webClient = new WebClient())
                {
                    json = await webClient.DownloadStringTaskAsync(url);
                }

                Currency c = JsonSerializer.Deserialize<Currency>(json);
                return c?.rates[0]?.mid?.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wyjątek: {ex.Message}");
                await DisplayAlert("Błąd", $"Nie udało się pobrać danych dla waluty {currencyCode}.", "OK");
                return null;
            }
        }
    }
}




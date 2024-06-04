﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MyaFinance.MVVM.Views;
using MyaFinance.MVVM.Models;
using Newtonsoft.Json.Linq;

namespace MyaFinance
{
    public partial class MainPage : ContentPage
    {
        private HttpClient _client = new HttpClient();
        private const string BaseUrl = "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/";

        public static User currentUser;

        public MainPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            helloLabel.Text = $"Merhaba, {user.Name}";
            LoadCurrencyData();
        }

        private async void LoadCurrencyData()
        {
            try
            {
                var euroCurrencies = await GetEuroBasedCurrenciesFromFallback();
                if (!string.IsNullOrEmpty(euroCurrencies))
                {
                    var currencyRates = ParseCurrencyRates(euroCurrencies);
                    if (currencyRates != null)
                    {
                        var formattedRates = FormatCurrencyRates(currencyRates);
                        euroCurrencyLabel.Text = formattedRates;
                    }
                    else
                    {
                        euroCurrencyLabel.Text = "Error: Parsed currency rates are null.";
                    }
                }
                else
                {
                    euroCurrencyLabel.Text = "Error: Euro-based currencies response is null or empty.";
                }
            }
            catch (Exception ex)
            {
                euroCurrencyLabel.Text = $"Error: {ex.Message}";
            }
        }

        private Dictionary<string, double> ParseCurrencyRates(string json)
        {
            var currencyRates = new Dictionary<string, double>();
            try
            {
                JObject jsonObject = JObject.Parse(json);
                var rates = jsonObject["eur"];
                foreach (JProperty rate in rates.Children())
                {
                    var currencyCode = rate.Name;
                    var exchangeRate = (double)rate.Value;
                    currencyRates.Add(currencyCode, exchangeRate);
                }
                return currencyRates;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JSON parsing error: {ex.Message}");
                return null;
            }
        }

        private string FormatCurrencyRates(Dictionary<string, double> currencyRates)
        {
            if (currencyRates == null || currencyRates.Count == 0)
            {
                return "No currency rates available.";
            }

            // Sadece Euro bazında döviz kurlarını al
            double euroRate = currencyRates["try"];
            double usdRate = currencyRates["usd"];
            double usdToTry = euroRate  - ((euroRate * usdRate) - euroRate);
            dollarCurrencyLabel.Text = " " + usdToTry.ToString("F6") + " TL";
            return $" {euroRate:F6} TL";
        }

        private async Task<string> GetEuroBasedCurrencies()
        {
            try
            {
                var response = await _client.GetAsync($"{BaseUrl}currencies/eur.json");
                if (response != null && response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception("Error fetching Euro-based currencies");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching Euro-based currencies: {ex.Message}");
            }
        }

        private async Task<string> GetEuroBasedCurrenciesFromFallback()
        {
            try
            {
                var fallbackUrl = "https://latest.currency-api.pages.dev/v1/currencies/eur.json";
                var response = await _client.GetAsync(fallbackUrl);
                if (response != null && response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception("Fallback Error fetching Euro-based currencies");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fallback Error fetching Euro-based currencies: {ex.Message}");
            }
        }

        private void IncomeClicked(object sender, EventArgs e)
        {
            MyIncome myIncome = new MyIncome(currentUser.Id);
            Application.Current.MainPage.Navigation.PushModalAsync(myIncome);
        }

        private void ExpenseClicked(object sender, EventArgs e)
        {
            MyExpense myExpense = new MyExpense(currentUser.Id);
            Application.Current.MainPage.Navigation.PushModalAsync(myExpense);
        }

        private void Logout(object sender, EventArgs e)
        {
            Login login = new Login();
            Application.Current.MainPage.Navigation.PushModalAsync(login);
        }
    }
}

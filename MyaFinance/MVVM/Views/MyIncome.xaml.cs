using MyaFinance.MVVM.Models;
using MyaFinance.MVVM.Views;
using MyaFinance.Repositories;
using System;
using System.Collections.Generic;

namespace MyaFinance;

public partial class MyIncome : ContentPage
{
    private int _userId;
    private IncomeRepository _incomeRepository;

    public MyIncome(int id)
    {
        _userId = id;
        _incomeRepository = new IncomeRepository();
        InitializeComponent();
        LoadIncomes();
    }

    private void LoadIncomes()
    {
        List<Income> incomes = _incomeRepository.GetAll(_userId);
        incomeListView.ItemsSource = incomes;
    }

    private void GoBack(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }

    private void AddNewIncomeButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new AddNewIncome(_userId, RefreshIncomeList));
    }

    private void EditButton_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var incomeId = (int)button.CommandParameter;

        // Fetch the income details using the incomeId
        var income = _incomeRepository.Get(incomeId);

        if (income != null)
        {
            // Navigate to the EditIncome page with the income details
            Navigation.PushModalAsync(new EditIncome(income, RefreshIncomeList));
        }
        else
        {
            DisplayAlert("Error", "Income not found", "OK");
        }
    }

    private void DeleteButton_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var incomeId = (int)button.CommandParameter;
        _incomeRepository.Delete(incomeId);
        LoadIncomes();
    }

    private void DetailsButton_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var incomeId = (int)button.CommandParameter;
        // Handle details logic here
        // For example, navigate to a details page with the incomeId
    }

    private void RefreshIncomeList()
    {
        var incomes = _incomeRepository.GetAll(_userId);
        incomeListView.ItemsSource = incomes;
    }
}
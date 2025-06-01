using BillTrackingAppBackend.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace MauiBillTrackingApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDataFromRestAPI();
    }

    public async Task LoadDataFromRestAPI()
    {
        try
        {
            Loading_label.IsVisible = true;

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://10.0.2.2:7234/")
            };

            string json = await client.GetStringAsync("api/BillTrackings");

            var allBills = JsonConvert.DeserializeObject<List<BillTracking>>(json);

            // Suodatetaan VAIN avoimet laskut MainPagea varten
            var openBills = new ObservableCollection<BillTracking>(
                allBills.Where(b => b.Tila == "Avoin")
            );

            billList.ItemsSource = openBills;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", ex.Message, "OK");
        }
        finally
        {
            Loading_label.IsVisible = false;
        }
    }

    private async void OnAddNewBillClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddNewBill), true); // true = modaalina
    }

    private async void billList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is BillTracking bill)
        {
            string json = JsonConvert.SerializeObject(bill);
            await Shell.Current.GoToAsync($"{nameof(UpdateBill)}?bill={Uri.EscapeDataString(json)}", true);
        }
    }

    private async void selectBill_Clicked(object sender, EventArgs e)
    {
        if (billList.SelectedItem is BillTracking bill)
        {
            string json = JsonConvert.SerializeObject(bill);
            await Shell.Current.GoToAsync($"{nameof(UpdateBill)}?bill={Uri.EscapeDataString(json)}", true);
        }
        else
        {
            await DisplayAlert("Huomio", "Valitse lasku listalta ensin", "OK");
        }
    }
}

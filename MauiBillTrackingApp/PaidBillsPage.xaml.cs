using BillTrackingAppBackend.Models;
using Newtonsoft.Json;

namespace MauiBillTrackingApp;

public partial class PaidBillsPage : ContentPage
{
    public PaidBillsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            using var client = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) => true
            });

            client.BaseAddress = new Uri("https://10.0.2.2:7234");
            var json = await client.GetStringAsync("api/BillTrackings");
            var bills = JsonConvert.DeserializeObject<List<BillTracking>>(json);

            var paidBills = bills.Where(b => b.Tila == "Maksettu").ToList();
            PaidList.ItemsSource = paidBills;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", ex.Message, "OK");
        }
    }
}
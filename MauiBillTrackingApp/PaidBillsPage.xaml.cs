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

            // J�rjest� maksetut laskut maksup�iv�n mukaan uusimmat ensin
            var paidBills = bills
                .Where(b => b.Tila == "Maksettu")
                .OrderByDescending(b => b.Maksup�iv�) // tai b.Er�p�iv�, jos Maksup�iv� puuttuu
                .ToList();

            PaidList.ItemsSource = paidBills;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", ex.Message, "OK");
        }
    }
    private async void OnDeleteBillClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is BillTracking bill)
        {
            bool confirm = await DisplayAlert("Vahvista poisto", "Haluatko varmasti poistaa t�m�n laskun?", "Poista", "Peruuta");
            if (!confirm) return;

            try
            {
                using var client = new HttpClient(new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (msg, cert, chain, err) => true
                });

                client.BaseAddress = new Uri("https://10.0.2.2:7234");
                var response = await client.DeleteAsync($"api/BillTrackings/{bill.BillId}");

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Poistettu", "Lasku poistettu onnistuneesti", "OK");
                    OnAppearing(); // lataa p�ivitetty lista uudelleen
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Virhe", $"Poistaminen ep�onnistui: {error}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Poikkeus tapahtui: {ex.Message}", "OK");
            }
        }
    }
}
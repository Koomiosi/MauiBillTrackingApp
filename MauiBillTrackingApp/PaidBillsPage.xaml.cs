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

            // Järjestä maksetut laskut maksupäivän mukaan uusimmat ensin
            var paidBills = bills
                .Where(b => b.Tila == "Maksettu")
                .OrderByDescending(b => b.Maksupäivä) // tai b.Eräpäivä, jos Maksupäivä puuttuu
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
            bool confirm = await DisplayAlert("Vahvista poisto", "Haluatko varmasti poistaa tämän laskun?", "Poista", "Peruuta");
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
                    OnAppearing(); // lataa päivitetty lista uudelleen
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Virhe", $"Poistaminen epäonnistui: {error}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Poikkeus tapahtui: {ex.Message}", "OK");
            }
        }
    }
}
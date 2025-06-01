using BillTrackingAppBackend.Models;
using Newtonsoft.Json;
using System.Text;

namespace MauiBillTrackingApp;

[QueryProperty(nameof(BillJson), "bill")]
public partial class UpdateBill : ContentPage
{
    private BillTracking _bill;

    public UpdateBill()
    {
        InitializeComponent();
    }

    public string BillJson
    {
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _bill = JsonConvert.DeserializeObject<BillTracking>(Uri.UnescapeDataString(value));
                L�hett�j�Entry.Text = _bill.L�hett�j�;
                SummaEntry.Text = _bill.Summa.ToString();
                Er�p�iv�Picker.Date = _bill.Er�p�iv�;
                TilaPicker.SelectedItem = _bill.Tila;
            }
            if (_bill.Maksup�iv�.HasValue)
            {
                Maksup�iv�Picker.Date = _bill.Maksup�iv�.Value;
            }
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (TilaPicker.SelectedIndex == -1)
            TilaPicker.SelectedIndex = 0;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (!decimal.TryParse(SummaEntry.Text, out decimal summa))
        {
            await DisplayAlert("Virhe", "Anna kelvollinen summa", "OK");
            return;
        }

        _bill.L�hett�j� = L�hett�j�Entry.Text;
        _bill.Summa = summa;
        _bill.Er�p�iv� = Er�p�iv�Picker.Date;
        _bill.Maksup�iv� = Maksup�iv�Picker.Date;
        _bill.Tila = TilaPicker.SelectedItem?.ToString();

        try
        {
            var json = JsonConvert.SerializeObject(_bill);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, err) => true
            });

            client.BaseAddress = new Uri("https://10.0.2.2:7234");

            var response = await client.PutAsync($"/api/BillTrackings/{_bill.BillId}", content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Onnistui", "Lasku p�ivitetty", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Virhe", error, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", ex.Message, "OK");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Vahvista poisto", "Haluatko varmasti poistaa t�m�n laskun?", "Poista", "Peruuta");

        if (!confirm || _bill == null)
            return;

        try
        {
            using var client = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            });

            client.BaseAddress = new Uri("https://10.0.2.2:7234");

            var response = await client.DeleteAsync($"api/BillTrackings/{_bill.BillId}");

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Poistettu", "Lasku poistettu onnistuneesti", "OK");
                await Shell.Current.GoToAsync(".."); // palaa takaisin
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
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
                LähettäjäEntry.Text = _bill.Lähettäjä;
                SummaEntry.Text = _bill.Summa.ToString();
                EräpäiväPicker.Date = _bill.Eräpäivä;
                TilaPicker.SelectedItem = _bill.Tila;
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

        _bill.Lähettäjä = LähettäjäEntry.Text;
        _bill.Summa = summa;
        _bill.Eräpäivä = EräpäiväPicker.Date;
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
                await DisplayAlert("Onnistui", "Lasku päivitetty", "OK");
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

}
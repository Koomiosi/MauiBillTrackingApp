using BillTrackingAppBackend.Models;
using Newtonsoft.Json;
using System.Text;

namespace MauiBillTrackingApp;

public partial class AddNewBill : ContentPage
{
    public AddNewBill()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (TilaPicker.SelectedIndex == -1)
            TilaPicker.SelectedIndex = 0;
    }

    private async void AddBtn_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(Lähettäjä.Text))
        {
            await DisplayAlert("Tieto puuttuu", "Anna lähettäjä", "Ok");
            return;
        }

        if (string.IsNullOrEmpty(Summa.Text))
        {
            await DisplayAlert("Tieto puuttuu", "Anna summa", "Ok");
            return;
        }

        if (!decimal.TryParse(Summa.Text, out decimal summa))
        {
            await DisplayAlert("Virheellinen syöte", "Syötä kelvollinen summa", "Ok");
            return;
        }

        DateTime minimiEräpäivä = new DateTime(2000, 1, 1);
        if (EräpäiväPicker.Date <= minimiEräpäivä)
        {
            await DisplayAlert("Tieto puuttuu", "Valitse kelvollinen eräpäivä", "Ok");
            return;
        }

        try
        {
            BillTracking newBill = new BillTracking
            {
                Lähettäjä = Lähettäjä.Text,
                Summa = summa,
                Eräpäivä = EräpäiväPicker.Date,
                Tila = TilaPicker.SelectedItem?.ToString() ?? "Avoin"
            };

            var input = JsonConvert.SerializeObject(newBill);
            HttpContent content = new StringContent(input, Encoding.UTF8, "application/json");

            using var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            using var client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://10.0.2.2:7234");

            var res = await client.PostAsync("/api/BillTrackings", content);

            if (res.IsSuccessStatusCode)
            {
                Lähettäjä.Text = "";
                Summa.Text = "";
                EräpäiväPicker.Date = DateTime.Today;
                TilaPicker.SelectedIndex = 0;

                await DisplayAlert("Onnistui", "Lasku lisätty", "Ok");

                // Päivitä pääsivu
                if (Shell.Current.CurrentPage is MainPage mainPage)
                {
                    await mainPage.LoadDataFromRestAPI();
                }

                // Palaa modaalista
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                string errorMsg = await res.Content.ReadAsStringAsync();
                await DisplayAlert("Virhe palvelimelta", $"Status: {res.StatusCode}\n{errorMsg}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Poikkeus tapahtui:\n{ex.Message}", "OK");
        }
    }
}

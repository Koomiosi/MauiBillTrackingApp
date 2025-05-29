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

    private async Task AddBtn_Clicked(object sender, EventArgs e)
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

        // Tarkista tila
        if (TilaPicker.SelectedIndex == -1)
        {
            await DisplayAlert("Tieto puuttuu", "Valitse tila", "Ok");
            return;
        }

        // Tarkista eräpäivä – käytä järkevää minimiarvoa
        DateTime minimiEräpäivä = new DateTime(2000, 1, 1);
        if (EräpäiväPicker.Date <= minimiEräpäivä)
        {
            await DisplayAlert("Tieto puuttuu", "Valitse kelvollinen eräpäivä", "Ok");
            return;
        }

        // Luodaan uusi lasku
        BillTracking newBill = new BillTracking
        {
            Lähettäjä = Lähettäjä.Text,
            Summa = summa,
            Eräpäivä = EräpäiväPicker.Date,
            Tila = TilaPicker.SelectedItem.ToString()
        };

        // Tallenna lasku esim. tietokantaan (tähän oma logiikka)
        // await SaveBillAsync(newBill);

        await DisplayAlert("Onnistui", "Lasku lisätty", "Ok");
    }



}
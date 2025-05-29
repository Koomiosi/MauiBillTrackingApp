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
        if (string.IsNullOrEmpty(L�hett�j�.Text))
        {
            await DisplayAlert("Tieto puuttuu", "Anna l�hett�j�", "Ok");
            return;
        }

        if (string.IsNullOrEmpty(Summa.Text))
        {
            await DisplayAlert("Tieto puuttuu", "Anna summa", "Ok");
            return;
        }

        if (!decimal.TryParse(Summa.Text, out decimal summa))
        {
            await DisplayAlert("Virheellinen sy�te", "Sy�t� kelvollinen summa", "Ok");
            return;
        }

        // Tarkista tila
        if (TilaPicker.SelectedIndex == -1)
        {
            await DisplayAlert("Tieto puuttuu", "Valitse tila", "Ok");
            return;
        }

        // Tarkista er�p�iv� � k�yt� j�rkev�� minimiarvoa
        DateTime minimiEr�p�iv� = new DateTime(2000, 1, 1);
        if (Er�p�iv�Picker.Date <= minimiEr�p�iv�)
        {
            await DisplayAlert("Tieto puuttuu", "Valitse kelvollinen er�p�iv�", "Ok");
            return;
        }

        // Luodaan uusi lasku
        BillTracking newBill = new BillTracking
        {
            L�hett�j� = L�hett�j�.Text,
            Summa = summa,
            Er�p�iv� = Er�p�iv�Picker.Date,
            Tila = TilaPicker.SelectedItem.ToString()
        };

        // Tallenna lasku esim. tietokantaan (t�h�n oma logiikka)
        // await SaveBillAsync(newBill);

        await DisplayAlert("Onnistui", "Lasku lis�tty", "Ok");
    }



}
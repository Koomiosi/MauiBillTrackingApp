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

            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            HttpClient client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://10.0.2.2:7234/")
            };

            string json = await client.GetStringAsync("api/BillTrackings");

            IEnumerable<BillTracking> ienumBillList = JsonConvert.DeserializeObject<BillTracking[]>(json);
            ObservableCollection<BillTracking> BillList = new ObservableCollection<BillTracking>(ienumBillList);

            billList.ItemsSource = BillList;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
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

    private void selectBill_Clicked(object sender, EventArgs e)
    {

    }

    private void billList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {

    }
}

namespace MauiBillTrackingApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(AddNewBill), typeof(AddNewBill));

            Routing.RegisterRoute(nameof(UpdateBill), typeof(UpdateBill));

            Routing.RegisterRoute(nameof(PaidBillsPage), typeof(PaidBillsPage));
        }
    }
}

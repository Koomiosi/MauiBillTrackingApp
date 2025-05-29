namespace MauiBillTrackingApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(AddNewBill), typeof(AddNewBill));
        }
    }
}

﻿namespace MauiBillTrackingApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell(); // Shell-rakenne käyttöön
        }
    }

}

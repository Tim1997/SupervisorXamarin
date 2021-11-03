using BaseXamarin.IoC;
using SupervisorMaster.ViewModels;
using SupervisorMaster.Views;
using SupervisorMaster.Views.Settings;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SupervisorMaster
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(HistoryPage), typeof(HistoryPage));
            Routing.RegisterRoute(nameof(TimerPage), typeof(TimerPage));
            Routing.RegisterRoute(nameof(WebsitePage), typeof(WebsitePage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));

            Container.Current.Setup();

            //if (!string.IsNullOrEmpty(Preferences.Get("MyFirebaseRefreshToken", "")))
            //{
            //    MainPage = new NavigationPage(new MyDashboardPage());
            //}
            //else
            //{
            //    MainPage = new NavigationPage(new MainPage());
            //}
        }

    }
}

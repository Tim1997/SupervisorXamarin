using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using SupervisorMaster.Models;
using SupervisorMaster.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SupervisorMaster.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimerPage : PopupPage
    {
        public TimerPage(Action<ResponseDialog> setResultAction)
        {
            InitializeComponent();
            this.setResultAction = setResultAction;
            sClock.Toggled += (s, e) =>
            {
                if (sClock.IsToggled)
                {
                    tpClock.IsVisible = true;
                    tfMinute.IsVisible = false;

                    lState.Text = "SETTING CLOCK";
                }
                else
                {
                    tfMinute.IsVisible = true;
                    tpClock.IsVisible = false;

                    lState.Text = "SETTING COUNTDOWN";
                }
            };
        }

        private readonly Action<ResponseDialog> setResultAction;

        public void CancelClicked(object sender, EventArgs e)
        {
            setResultAction?.Invoke(new ResponseDialog { Result = false, Objs = null });
            this.Navigation.PopPopupAsync().ConfigureAwait(false);
        }

        public void OKClicked(object sender, EventArgs e)
        {
            var isclock = sClock.IsToggled;
            var clock = tpClock.Time;
            var minute = int.Parse(string.IsNullOrEmpty(tfMinute.Text) ? "0" : tfMinute.Text);
            var isonce = gRepeat.SelectedIndex == 0 ? true : false;

            setResultAction?.Invoke(new ResponseDialog { Result = true, Objs = new object[] { isclock, clock, minute, isonce } });
            this.Navigation.PopPopupAsync().ConfigureAwait(false);
        }

        public static async Task<ResponseDialog> ShowDialog(INavigation navigation, TimerView view)
        {
            var completionSource = new TaskCompletionSource<ResponseDialog>();

            void callback(ResponseDialog response)
            {
                completionSource.TrySetResult(response);
            }
            var popup = new TimerPage(callback);

            if (view != null)
            {
                popup.tpClock.Time = view.Clock;
                popup.tfMinute.Text = view.Minute.ToString();
                popup.gRepeat.SelectedIndex = view.IsOnce ? 0 : 1;

                if (view.IsClock)
                {
                    popup.sClock.IsToggled = true;
                    popup.tpClock.IsVisible = true;
                    popup.tfMinute.IsVisible = false;

                    popup.lState.Text = "SETTING CLOCK";
                }
                else
                {
                    popup.sClock.IsToggled = false;
                    popup.tfMinute.IsVisible = true;
                    popup.tpClock.IsVisible = false;

                    popup.lState.Text = "SETTING COUNTDOWN";
                }
            }
            else
            {
                popup.tfMinute.IsVisible = true;
                popup.tpClock.IsVisible = false;

                popup.lState.Text = "SETTING COUNTDOWN";
            }

            await navigation.PushPopupAsync(popup);
            return await completionSource.Task;
        }
    }
}
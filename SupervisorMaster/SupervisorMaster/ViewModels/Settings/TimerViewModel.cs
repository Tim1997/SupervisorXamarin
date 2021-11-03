using Firebase.Database.Query;
using SupervisorMaster.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace SupervisorMaster.ViewModels.Settings
{
    public class TimerViewModel : BaseXamarin.ViewModels.BaseViewModel
    {
        #region Properties
        private string[] repeat;

        public string[] Repeat { get => repeat; set => SetProperty(ref repeat, value); }
        #endregion

        public TimerViewModel() : base("Timer")
        {
            Repeat = new string[] { "Once", "Everyday" };
        }
    }
}

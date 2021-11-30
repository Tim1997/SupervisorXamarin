using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SupervisorMaster.ViewModels.Image
{
    [QueryProperty(nameof(ImageName), nameof(ImageName))]
    public class ImageInfoViewModel : BaseXamarin.ViewModels.BaseViewModel
    {
        private string _imageName;
        private string imageUrl;

        public string ImageName
        {
            get => _imageName;
            set
            {
                _imageName = Uri.UnescapeDataString(value ?? string.Empty);
                SetProperty(ref _imageName, value);
            }
        }
        public string ImageUrl { get => imageUrl; set => SetProperty(ref imageUrl, value); }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            ImageUrl = await FirebaseStorage
                     .Child(User.LocalId).Child("Screenshots").Child(ImageName).GetDownloadUrlAsync();
        });


        public ImageInfoViewModel() : base("Image Infomation")
        {
        }
    }
}

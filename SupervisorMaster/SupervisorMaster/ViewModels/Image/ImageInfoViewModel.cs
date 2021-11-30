using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SupervisorMaster.ViewModels.Image
{
    [QueryProperty(nameof(ImagePath), nameof(ImagePath))]
    public class ImageInfoViewModel : BaseXamarin.ViewModels.BaseViewModel
    {
        private string _imagePath;
        private string _image;

        public string ImagePath 
        { 
            get => _imagePath;
            set 
            {
                _imagePath = Uri.UnescapeDataString(value ?? string.Empty);
                SetProperty(ref _imagePath, value);
                Image = _imagePath;
            } 
        }

        public string Image { get => _image; set => SetProperty(ref _image, value); }

        public ImageInfoViewModel() : base("Image Infomation")
        { }

    }
}

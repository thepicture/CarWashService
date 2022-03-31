using System;
using System.ComponentModel;

namespace CarWashService.MobileApp.Services
{
    public class CaptchaService : ICaptchaService, INotifyPropertyChanged
    {
        private string text;

        public string Text
        {
            get => text; private set
            {
                text = value;
                PropertyChanged?.Invoke(this,
                                        new PropertyChangedEventArgs(nameof(Text)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Check(string textToValidate)
        {
            return Text == textToValidate;
        }

        public void GenerateNew()
        {
            Text = Guid
                .NewGuid()
                .ToString()
                .Substring(0, 6);
        }

        public void Invalidate()
        {
            Text = null;
        }
    }
}

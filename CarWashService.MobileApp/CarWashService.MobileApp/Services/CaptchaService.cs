using System;
using System.ComponentModel;

namespace CarWashService.MobileApp.Services
{
    public class CaptchaService : ICaptchaService, INotifyPropertyChanged
    {
        private string text;
        private int countOfAttempts;

        public string Text
        {
            get => text; private set
            {
                text = value;
                PropertyChanged?.Invoke(this,
                                        new PropertyChangedEventArgs(nameof(Text)));
            }
        }

        public int CountOfAttempts
        {
            get => countOfAttempts;
            set
            {
                countOfAttempts = value;
                PropertyChanged?.Invoke(this,
                                        new PropertyChangedEventArgs(nameof(CountOfAttempts)));
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
            CountOfAttempts = 0;
        }
    }
}

namespace CarWashService.MobileApp.Services
{
    public interface ICaptchaService
    {
        string Text { get; }
        int CountOfAttempts { get; set; }
        void GenerateNew();
        bool Check(string text);
        void Invalidate();
    }
}

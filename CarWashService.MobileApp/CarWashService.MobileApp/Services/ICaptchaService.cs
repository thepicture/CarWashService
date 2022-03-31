namespace CarWashService.MobileApp.Services
{
    public interface ICaptchaService
    {
        string Text { get; }
        void GenerateNew();
        bool Check(string text);
        void Invalidate();
    }
}

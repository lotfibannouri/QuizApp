namespace Authentication.web.Services
{
    public interface IJdoodleService
    {
        Task<string> GetOutputCodeQuestion(string code);

    }
}

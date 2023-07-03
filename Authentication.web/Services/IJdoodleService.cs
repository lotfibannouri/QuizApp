namespace Authentication.web.Services
{
    public interface IJdoodleService
    {
        Task<string> GetOutput(string code, string language, string versionIndex);

    }
}

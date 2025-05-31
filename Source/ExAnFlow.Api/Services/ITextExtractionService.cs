namespace ExAnFlow.Api.Services
{
    public interface ITextExtractionService
    {
        Task<string> ExtractTextFromFile(Stream fileStream, string fileName);
    }
}

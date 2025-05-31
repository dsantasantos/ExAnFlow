using Tesseract;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace ExAnFlow.Api.Services
{
    public class TextExtractionTesseractService : ITextExtractionService
    {
        private readonly string _tessDataPath;
        private static bool _isInitialized;
        private readonly ILogger<TextExtractionTesseractService> _logger;

        public TextExtractionTesseractService(ILogger<TextExtractionTesseractService> logger)
        {
            _logger = logger;
            _tessDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
            _logger.LogInformation("TessData path: {TessDataPath}", _tessDataPath);
            InitializeTesseract();
        }

        private void InitializeTesseract()
        {
            if (_isInitialized) return;

            // Cria o diretório tessdata se não existir
            if (!Directory.Exists(_tessDataPath))
            {
                _logger.LogInformation("Criando diretório tessdata em: {TessDataPath}", _tessDataPath);
                Directory.CreateDirectory(_tessDataPath);
            }

            // Caminho para o arquivo de treinamento embutido
            var resourcePath = Path.Combine(_tessDataPath, "por.traineddata");
            _logger.LogInformation("Procurando arquivo por.traineddata em: {ResourcePath}", resourcePath);

            // Se o arquivo não existir, copia do recurso embutido
            if (!File.Exists(resourcePath))
                _logger.LogError("Arquivo por.traineddata não encontrado em: {ResourcePath}", resourcePath);

            _logger.LogInformation("Arquivo por.traineddata encontrado com sucesso");
            _isInitialized = true;
        }

        public async Task<string> ExtractTextFromFile(Stream fileStream, string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();

            return extension switch
            {
                ".pdf" => await ExtractTextFromPdf(fileStream),
                ".jpg" or ".jpeg" or ".png" or ".bmp" or ".tiff" => await ExtractTextFromImage(fileStream),
                _ => throw new ArgumentException("Formato de arquivo não suportado. Use PDF ou imagens (JPG, PNG, BMP, TIFF).")
            };
        }

        private async Task<string> ExtractTextFromPdf(Stream pdfStream)
        {
            using var pdfReader = new PdfReader(pdfStream);
            using var pdfDocument = new PdfDocument(pdfReader);
            var text = new System.Text.StringBuilder();

            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
            {
                var page = pdfDocument.GetPage(i);
                var strategy = new LocationTextExtractionStrategy();
                var currentText = PdfTextExtractor.GetTextFromPage(page, strategy);
                text.AppendLine(currentText);
            }

            return text.ToString();
        }

        private async Task<string> ExtractTextFromImage(Stream imageStream)
        {
            using var engine = new TesseractEngine(_tessDataPath, "por", EngineMode.Default);
            using var img = Pix.LoadFromMemory(await ReadFully(imageStream));
            using var page = engine.Process(img);
            return page.GetText();
        }

        private static async Task<byte[]> ReadFully(Stream input)
        {
            using var ms = new MemoryStream();
            await input.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
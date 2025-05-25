using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ExAnFlow.Ocr.Api.Services;

namespace ExAnFlow.Ocr.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TextExtractionController : ControllerBase
    {
        private readonly ITextExtractionService _textExtractionService;

        public TextExtractionController(ITextExtractionService textExtractionService)
        {
            _textExtractionService = textExtractionService;
        }

        /// <summary>
        /// Extrai texto de um arquivo PDF ou imagem
        /// </summary>
        /// <param name="file">Arquivo PDF ou imagem (JPG, PNG, BMP, TIFF)</param>
        /// <returns>Texto extraído do arquivo</returns>
        [HttpPost("extract")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExtractText(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }

            try
            {
                using var stream = file.OpenReadStream();
                var extractedText = await _textExtractionService.ExtractTextFromFile(stream, file.FileName);
                return Ok(extractedText);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao processar o arquivo: {ex.Message}");
            }
        }
    }
} 
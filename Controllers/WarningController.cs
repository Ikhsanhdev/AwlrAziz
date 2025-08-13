using System.Globalization;
using System.Text;
using AwlrAziz.Data;
using AwlrAziz.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class WarningController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly AwlrAzizContext _context;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;
    public WarningController(AwlrAzizContext context, IUnitOfWorkRepository unitOfWorkRepository)
    {
        _httpClient = new HttpClient();
        _context = context;
        _unitOfWorkRepository = unitOfWorkRepository;
    }

    [HttpPost]
    public async Task<IActionResult> SendWarning(string number)
    {
        string apiUrl = "http://103.186.1.92:3000/send/message";
        try
        {
            using (HttpClient client = new HttpClient())
            {
                var data = await _unitOfWorkRepository.Devices.GetLatestReading();

                if (data == null)
                {
                    return StatusCode(500, "Respons API kosong atau tidak valid");
                }

                string siagaLogo = "";
                string ketSiaga = "";
                if (data.water_level >= data.siaga3 && data.water_level < data.siaga2)
                {
                    siagaLogo = "🟡";
                    ketSiaga = "WASPADA";
                }
                else if (data.water_level >= data.siaga2 && data.water_level < data.siaga1)
                {
                    siagaLogo = "🟠";
                    ketSiaga = "SIAGA";
                }
                else if (data.water_level >= data.siaga1)
                {
                    siagaLogo = "🔴";
                    ketSiaga = "AWAS";
                }
                else
                {
                    siagaLogo = "🟢";
                    ketSiaga = "Normal";
                }

                string readingAtRaw = data.reading_at?.ToString();
                string formattedDate = "Tidak tersedia";
                if (!string.IsNullOrEmpty(readingAtRaw) && DateTime.TryParse(readingAtRaw, out DateTime readingAt)) {
                    formattedDate = readingAt.ToString("d MMMM yyyy HH:mm", new CultureInfo("id-ID"));
                }

                string msg = $"{siagaLogo} *[Status: {ketSiaga ?? "Tidak tersedia"}]* \n";
                msg += "\n";
                msg += $"Nama Pos : *{data?.name?.ToString() ?? "Tidak tersedia"}* \n";
                msg += $"Device : *{data?.brand_name?.ToString() ?? "Tidak tersedia"} - {data?.device_id?.ToString() ?? "Tidak tersedia"}* \n";
                msg += $"Waktu : *{formattedDate} WIB* \n";
                msg += $"Tinggi Muka Air : *{data?.water_level?.ToString() ?? "Tidak tersedia"} cm*";
                msg = msg.Replace("\n", "\\n");

                string jsonBody = $@"{{ 
                    ""phone"" : ""{number}"",
                    ""message"" : ""{msg}""
                }}";

                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("API Response:");
                    Console.WriteLine(apiResponse);
                    return Ok(apiResponse);
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    Console.WriteLine("Isi error dari API:");
                    Console.WriteLine(errorResponse);
                    return StatusCode((int)response.StatusCode, errorResponse);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return StatusCode(500, "An error occurred");
        }

        return StatusCode(500, "Fatal Error !");
    }
}
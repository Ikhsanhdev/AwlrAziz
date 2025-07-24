using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AwlrAziz.Models;
using AwlrAziz.Interfaces;

namespace AwlrAziz.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWorkRepository _unitOfWorkRepository;

    public HomeController(ILogger<HomeController> logger, IUnitOfWorkRepository unitOfWorkRepository)
    {
        _logger = logger;
         _unitOfWorkRepository = unitOfWorkRepository;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult StationDetail()
    {
        return View();
    }

    [HttpGet]
    public JsonResult GetLastReadingDate(string deviceId)
    {
        DateTime result = _unitOfWorkRepository.Devices.LastReading(deviceId);
        return Json(new { lastReading = result });
    }

    public async Task<IActionResult> GetDataTableAwlr(string periode, string periodEnd)
    {
        if (!DateTime.TryParse(periode, out var startDate) || 
            !DateTime.TryParse(periodEnd, out var endDate))
        {
            return BadRequest("Format tanggal salah.");
        }

        var data = await _unitOfWorkRepository.Devices.GetReadingsByPeriodeAsync(startDate, endDate);
        
        var result = data.Select(r => new {
            readingAt = r.ReadingAt,
            waterLevel = r.WaterLevel
        });

        return Json(result);
    }

    public async Task<IActionResult> GetLatestReading()
    {
        var data =  await _unitOfWorkRepository.Devices.GetLatestReading();
        if (data == null)
            return NotFound();

        return Ok(data);
    }

    public async Task<IActionResult> GetLastGrafik()
    {
        var data = await _unitOfWorkRepository.Devices.GetLastGrafik();
        return Ok(data);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

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

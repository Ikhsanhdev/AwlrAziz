using Hangfire;
using Microsoft.AspNetCore.Mvc;
using AwlrAziz.Interfaces;
using AwlrAziz.Models.Customs;
using Microsoft.AspNetCore.Http;
using AspNetCoreRateLimit;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace AwlrAziz.Controllers
{
    [Route("api/device")]
    public class ApiController : BaseController
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public ApiController(IUnitOfWorkRepository unitOfWorkRepository)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        [HttpGet("store")]
        public async Task<IActionResult> Store([FromQuery] string id, [FromQuery] double tma)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Parameter 'id' tidak boleh kosong.");

                await _unitOfWorkRepository.Devices.InsertAsync(id, tma);
                return Ok(new { success = true, message = "Data berhasil disimpan." });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Gagal menyimpan data dengan id={Id}, tma={Tma}", id, tma);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
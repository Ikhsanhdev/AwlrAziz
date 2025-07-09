using System.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Npgsql;
using Dapper;

using AwlrAziz.Data;
// using AwlrAziz.Helpers;
using AwlrAziz.Models;

namespace AwlrAziz.Repositories
{
    public interface IDeviceRepository
    {
        Task<MvDevice?> GetExistingDeviceAsync(string deviceId);
        Task<MvDevice?> GetMvDeviceAsync(string brandCode, string deviceId);
    }

    public class DeviceRepository : IDeviceRepository
    {
        private readonly string? _connectionString;

        private readonly AwlrAzizContext _context;
        private readonly IConfiguration _config;

        public DeviceRepository(AwlrAzizContext context, IConfiguration configuration)
        {
            _context = context;

            _config = configuration;
            _connectionString = _config.GetConnectionString("DefaultConnection");

            if (_connectionString == null)
            {
                Log.Error("Connection string is null. Check your configuration.");
            }
        }

        public async Task<MvDevice?> GetExistingDeviceAsync(string deviceId)
        {
            try
            {
                using var _db = new NpgsqlConnection(_connectionString);
                var query = @"SELECT * FROM ""MvDevices"" WHERE ""DeviceId"" = @DeviceId LIMIT 1";
                var result = await _db.QueryFirstOrDefaultAsync<MvDevice>(query, new { DeviceId = deviceId });
                return result;
            }
            catch (NpgsqlException ex)
            {
                Log.Error(ex, "PostgreSQL Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace, DeviceId = deviceId });
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "General Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace, DeviceId = deviceId });
                throw;
            }
        }
        
        public async Task<MvDevice?> GetMvDeviceAsync(string brandCode, string deviceId)
        {
            try
            {    
                using var _db = new NpgsqlConnection(_connectionString);
                var query = @"SELECT * FROM ""MvDevices"" WHERE ""BrandCode"" = @BrandCode AND ""DeviceId"" = @DeviceId LIMIT 1";
                var result = await _db.QueryFirstOrDefaultAsync<MvDevice>(query, new { BrandCode = brandCode, DeviceId = deviceId });
                return result;
            }
            catch (NpgsqlException ex)
            {
                Log.Error(ex, "PostgreSQL Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace, BrandCode = brandCode, DeviceId = deviceId });
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "General Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace, BrandCode = brandCode, DeviceId = deviceId });
                throw;
            }
        }
    }
}
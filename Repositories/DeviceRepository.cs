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
        Task InsertAsync(string deviceId, double tma);
        Task<List<dynamic>> GetReadingDevice(string id);
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

        public async Task InsertAsync(string deviceId, double tma)
        {
            var device = await GetExistingDeviceAsync(deviceId);
            if (device == null)
            {
                Log.Warning("Device ID {DeviceId} tidak ditemukan di MvDevices", deviceId);
                return; // atau throw error, tergantung logikamu
            }

            var data = new AwlrLastReading
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                StationId = device.StationId,
                WaterLevel = tma,
                ReadingAt = DateTime.Now
            };

            _context.AwlrLastReadings.Add(data);
            await _context.SaveChangesAsync();
        }

        public async Task<List<dynamic>> GetReadingDevice(string id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var sql = @"
                SELECT 
                    st.""Name"",
                    st.""Type"",
                    r.""DeviceId"",
                    st.""Latitude"",
                    st.""Longitude"",
                    set.""Siaga1"",
                    set.""Siaga2"",
                    set.""Siaga3"",
                    set.""UnitDisplay"",
                    set.""UnitSensor"",
                    r.""ReadingAt"",
                    r.""WaterLevel"",
                    r.""ChangeValue"",
                    r.""ChangeStatus"",
                    r.""WarningStatus""
                FROM ""AwlrLastReadings"" AS r 
                LEFT JOIN ""Stations"" AS st ON r.""StationId"" = st.""Id""
                LEFT JOIN ""AwlrSettings"" AS set ON st.""Id"" = set.""StationId""
                WHERE r.""DeviceId"" = @DeviceId";

            var result = await connection.QueryAsync(sql, new { DeviceId = id });
            return result.ToList();
        }
    }
}
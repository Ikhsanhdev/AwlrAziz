using System.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Npgsql;
using Dapper;

using AwlrAziz.Data;
using AwlrAziz.ViewModels;
using AwlrAziz.Models;
using Microsoft.AspNetCore.JsonPatch.Internal;

namespace AwlrAziz.Repositories
{
    public interface IDeviceRepository
    {
        Task<MvDevice?> GetExistingDeviceAsync(string deviceId);
        Task<MvDevice?> GetMvDeviceAsync(string brandCode, string deviceId);
        Task InsertAsync(string deviceId, decimal tma);
        Task<List<dynamic>> GetReadingDevice(string id);
        Task<VMAwlrLastReading> GetAwlrLastReading(string id);
        Task<AwlrSetting> GetAwlrSetting(string id);
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

        public async Task<VMAwlrLastReading?> GetAwlrLastReading(string id)
        {
            try
            {
                using var _db = new NpgsqlConnection(_connectionString);
                var query = @"SELECT * FROM ""AwlrLastReadings"" WHERE ""DeviceId"" = @DeviceId  ORDER BY ""ReadingAt"" DESC LIMIT 1";
                var result = await _db.QueryFirstOrDefaultAsync<VMAwlrLastReading>(query, new { DeviceId = id });
                return result;
            }
            catch (NpgsqlException ex)
            {
                Log.Error(ex, "PostgreSQL Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace, DeviceId = id });
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "General Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace, DeviceId = id });
                throw;
            }
        }

        public async Task<AwlrSetting?> GetAwlrSetting(string id)
        {
            try
            {
                using var _db = new NpgsqlConnection(_connectionString);
                var query = @"SELECT * FROM ""AwlrSettings"" WHERE ""DeviceId"" = @DeviceId LIMIT 1";
                var result = await _db.QueryFirstOrDefaultAsync<AwlrSetting>(query, new { DeviceId = id });
                return result;
            }
            catch (NpgsqlException ex)
            {
                Log.Error(ex, "PostgreSQL Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace, DeviceId = id });
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "General Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace, DeviceId = id });
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

        public async Task InsertAsync(string deviceId, decimal tma)
        {
            var device = await GetExistingDeviceAsync(deviceId);
            var reading = await GetAwlrLastReading(deviceId);
            var setting = await GetAwlrSetting(deviceId);

            if (device == null)
            {
                Log.Warning("Device ID {DeviceId} tidak ditemukan di MvDevices", deviceId);
                return; // atau throw error, tergantung logikamu
            }

            decimal? changeValue = reading != null ? tma - reading.WaterLevel : (decimal?)null;
            string changeStatus = null;
            string warningStatus = null;

            if (changeValue.HasValue)
            {
                if (changeValue.Value == 0)
                    changeStatus = "constant";
                else if (changeValue.Value < 0)
                    changeStatus = "decrease";
                else
                    changeStatus = "increase";
            }

            if (tma < setting.Siaga3)
            {
                warningStatus = "Normal";
            }
            else if (tma < setting.Siaga2 && tma >= setting.Siaga3)
            {
                warningStatus = "Siaga 3";
            }
            else if (tma < setting.Siaga1 && tma >= setting.Siaga2)
            {
                warningStatus = "Siaga 2";
            }
            else if (tma >= setting.Siaga1)
            {
                warningStatus = "Siaga 1";
            }

            var data = new AwlrLastReading
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                StationId = device.StationId,
                WaterLevel = (double)tma,
                ReadingAt = DateTime.Now,
                ChangeValue = changeValue,
                ChangeStatus = changeStatus,
                WarningStatus = warningStatus
            };

            _context.AwlrLastReadings.Add(data);
            await _context.SaveChangesAsync();
        }

        public async Task<List<dynamic>> GetReadingDevice(string id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var sql = @"
                SELECT 
                    st.""Name"" AS station,
                    st.""Type"" AS type,
                    r.""DeviceId"" AS device_id,
                    st.""Latitude"" AS latitude,
                    st.""Longitude"" AS longitude,
                    set.""Siaga1"" AS siaga1,
                    set.""Siaga2"" AS siaga2,
                    set.""Siaga3"" AS siaga3,
                    set.""UnitDisplay"" AS unit_display,
                    set.""UnitSensor"" AS unit_sensor,
                    r.""ReadingAt"" AS reading_at,
                    r.""WaterLevel"" AS water_level,
                    r.""ChangeValue"" AS change_value,
                    r.""ChangeStatus"" AS change_status,
                    r.""WarningStatus"" AS warning_status
                FROM ""AwlrLastReadings"" AS r 
                LEFT JOIN ""Stations"" AS st ON r.""StationId"" = st.""Id""
                LEFT JOIN ""AwlrSettings"" AS set ON st.""Id"" = set.""StationId""
                WHERE r.""DeviceId"" = @DeviceId";

            var result = await connection.QueryAsync(sql, new { DeviceId = id });
            return result.ToList();
        }
    }
}
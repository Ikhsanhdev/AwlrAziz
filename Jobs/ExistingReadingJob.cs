using Newtonsoft.Json;
using MQTTnet;
using MQTTnet.LowLevelClient;
using System.Security.Authentication;
using RestSharp;
// using AwlrAziz.Helpers;
using AwlrAziz.Interfaces;
using AwlrAziz.Models.Customs;
using AwlrAziz.Options;

namespace AwlrAziz.Jobs {
    public interface IExistingReadingJob
    {
       Task Reading(ExistingRequest request);
    }
    public class ExistingReadingJob : IExistingReadingJob
    {
        private readonly Serilog.ILogger _logger;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IConfiguration _config;
        IMqttClient? _client;
        private MqttClientOptions? _clientOptions;

        public ExistingReadingJob(Serilog.ILogger logger, IUnitOfWorkRepository unitOfWorkRepository, IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWorkRepository = unitOfWorkRepository;
            _config = configuration;
        }

        public async Task Reading(ExistingRequest request)
        {
            try
            {
                string deviceId = request.id;

                if(!string.IsNullOrEmpty(deviceId)) {
                    var existingDevice = await _unitOfWorkRepository.Devices.GetExistingDeviceAsync(deviceId);
                    
                    if(existingDevice == null) {
                        _logger.Warning($"Device with ID {deviceId} is not registered.");
                    }
                    else {

                        if(!string.IsNullOrEmpty(existingDevice.BrandCode) && !string.IsNullOrEmpty(existingDevice.StationType)) {
                            string brandCode = existingDevice.BrandCode;
                            string stationType = existingDevice.StationType;

                            var readingData = (object)null;

                            switch (stationType)
                            {
                                case "AWLR":
                                    if (request.tma.HasValue)
                                    {
                                        readingData = new
                                        {
                                            brandCode = brandCode,
                                            deviceId = deviceId,
                                            deviceType = "AWLR",
                                            readingAt = request.reading_at,
                                            waterLevel = request.tma,
                                            battery = request.volt
                                        };
                                    }
                                    break;

                                case "ARR":
                                    if (request.rain.HasValue)
                                    {
                                        readingData = new
                                        {
                                            brandCode = brandCode,
                                            deviceId = deviceId,
                                            deviceType = "ARR",
                                            readingAt = request.reading_at,
                                            rainfall = request.rain,
                                            battery = request.volt
                                        };
                                    }
                                    break;

                                case "AWLR_ARR":
                                    if (request.tma.HasValue && request.rain.HasValue)
                                    {
                                        readingData = new
                                        {
                                            brandCode = brandCode,
                                            deviceId = deviceId,
                                            deviceType = "AWLR_ARR",
                                            readingAt = request.reading_at,
                                            waterLevel = request.tma,
                                            rainfall = request.rain,
                                            battery = request.volt
                                        };
                                    }
                                    break;

                                case "AWS":
                                    readingData = new
                                    {
                                        brandCode = brandCode,
                                        deviceId = deviceId,
                                        deviceType = "AWS",
                                        readingAt = request.reading_at,
                                        humidity = ((bool)existingDevice.IsHumidity) ? request.hmd : null,
                                        rainfall = ((bool)existingDevice.IsRainfall) ? request.rf : null,
                                        pressure = ((bool)existingDevice.IsPressure) ? request.pr : null,
                                        solarRadiation = ((bool)existingDevice.IsSolarRadiation) ? request.sr : null,
                                        temperature = ((bool)existingDevice.IsTemperature) ? request.tmp : null,
                                        windDirection = ((bool)existingDevice.IsWindDirection) ? request.wd : null,
                                        windSpeed = ((bool)existingDevice.IsWindSpeed) ? request.ws : null,
                                        evaporation = ((bool)existingDevice.IsEvaporation) ? request.evp : null,
                                        battery = request.volt
                                    };
                                    break;

                                case "V-Notch":
                                    if (request.tma.HasValue)
                                    {
                                        readingData = new
                                        {
                                            brandCode = brandCode,
                                            deviceId = deviceId,
                                            deviceType = "V-Notch",
                                            readingAt = request.reading_at,
                                            waterLevel = request.tma,
                                            battery = request.volt
                                        };
                                    }
                                    break;

                                case "Piezometer":
                                    if (request.tma.HasValue)
                                    {
                                        readingData = new
                                        {
                                            brandCode = brandCode,
                                            deviceId = deviceId,
                                            deviceType = "Piezometer",
                                            readingAt = request.reading_at,
                                            waterLevel = request.tma,
                                            battery = request.volt
                                        };
                                    }
                                    break;

                                case "FlowMeter":
                                    if (request.flow_rate.HasValue)
                                    {
                                        readingData = new
                                        {
                                            brandCode = brandCode,
                                            deviceId = deviceId,
                                            deviceType = "FlowMeter",
                                            readingAt = request.reading_at,
                                            flowRate = request.flow_rate,
                                            battery = request.volt
                                        };
                                    }
                                    break;

                                case "WQMS":
                                    readingData = new
                                    {
                                        brandCode = brandCode,
                                        deviceId = deviceId,
                                        deviceType = "WQMS",
                                        readingAt = request.reading_at,
                                        temperature = request.tmp,
                                        ph = request.ph,
                                        orp = request.orp,
                                        turbidity = request.tbd,
                                        battery = request.volt
                                    };
                                    break;
                            }


                            if (readingData != null)
                            {
                                var loggerServiceUrl = _config.GetSection("LoggerServiceUrl").Get<string>();
                                var url = $"{loggerServiceUrl}/api/reading/device";

                                using (var client = new RestClient(url))
                                {
                                    var restRequest = new RestRequest(url, Method.Post);
                                    restRequest.AddHeader("Content-Type", "application/json");
                                    restRequest.AddJsonBody(readingData);

                                    try
                                    {
                                        var response = await client.ExecuteAsync(restRequest);

                                        if (!response.IsSuccessful)
                                        {
                                            _logger.Error("Error Send to Logger Service: {@ErrorMessage}", new { ErrorMessage = response.ErrorMessage });
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Error(ex, "Exception while sending request to Logger Service: {@ExceptionDetails}", new { ex.Message, ex.StackTrace, ReadingData = readingData });
                                    }
                                }
                            }

                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "General Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace, ExistingRequest = request });
                throw;
            }
        }
    }
}
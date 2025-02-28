using Gpib.Web.Data;
using Gpib.Web.Data.DBClasses;

//using Ivi.Visa;
//using NationalInstruments.Visa;

namespace Gpib.Web.DeviceHandling
{
    public class GPIBHandler
    {
        public Task ScanGpibDevices()
        {
            var ret = GlobalStaticVariables.PyVisaWrapper.ScanDevices();
            return ret;
        }
        public MeasurePointVariable Measure(FunctionDevice function, GPIBDevice device)
        {
            MeasurePointVariable result = new MeasurePointVariable();
            string response = "";
            //TODO: Implement the GPIB communication

            if (function.ExpectedReturns == 0)
            {
                GlobalStaticVariables.DbContext.LogMessages.Add(new()
                {
                    Message = "Measure Returns 0 values",
                    Source = "GPIBHandler",
                    CreationDateTime = System.DateTime.UtcNow,
                    Severity = LogSeverity.Info
                });
                _ =  GlobalStaticVariables.DbContext.SaveChanges();
                return new()
                {
                    IsValid = false,
                    IsSet = false,
                    TimeStamp = System.DateTime.UtcNow

                };
            }else if (function.ExpectedReturns == 1)
            {
                result.OrginalValue = response;
                result.IsValid = true;
                result.IsSet = true;
                result.TimeStamp = System.DateTime.UtcNow;
                
                try
                {
                    result.Value = decimal.Parse(response);
                }
                catch (System.Exception e)
                {
                    result.IsValid = false;
                    GlobalStaticVariables.DbContext.LogMessages.Add(new()
                    {
                        Message = "Convertion Error in Measure Returns 1 value",
                        Source = "GPIBHandler",
                        CreationDateTime = System.DateTime.UtcNow,
                        Severity = LogSeverity.Error
                    });
                    _ = GlobalStaticVariables.DbContext.SaveChanges();
                }

                result.Type = MPVPrimeType.Number;
                result.Unit = function.ReturnUnit;
                GlobalStaticVariables.DbContext.LogMessages.Add(new()
                {
                    Message = "Measure Returns 1 value",
                    Source = "GPIBHandler",
                    CreationDateTime = System.DateTime.UtcNow,
                    Severity = LogSeverity.Info
                });
                _ = GlobalStaticVariables.DbContext.SaveChanges();
                return result;
            }
            else
            {
                result.OrginalValue = response;
                result.IsValid = true;
                result.IsSet = true;
                result.TimeStamp = System.DateTime.UtcNow;
                try
                {
                    string[] values = [];
                    if (response.Contains(','))
                    {
                        values = response.Split(',');
                    } 
                    else if(response.Contains(';'))
                    {
                        values = response.Split(';');
                    }
                    else if (response.Contains(' '))
                    {
                        values = response.Split(' ');
                    }
                    else if(response.Contains('\n'))
                    {
                        values = response.Split('\n');
                    }

                    foreach (string value in values)
                    {
                        result.ValueList.Add(decimal.Parse(value));
                    }
                }
                catch (System.Exception e)
                {
                    result.IsValid = false;
                    LogHandling("Multi Convertion Error", "", LogSeverity.Error);
                }
                result.Type = MPVPrimeType.List;
                result.Unit = function.ReturnUnit;
                return result;
            }

            return new MeasurePointVariable()
            {
                IsValid = false,
                IsSet = false,
                TimeStamp = System.DateTime.UtcNow

            };
        }

        public MeasurePointVariable Mapping(MeasurePointVariable Input, MeasurePointVariable Start,
            MeasurePointVariable End)
        {
            if (Input.Type != MPVPrimeType.List || Start.Type == MPVPrimeType.Number || End.Type == MPVPrimeType.Number)
            {
                
            }

            return new();
        }

        private void LogHandling(string message, string source, LogSeverity severity)
        {
            GlobalStaticVariables.DbContext.LogMessages.Add(new()
            {
                Id = Guid.NewGuid().ToString(),
                Message = message,
                Source = source,
                CreationDateTime = System.DateTime.UtcNow,
                Severity = severity
            });
            _ = GlobalStaticVariables.DbContext.SaveChanges();
        }
    }
}

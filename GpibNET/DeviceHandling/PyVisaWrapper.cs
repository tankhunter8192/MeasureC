using Gpib.Web.Data.DBClasses;

namespace Gpib.Web.DeviceHandling
{
    using System;
    using System.Collections.Generic;
    using IronPython.Hosting;
    using Microsoft.Scripting.Hosting;
    public class PyVisaWrapper
    {
        private readonly ScriptEngine _engine;
        private readonly ScriptScope _scope;

        public PyVisaWrapper()
        {
            _engine = Python.CreateEngine();
            _scope = _engine.CreateScope();

            var paths = _engine.GetSearchPaths();
            paths.Add(@"./PythonLibs");
            _engine.SetSearchPaths(paths);
        }

        public dynamic ExecuteScript(string script)
        {
            return _engine.Execute(script, _scope);
        }

        public dynamic ExecuteFunction(string script, string functionName, params object[] args)
        {
            _engine.Execute(script, _scope);
            dynamic func = _scope.GetVariable(functionName);
            return func(args);
        }

        public Task ScanDevices()
        {
            string script = @"
import pyvisa as visa

def scan_devices():
    rm = visa.ResourceManager()
    devices = rm.list_resources()
    return devices";

            dynamic devices  = ExecuteFunction(script, "scan_devices");
            List<string> deviceList = new List<string>();
            foreach (var device in devices)
            {
                deviceList.Add(device.ToString());
            }
            //TODO: Save devices to database
            return Task.CompletedTask;
        }

        public MeasurePointVariable Measure(GPIBDevice device, FunctionDevice function, params object[] args)
        {
            string script = @"
import pyvisa as visa

def Connect(connectionstring,command):
    rm = visa.ResourceManager()
    device = rm.";
            return null!;
        }

    }
}

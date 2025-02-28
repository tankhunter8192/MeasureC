namespace Gpib.Web.Data.DBClasses;

public class DataBaseDevice : IDevice
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public EnumPrimaryDeviceTyp DeviceTyp { get; set; } = EnumPrimaryDeviceTyp.DataBase;
    public string Description { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = String.Empty; //version?
    public string SerialNumber { get; set; } = String.Empty;//here MAC?
    //TODO: Login Data store

    public bool Write(string functionName)
    {
        switch (Manufacturer)
        {
            //TODO: Add more DB types
            case "Microsoft SQL Server":
            {
                
            }
                break;
            case "InfluxDB V2":
            {
                
            }
                break;
            case "InfluxDB V1":
            {
                
            }
                break;
            case "MySQL":
            case "MariaDB":
            {

            }
                break;
            default:
                return false;
                
        }
        return false;
    }

}
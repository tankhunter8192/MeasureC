using Gpib.Web.Data.DBClasses;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Gpib.Web.Data.DBClasses
{
    public enum MPVPrimeType
    {
        Error,
        String,
        Number,
        DateTime,
        TimeSpan,
        StringMap,
        NumberMap,
        List,
        Empty,
        Other
    }

    public class MeasurePointVariable
    {
        public int Id { set; get; }
        public string Name { set; get; } = "";
        public MPVPrimeType Type { set; get; } = MPVPrimeType.Other;
        public string Text { set; get; } = "";
        public string OrginalValue { set; get; } = "";
        public string Unit { set; get; } = "";
        public decimal Value { set; get; } = decimal.Zero;
        public DateTime DateTime { set; get; } = DateTime.MinValue;
        public TimeSpan TimeSpan { set; get; } = TimeSpan.Zero;
        public List<decimal> ValueList { set; get; } = new List<decimal>();
        public Dictionary<string, decimal> StringMap { set; get; } = new Dictionary<string, decimal>();
        public Dictionary<decimal, decimal> NumberMap { set; get; } = new Dictionary<decimal, decimal>();
        public bool IsSet { set; get; } = false;
        public bool IsValid { set; get; } = false;
        public DateTime TimeStamp { set; get; } = DateTime.UtcNow; //Gets the latest Change of the variable

        public MeasurePointVariable(MPVPrimeType type)
        {
            Type = type;
            IsSet = false;
            IsValid = false;
            TimeStamp = DateTime.UtcNow;
        }
        public MeasurePointVariable(int a)
        {
            OrginalValue = a.ToString();
            Type = MPVPrimeType.Number;
            Value = (decimal)a;
            IsSet = true;
            IsValid = true;
            TimeStamp = DateTime.UtcNow;
        }

        public MeasurePointVariable(decimal a)
        {
            OrginalValue = a.ToString();
            Type = MPVPrimeType.Number;
            Value = a;
            IsSet = true;
            IsValid = true;
            TimeStamp = DateTime.UtcNow;
        }

        public MeasurePointVariable(string a)
        {
            OrginalValue = a;
            Type = MPVPrimeType.String;
            Text = a;
            IsSet = true;
            IsValid = true;
            TimeStamp = DateTime.UtcNow;
        }

        public MeasurePointVariable(DateTime a)
        {
            OrginalValue = a.ToString();
            Type = MPVPrimeType.DateTime;
            DateTime = a;
            IsSet = true;
            IsValid = true;
            TimeStamp = DateTime.UtcNow;
        }

        public MeasurePointVariable(TimeSpan a)
        {
            OrginalValue = a.ToString();
            Type = MPVPrimeType.TimeSpan;
            TimeSpan = a;
            IsSet = true;
            IsValid = true;
            TimeStamp = DateTime.UtcNow;
        }

        public MeasurePointVariable(List<decimal> a)
        {
            OrginalValue = a.ToString();
            Type = MPVPrimeType.List;
            ValueList = a;
            IsSet = true;
            IsValid = true;
            TimeStamp = DateTime.UtcNow;
        }

        public MeasurePointVariable(Dictionary<string, decimal> a)
        {
            OrginalValue = a.ToString();
            Type = MPVPrimeType.StringMap;
            StringMap = a;
            IsSet = true;
            IsValid = true;
            TimeStamp = DateTime.UtcNow;
        }

        public MeasurePointVariable(Dictionary<decimal, decimal> a)
        {
            OrginalValue = a.ToString();
            Type = MPVPrimeType.NumberMap;
            NumberMap = a;
            IsSet = true;
            IsValid = true;
            TimeStamp = DateTime.UtcNow;
        }

        public MeasurePointVariable()
        {
            OrginalValue = "";
            Type = MPVPrimeType.Empty;
            IsSet = false;
            IsValid = false;
            TimeStamp = DateTime.UtcNow;
        }
        //Operators
        public static MeasurePointVariable operator +(MeasurePointVariable a, MeasurePointVariable b)
        {
            MeasurePointVariable result = new MeasurePointVariable();
            if (a.IsSet && b.IsSet)
            {
                result.IsSet = true;
            }
            else
            {
                result.IsSet = false;

            }
            if (a.Type == MPVPrimeType.Number && b.Type == MPVPrimeType.Number)
            {
                result.Type = MPVPrimeType.Number;
                result.Value = a.Value + b.Value;
            }
            else if (a.Type == MPVPrimeType.Number && b.Type == MPVPrimeType.List)
            {
                result.Type = MPVPrimeType.List;
                result.ValueList = a.ValueList;
                result.ValueList.Add(b.Value);
            }
            else if (a.Type == MPVPrimeType.List && b.Type == MPVPrimeType.Number)
            {
                result.Type = MPVPrimeType.List;
                result.ValueList = b.ValueList;
                result.ValueList.Add(a.Value);
            }
            else if (a.Type == MPVPrimeType.List && b.Type == MPVPrimeType.List)
            {
                result.Type = MPVPrimeType.List;
                result.ValueList = a.ValueList;
                result.ValueList.AddRange(b.ValueList);
            }
            else if (a.Type == MPVPrimeType.String && b.Type == MPVPrimeType.String)
            {
                result.Type = MPVPrimeType.String;
                result.Text = a.Text + b.Text;
            } else if (a.Type == MPVPrimeType.DateTime && b.Type == MPVPrimeType.DateTime)
            {
                result.Type = MPVPrimeType.DateTime;
                result.DateTime = a.DateTime;
                result.DateTime= result.DateTime.Add(b.TimeSpan);
            }else if (a.Type == MPVPrimeType.DateTime && b.Type == MPVPrimeType.TimeSpan)
            {
                result.Type = MPVPrimeType.DateTime;
                result.DateTime = a.DateTime;
                result.DateTime = result.DateTime.Add(b.TimeSpan);
            }
            else if (a.Type == MPVPrimeType.TimeSpan && b.Type == MPVPrimeType.TimeSpan)
            {
                result.Type = MPVPrimeType.TimeSpan;
                result.TimeSpan = a.TimeSpan + b.TimeSpan;
            }else if (a.Type == MPVPrimeType.TimeSpan && b.Type == MPVPrimeType.DateTime)
            {
                result.Type = MPVPrimeType.DateTime;
                result.DateTime = b.DateTime;
                result.DateTime = result.DateTime.Add(a.TimeSpan);
            }
            else
            {
                result.Type = MPVPrimeType.Error;
                result.Text = "Addition is not supported for the given input types";
                result.OrginalValue = "(" + a.OrginalValue + " + " + b.OrginalValue + ")";
                result.IsSet = false;
                result.IsValid = false;
                result.TimeStamp = DateTime.UtcNow;
                return result;
            }
            result.OrginalValue = "(" + a.OrginalValue + " + " + b.OrginalValue + ")";
            result.IsValid = a.IsValid && b.IsValid;
            result.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            return result;
        }

        public static MeasurePointVariable operator -(MeasurePointVariable a, MeasurePointVariable b)
        {
            MeasurePointVariable result = new MeasurePointVariable();
            result.IsSet = a.IsSet && b.IsSet;
            if (a.Type == MPVPrimeType.Number && b.Type == MPVPrimeType.Number)
            {
                result.Type = MPVPrimeType.Number;
                result.Value = a.Value - b.Value;
            }
            else if (a.Type == MPVPrimeType.List && b.Type == MPVPrimeType.Number)
            {
                result.Type = MPVPrimeType.List;
                result.ValueList = a.ValueList;
                result.ValueList.Remove(b.Value);
            }
            else if (a.Type == MPVPrimeType.List && b.Type == MPVPrimeType.List)
            {
                result.Type = MPVPrimeType.List;
                result.ValueList = a.ValueList;
                foreach (var item in b.ValueList)
                {
                    result.ValueList.Remove(item);
                }
            }else if (a.Type == MPVPrimeType.DateTime && b.Type == MPVPrimeType.DateTime)
            {
                result.Type = MPVPrimeType.TimeSpan;
                result.TimeSpan = a.DateTime - b.DateTime;
            }else if (a.Type == MPVPrimeType.DateTime && b.Type == MPVPrimeType.TimeSpan)
            {
                result.Type = MPVPrimeType.DateTime;
                result.DateTime = a.DateTime;
                result.DateTime = result.DateTime.Subtract(b.TimeSpan);
            }else if (a.Type == MPVPrimeType.TimeSpan && b.Type == MPVPrimeType.TimeSpan)
            {
                result.Type = MPVPrimeType.TimeSpan;
                result.TimeSpan = a.TimeSpan - b.TimeSpan;
            }
            else
            {
                result.Type = MPVPrimeType.Error;
                result.Text = "Subtraction is not supported for the given input types";
                result.OrginalValue = "(" + a.OrginalValue + " - " + b.OrginalValue + ")"; ;
                result.IsSet = false;
                result.IsValid = false;
                result.TimeStamp = DateTime.UtcNow;
                return result;
            }
            result.OrginalValue = "(" + a.OrginalValue + " - " + b.OrginalValue + ")";
            result.IsValid = a.IsValid && b.IsValid;
            result.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            return result;
        }

        public static MeasurePointVariable operator *(MeasurePointVariable a, MeasurePointVariable b)
        {
            MeasurePointVariable result = new MeasurePointVariable();
            result.IsSet = a.IsSet && b.IsSet;
            if (a.Type == MPVPrimeType.Number && b.Type == MPVPrimeType.Number)
            {
                result.Type = MPVPrimeType.Number;
                result.Value = a.Value * b.Value;
            }
            else if (a.Type == MPVPrimeType.Number && b.Type == MPVPrimeType.List)
            {
                result.Type = MPVPrimeType.List;
                result.ValueList = b.ValueList;
                for (int i = 0; i < result.ValueList.Count; i++)
                {
                    result.ValueList[i] = result.ValueList[i] * a.Value;
                }
            }
            else if (a.Type == MPVPrimeType.List && b.Type == MPVPrimeType.Number)
            {
                result.Type = MPVPrimeType.List;
                result.ValueList = a.ValueList;
                for (int i = 0; i < result.ValueList.Count; i++)
                {
                    result.ValueList[i] = a.ValueList[i] * b.Value;
                }
            }
            else if (a.Type == MPVPrimeType.List && b.Type == MPVPrimeType.List)
            {
                result.Type = MPVPrimeType.List;
                result.ValueList = a.ValueList;
                for (int i = 0; i < result.ValueList.Count; i++)
                {
                    result.ValueList[i] = a.ValueList[i] * b.ValueList[i];
                }
            }//TODO: Implement Maps
            else
            {
                result.Type = MPVPrimeType.Error;
                result.Text = "Multiplication is not supported for the given input types";
                result.OrginalValue = "(" + a.OrginalValue + " * " + b.OrginalValue + ")";
                result.IsSet = false;
                result.IsValid = false;
                result.TimeStamp = DateTime.UtcNow;
                return result;
            }

            
            result.OrginalValue = "(" + a.OrginalValue + " * " + b.OrginalValue + ")";
            result.IsValid = a.IsValid && b.IsValid;
            result.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            return result;
        }

        public static MeasurePointVariable operator /(MeasurePointVariable a, MeasurePointVariable b)
        {
            MeasurePointVariable result = new MeasurePointVariable();
            result.IsSet = a.IsSet && b.IsSet;
            if (a.Type == MPVPrimeType.Number && b.Type == MPVPrimeType.Number)
            {
                result.Type = MPVPrimeType.Number;
                result.Value = a.Value / b.Value;
            }
            else if (a.Type == MPVPrimeType.List && b.Type == MPVPrimeType.Number)
            {
                result.Type = MPVPrimeType.List;
                result.ValueList = a.ValueList;
                for (int i = 0; i < result.ValueList.Count; i++)
                {
                    result.ValueList[i] = b.Value !=0 ? result.ValueList[i] / b.Value : 0;
                    result.IsValid = b.Value != 0 && (a.IsValid && (b.IsValid));
                }
            }
            else if (a.Type == MPVPrimeType.List && b.Type == MPVPrimeType.List)
            {
                result.Type = MPVPrimeType.List;
                result.ValueList = a.ValueList;
                for (int i = 0; i < result.ValueList.Count; i++)
                {
                    result.ValueList[i] = a.ValueList[i] / b.ValueList[i];
                }
            }
            else
            {
                result.Type = MPVPrimeType.Error;
                result.Text = "Division is not supported for the given input types";
                result.OrginalValue = "(" + a.OrginalValue + " / " + b.OrginalValue + ")";
                result.IsSet = false;
                result.IsValid = false;
                result.TimeStamp = DateTime.UtcNow;
                return result;
            }
            result.OrginalValue = "(" + a.OrginalValue + " / " + b.OrginalValue + ")";
            result.IsValid = a.IsValid && b.IsValid;
            result.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            return result;
        }

        public static MeasurePointVariable operator %(MeasurePointVariable a, MeasurePointVariable b)
        {
            MeasurePointVariable result = new MeasurePointVariable();
            result.IsSet = a.IsSet && b.IsSet;
            if (a.Type == MPVPrimeType.Number && b.Type == MPVPrimeType.Number)
            {
                result.Type = MPVPrimeType.Number;
                result.Value = a.Value % b.Value;
            }
            else if (a.Type == MPVPrimeType.List && b.Type == MPVPrimeType.Number)
            {
                result.Type = MPVPrimeType.List;
                result.ValueList = a.ValueList;
                for (int i = 0; i < result.ValueList.Count; i++)
                {
                    result.ValueList[i] = b.Value != 0 ? result.ValueList[i] % b.Value : 0;
                    result.IsValid = b.Value != 0 && (a.IsValid && (b.IsValid));
                }
            }
            else if (a.Type == MPVPrimeType.List && b.Type == MPVPrimeType.List)
            {
                result.Type = MPVPrimeType.List;
                result.ValueList = a.ValueList;
                for (int i = 0; i < result.ValueList.Count; i++)
                {
                    result.ValueList[i] = a.ValueList[i] % b.ValueList[i];
                }
            }
            else
            {
                result.Type = MPVPrimeType.Error;
                result.Text = "Modulo is not supported for the given input types";
                result.OrginalValue = "(" + a.OrginalValue + " % " + b.OrginalValue + ")";
                result.IsSet = false;
                result.IsValid = false;
                result.TimeStamp = DateTime.UtcNow;
                return result;
            }
            result.OrginalValue = "(" + a.OrginalValue + " % " + b.OrginalValue + ")";
            result.IsValid = a.IsValid && b.IsValid;
            result.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            return result;
        }

        public static MeasurePointVariable operator !(MeasurePointVariable a)
        {
            MeasurePointVariable result = new MeasurePointVariable();
            result.IsSet = a.IsSet;
            switch (a.Type)
            {
                case MPVPrimeType.Error:
                    result = a;
                    break;
                case MPVPrimeType.Number:
                    result.Value = (a.Value != Decimal.Zero) ? (Decimal.Zero) : (Decimal.One);
                    result.Type = MPVPrimeType.Number;
                    result.OrginalValue = "!(" + a.OrginalValue + ")";
                    result.IsValid = a.IsValid;
                    break;
                case MPVPrimeType.List:
                    result.ValueList = new List<decimal>();
                    foreach (var item in a.ValueList)
                    {
                        result.ValueList.Add(item != Decimal.Zero ? Decimal.Zero : Decimal.One);
                    }
                    result.Type = MPVPrimeType.List;
                    result.OrginalValue = "!(" + a.OrginalValue + ")";
                    result.IsValid = a.IsValid;
                    break;
                case MPVPrimeType.NumberMap:
                    result.NumberMap = new Dictionary<decimal, decimal>();
                    foreach (var item in a.NumberMap)
                    {
                        result.NumberMap.Add(item.Key, item.Value != Decimal.Zero ? Decimal.Zero : Decimal.One);
                    }
                    result.Type = MPVPrimeType.NumberMap;
                    result.OrginalValue = "!(" + a.OrginalValue + ")";
                    result.IsValid = a.IsValid;
                    break;
                case MPVPrimeType.StringMap:
                    result.StringMap = new Dictionary<string, decimal>();
                    foreach (var item in a.StringMap)
                    {
                        result.StringMap.Add(item.Key, item.Value != Decimal.Zero ? Decimal.Zero : Decimal.One);
                    }
                    result.Type = MPVPrimeType.StringMap;
                    result.OrginalValue = "!(" + a.OrginalValue + ")";
                    result.IsValid = a.IsValid;
                    break;
                default:
                    result.Type = MPVPrimeType.Error;
                    result.Text = "Not operation is not supported for the given input type";
                    result.OrginalValue = "!(" + a.OrginalValue + ")";
                    result.IsSet = false;
                    result.IsValid = false;
                    result.TimeStamp = DateTime.UtcNow;
                    return result;
            }
            result.TimeStamp = a.TimeStamp;
            return result;
        }

        public static MeasurePointVariable operator &(MeasurePointVariable a, MeasurePointVariable b)
        {
            MeasurePointVariable res = new MeasurePointVariable();
            if (!(a.IsValid && b.IsValid))
            {
                res.IsValid = false;
            }
            if (a.Type == MPVPrimeType.Number && b.Type == MPVPrimeType.Number)
            {
                res.Value = ((int)a.Value) & ((int)b.Value);
                res.Type = MPVPrimeType.Number;
                res.OrginalValue = "(" + a.OrginalValue + " & " + b.OrginalValue + ")"; ;
                res.IsSet = a.IsSet && b.IsSet;
                res.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            }
            else if (a.Type == MPVPrimeType.List && b.Type == MPVPrimeType.Number)
            {
                for (int i = 0; i < a.ValueList.Count; i++)
                {
                    res.ValueList.Add(((int)a.ValueList[i]) & ((int)b.Value));
                }
                res.Type = MPVPrimeType.List;
                res.OrginalValue = "(" + a.OrginalValue + " & " + b.OrginalValue + ")"; ;
                res.IsSet = a.IsSet && b.IsSet;
                res.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            }
            else if(a.Type == MPVPrimeType.Number && b.Type == MPVPrimeType.List)
            {
                for (int i = 0; i < b.ValueList.Count; i++)
                {
                    res.ValueList.Add(((int)a.Value) & ((int)b.ValueList[i]));
                }
                res.Type = MPVPrimeType.List;
                res.OrginalValue = "(" + a.OrginalValue + " & " + b.OrginalValue + ")"; ;
                res.IsSet = a.IsSet && b.IsSet;
                res.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            }
            else if(a.Type == MPVPrimeType.List && b.Type == MPVPrimeType.List)
            {
                for (int i = 0;
                     i < (a.ValueList.Count > b.ValueList.Count ? b.ValueList.Count : a.ValueList.Count);
                     i++)
                {
                    res.ValueList.Add(((int)a.ValueList[i]) & ((int)b.ValueList[i]));
                }
                res.Type = MPVPrimeType.List;
                res.OrginalValue = "(" + a.OrginalValue + " & " + b.OrginalValue + ")"; ;
                res.IsSet = a.IsSet && b.IsSet;
                res.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            }
            else
            {
                res.Type = MPVPrimeType.Error;
                res.Text = "AND is not supported for the given input types";
                res.OrginalValue = "(" + a.OrginalValue + " & " + b.OrginalValue + ")"; ;
                res.IsSet = false;
                res.IsValid = false;
                res.TimeStamp = DateTime.UtcNow;
            }
            return res;
        }

        public static MeasurePointVariable operator |(MeasurePointVariable a, MeasurePointVariable b)
        {
            MeasurePointVariable res = new MeasurePointVariable();
            if (!(a.IsValid && b.IsValid))
            {
                res.IsValid = false;
            }
            if (a.Type == MPVPrimeType.Number && b.Type == MPVPrimeType.Number)
            {
                res.Value = ((int)a.Value) | ((int)b.Value);
                res.Type = MPVPrimeType.Number;
                res.OrginalValue = "(" + a.OrginalValue + " | " + b.OrginalValue + ")";
                res.IsSet = a.IsSet && b.IsSet;
                res.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            }
            else if (a.Type == MPVPrimeType.List && b.Type == MPVPrimeType.Number)
            {
                for (int i = 0; i < a.ValueList.Count; i++)
                {
                    res.ValueList.Add(((int)a.ValueList[i]) | ((int)b.Value));
                }
                res.Type = MPVPrimeType.List;
                res.OrginalValue = "(" + a.OrginalValue + " | " + b.OrginalValue + ")";
                res.IsSet = a.IsSet && b.IsSet;
                res.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            }
            else if (a.Type == MPVPrimeType.Number && b.Type == MPVPrimeType.List)
            {
                for (int i = 0; i < b.ValueList.Count; i++)
                {
                    res.ValueList.Add(((int)a.Value) | ((int)b.ValueList[i]));
                }
                res.Type = MPVPrimeType.List;
                res.OrginalValue = "(" + a.OrginalValue + " | " + b.OrginalValue + ")";
                res.IsSet = a.IsSet && b.IsSet;
                res.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            }
            else if (a.Type == MPVPrimeType.List && b.Type == MPVPrimeType.List)
            {
                for (int i = 0; i < (a.ValueList.Count>b.ValueList.Count?b.ValueList.Count:a.ValueList.Count); i++)
                {
                    res.ValueList.Add(((int)a.ValueList[i]) | ((int)b.ValueList[i]));
                }
                res.Type = MPVPrimeType.List;
                res.OrginalValue = "(" + a.OrginalValue + " | " + b.OrginalValue + ")";
                res.IsSet = a.IsSet && b.IsSet;
                res.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            }
            else
            {
                res.Type = MPVPrimeType.Error;
                res.Text = "OR is not supported for the given input types";
                res.OrginalValue = "(" + a.OrginalValue + " | " + b.OrginalValue + ")";
                res.IsSet = false;
                res.IsValid = false;
                res.TimeStamp = DateTime.UtcNow;
            }
            return res;
        }

        public static MeasurePointVariable operator ^(MeasurePointVariable a, MeasurePointVariable b)
        {
            MeasurePointVariable res = new MeasurePointVariable();
            if (!(a.IsValid && b.IsValid))
            {
                res.IsValid = true;
            }
            if (a.Type == MPVPrimeType.Number && b.Type == MPVPrimeType.Number)
            {
               res.Value = ((int)a.Value) ^ ((int)b.Value);
               res.Type = MPVPrimeType.Number;
               res.OrginalValue = "(" + a.OrginalValue + " ^ " + b.OrginalValue + ")";
               res.IsSet = a.IsSet && b.IsSet;
               res.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            }
            else if (a.Type == MPVPrimeType.Number && b.Type == MPVPrimeType.List)
            {
                for (int i = 0; i < b.ValueList.Count; i++)
                {
                    res.ValueList.Add(((int)a.Value) ^ ((int)b.ValueList[i]));
                }
                res.Type = MPVPrimeType.List;
                res.OrginalValue = "(" + a.OrginalValue + " ^ " + b.OrginalValue + ")";
                res.IsSet = a.IsSet && b.IsSet;
                res.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            }
            else if (a.Type == MPVPrimeType.List && b.Type == MPVPrimeType.Number)
            {
                for (int i = 0; i < a.ValueList.Count; i++)
                {
                    res.ValueList.Add(((int)a.ValueList[i]) ^ ((int)b.Value));
                }
                res.Type = MPVPrimeType.List;
                res.OrginalValue = "(" + a.OrginalValue + " ^ " + b.OrginalValue + ")";
                res.IsSet = a.IsSet && b.IsSet;
                res.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            }
            else if(a.Type == MPVPrimeType.List && b.Type == MPVPrimeType.List)
            {
                for(int i = 0; i < (a.ValueList.Count <= b.ValueList.Count ? a.ValueList.Count : b.ValueList.Count); i++)
                {
                    res.ValueList.Add(((int)a.ValueList[i]) ^ ((int)b.ValueList[i]));
                }
                res.Type = MPVPrimeType.List;
                res.OrginalValue = "(" + a.OrginalValue + " ^ " + b.OrginalValue + ")";
                res.IsSet = a.IsSet && b.IsSet;
                res.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            }
            else
            {
                res.Type = MPVPrimeType.Error;
                res.Text = "XOR is not supported for the given input types";
                res.OrginalValue = "(" + a.OrginalValue + " ^ " + b.OrginalValue + ")";
                res.IsSet = false;
                res.IsValid = false;
                res.TimeStamp = DateTime.UtcNow;
            }
            return res;
        }

        public static MeasurePointVariable operator ==(MeasurePointVariable a, MeasurePointVariable b)
        {
            MeasurePointVariable res = new MeasurePointVariable();
            if (!(a.IsValid && b.IsValid))
            {
                res.IsValid = false;
            }

            if (a.Type == b.Type)
            {
                switch (a.Type)
                {
                    case MPVPrimeType.Error:
                        if (res.Type == MPVPrimeType.Empty) res.Type = MPVPrimeType.Error;
                        goto case MPVPrimeType.String;
                    case MPVPrimeType.String:
                        if (a.Text == b.Text)
                        {
                            res.Text = a.Text;
                        }
                        else
                        {
                            res.Text = a.Text + " != " + b.Text;
                            
                        }
                        break;
                    case MPVPrimeType.Number:
                        if (a.Value == b.Value)
                        {
                            res.Value = a.Value;
                        }
                        else
                        {
                            res.Value = Decimal.Zero;
                            res.Text = a.Value + " != " + b.Value;
                        }
                        break;
                    case MPVPrimeType.DateTime:
                        if (a.DateTime == b.DateTime)
                        {
                            res.DateTime = a.DateTime;
                        }
                        else
                        {
                            res.DateTime = DateTime.MinValue;
                            res.Text = a.DateTime + " != " + b.DateTime;
                        }
                        break;
                    case MPVPrimeType.TimeSpan:
                        if (a.TimeSpan == b.TimeSpan)
                        {
                            res.TimeSpan = a.TimeSpan;
                        }
                        else
                        {
                            res.TimeSpan = TimeSpan.Zero;
                            res.Text = a.TimeSpan + " != " + b.TimeSpan;
                        }
                        break;
                    case MPVPrimeType.List:
                        if(a.ValueList.Count == b.ValueList.Count)
                        {
                            for (int i = 0; i < a.ValueList.Count; i++)
                            {
                                res.ValueList.Add(a.ValueList[i] == b.ValueList[i] ? a.ValueList[i] : Decimal.Zero);
                            }
                        }
                        else
                        {
                            res.ValueList = a.ValueList;
                            res.Text = a.Text + " != " + b.Text;
                        }
                        break;
                    case MPVPrimeType.StringMap:
                        if (a.StringMap.Count == b.StringMap.Count)
                        {
                            foreach (var item in a.StringMap)
                            {
                                if (b.StringMap.ContainsKey(item.Key))
                                {
                                    if (b.StringMap[item.Key] == item.Value)
                                    {
                                        res.StringMap.Add(item.Key, item.Value);
                                    }
                                    else
                                    {
                                        res.StringMap.Add(item.Key, Decimal.Zero);
                                    }
                                }
                                else
                                {
                                    res.StringMap.Add(item.Key, Decimal.Zero);
                                }
                            }
                        }
                        else
                        {
                            res.StringMap = a.StringMap;
                            res.Text = a.Text + " != " + b.Text;
                        }
                        break;
                    case MPVPrimeType.NumberMap:
                        if (a.NumberMap.Count == b.NumberMap.Count)
                        {
                            foreach (var item in a.NumberMap)
                            {
                                if (b.NumberMap.ContainsKey(item.Key))
                                {
                                    if (b.NumberMap[item.Key] == item.Value)
                                    {
                                        res.NumberMap.Add(item.Key, item.Value);
                                    }
                                    else
                                    {
                                        res.NumberMap.Add(item.Key, Decimal.Zero);
                                    }
                                }
                                else
                                {
                                    res.NumberMap.Add(item.Key, Decimal.Zero);
                                }
                            }
                        }
                        else
                        {
                            res.NumberMap = a.NumberMap;
                            res.Text = a.Text + " != " + b.Text;
                        }
                        break;
                    case MPVPrimeType.Empty:
                        res.Type = MPVPrimeType.Empty;
                        break;
                    case MPVPrimeType.Other:
                        res.Type = MPVPrimeType.Other;
                        break;
                    default:
                        res.Type = MPVPrimeType.Error;
                        res.Text = "Equality is not supported for the given input types, not implemented yet";
                        res.OrginalValue = "(" + a.OrginalValue + " == " + b.OrginalValue + ")";
                        res.IsSet = false;
                        res.IsValid = false;
                        res.TimeStamp = DateTime.UtcNow;
                        break;
                }
                res.OrginalValue = "(" + a.OrginalValue + " == " + b.OrginalValue + ")";
            }
            else
            {
                
                res.Type = MPVPrimeType.Number;
                res.Value = Decimal.Zero;
                res.Text = "Different types";
                res.OrginalValue = "(" + a.OrginalValue + " == " + b.OrginalValue + ")";
                res.IsSet = true;
                res.IsValid = false; // Compare Apples with Rasberries #Pi4Fun
                res.TimeStamp = DateTime.UtcNow;
            }
            return res;
        }
        
        public static MeasurePointVariable operator !=(MeasurePointVariable a, MeasurePointVariable b)
        {
            MeasurePointVariable res = new MeasurePointVariable();
            res = a == b;
            return res;
        }

        public static MeasurePointVariable operator >(MeasurePointVariable a, MeasurePointVariable b)
        {
            MeasurePointVariable res = new MeasurePointVariable();
            if (a.Type == b.Type)
            {
                switch (a.Type)
                {
                    case MPVPrimeType.Error:
                        //TODO: Implement error comparison
                        if (res.Type == MPVPrimeType.Empty) res.Type = MPVPrimeType.Error;
                        goto case MPVPrimeType.String;
                    case MPVPrimeType.String:
                        //TODO: Implement string comparison
                        if (a.Text.CompareTo(b.Text) > 0)
                        {
                            res.Text = a.Text;
                        }
                        else
                        {
                            res.Text = a.Text + " <= " + b.Text;
                        }

                        break;
                    case MPVPrimeType.Number:
                        if (a.Value > b.Value)
                        {
                            res.Value = Decimal.One;
                        }
                        else
                        {
                            res.Value = Decimal.Zero;

                        }

                        res.Type = MPVPrimeType.Number;
                        break;
                    case MPVPrimeType.DateTime:
                        if (a.DateTime > b.DateTime)
                        {
                            res.Value = Decimal.One;
                        }
                        else
                        {
                            res.Value = Decimal.Zero;

                        }

                        res.Type = MPVPrimeType.Number;
                        break;
                    case MPVPrimeType.TimeSpan:
                        if (a.TimeSpan > b.TimeSpan)
                        {
                            res.Value = Decimal.One;
                        }
                        else
                        {
                            res.Value = Decimal.Zero;
                        }

                        res.Type = MPVPrimeType.Number;
                        break;
                    case MPVPrimeType.List:
                        if (a.ValueList.Count == b.ValueList.Count)
                        {
                            for (int i = 0; i < a.ValueList.Count; i++)
                            {
                                res.ValueList.Add(a.ValueList[i] > b.ValueList[i] ? Decimal.One : Decimal.Zero);
                            }
                        }
                        else
                        {
                            //TODO: find logic for different length lists
                            res.ValueList = a.ValueList;
                            res.Text = a.Text + " <= " + b.Text;
                        }

                        res.Type = MPVPrimeType.List;
                        break;
                    case MPVPrimeType.StringMap:
                        if (a.StringMap.Count == b.StringMap.Count)
                        {
                            foreach (var item in a.StringMap)
                            {
                                if (b.StringMap.ContainsKey(item.Key))
                                {
                                    if (b.StringMap[item.Key] > item.Value)
                                    {
                                        res.StringMap.Add(item.Key, Decimal.One);
                                    }
                                    else
                                    {
                                        res.StringMap.Add(item.Key, Decimal.Zero);
                                    }
                                }
                                else
                                {
                                    res.StringMap.Add(item.Key, Decimal.Zero);
                                }
                            }
                        }
                        else
                        {
                            //TODO: find logic for different length lists
                            res.StringMap = a.StringMap;
                            res.Text = a.Text + " <= " + b.Text;
                        }

                        res.Type = MPVPrimeType.StringMap;
                        break;
                    case MPVPrimeType.NumberMap:
                        if (a.NumberMap.Count == b.NumberMap.Count)
                        {
                            foreach (var item in a.NumberMap)
                            {
                                if (b.NumberMap.ContainsKey(item.Key))
                                {
                                    if (b.NumberMap[item.Key] > item.Value)
                                    {
                                        res.NumberMap.Add(item.Key, Decimal.One);
                                    }
                                    else
                                    {
                                        res.NumberMap.Add(item.Key, Decimal.Zero);
                                    }
                                }
                                else
                                {
                                    res.NumberMap.Add(item.Key, Decimal.Zero);
                                }
                            }
                        }
                        else
                        {
                            //TODO: find logic for different length lists
                            //find all indexs
                            // if not there assume 0
                            //like equal length
                        }
                        res.Type = MPVPrimeType.NumberMap;
                        break;
                    case MPVPrimeType.Empty:
                        res.Type = MPVPrimeType.Empty;
                        break;
                    case MPVPrimeType.Other:
                        res.Type = MPVPrimeType.Other;
                        break;
                    default:
                        res.Type = MPVPrimeType.Error;
                        res.Text = "Greater than is not supported for the given input types, not implemented yet";
                        res.OrginalValue = "(" + a.OrginalValue + " > " + b.OrginalValue + ")";
                        res.IsSet = false;
                        res.IsValid = false;
                        res.TimeStamp = DateTime.UtcNow;
                        break;
                }

                res.Text = a.Value + " <= " + b.Value;
                res.OrginalValue = "(" + a.OrginalValue + " > " + b.OrginalValue + ")";
                res.IsSet = a.IsSet && b.IsSet;
                res.IsValid = a.IsValid && b.IsValid;
                res.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            }
            else
            {
                res.Type = MPVPrimeType.Error;
                res.Text = "Greater than is not supported for the given input types";
                res.OrginalValue = "(" + a.OrginalValue + " > " + b.OrginalValue + ")";
                res.IsSet = false;
                res.IsValid = false;
                res.TimeStamp = DateTime.UtcNow;
            }
            return res;
        }

        public static MeasurePointVariable operator <(MeasurePointVariable a, MeasurePointVariable b)
        {
            MeasurePointVariable res = !(a>=b);
            return res;
        }

        public static MeasurePointVariable operator >=(MeasurePointVariable a, MeasurePointVariable b)
        {
            MeasurePointVariable res = new MeasurePointVariable();
            if (a.Type == b.Type)
            {
                switch (a.Type)
                {
                    case MPVPrimeType.Error:
                        res.Type = MPVPrimeType.Error;
                        res.Text = "Greater than or equal is not supported for the given input types";
                        res.OrginalValue = "(" + a.OrginalValue + " >= " + b.OrginalValue + ")";
                        res.IsSet = false;
                        res.IsValid = false;
                        res.TimeStamp = DateTime.UtcNow;
                        return res;
                        break;
                    case MPVPrimeType.String:
                        //TODO: Implement string comparison
                        if (a.Text.Length >= b.Text.Length)
                        {
                            res.Text = a.Text;
                        }
                        else
                        {
                            res.Text = a.Text + " < " + b.Text;
                        }
                        break;
                    case MPVPrimeType.Number:
                        if (a.Value >= b.Value)
                        {
                            res.Value = Decimal.One;
                        }
                        else
                        {
                            res.Value = Decimal.Zero;
                        }
                        res.Type = MPVPrimeType.Number;
                        break;
                    case MPVPrimeType.DateTime:
                        if (a.DateTime >= b.DateTime)
                        {
                            res.Value = Decimal.One;
                        }
                        else
                        {
                            res.Value = Decimal.Zero;
                        }
                        res.Type = MPVPrimeType.Number;
                        break;
                    case MPVPrimeType.TimeSpan:
                        if (a.TimeSpan >= b.TimeSpan)
                        {
                            res.Value = Decimal.One;
                        }
                        else
                        {
                            res.Value = Decimal.Zero;
                        }
                        res.Type = MPVPrimeType.Number;
                        break;
                    case MPVPrimeType.List:
                        if (a.ValueList.Count == b.ValueList.Count)
                        {
                            for (int i = 0; i < a.ValueList.Count; i++)
                            {
                                res.ValueList.Add(a.ValueList[i] >= b.ValueList[i] ? Decimal.One : Decimal.Zero);
                            }
                        }
                        else
                        {
                            //TODO: find logic for different length lists
                        }
                        res.Type = MPVPrimeType.List;
                        break;
                    case MPVPrimeType.StringMap:
                        if (a.StringMap.Count == b.StringMap.Count)
                        {
                            foreach (var item in a.StringMap)
                            {
                                if (b.StringMap.ContainsKey(item.Key))
                                {
                                    if (b.StringMap[item.Key] >= item.Value)
                                    {
                                        res.StringMap.Add(item.Key, Decimal.One);
                                    }
                                    else
                                    {
                                        res.StringMap.Add(item.Key, Decimal.Zero);
                                    }
                                }
                                else
                                {
                                    res.StringMap.Add(item.Key, Decimal.Zero);
                                }
                            }
                        }
                        else
                        {
                            //TODO: find logic for different length lists
                        }
                        res.Type = MPVPrimeType.StringMap;
                        break;
                    case MPVPrimeType.NumberMap:
                        if (a.NumberMap.Count == b.NumberMap.Count)
                        {
                            foreach (var item in a.NumberMap)
                            {
                                if (b.NumberMap.ContainsKey(item.Key))
                                {
                                    if (b.NumberMap[item.Key] >= item.Value)
                                    {
                                        res.NumberMap.Add(item.Key, Decimal.One);
                                    }
                                    else
                                    {
                                        res.NumberMap.Add(item.Key, Decimal.Zero);
                                    }
                                }
                                else
                                {
                                    res.NumberMap.Add(item.Key, Decimal.Zero);
                                }
                            }
                        }
                        else
                        {
                            //TODO: find logic for different length lists
                        }
                        res.Type = MPVPrimeType.NumberMap;
                        break;
                    case MPVPrimeType.Empty:
                        res.Type = MPVPrimeType.Empty;
                        break;
                    case MPVPrimeType.Other:
                        res.Type = MPVPrimeType.Other;
                        break;
                    default:
                        res.Type = MPVPrimeType.Error;
                        res.Text = "Greater than or equal is not supported for the given input types, not implemented yet";
                        res.OrginalValue = "(" + a.OrginalValue + " >= " + b.OrginalValue + ")";
                        res.IsSet = false;
                        res.IsValid = false;
                        res.TimeStamp = DateTime.UtcNow;
                        return res;
                        break;
                }
                res.OrginalValue = "(" + a.OrginalValue + " >= " + b.OrginalValue + ")";
                res.IsSet = a.IsSet && b.IsSet;
                res.IsValid = a.IsValid && b.IsValid;
                res.TimeStamp = a.TimeStamp > b.TimeStamp ? a.TimeStamp : b.TimeStamp;
            }
            else
            {
                res.Type = MPVPrimeType.Error;
                res.Text = "Greater than or equal is not supported for the given input types";
                res.OrginalValue = "(" + a.OrginalValue + " >= " + b.OrginalValue + ")";
                res.IsSet = false;
                res.IsValid = false;
                res.TimeStamp = DateTime.UtcNow;
            }
            return res;
        }

        public static MeasurePointVariable operator <=(MeasurePointVariable a, MeasurePointVariable b)
        {
            MeasurePointVariable res = !(a > b);
            return res;
        }
        
        public static MeasurePointVariable Map(MeasurePointVariable listInput, MeasurePointVariable start, MeasurePointVariable end)
        {
            MeasurePointVariable result = new MeasurePointVariable();
            result.IsSet = listInput.IsSet && start.IsSet && end.IsSet;
            if (listInput.Type == MPVPrimeType.NumberMap && start.Type == MPVPrimeType.Number &&
                end.Type == MPVPrimeType.Number)
            {
                int numbercount = listInput.ValueList.Count;
                decimal delta = (end.Value - start.Value) / numbercount;
                for(int runner = 0; runner < numbercount; runner++)
                {
                    result.NumberMap.Add(start.Value + delta * runner, listInput.ValueList[runner]);
                }
                result.Type = MPVPrimeType.NumberMap;
            }
            else
            {
                result.Type = MPVPrimeType.Error;
                result.Text = "Map function is not supported for the given input types";
                result.IsSet = false;
                result.IsValid = false;
                result.TimeStamp = DateTime.UtcNow;
                return result;
            }

            result.IsValid = listInput.IsValid && start.IsValid && end.IsValid;
            result.TimeStamp = listInput.TimeStamp > start.TimeStamp ? listInput.TimeStamp : start.TimeStamp;
            return result;
        }

        public static MeasurePointVariable Map(MeasurePointVariable listValues, MeasurePointVariable listMappings)
        {
            MeasurePointVariable result = new MeasurePointVariable();
            result.IsSet = listValues.IsSet && listMappings.IsSet;
            if (listValues.Type == MPVPrimeType.List && listMappings.Type == MPVPrimeType.List)
            {
                int count = listValues.ValueList.Count <= listMappings.ValueList.Count ? listMappings.ValueList.Count : listValues.ValueList.Count;
                for (int runner = 0; runner < count; runner++)
                {
                    result.NumberMap.Add(listMappings.ValueList[runner], listValues.ValueList[runner]);
                }
                result.Type = MPVPrimeType.NumberMap;
            }
            else
            {
                result.Type = MPVPrimeType.Error;
                result.Text = "Map function is not supported for the given input types";
                result.IsSet = false;
                result.IsValid = false;
                result.TimeStamp = DateTime.UtcNow;
                return result;
            }

            result.IsValid = listValues.IsValid && listMappings.IsValid;
            result.TimeStamp = listValues.TimeStamp > listMappings.TimeStamp ? listValues.TimeStamp : listMappings.TimeStamp;
            return result;
        }


        public static MeasurePointVariable PointAccess(MeasurePointVariable target, MeasurePointVariable index)
        {
            switch (target.Type)
            {
                case MPVPrimeType.Error:
                    return new MeasurePointVariable(MPVPrimeType.Error);
                case MPVPrimeType.String:
                    switch (index.Type)
                    {
                        case MPVPrimeType.Error:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        case MPVPrimeType.String:
                            return new MeasurePointVariable(target.Text[target.Text.Length < index.Text.Length ? 0 : index.Text.Length].ToString());
                        case MPVPrimeType.Number:
                            return new MeasurePointVariable(target.Text[target.Text.Length < index.Value ? 0 : (int)index.Value].ToString());
                        case MPVPrimeType.DateTime:
                            return new MeasurePointVariable(target.Text[target.Text.Length < index.DateTime.Ticks ? 0 : (int)index.DateTime.Ticks].ToString());
                        case MPVPrimeType.TimeSpan:
                            return new MeasurePointVariable(target.Text[target.Text.Length < index.TimeSpan.Ticks ? 0 : (int)index.TimeSpan.Ticks].ToString());
                        case MPVPrimeType.StringMap:
                            return new MeasurePointVariable(target.Text[target.Text.Length < index.StringMap.Count? 0 : index.StringMap.Count].ToString());
                        case MPVPrimeType.NumberMap:
                            return new MeasurePointVariable(target.Text[target.Text.Length < index.NumberMap.Count ? 0 : index.NumberMap.Count].ToString());
                        case MPVPrimeType.List:
                            return new MeasurePointVariable(target.Text[target.Text.Length < index.ValueList.Count ? 0 : index.ValueList.Count].ToString());
                        case MPVPrimeType.Empty:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        case MPVPrimeType.Other:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        default:
                            return new MeasurePointVariable(Decimal.Zero);
                    }
                case MPVPrimeType.Number:
                    string s = target.Value.ToString();
                    switch (index.Type)
                    {
                        case MPVPrimeType.Error:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        case MPVPrimeType.String:
                            return new MeasurePointVariable(s[s.Length < index.Text.Length ? 0 : index.Text.Length].ToString());
                        case MPVPrimeType.Number:
                            return new MeasurePointVariable(s[s.Length < index.Value ? 0 : (int)index.Value].ToString());
                        case MPVPrimeType.DateTime:
                            return new MeasurePointVariable(s[s.Length < index.DateTime.Ticks ? 0 : (int)index.DateTime.Ticks].ToString());
                        case MPVPrimeType.TimeSpan:
                            return new MeasurePointVariable(s[s.Length < index.TimeSpan.Ticks ? 0 : (int)index.TimeSpan.Ticks].ToString());
                        case MPVPrimeType.StringMap:
                            return new MeasurePointVariable(s[s.Length < index.StringMap.Count ? 0 : index.StringMap.Count].ToString());
                        case MPVPrimeType.NumberMap:
                            return new MeasurePointVariable(s[s.Length < index.NumberMap.Count ? 0 : index.NumberMap.Count].ToString());
                        case MPVPrimeType.List:
                            return new MeasurePointVariable(s[s.Length < index.ValueList.Count ? 0 : index.ValueList.Count].ToString());
                        case MPVPrimeType.Empty:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        case MPVPrimeType.Other:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        default:
                            return new MeasurePointVariable(Decimal.Zero);
                    }
                case MPVPrimeType.DateTime:
                    s = target.DateTime.Ticks.ToString();
                    switch (index.Type)
                    {
                        case MPVPrimeType.Error:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        case MPVPrimeType.String:
                            return new MeasurePointVariable(s[s.Length < index.Text.Length ? 0 : index.Text.Length].ToString());
                        case MPVPrimeType.Number:
                            return new MeasurePointVariable(s[s.Length < index.Value ? 0 : (int)index.Value].ToString());
                        case MPVPrimeType.DateTime:
                            return new MeasurePointVariable(s[s.Length < index.DateTime.Ticks ? 0 : (int)index.DateTime.Ticks].ToString());
                        case MPVPrimeType.TimeSpan:
                            return new MeasurePointVariable(s[s.Length < index.TimeSpan.Ticks ? 0 : (int)index.TimeSpan.Ticks].ToString());
                        case MPVPrimeType.StringMap:
                            return new MeasurePointVariable(s[s.Length < index.StringMap.Count ? 0 : index.StringMap.Count].ToString());
                        case MPVPrimeType.NumberMap:
                            return new MeasurePointVariable(s[s.Length < index.NumberMap.Count ? 0 : index.NumberMap.Count].ToString());
                        case MPVPrimeType.List:
                            return new MeasurePointVariable(s[s.Length < index.ValueList.Count ? 0 : index.ValueList.Count].ToString());
                        case MPVPrimeType.Empty:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        case MPVPrimeType.Other:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        default:
                            return new MeasurePointVariable(Decimal.Zero);
                    }
                case MPVPrimeType.TimeSpan:
                    s = target.TimeSpan.Ticks.ToString();
                    switch (index.Type)
                    {
                        case MPVPrimeType.Error:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        case MPVPrimeType.String:
                            return new MeasurePointVariable(s[s.Length < index.Text.Length ? 0 : index.Text.Length].ToString());
                        case MPVPrimeType.Number:
                            return new MeasurePointVariable(s[s.Length < index.Value ? 0 : (int)index.Value].ToString());
                        case MPVPrimeType.DateTime:
                            return new MeasurePointVariable(s[s.Length < index.DateTime.Ticks ? 0 : (int)index.DateTime.Ticks].ToString());
                        case MPVPrimeType.TimeSpan:
                            return new MeasurePointVariable(s[s.Length < index.TimeSpan.Ticks ? 0 : (int)index.TimeSpan.Ticks].ToString());
                        case MPVPrimeType.StringMap:
                            return new MeasurePointVariable(s[s.Length < index.StringMap.Count ? 0 : index.StringMap.Count].ToString());
                        case MPVPrimeType.NumberMap:
                            return new MeasurePointVariable(s[s.Length < index.NumberMap.Count ? 0 : index.NumberMap.Count].ToString());
                        case MPVPrimeType.List:
                            return new MeasurePointVariable(s[s.Length < index.ValueList.Count ? 0 : index.ValueList.Count].ToString());
                        case MPVPrimeType.Empty:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        case MPVPrimeType.Other:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        default:
                            return new MeasurePointVariable(Decimal.Zero);
                    }
                case MPVPrimeType.List:
                    switch (index.Type)
                    {
                        case MPVPrimeType.Error:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        case MPVPrimeType.String:
                            return new MeasurePointVariable(target.ValueList[target.ValueList.Count < index.Text.Length ? 0 : index.Text.Length]);
                        case MPVPrimeType.Number:
                            return new MeasurePointVariable(target.ValueList[target.ValueList.Count < index.Value ? 0 : (int)index.Value]);
                        case MPVPrimeType.DateTime:
                            return new MeasurePointVariable(target.ValueList[target.ValueList.Count < index.DateTime.Ticks ? 0 : (int)index.DateTime.Ticks]);
                        case MPVPrimeType.TimeSpan:
                            return new MeasurePointVariable(target.ValueList[target.ValueList.Count < index.TimeSpan.Ticks ? 0 : (int)index.TimeSpan.Ticks]);
                        case MPVPrimeType.StringMap:
                            return new MeasurePointVariable(target.ValueList[target.ValueList.Count < index.StringMap.Count ? 0 : index.StringMap.Count]);
                        case MPVPrimeType.NumberMap:
                            return new MeasurePointVariable(target.ValueList[target.ValueList.Count < index.NumberMap.Count ? 0 : index.NumberMap.Count]);
                        case MPVPrimeType.List:
                            return new MeasurePointVariable(target.ValueList[target.ValueList.Count < index.ValueList.Count ? 0 : index.ValueList.Count]);
                        case MPVPrimeType.Empty:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        case MPVPrimeType.Other:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        default:
                            return new MeasurePointVariable(Decimal.Zero);
                    }
                case MPVPrimeType.StringMap:
                    switch (index.Type)
                    {
                        case MPVPrimeType.Error:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        case MPVPrimeType.String:
                            return new MeasurePointVariable(target.StringMap.ContainsKey(index.Text) ? target.StringMap[index.Text] : Decimal.Zero);
                        case MPVPrimeType.Number:
                            return new MeasurePointVariable(target.StringMap.ContainsKey(index.Value.ToString()) ? target.StringMap[index.Value.ToString()] : Decimal.Zero);
                        case MPVPrimeType.DateTime:
                            return new MeasurePointVariable(target.StringMap.ContainsKey(index.DateTime.Ticks.ToString()) ? target.StringMap[index.DateTime.Ticks.ToString()] : Decimal.Zero);
                        case MPVPrimeType.TimeSpan:
                            return new MeasurePointVariable(target.StringMap.ContainsKey(index.TimeSpan.Ticks.ToString()) ? target.StringMap[index.TimeSpan.Ticks.ToString()] : Decimal.Zero);
                        case MPVPrimeType.StringMap:
                            return new MeasurePointVariable(target.StringMap.ContainsKey(index.StringMap.Count.ToString()) ? target.StringMap[index.StringMap.Count.ToString()] : Decimal.Zero);
                        case MPVPrimeType.NumberMap:
                            return new MeasurePointVariable(target.StringMap.ContainsKey(index.NumberMap.Count.ToString()) ? target.StringMap[index.NumberMap.Count.ToString()] : Decimal.Zero);
                        case MPVPrimeType.List:
                            return new MeasurePointVariable(target.StringMap.ContainsKey(index.ValueList.Count.ToString()) ? target.StringMap[index.ValueList.Count.ToString()] : Decimal.Zero);
                        case MPVPrimeType.Empty:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        case MPVPrimeType.Other:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        default:
                            return new MeasurePointVariable(Decimal.Zero);
                    }
                case MPVPrimeType.NumberMap:
                    switch (index.Type)
                    {
                        case MPVPrimeType.Error:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        case MPVPrimeType.String:
                            return new MeasurePointVariable(target.NumberMap.ContainsKey(index.Text.Length)
                                ? target.NumberMap[(decimal)index.Text.Length]
                                : target.NumberMap[target.NumberMap.Keys.Min()]);
                        case MPVPrimeType.Number:
                            return new MeasurePointVariable(target.NumberMap.ContainsKey(index.Value)
                                ? target.NumberMap[index.Value]
                                : target.NumberMap[target.NumberMap.Keys.Min()]);
                        case MPVPrimeType.DateTime:
                            return new MeasurePointVariable(target.NumberMap.ContainsKey(index.DateTime.Ticks)
                                ? target.NumberMap[(decimal)index.DateTime.Ticks]
                                : target.NumberMap[target.NumberMap.Keys.Min()]);
                        case MPVPrimeType.TimeSpan:
                            return new MeasurePointVariable(target.NumberMap.ContainsKey(index.TimeSpan.Ticks)
                                ? target.NumberMap[(decimal)index.TimeSpan.Ticks]
                                : target.NumberMap[target.NumberMap.Keys.Min()]);
                        case MPVPrimeType.StringMap:
                            return new MeasurePointVariable(target.NumberMap.ContainsKey(index.StringMap.Count)
                                ? target.NumberMap[(decimal)index.StringMap.Count]
                                : target.NumberMap[target.NumberMap.Keys.Min()]);
                        case MPVPrimeType.NumberMap:
                            return new MeasurePointVariable(target.NumberMap.ContainsKey(index.NumberMap.Count)
                                ? target.NumberMap[(decimal)index.NumberMap.Count]
                                : target.NumberMap[target.NumberMap.Keys.Min()]);
                        case MPVPrimeType.List:
                            return new MeasurePointVariable(target.NumberMap.ContainsKey(index.ValueList.Count)
                                ? target.NumberMap[(decimal)index.ValueList.Count]
                                : target.NumberMap[target.NumberMap.Keys.Min()]);
                        case MPVPrimeType.Empty:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        case MPVPrimeType.Other:
                            return new MeasurePointVariable(MPVPrimeType.Error);
                        default:
                            return new MeasurePointVariable(Decimal.Zero);
                    }
                case MPVPrimeType.Empty:
                    return new MeasurePointVariable(MPVPrimeType.Error);
                case MPVPrimeType.Other:
                    return new MeasurePointVariable(MPVPrimeType.Error);

            }
            return new MeasurePointVariable(MPVPrimeType.Error);
        }

        public static void PointWrite(MeasurePointVariable target, MeasurePointVariable index,
            MeasurePointVariable value)
        {
            if (value.Type != decimal)
            {
                return;
            }

            switch (target.Type)
            {
                case MPVPrimeType.List:
                    switch (index.Type)
                    {
                        case MPVPrimeType.Error:
                        case MPVPrimeType.Empty:
                        case MPVPrimeType.Other:
                        case MPVPrimeType.DateTime:
                        case MPVPrimeType.TimeSpan:
                        case MPVPrimeType.NumberMap:
                        case MPVPrimeType.StringMap:
                            return;
                        case MPVPrimeType.String:
                            target.ValueList[index.Text.Length] = value.Value;
                            return;
                        case MPVPrimeType.Number:
                            target.ValueList[(int)index.Value] = value.Value;
                            return;
                        default:
                            return;

                    }
                case MPVPrimeType.StringMap:
                    switch (index.Type)
                    {
                        case MPVPrimeType.Error:
                        case MPVPrimeType.Empty:
                        case MPVPrimeType.Other:
                        case MPVPrimeType.DateTime:
                        case MPVPrimeType.TimeSpan:
                        case MPVPrimeType.NumberMap:
                        case MPVPrimeType.StringMap:
                            return;
                        case MPVPrimeType.String:
                            target.StringMap[index.Text] = value.Value;
                            return;
                        case MPVPrimeType.Number:
                            target.StringMap[index.Value.ToString()] = value.Value;
                            return;
                        default:
                            return;
                    }
                case MPVPrimeType.NumberMap:
                    switch (index.Type)
                    {
                        case MPVPrimeType.Error:
                        case MPVPrimeType.Empty:
                        case MPVPrimeType.Other:
                        case MPVPrimeType.DateTime:
                        case MPVPrimeType.TimeSpan:
                        case MPVPrimeType.NumberMap:
                        case MPVPrimeType.String:
                        case MPVPrimeType.StringMap:
                            return;
                        case MPVPrimeType.Number:
                            target.NumberMap[index.Value] = value.Value;
                            return;
                        default:
                            return;
                    }
                case MPVPrimeType.String:
                case MPVPrimeType.DateTime:
                case MPVPrimeType.Number:
                case MPVPrimeType.TimeSpan:
                case MPVPrimeType.Error:
                case MPVPrimeType.Other:
                case MPVPrimeType.Empty:
                default:
                    target.Type = MPVPrimeType.List;
                    target.ValueList = new List<decimal>();
                    PointWrite(target, index, value);
                    return;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is MeasurePointVariable other)
            {
                MeasurePointVariable res = this == other;
                return res.Value == Decimal.One;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Type, Text, Value, ValueList, HashCode.Combine(StringMap, NumberMap, IsSet, IsValid, TimeStamp));
        }

    }
}

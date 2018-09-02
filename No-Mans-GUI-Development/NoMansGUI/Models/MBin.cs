using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NoMansGUI.Models
{
    public class MBin
    {
        public string Filepath { get; set; }
        public string Name { get; set; }
        public string VanillaFilePath { get; set; }     // For comparison purposes.  Might not ever use this and end up removing it.
        public string vanillaName { get; set; }         // Used when creating new files that are based on vanilla (custom models, etc)
    }

    public class MBINField : PropertyChangedBase
    {
        //Boxed Struct - The actual NMSTemplate
        public object dataOwner;
        public FieldInfo fieldInfo;

        public string Name { get; set; }
        public Array EnumValues { get; set; }
        public virtual object Value
        {
            get
            {
                return fieldInfo.GetValue(dataOwner);
            }
            set
            {
                if(fieldInfo == null || dataOwner == null)
                {
                    return;
                }
                //fieldInfo.SetValue(dataOwner, Convert.ChangeType(value, fieldInfo.FieldType));
                NotifyOfPropertyChange(() => Value);
            }
        }
        public string TemplateType { get; set; }
        public Type NMSType { get; set; }          
    }

    /// <summary>
    /// Struct don't get set (for now)
    /// </summary>
    public class MBINStructField : MBINField
    {
        private object _value;

        //Boxed Struct - The actual NMSTemplate
        public override object Value
        {
            get { return _value; }
            set
            {
                _value = value;
            }
        }
    }

    public class MBINArrayElementField : MBINField, IConvertible
    {
        private object _value;

        public MBINField Parent;
        public int Index;

        public override object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                var array = Parent.Value as Array;
                array.SetValue(_value, Index);
            }
        }
        #region Convetable Interface
        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return (bool)Value;
        }

        public char ToChar(IFormatProvider provider)
        {
            return (char)Value;
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return (SByte)Value;
        }

        public byte ToByte(IFormatProvider provider)
        {
            return (Byte)Value;
        }

        public short ToInt16(IFormatProvider provider)
        {
            return (Int16)Value;
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return (UInt16)Value;
        }

        public int ToInt32(IFormatProvider provider)
        {
           return (Int32)Value;
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return (UInt32)Value;
        }

        public long ToInt64(IFormatProvider provider)
        {
            return (Int64)Value;
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return (UInt64)Value;
        }

        public float ToSingle(IFormatProvider provider)
        {
            return (Single)Value;
        }

        public double ToDouble(IFormatProvider provider)
        {
            return (Double)Value;
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return (Decimal)Value;
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return (DateTime)Value;
        }

        public string ToString(IFormatProvider provider)
        {
            return (String)Value;
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(Value, conversionType);
        }
        #endregion
    }

}

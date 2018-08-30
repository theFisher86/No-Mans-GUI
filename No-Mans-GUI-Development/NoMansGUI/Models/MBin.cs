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
                fieldInfo.SetValue(dataOwner, Convert.ChangeType(value, fieldInfo.FieldType));
                NotifyOfPropertyChange(() => Value);
            }
        }
        public string TemplateType { get; set; }
        public Type NMSType { get; set; }          
    }

    /// <summary>
    /// Struct don't get set (for now) so we
    /// </summary>
    public class MBINStructField : MBINField
    {
        //Boxed Struct - The actual NMSTemplate
        public override object Value
        {
            get;
            set;
        }
    }

    public class DataBoundField
    {
        private FieldInfo _fieldInfo;

        public Type FieldType;
        public MBINStruct Parent;

        public string FieldName { get; set; }
        public object Value
        {
            get { return Parent.GetValue(_fieldInfo); }
            set
            {
                Parent.SetValue(value, _fieldInfo);
            }
        }
        public Type Type
        {
            get { return FieldType; }
        }

        public DataBoundField(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
            this.FieldName = fieldInfo.Name;
            this.FieldType = fieldInfo.FieldType;
        }
    }

    public class MBINStruct : PropertyChangedBase
    {
        #region Fields    
        private ObservableCollection<DataBoundField> _fields;
        #endregion

        #region Properties
        public Object DataObject
        {
            get;
            set;
        }

        public ObservableCollection<DataBoundField> Fields
        {
            get { return _fields; }
            set
            {
                _fields = value;
                NotifyOfPropertyChange(() => Fields);
            }
        }

        internal object GetValue(FieldInfo info)
        {
            return info.GetValue(DataObject);
        }

        internal void SetValue(object value, FieldInfo info)
        {
            info.SetValue(DataObject, value);
        }
        #endregion

        #region Constructor
        public MBINStruct(object dataObject, IOrderedEnumerable<FieldInfo> fields)
        {
            this.DataObject = dataObject;
            CreateFields(fields);
        }
        #endregion

        #region Methods
        private void CreateFields(IOrderedEnumerable<FieldInfo> fields)
        {
            Fields = new ObservableCollection<DataBoundField>();
            foreach(FieldInfo fieldInfo in fields)
            {
                Fields.Add(new DataBoundField(fieldInfo));
            }
        }
        #endregion
    }

    public class MBINField<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }
        public string NMSType { get; set; }
    }
}

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

    public class MBINArrayField : MBINField
    {
        private ObservableCollection<MBINArrayElementField> _value;

        public override object Value
        {
            get
            {
                return _value;
            }
            set
            {
                
                if (value is List<MBINArrayElementField>)
                {
                    _value = new ObservableCollection<MBINArrayElementField>(value as List<MBINArrayElementField>);
                    BindUp();
                }
            }
        }

        private void BindUp()
        {
            foreach(MBINArrayElementField field in _value)
            {
                field.PropertyChanged += Field_PropertyChanged;
            }
        }

        private void Field_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdatOrigin();

        }

        void UpdatOrigin()
        {
            //Get Origin Type.
            Type elementType = fieldInfo.FieldType.GetElementType();
            //Convert the list into origin type array.
            var origin = Array.CreateInstance(elementType, _value.Count);

            for(int i = 0; i < _value.Count; i++)
            {
                origin.SetValue(Convert.ChangeType(_value[i].Value, elementType), _value[i].Index);
            }

            //Save back to origin
            fieldInfo.SetValue(dataOwner, origin);
        }
    }

    public class MBINArrayElementField : MBINField
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
                NotifyOfPropertyChange(() => Value);
            }
        }
    }

}

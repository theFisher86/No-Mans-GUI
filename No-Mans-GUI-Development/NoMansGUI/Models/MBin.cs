using Caliburn.Micro;
using libMBIN.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
        private ObservableCollection<MBINField> _value;

        //Boxed Struct - The actual NMSTemplate
        public override object Value
        {
            get { return _value; }
            set
            {
                if (value is List<MBINField>)
                {
                    _value = new ObservableCollection<MBINField>(value as List<MBINField>);
                    BindUp();
                    NotifyOfPropertyChange(() => Value);
                }
            }
        }

        private void BindUp()
        {
            foreach (MBINField field in _value)
            {
                field.PropertyChanged += Field_PropertyChanged;
            }
        }

        private void Field_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateOrigin();
        }

        void UpdateOrigin()
        {
            NotifyOfPropertyChange(() => Value);
            // fieldInfo.SetValue(dataOwner, _value);
        }
    }

    public class MBINArrayField : MBINField
    {
        private ObservableCollection<MBINArrayElementField> _value;

        public override object Value
        {
            get  { return _value; }
            set
            {
                
                if (value is List<MBINArrayElementField>)
                {
                    _value = new ObservableCollection<MBINArrayElementField>(value as List<MBINArrayElementField>);
                    BindUp();
                    NotifyOfPropertyChange(() => Value);
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
            UpdateOrigin();

        }

        void UpdateOrigin()
        {
            //Get Origin Type.
            Type elementType = fieldInfo.FieldType.GetElementType();
            //Convert the list into origin type array.
            var origin = Array.CreateInstance(elementType, _value.Count);

            for(int i = 0; i < _value.Count; i++)
            {
                if (elementType.BaseType == typeof(NMSTemplate))
                {
                    //No error no save. 
                    origin.SetValue(_value[i].dataOwner, _value[i].Index);
                }
                else
                {
                    origin.SetValue(Convert.ChangeType(_value[i].Value, elementType), _value[i].Index);
                }
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

    //This is an element of an array, that represents a struct.
    public class MBINArrayStructElementField : MBINArrayElementField
    {
        private ObservableCollection<MBINField> _value;

        //Boxed Struct - The actual NMSTemplate
        public override object Value
        {
            get { return _value; }
            set
            {
                if (value is List<MBINField>)
                {
                    _value = new ObservableCollection<MBINField>(value as List<MBINField>);
                    BindUp();
                    NotifyOfPropertyChange(() => Value);
                }
            }
        }

        private void BindUp()
        {
            foreach (MBINField field in _value)
            {
                field.PropertyChanged += Field_PropertyChanged;
            }
        }

        private void Field_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateOrigin();
        }

        void UpdateOrigin()
        {
           NotifyOfPropertyChange(() => Value);
        }
    }

    public class MBINListStructElementField : MBINListElementField
    {
        private ObservableCollection<MBINField> _value;

        //Boxed Struct - The actual NMSTemplate
        public override object Value
        {
            get { return _value; }
            set
            {
                if (value is List<MBINField>)
                {
                    _value = new ObservableCollection<MBINField>(value as List<MBINField>);
                    BindUp();
                    NotifyOfPropertyChange(() => Value);
                }
            }
        }

        private void BindUp()
        {
            foreach (MBINField field in _value)
            {
                field.PropertyChanged += Field_PropertyChanged;
            }
        }

        private void Field_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateOrigin();
        }

        void UpdateOrigin()
        {
            NotifyOfPropertyChange(() => Value);
        }
    }

    public class MBINListElementField : MBINField
    {
        private object _value;

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

    public class MBINListField : MBINField
    {
        private ObservableCollection<MBINListElementField> _value;

        public override object Value
        {
            get { return _value; }
            set
            {
                if (value is List<MBINListElementField>)
                {
                    _value = new ObservableCollection<MBINListElementField>(value as List<MBINListElementField>);
                    BindUp();
                    NotifyOfPropertyChange(() => Value);
                }
            }
        }

        private void BindUp()
        {
            foreach (MBINListElementField field in _value)
            {
                field.PropertyChanged += Field_PropertyChanged;
            }
        }

        private void Field_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateOrigin();

        }

        void UpdateOrigin()
        {
            //Get Origin Type.
            Type elementType = fieldInfo.FieldType.GetGenericArguments()[0];
            //Convert the list into origin type array.
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(elementType);
            var origin = (IList)Activator.CreateInstance(constructedListType);
            //var origin =  //List.CreateInstance(elementType, _value.Count);

            for (int i = 0; i < _value.Count; i++)
            {
                if (elementType.BaseType == typeof(NMSTemplate))
                {
                    //No error no save. 
                    origin.Add(_value[i].dataOwner);
                }
                else
                {
                    origin.Add(Convert.ChangeType(_value[i].Value, elementType));
                }
            }

            //Save back to origin
            fieldInfo.SetValue(dataOwner, origin);
        }
    }
}

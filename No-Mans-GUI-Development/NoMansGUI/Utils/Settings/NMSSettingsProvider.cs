using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace NoMansGUI.Utils.Settings
{
    public class NMSSettingsProvider : SettingsProvider
    {
        const string SETTINGSROOT = "Settings";
        private XmlDocument m_SettingsXML = null;

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(this.ApplicationName, config);
        }

        public override string ApplicationName
        {
            get
            {
                if(Application.ProductName.Trim().Length > 0)
                {
                    return Application.ProductName;
                }
                else
                {
                    FileInfo fi = new FileInfo(Application.ExecutablePath);
                    return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
                }
            }
            set
            {

            }
        }

        public virtual string GetAppSettingsPath()
        {
            var fi = new FileInfo(Application.ExecutablePath);
            return fi.DirectoryName;
        }

        public virtual string GetAppSettingsFilename()
        {
            return ApplicationName + ".settings";
        }

        private XmlDocument SettingsXML
        {
            // If we dont hold an xml document, try opening one.  
            // If it doesnt exist then create a new one ready.
            get
            {
                if(m_SettingsXML == null)
                {
                    m_SettingsXML = new XmlDocument();

                    try
                    {
                        m_SettingsXML.Load(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
                    }
                    catch(Exception ex)
                    {
                        XmlDeclaration dec = m_SettingsXML.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
                        m_SettingsXML.AppendChild(dec);
                        XmlNode nodeRoot;

                        nodeRoot = m_SettingsXML.CreateNode(XmlNodeType.Element, SETTINGSROOT, "");
                        m_SettingsXML.AppendChild(nodeRoot);
                    }
                }
                return m_SettingsXML;
            }
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            foreach(SettingsPropertyValue propVal in collection)
            {
                SetValue(propVal);
            }

            try
            {
                SettingsXML.Save(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
            }
            catch(Exception ex)
            {

            }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            //Create new collection of values
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

            //Iterate through the settings to be retrieved
            foreach (SettingsProperty setting in collection)
            {
                SettingsPropertyValue value = new SettingsPropertyValue(setting);
                value.IsDirty = false;
                value.SerializedValue = GetValue(setting);
                values.Add(value);
            }
            return values;
        }

        private bool IsRoaming(SettingsProperty prop)
        {
            // Determine if the setting is marked as Roaming
            foreach (DictionaryEntry d in prop.Attributes)
            {
                Attribute a = (Attribute)d.Value;
                if (a is System.Configuration.SettingsManageabilityAttribute)
                    return true;
            }
            return false;
        }

        private void SetValue(SettingsPropertyValue propVal)
        {
            XmlElement MachineNode;
            XmlElement SettingNode;

            //Determine if the setting is roaming.
            //If roaming then the value is stored as an element under the root
            //Otherwise it is stored under a machine name node 
            try
            {
                if (IsRoaming(propVal.Property))
                {
                    SettingNode = (XmlElement)SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + propVal.Name);
                }
                else
                {
                    SettingNode = (XmlElement)SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + System.Environment.MachineName + "/" + propVal.Name);
                }
            }
            catch (Exception ex)
            {
                SettingNode = null;
            }

            //Check to see if the node exists, if so then set its new value
            if (SettingNode != null)
            {
                SettingNode.InnerText = propVal.SerializedValue.ToString();
            }
            else
            {
                if (IsRoaming(propVal.Property))
                {
                    //Store the value as an element of the Settings Root Node
                    SettingNode = SettingsXML.CreateElement(propVal.Name);
                    SettingNode.InnerText = propVal.SerializedValue.ToString();
                    SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(SettingNode);
                }
                else
                {
                    //Its machine specific, store as an element of the machine name node,
                    //creating a new machine name node if one doesnt exist.
                    try
                    {
                        MachineNode = (XmlElement)SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + System.Environment.MachineName);
                    }
                    catch (Exception ex)
                    {
                        MachineNode = SettingsXML.CreateElement(System.Environment.MachineName);
                        SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(MachineNode);
                    }

                    if (MachineNode == null)
                    {
                        MachineNode = SettingsXML.CreateElement(System.Environment.MachineName);
                        SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(MachineNode);
                    }

                    SettingNode = SettingsXML.CreateElement(propVal.Name);
                    SettingNode.InnerText = propVal.SerializedValue.ToString();
                    MachineNode.AppendChild(SettingNode);
                }
            }
        }

        private string GetValue(SettingsProperty setting)
        {
            string ret = "";

            try
            {
                if (IsRoaming(setting))
                {
                    ret = SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + setting.Name).InnerText;
                }
                else
                {
                    ret = SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + System.Environment.MachineName + "/" + setting.Name).InnerText;
                }
            }
            catch (Exception ex)
            {
                if (setting.DefaultValue != null)
                    ret = setting.DefaultValue.ToString();
                else
                    ret = "";
            }

            return ret;
        }
    }
}

using NoMansGUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NoMansGUI.Utils.AdminTools
{
    public static class NMSTemplateTypeList
    {
        private static Dictionary<string, List<string>> _fieldTypes;
        public static Dictionary<string, List<string>> GetList()
        {
            return _fieldTypes;
        }
        public static void AddToDebugList(string name, MBINField field)
        {
            if (_fieldTypes == null)
            {
                _fieldTypes = new Dictionary<string, List<string>>();
            }
            if (_fieldTypes.ContainsKey(name))
            {
                if (_fieldTypes[name].Contains(field.NMSType) == false)
                {
                    _fieldTypes[name].Add(field.NMSType);
                }
            }
            else
            {
                _fieldTypes.Add(name, new List<string>() { field.NMSType });
            }
        }
        public static void PrintToFile(string mbinName)
        {
            if (_fieldTypes == null || _fieldTypes.Count < 1)
            {
                return;
            }
            string folder = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory), "Types");
            string filename = string.Format("{0}_mbinTypes.txt", mbinName);
            using (StreamWriter file = new StreamWriter(Path.Combine(folder, filename)))
            {
                foreach (var entry in _fieldTypes)
                {
                    foreach (string field in entry.Value)
                    {
                        file.WriteLine("[{0} {1}]", entry.Key, field);
                    }
                }
            }
            _fieldTypes = new Dictionary<string, List<string>>();
        }
    }
}
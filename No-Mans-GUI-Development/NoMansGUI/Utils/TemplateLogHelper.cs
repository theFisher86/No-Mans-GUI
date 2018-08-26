using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoMansGUI.Utils
{
    public static class TemplateLogHelper
    {
        private static List<string> _missingTemplates = new List<string>();

        public static List<string> GetMissingTemplates()
        {
            return _missingTemplates;
        }

        public static void AddMissingTemplate(string templateName)
        {
            if(_missingTemplates.Contains(templateName) == false)
            {
                _missingTemplates.Add(templateName);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
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

    public class MBINField
    {
        public string Name { get; set; }
        public dynamic Value { get; set; }
        public string NMSType { get; set; }
    }

    public class MBINField<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }
        public string NMSType { get; set; }
    }
}

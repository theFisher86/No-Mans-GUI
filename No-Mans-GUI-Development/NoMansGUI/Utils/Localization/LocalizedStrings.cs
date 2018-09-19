using NoMansGUI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoMansGUI.Utils.Localization
{
    /// <summary>
    /// Localized strings.
    /// </summary>
    public class LocalizedStrings
    {
        private static readonly StringResources _resources = new StringResources();

        /// <summary>
        /// String resources.
        /// </summary>
        static public StringResources Resources
        {
            get { return _resources; }
        }

        /// <summary>
        /// Format the specified text template filling it with the specified arguments.
        /// </summary>
        /// <remarks>This is just a wrapper for <c>String.Format</c> using the culture
        /// of this object resources.</remarks>
        /// <param name="sTemplate">template</param>
        /// <param name="args">arguments</param>
        /// <returns>formatted string</returns>
        /// <exception cref="ArgumentNullException">null template</exception>
        static public string Format(string sTemplate, params object[] args)
        {
            if (sTemplate == null) throw new ArgumentNullException("sTemplate");
            return String.Format(StringResources.Culture, sTemplate, args);
        }
    }
}

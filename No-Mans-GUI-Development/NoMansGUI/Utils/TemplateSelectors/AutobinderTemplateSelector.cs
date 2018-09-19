using System.Windows;
using System.Windows.Controls;

namespace NoMansGUI.Utils.TemplateSelectors
{
    public class AutobinderTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Template { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return Template;
        }
    }
}

using NMGUIFramework.Layout;

namespace NMGUIFramework.Interfaces
{
    public interface ITool : ILayoutItem
    {
        PaneLocation PreferredLocation { get; }
        double PreferredWidth { get; }
        double PreferredHeight { get; }

        bool IsVisible { get; set; }
    }
}

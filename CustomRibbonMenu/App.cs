using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace CustomRibbonMenu;

public class App : IExternalApplication
{
    public Result OnStartup(UIControlledApplication application)
    {
        //Define custom ribbon tab name
        string tabName = "My Custom Tools";
        application.CreateRibbonTab(tabName);

        //Create ribbon panel inside the tab
        RibbonPanel panel = application.CreateRibbonPanel(tabName, "Layout helpers");

        //Get the path to this compiled .dll assembly
        string assemblyPath = Assembly.GetExecutingAssembly().Location;

        //Configure the PushButtonData
        //  - Internal name (unique ID, no spaces)
        //  - Button text
        //  - Path to the .dll
        //  - Full namespace + class name of your IExternalCommand
        PushButtonData buttonData = new(
            name: "cmdPlaceGroup",
            text: "Place Group",
            assemblyName: assemblyPath,
            className: typeof(CopyFurnitureGroup).FullName!
        );

        //Add the button to the panel
        if (panel.AddItem(buttonData) is PushButton button)
        {
            button.ToolTip = "Pick an existing model group and place a copy at a selected point.";
            button.LongDescription = "This tool filters selection to Model Groups only, then places a new instance at your chosen point.";
            Uri iconUri = new("pack://application:,,,/CustomRibbonMenu;component/Resources/copyIcon.png");
            button.LargeImage = new BitmapImage(iconUri);
        }

        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        return Result.Succeeded;
    }
}

using System.Reflection;
using Autodesk.Revit.UI;
using CustomRibbonMenu.Commands;
using CustomRibbonMenu.Utilities;

namespace CustomRibbonMenu;

public class Application : IExternalApplication
{
    public Result OnStartup(UIControlledApplication uiCtlApp)
    {
        // Variables
        var tabName = "My Custom Tools";
        var assemblyPath = Assembly.GetExecutingAssembly().Location;

        RibbonUtils.AddRibbonTab(uiCtlApp, tabName);

        var panel = RibbonUtils.AddRibbonPanelToTab(uiCtlApp, tabName, "General");

        var placeGroupButton = RibbonUtils.AddButtonToPanel(
            panel: panel,
            buttonName: "Place Group",
            className: typeof(CopyFurnitureGroup).FullName!,
            internalName: "cmdPlaceGroup",
            assemblyPath: assemblyPath,
            toolTip: "Pick an existing model group and place a copy at a selected point.",
            description: "This tool filters selection to Model Groups only, then places a new instance at your chosen point."
        );

        var checkAirflowButton = RibbonUtils.AddButtonToPanel(
            panel: panel,
            buttonName: "Check\nAirflow",
            className: typeof(AirFlowCompare).FullName!,
            internalName: "cmdCheckAirflow",
            assemblyPath: assemblyPath,
            toolTip: "Check whether the airflow of the given space is within the given tolerance",
            description: "Long Description"
        );

        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication uiCtlApp)
    {
        return Result.Succeeded;
    }
}

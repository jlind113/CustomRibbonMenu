using System.Diagnostics;
using Autodesk.Revit.UI;

namespace CustomRibbonMenu.Utilities;

internal class RibbonUtils
{
    // Method to add a tab
    public static Result AddRibbonTab(UIControlledApplication uiCtlApp, string tabName)
    {
        try
        {
            uiCtlApp.CreateRibbonTab(tabName);
            return Result.Succeeded;
        }
        catch (Exception ex) 
        {
            Debug.WriteLine($"ERROR: Could not create tab {tabName}\n" + ex);
            return Result.Failed;
        }
    }

    // Method to add a panel to a tab
    public static RibbonPanel? AddRibbonPanelToTab(UIControlledApplication uiCtlApp, string tabName, string panelName)
    {
        try
        {
            uiCtlApp.CreateRibbonPanel(tabName, panelName);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR: Could not add {panelName} to {tabName}\n" + ex);
            return null;
        }

        return GetRibbonPanelByName(uiCtlApp, tabName, panelName);
    }

    // Method to get a panel
    public static RibbonPanel? GetRibbonPanelByName(UIControlledApplication uiCtlApp, string tabName, string panelName)
    {
        var panels = uiCtlApp.GetRibbonPanels(tabName);

        foreach (var panel in panels)
        {
            if (panel.Name == panelName)
            {
                return panel;
            }
        }
        return null;
    }

    // Method to add a push button to the panel
    public static PushButton? AddButtonToPanel(RibbonPanel panel, string buttonName, string className, string internalName, string assemblyPath, string toolTip, string description)
    {
        if (panel is null)
        {
            Debug.WriteLine($"ERROR: Could not add {buttonName} to {panel}");
            return null;
        }

        var pushButtonData = new PushButtonData(internalName, buttonName, assemblyPath, className);

        if (panel.AddItem(pushButtonData) is PushButton pushButton)
        {
            pushButton.ToolTip = toolTip;
            pushButton.LongDescription = description;
            //pushButton.Image = null;
            //pushButton.LargeImage = null;

            return pushButton;
        }
        else
        {
            Debug.WriteLine($"ERROR: Could not create {buttonName}");
            return null;
        }
    }
}

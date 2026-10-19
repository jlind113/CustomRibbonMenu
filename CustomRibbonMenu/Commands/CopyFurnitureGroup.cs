using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace CustomRibbonMenu.Commands;

[Transaction(TransactionMode.Manual)]
public class CopyFurnitureGroup : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        try
        {
            //Get application and document objects
            UIApplication uiapp = commandData.Application;
            Document document = uiapp.ActiveUIDocument.Document;

            //Define a reference Object to accept the pick result
            Reference pickedref;

            //Pick a group
            Selection selection = uiapp.ActiveUIDocument.Selection;
            GroupPickFilter selectionFilter = new();
            pickedref = selection.PickObject(ObjectType.Element, selectionFilter, "Please select a group");

            Element element = document.GetElement(pickedref);
            Group? group = element as Group;

            //Pick point
            XYZ point = selection.PickPoint("Please pick a point to place group");

            //Place the group
            Transaction transaction = new(document);
            transaction.Start("Lab");
            document.Create.PlaceGroup(point, group?.GroupType);
            transaction.Commit();
        }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
        {
            return Result.Cancelled;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return Result.Failed;
        }

        return Result.Succeeded;
    }
}

public class GroupPickFilter : ISelectionFilter
{
    public bool AllowElement(Element element)
    {
        if (element.Category?.Id?.Value == (int)BuiltInCategory.OST_IOSModelGroups)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool AllowReference(Reference reference, XYZ position)
    {
        return false;
    }
}
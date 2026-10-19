using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using CustomRibbonMenu.Forms;

namespace CustomRibbonMenu.Commands;

[Transaction(TransactionMode.Manual)]
public class AirFlowCompare : IExternalCommand
{
    private Space? targetSpace;
    private Phase? phase;

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        Document document = commandData.Application.ActiveUIDocument.Document;

        // 1. Show the WPF Input Dialog
        SpaceInputWindow inputWindow = new SpaceInputWindow();

        bool? result = inputWindow.ShowDialog();

        if (result != true)
        {
            return Result.Cancelled;
        }

        // Assign the user inputs to variables
        string targetSpaceNumber = inputWindow.SpaceNumber.ToUpper();
        double specifiedFlowCFM = inputWindow.SpecifiedFlow;
        double toleranceCFM = inputWindow.Tolerance;

        // Get the target space
        targetSpace = new FilteredElementCollector(document)
            .OfCategory(BuiltInCategory.OST_MEPSpaces)
            .OfClass(typeof(SpatialElement))
            .OfType<Space>()
            .FirstOrDefault(s => s.Number == targetSpaceNumber);

        if (targetSpace == null)
        {
            TaskDialog.Show("Error", $"Could not find Space number '{targetSpaceNumber}' in the project.");
            return Result.Failed;
        }

        Parameter phaseParam = targetSpace.get_Parameter(BuiltInParameter.ROOM_PHASE);

        if (phaseParam != null && phaseParam.HasValue)
        {
            phase = document.GetElement(phaseParam.AsElementId()) as Phase;
        }

        if (phase == null)
        {
            TaskDialog.Show("Error", "Could not determine the phase of the target space.");
            return Result.Failed;
        }

        // Get the air terminals within the specified space
        var terminalsInSpace = GetTerminalsInSpace(document, targetSpace, phase);

        // Calculate the total flow from the air terminals
        var totalActualFlowCFM = CalculateTotalFlow(terminalsInSpace);

        // Check and build results
        var resultBuilder = BuildResult(specifiedFlowCFM, toleranceCFM, targetSpace, terminalsInSpace, totalActualFlowCFM);

        // Display the final result
        TaskDialog.Show("Airflow Analysis Result", resultBuilder.ToString());

        return Result.Succeeded;
    }

    private static List<FamilyInstance> GetTerminalsInSpace(Document document, Space targetSpace, Phase spacePhase)
    {
        return new FilteredElementCollector(document)
            .OfCategory(BuiltInCategory.OST_DuctTerminal)
            .OfClass(typeof(FamilyInstance))
            .Cast<FamilyInstance>()
            .Where(terminal =>
            {
                Space terminalSpace = terminal.get_Space(spacePhase);
                return terminalSpace != null && terminalSpace.Id == targetSpace.Id;
            })
            .ToList();
    }

    private static double CalculateTotalFlow(List<FamilyInstance> terminalsInSpace)
    {
        double totalActualFlowCFM = 0.0;

        foreach (FamilyInstance terminal in terminalsInSpace)
        {
            Parameter flowParam = terminal.get_Parameter(BuiltInParameter.RBS_DUCT_FLOW_PARAM);

            if (flowParam != null && flowParam.HasValue)
            {
                // Convert CFS to CFM
                totalActualFlowCFM += (flowParam.AsDouble() * 60.0);
            }
        }

        return totalActualFlowCFM;
    }

    private static StringBuilder BuildResult(double specifiedFlowCFM, double toleranceCFM, Space targetSpace, List<FamilyInstance> terminalsInSpace, double totalActualFlowCFM)
    {
        double flowDifference = Math.Abs(totalActualFlowCFM - specifiedFlowCFM);

        bool isWithinTolerance = flowDifference <= toleranceCFM;

        string status = isWithinTolerance
            ? "PASS"
            : $"FAIL (Off by {Math.Round(flowDifference, 1)})";

        StringBuilder result = new();
        result.AppendLine($"Space {targetSpace.Number} - {targetSpace.Name}\n");
        result.AppendLine($"Specified Flow: {specifiedFlowCFM} CFM");
        result.AppendLine($"Actual Total Flow: {Math.Round(totalActualFlowCFM, 1)} CFM");
        result.AppendLine($"Tolerance: ±{toleranceCFM} CFM");
        result.AppendLine($"Status: {status}\n");
        result.AppendLine($"Terminals Found: {terminalsInSpace.Count}");

        int count = 1;

        foreach (FamilyInstance terminal in terminalsInSpace)
        {
            Parameter flow = terminal.get_Parameter(BuiltInParameter.RBS_DUCT_FLOW_PARAM);
            double terminalFlowCFM = flow != null ? (flow.AsDouble() * 60.0) : 0.0;
            result.AppendLine($"   {count}. {terminal.Name} | {Math.Round(terminalFlowCFM, 1)} CFM");
            count++;
        }

        return result;
    }
}

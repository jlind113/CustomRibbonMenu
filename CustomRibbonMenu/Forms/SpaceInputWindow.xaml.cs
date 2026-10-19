using System.Windows;

namespace CustomRibbonMenu.Forms;

public partial class SpaceInputWindow : Window
{
    public string SpaceNumber
    {
        get; private set;
    }
    public double SpecifiedFlow
    {
        get; private set;
    }
    public double Tolerance
    {
        get; private set;
    }

    public SpaceInputWindow()
    {
        InitializeComponent();
    }

    private void btnRun_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtSpaceNumber.Text))
        {
            MessageBox.Show("Please enter a Space Number.", "Input Error");
            return;
        }

        if (!double.TryParse(txtTargetFlow.Text, out double flow))
        {
            MessageBox.Show("Target Flow must be a valid number.", "Input Error");
            return;
        }

        if (!double.TryParse(txtTolerance.Text, out double tolerance))
        {
            MessageBox.Show("Tolerance must be a valid number.", "Input Error");
            return;
        }

        SpaceNumber = txtSpaceNumber.Text;
        SpecifiedFlow = flow;
        Tolerance = tolerance;

        DialogResult = true;
        Close();
    }
}

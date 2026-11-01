using System.Windows;
using PCBManufacturing.PresentationLogic.ViewModels;

namespace PCBManufacturing.Wpf.Views;

public partial class MessageDialogWindow : Window
{
    public MessageDialogWindow(DialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.CloseRequested += (_, result) => DialogResult = result;
    }
}

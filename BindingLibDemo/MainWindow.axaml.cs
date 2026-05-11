using Avalonia.Controls;
using Avalonia.Interactivity;

namespace BindingLibDemo;

public partial class MainWindow : Window
{
    private MainViewModel Vm => (MainViewModel)DataContext!;
    public MainWindow() { InitializeComponent(); }

    private void OnSwitchDataTable(object? s, RoutedEventArgs e) => Vm.SwitchToDataTable();
    private void OnSwitchList(object? s, RoutedEventArgs e)      => Vm.SwitchToList();
    private void OnAddRow(object? s, RoutedEventArgs e)           => Vm.AddRow();
    private void OnDeleteRow(object? s, RoutedEventArgs e)        => Vm.DeleteSelected();
    private void OnClearFilter(object? s, RoutedEventArgs e)      => Vm.ClearFilter();
    private void OnExpandAll(object? s, RoutedEventArgs e)        => Vm.ExpandAllNodes();
    private void OnCollapseAll(object? s, RoutedEventArgs e)      => Vm.CollapseAllNodes();
    private void OnAddNode(object? s, RoutedEventArgs e)          => Vm.AddTreeNode();
    private void OnRemoveNode(object? s, RoutedEventArgs e)       => Vm.RemoveSelectedNode();
}

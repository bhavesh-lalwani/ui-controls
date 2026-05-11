using Avalonia;
using Avalonia.Controls; // Core controls
using Avalonia.Media;
using System;

namespace Lib;

// Ensure this inherits from DataGrid
public class CustomDataGrid : DataGrid
{
    protected override Type StyleKeyOverride => typeof(DataGrid);

    public CustomDataGrid()
    {
        this.AutoGenerateColumns = true;
        
        // This property belongs to Avalonia.Controls.DataGrid
        this.GridLinesVisibility = DataGridGridLinesVisibility.All;
        
    }
}
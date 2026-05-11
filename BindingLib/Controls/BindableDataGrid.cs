using System.Collections;
using System.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using BindingLib.Adapters;

namespace BindingLib.Controls
{
    public class BindableDataGrid : DataGrid
    {
        // ── DataSource property ───────────────────────────────────────────────
        public static readonly StyledProperty<object?> DataSourceProperty =
            AvaloniaProperty.Register<BindableDataGrid, object?>(
                nameof(DataSource), defaultBindingMode: BindingMode.TwoWay);

        public object? DataSource
        {
            get => GetValue(DataSourceProperty);
            set => SetValue(DataSourceProperty, value);
        }

        // ── SelectedRow property ──────────────────────────────────────────────
        public static readonly StyledProperty<object?> SelectedRowProperty =
            AvaloniaProperty.Register<BindableDataGrid, object?>(
                nameof(SelectedRow), defaultBindingMode: BindingMode.TwoWay);

        public object? SelectedRow
        {
            get => GetValue(SelectedRowProperty);
            set => SetValue(SelectedRowProperty, value);
        }

        // ── Constructor ───────────────────────────────────────────────────────
        public BindableDataGrid()
        {
            // Sync grid selection → SelectedRow binding
            SelectionChanged += (_, _) => SelectedRow = SelectedItem;
        }

        // ── React to property changes ─────────────────────────────────────────
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == DataSourceProperty)
                ApplyDataSource(change.NewValue as object);

            if (change.Property == SelectedRowProperty)
            {
                var incoming = change.NewValue as object;
                if (SelectedItem != incoming)
                    SelectedItem = incoming;
            }
        }

        // ── Core: wire the right ItemsSource depending on what was passed ─────
        private void ApplyDataSource(object? source)
        {
            Columns.Clear();

            switch (source)
            {
                case null:
                    ItemsSource = null;
                    break;

                case DataTable table:
                    // DataTable needs an adapter because DataRowView bindings
                    // are not plain properties — we must use Row["ColName"] syntax
                    AutoGenerateColumns = false;
                    var adapter = new DataTableAdapter(table);
                    foreach (DataColumn col in table.Columns)
                        Columns.Add(new DataGridTextColumn
                        {
                            Header    = col.ColumnName,
                            Binding   = new Binding($"Row[\"{col.ColumnName}\"]"),
                            IsReadOnly = true
                        });
                    ItemsSource = adapter;
                    break;

                case IEnumerable items:
                    // List<T>, ObservableCollection<T>, or any IEnumerable —
                    // bind directly, let AutoGenerateColumns reflect the type
                    AutoGenerateColumns = true;
                    ItemsSource = items;
                    break;
            }
        }
    }
}
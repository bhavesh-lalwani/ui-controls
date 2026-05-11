using System;
using System.Collections.ObjectModel;
using System.Data;

namespace BindingLib.Adapters
{
    public class DataTableAdapter : ObservableCollection<DataRowView>
    {
        private readonly DataTable _table;
        private bool _refreshing;

        public DataTableAdapter(DataTable table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
            Populate();
            _table.RowChanged             += (_, _) => Refresh();
            _table.RowDeleted             += (_, _) => Refresh();
            _table.TableCleared           += (_, _) => Refresh();
            _table.DefaultView.ListChanged += (_, _) => Refresh();
        }

        public DataTable            Table   => _table;
        public DataColumnCollection Columns => _table.Columns;

        public string Filter { get => _table.DefaultView.RowFilter; set { _table.DefaultView.RowFilter = value; Refresh(); } }
        public string Sort   { get => _table.DefaultView.Sort;      set { _table.DefaultView.Sort = value;      Refresh(); } }

        public void Refresh()
        {
            if (_refreshing) return;
            _refreshing = true;
            try { Populate(); } finally { _refreshing = false; }
        }

        private void Populate() { Clear(); foreach (DataRowView r in _table.DefaultView) Add(r); }
    }
}

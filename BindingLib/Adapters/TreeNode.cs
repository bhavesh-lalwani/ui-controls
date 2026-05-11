using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace BindingLib.Adapters
{
    public class TreeNode : INotifyPropertyChanged
    {
        private string _title = ""; private bool _isExpanded; private string? _icon; private object? _tag;

        public TreeNode() { }
        public TreeNode(string title, object? tag = null) { _title = title; _tag = tag; }

        public string  Title      { get => _title;      set => Set(ref _title, value); }
        public string? Icon       { get => _icon;       set => Set(ref _icon, value); }
        public object? Tag        { get => _tag;        set => Set(ref _tag, value); }
        public bool    IsExpanded { get => _isExpanded; set => Set(ref _isExpanded, value); }
        public ObservableCollection<TreeNode> Children { get; } = new();

        public static ObservableCollection<TreeNode> FromDataTable(
            DataTable table, string idColumn, string parentColumn,
            string labelColumn, string? iconColumn = null)
        {
            var lookup = new Dictionary<object, TreeNode>();
            foreach (DataRow row in table.Rows)
                lookup[row[idColumn]] = new TreeNode(row[labelColumn]?.ToString() ?? "")
                    { Tag = row, Icon = iconColumn != null ? row[iconColumn]?.ToString() : null };
            var roots = new ObservableCollection<TreeNode>();
            foreach (DataRow row in table.Rows)
            {
                var node = lookup[row[idColumn]]; var pid = row[parentColumn];
                if (pid == DBNull.Value || !lookup.ContainsKey(pid)) roots.Add(node);
                else lookup[pid].Children.Add(node);
            }
            return roots;
        }

        public static ObservableCollection<TreeNode> FromList<T>(IEnumerable<T> items,
            Func<T, string> ls, Func<T, IEnumerable<T>?>? cs = null, Func<T, string?>? icons = null)
        {
            var roots = new ObservableCollection<TreeNode>();
            foreach (var item in items) roots.Add(Build(item, ls, cs, icons));
            return roots;
        }

        private static TreeNode Build<T>(T item, Func<T, string> ls,
            Func<T, IEnumerable<T>?>? cs, Func<T, string?>? icons)
        {
            var node = new TreeNode(ls(item), item) { Icon = icons?.Invoke(item) };
            var children = cs?.Invoke(item);
            if (children != null) foreach (var c in children) node.Children.Add(Build(c, ls, cs, icons));
            return node;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void Set<T>(ref T f, T v, [CallerMemberName] string? p = null)
        { if (EqualityComparer<T>.Default.Equals(f, v)) return; f = v; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p)); }
    }
}

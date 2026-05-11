using System;
using System.Collections.Generic;
using System.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using BindingLib.Adapters;

namespace BindingLib.Controls
{
    /// <summary>
    /// Drop-in TreeView that binds to ObservableCollection&lt;TreeNode&gt;.
    /// Build nodes via TreeNode.FromDataTable() or TreeNode.FromList().
    /// </summary>
    public class BindableTreeControl : TreeView
    {
        public static readonly StyledProperty<IEnumerable<TreeNode>?> NodesProperty =
            AvaloniaProperty.Register<BindableTreeControl, IEnumerable<TreeNode>?>(nameof(Nodes));
        public IEnumerable<TreeNode>? Nodes
        {
            get => GetValue(NodesProperty);
            set => SetValue(NodesProperty, value);
        }

        public static readonly StyledProperty<TreeNode?> SelectedNodeProperty =
            AvaloniaProperty.Register<BindableTreeControl, TreeNode?>(nameof(SelectedNode), defaultBindingMode: BindingMode.TwoWay);
        public TreeNode? SelectedNode
        {
            get => GetValue(SelectedNodeProperty);
            set => SetValue(SelectedNodeProperty, value);
        }

        public static readonly StyledProperty<bool> ShowIconsProperty =
            AvaloniaProperty.Register<BindableTreeControl, bool>(nameof(ShowIcons), true);
        public bool ShowIcons
        {
            get => GetValue(ShowIconsProperty);
            set => SetValue(ShowIconsProperty, value);
        }

        public event EventHandler<TreeNode?>? NodeSelected;

        public BindableTreeControl()
        {
            ItemTemplate = new FuncTreeDataTemplate<TreeNode>(
                build: (node, _) =>
                {
                    var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6 };
                    if (!string.IsNullOrEmpty(node.Icon))
                        panel.Children.Add(new TextBlock { Text = node.Icon, FontSize = 14, VerticalAlignment = VerticalAlignment.Center });
                    panel.Children.Add(new TextBlock { Text = node.Title, VerticalAlignment = VerticalAlignment.Center });
                    return panel;
                },
                itemsSelector: node => node.Children.Count > 0 ? node.Children : null
            );

            SelectionChanged += (_, _) =>
            {
                if (SelectedItem is TreeNode n) { SelectedNode = n; NodeSelected?.Invoke(this, n); }
            };
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == NodesProperty)   ItemsSource = change.NewValue as IEnumerable<TreeNode>;
            if (change.Property == SelectedNodeProperty && SelectedItem != (change.NewValue as TreeNode))
                SelectedItem = change.NewValue as TreeNode;
        }

        public void ExpandAll()   => Walk(Nodes, n => n.IsExpanded = true);
        public void CollapseAll() => Walk(Nodes, n => n.IsExpanded = false);

        public void LoadFromDataTable(DataTable t, string id, string parent, string label, string? icon = null)
            => Nodes = TreeNode.FromDataTable(t, id, parent, label, icon);

        public void LoadFromList<T>(IEnumerable<T> items, Func<T, string> ls,
            Func<T, IEnumerable<T>?>? cs = null, Func<T, string?>? icons = null)
            => Nodes = TreeNode.FromList(items, ls, cs, icons);

        private static void Walk(IEnumerable<TreeNode>? nodes, Action<TreeNode> action)
        {
            if (nodes == null) return;
            foreach (var n in nodes) { action(n); Walk(n.Children, action); }
        }
    }
}

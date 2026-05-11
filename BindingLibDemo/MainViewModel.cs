using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using BindingLib.Adapters;

namespace BindingLibDemo;

public class MainViewModel : INotifyPropertyChanged
{
    private object?   _dataSource;
    private string    _filterText     = "";
    private string    _statusText     = "Ready — add rows, select, filter, explore the tree!";
    private string    _lastAction     = "";
    private TreeNode? _selectedNode;
    private object?   _selectedRow;
    private string    _dataSourceMode = "DataTable";
    private string    _rowDetail      = "";
    private string    _treeDetail     = "";
    private bool      _showRowHighlight;

    private DataTable                      _table = null!;
    private ObservableCollection<Customer> _list  = null!;

    public MainViewModel()
    {
        var cust = new List<Customer>
        {
            new Customer { Name = "Alice", Id = 1 },
            new Customer { Name = "Bob", Id = 2 }
        };
        DataSource = new ObservableCollection<Customer>(cust);
        BuildTable();
        BuildList();
        BuildTree();
        SwitchToDataTable();
    }

    // ── Properties ────────────────────────────────────────────────────────────
    public object?  DataSource       { get => _dataSource;       set => Set(ref _dataSource, value); }
    public string   DataSourceMode   { get => _dataSourceMode;   set => Set(ref _dataSourceMode, value); }
    public string   FilterText       { get => _filterText;       set { Set(ref _filterText, value); ApplyFilter(value); } }
    public string   StatusText       { get => _statusText;       set => Set(ref _statusText, value); }
    public string   LastAction       { get => _lastAction;       set => Set(ref _lastAction, value); }
    public string   RowDetail        { get => _rowDetail;        set => Set(ref _rowDetail, value); }
    public string   TreeDetail       { get => _treeDetail;       set => Set(ref _treeDetail, value); }
    public bool     ShowRowHighlight { get => _showRowHighlight; set => Set(ref _showRowHighlight, value); }

    public object? SelectedRow
    {
        get => _selectedRow;
        set { Set(ref _selectedRow, value); OnRowSelected(value); }
    }

    public ObservableCollection<TreeNode>? TreeNodes { get; private set; }

    public TreeNode? SelectedNode
    {
        get => _selectedNode;
        set { Set(ref _selectedNode, value); OnNodeSelected(value); }
    }

    public ObservableCollection<string> ActivityLog { get; } = new();

    // ── DataGrid commands ─────────────────────────────────────────────────────
    public void SwitchToDataTable()
    {
        DataSourceMode = "DataTable";
        DataSource     = _table;
        Log($"📋 DataTable loaded — {_table.Rows.Count} rows");
        StatusText = "DataTable bound. Click a row to select it. Try Add / Delete / Filter.";
    }

    public void SwitchToList()
    {
        DataSourceMode = "List<T>";
        DataSource     = _list;
        Log($"📋 ObservableCollection<Customer> loaded — {_list.Count} items");
        StatusText = "ObservableCollection bound. Add/Delete update the grid instantly via INotifyCollectionChanged.";
    }

    public void AddRow()
    {
        var rng = new Random();
        var names     = new[]{"Alice","Bob","Carol","Dave","Eve","Frank","Grace","Hina","Ivan","Julia"};
        var countries = new[]{"India","Germany","USA","Japan","Brazil","UK","France","Australia"};
        var name    = names[rng.Next(names.Length)] + " " + (char)('A'+rng.Next(26)) + ".";
        var country = countries[rng.Next(countries.Length)];
        var balance = Math.Round(rng.NextDouble() * 15000, 2);

        if (DataSourceMode == "DataTable")
        {
            var row = _table.NewRow();
            row["Id"] = _table.Rows.Count + 1; row["Name"] = name;
            row["Email"] = $"{name.Split(' ')[0].ToLower()}@demo.com";
            row["Country"] = country; row["Balance"] = (decimal)balance; row["IsActive"] = rng.Next(2)==1;
            _table.Rows.Add(row);
            Log($"✅ Row added: {name} / {country} / {balance:C}");
            StatusText = $"Row added to DataTable ({_table.Rows.Count} total). DataTableAdapter notified grid automatically.";
        }
        else
        {
            _list.Add(new Customer { Id=_list.Count+1, Name=name, Email=$"{name.Split(' ')[0].ToLower()}@demo.com",
                Country=country, Balance=(decimal)balance, IsActive=rng.Next(2)==1 });
            Log($"✅ Customer added: {name} / {country}");
            StatusText = $"Customer added to ObservableCollection ({_list.Count} total). CollectionChanged event updated grid.";
        }
    }

    public void DeleteSelected()
    {
        if (SelectedRow is DataRowView drv)
        {
            var name = drv.Row["Name"]?.ToString();
            drv.Row.Delete(); _table.AcceptChanges();
            RowDetail = ""; ShowRowHighlight = false;
            Log($"🗑️ Deleted: {name}. {_table.Rows.Count} rows remain.");
            StatusText = $"Row deleted from DataTable. {_table.Rows.Count} rows remain.";
        }
        else if (SelectedRow is Customer c)
        {
            _list.Remove(c);
            RowDetail = ""; ShowRowHighlight = false;
            Log($"🗑️ Removed: {c.Name}. {_list.Count} items remain.");
            StatusText = $"Customer removed from ObservableCollection. {_list.Count} items remain.";
        }
        else { StatusText = "⚠️ Select a row first, then click Delete."; }
    }

    public void ClearFilter()
    {
        FilterText = "";
        if (DataSourceMode == "DataTable") _table.DefaultView.RowFilter = "";
        Log("🔍 Filter cleared");
        StatusText = "Filter cleared — all rows visible.";
    }

    // ── Tree commands ─────────────────────────────────────────────────────────
    public void ExpandAllNodes()   { Walk(TreeNodes, n => n.IsExpanded = true);  Log("🌳 All nodes expanded"); StatusText = "All TreeNode.IsExpanded set to true recursively."; }
    public void CollapseAllNodes() { Walk(TreeNodes, n => n.IsExpanded = false); Log("🌲 All nodes collapsed"); StatusText = "All nodes collapsed."; }

    public void AddTreeNode()
    {
        var names = new[]{"New Team","Interns","Contractors","QA","DevOps"};
        var icons = new[]{"🆕","👨‍💻","🔧","🧪","🚀"};
        var rng   = new Random();
        var name  = names[rng.Next(names.Length)];
        var icon  = icons[rng.Next(icons.Length)];
        var node  = new TreeNode(name) { Icon = icon };
        node.Children.Add(new TreeNode("Member 1") { Icon = "👤" });
        node.Children.Add(new TreeNode("Member 2") { Icon = "👤" });
        TreeNodes?.Add(node);
        Log($"🌿 Tree node added: {icon} {name}");
        StatusText = $"Node '{name}' added to ObservableCollection<TreeNode> — TreeView updated instantly.";
    }

    public void RemoveSelectedNode()
    {
        if (SelectedNode == null) { StatusText = "⚠️ Select a tree node first."; return; }
        var title = SelectedNode.Title;
        if (TreeNodes != null && TreeNodes.Contains(SelectedNode)) TreeNodes.Remove(SelectedNode);
        else RemoveFromTree(TreeNodes, SelectedNode);
        TreeDetail = ""; SelectedNode = null;
        Log($"🗑️ Tree node removed: {title}");
        StatusText = $"Node '{title}' removed from tree.";
    }

    // ── Private helpers ───────────────────────────────────────────────────────
    private void OnRowSelected(object? row)
    {
        if (row is DataRowView drv)
        {
            ShowRowHighlight = true;
            RowDetail = $"Name: {drv.Row["Name"]}  |  Country: {drv.Row["Country"]}  |  Balance: {drv.Row["Balance"]:C}  |  Active: {drv.Row["IsActive"]}";
            Log($"👆 Row selected: {drv.Row["Name"]} ({drv.Row["Country"]})");
            StatusText = "Row selected — SelectedRow property in ViewModel updated via two-way binding.";
        }
        else if (row is Customer c)
        {
            ShowRowHighlight = true;
            RowDetail = $"Name: {c.Name}  |  Country: {c.Country}  |  Balance: {c.Balance:C}  |  Active: {c.IsActive}";
            Log($"👆 Customer selected: {c.Name}");
            StatusText = "Customer selected via two-way SelectedRow binding.";
        }
        else { ShowRowHighlight = false; RowDetail = ""; }
    }

    private void OnNodeSelected(TreeNode? node)
    {
        if (node == null) return;
        TreeDetail = $"Title: {node.Title}  |  Icon: {node.Icon ?? "none"}  |  Children: {node.Children.Count}  |  Expanded: {node.IsExpanded}";
        Log($"🌿 Node selected: {node.Icon} {node.Title} ({node.Children.Count} children)");
        StatusText = $"TreeNode '{node.Title}' selected — SelectedNode updated via two-way binding.";
    }

    private void ApplyFilter(string text)
    {
        if (DataSourceMode != "DataTable" || DataSource is not DataTable dt) return;
        dt.DefaultView.RowFilter = string.IsNullOrWhiteSpace(text) ? ""
            : $"Name LIKE '%{text}%' OR Country LIKE '%{text}%'";
        Log($"🔍 Filter '{text}' → {dt.DefaultView.Count}/{dt.Rows.Count} rows");
        StatusText = $"DataView.RowFilter applied — {dt.DefaultView.Count} of {dt.Rows.Count} rows shown.";
    }

    private void Log(string msg)
    {
        LastAction = msg;
        ActivityLog.Insert(0, $"[{DateTime.Now:HH:mm:ss}]  {msg}");
        if (ActivityLog.Count > 50) ActivityLog.RemoveAt(ActivityLog.Count - 1);
    }

    private static void Walk(IEnumerable<TreeNode>? nodes, Action<TreeNode> act)
    { if (nodes==null) return; foreach (var n in nodes) { act(n); Walk(n.Children, act); } }

    private static bool RemoveFromTree(IEnumerable<TreeNode>? nodes, TreeNode target)
    {
        if (nodes is not ObservableCollection<TreeNode> oc) return false;
        if (oc.Contains(target)) { oc.Remove(target); return true; }
        foreach (var n in oc) if (RemoveFromTree(n.Children, target)) return true;
        return false;
    }

    private void BuildTable()
    {
        _table = new DataTable("Customers");
        _table.Columns.Add("Id", typeof(int)); _table.Columns.Add("Name", typeof(string));
        _table.Columns.Add("Email", typeof(string)); _table.Columns.Add("Country", typeof(string));
        _table.Columns.Add("Balance", typeof(decimal)); _table.Columns.Add("IsActive", typeof(bool));
        object[][] rows = {
            new object[]{1,"Priya Sharma",  "priya@demo.com",  "India",   9450.00m,true },
            new object[]{2,"Hans Müller",   "hans@demo.com",   "Germany", 3200.50m,true },
            new object[]{3,"Sarah Johnson", "sarah@demo.com",  "USA",     7800.75m,false},
            new object[]{4,"Yuki Tanaka",   "yuki@demo.com",   "Japan",   1200.00m,true },
            new object[]{5,"Carlos Silva",  "carlos@demo.com", "Brazil",  5500.20m,true },
            new object[]{6,"Emma Wilson",   "emma@demo.com",   "UK",      4300.00m,false},
            new object[]{7,"Ahmed Hassan",  "ahmed@demo.com",  "Egypt",   8900.90m,true },
            new object[]{8,"Li Wei",        "liwei@demo.com",  "China",   2100.30m,true },
            new object[]{9,"Marie Dupont",  "marie@demo.com",  "France",  6600.60m,false},
            new object[]{10,"John O'Brien", "john@demo.com",   "Ireland", 3800.00m,true },
        };
        foreach (var r in rows) _table.Rows.Add(r);
    }

    private void BuildList() => _list = new ObservableCollection<Customer>
    {
        new(){Id=1,Name="Priya Sharma", Email="priya@demo.com", Country="India",   Balance=9450.00m,IsActive=true },
        new(){Id=2,Name="Hans Müller",  Email="hans@demo.com",  Country="Germany", Balance=3200.50m,IsActive=true },
        new(){Id=3,Name="Sarah Johnson",Email="sarah@demo.com", Country="USA",     Balance=7800.75m,IsActive=false},
        new(){Id=4,Name="Yuki Tanaka",  Email="yuki@demo.com",  Country="Japan",   Balance=1200.00m,IsActive=true },
        new(){Id=5,Name="Carlos Silva", Email="carlos@demo.com",Country="Brazil",  Balance=5500.20m,IsActive=true },
    };

    private void BuildTree()
    {
        var eng = new Department{Id=1,Name="Engineering",Icon="⚙️",SubDepartments=new(){
            new(){Id=11,Name="Backend", Icon="🖥️",Employees=new(){new(){Id=101,Name="Priya Sharma",Role="Sr. Engineer"},new(){Id=102,Name="Li Wei",Role="Engineer"}}},
            new(){Id=12,Name="Frontend",Icon="🎨",Employees=new(){new(){Id=103,Name="Emma Wilson",Role="Lead UI Dev"},new(){Id=104,Name="Yuki Tanaka",Role="UI Dev"}}},
        }};
        var sales = new Department{Id=2,Name="Sales",Icon="💼",SubDepartments=new(){
            new(){Id=21,Name="APAC",Icon="🌏",Employees=new(){new(){Id=201,Name="Ahmed Hassan",Role="Account Mgr"}}},
            new(){Id=22,Name="EMEA",Icon="🌍",Employees=new(){new(){Id=202,Name="Marie Dupont",Role="Sales Lead"},new(){Id=203,Name="Hans Müller",Role="Account Mgr"}}},
        }};
        var hr = new Department{Id=3,Name="Human Resources",Icon="👥",Employees=new(){
            new(){Id=301,Name="Sarah Johnson",Role="HR Manager"},
            new(){Id=302,Name="John O'Brien", Role="Recruiter"},
        }};

        TreeNodes = TreeNode.FromList(new[] { eng, sales, hr },
            ls: d => d.Name,      // matches 'ls'
            icons: d => d.Icon,   // matches 'icons'
            cs: d => {            // matches 'cs'
                var all = new List<Department>(d.SubDepartments);
                foreach (var e in d.Employees) 
                    all.Add(new Department { Id = e.Id, Name = $"{e.Name} — {e.Role}", Icon = e.Icon });
                return all.Count > 0 ? all : null;
        });
        OnPropertyChanged(nameof(TreeNodes));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void Set<T>(ref T f, T v, [CallerMemberName] string? p=null)
    { if (Equals(f,v)) return; f=v; PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(p)); }
    private void OnPropertyChanged([CallerMemberName] string? p=null)
        => PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(p));
}

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BindingLibDemo;

// ── Customer (used in DataGrid demos) ────────────────────────────────────────
public class Customer : INotifyPropertyChanged
{
    private int _id; private string _name=""; private string _email="";
    private string _country=""; private decimal _balance; private bool _isActive;

    public int     Id       { get => _id;       set => Set(ref _id, value); }
    public string  Name     { get => _name;     set => Set(ref _name, value); }
    public string  Email    { get => _email;    set => Set(ref _email, value); }
    public string  Country  { get => _country;  set => Set(ref _country, value); }
    public decimal Balance  { get => _balance;  set => Set(ref _balance, value); }
    public bool    IsActive { get => _isActive; set => Set(ref _isActive, value); }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void Set<T>(ref T f, T v, [CallerMemberName] string? p = null)
    { if (Equals(f,v)) return; f=v; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p)); }
}

// ── Department + Employee (used in Tree demo) ─────────────────────────────────
public class Department
{
    public int    Id   { get; set; }
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "📁";
    public List<Department> SubDepartments { get; set; } = new();
    public List<Employee>   Employees      { get; set; } = new();
}

public class Employee
{
    public int    Id   { get; set; }
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public string Icon { get; set; } = "👤";
}

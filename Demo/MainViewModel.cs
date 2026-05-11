using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Demo;

public class MainViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Person> _people = new();
    public ObservableCollection<Person> People 
    { 
        get => _people;
        set 
        {
            _people = value;
            OnPropertyChanged(); // Notifies UI if the whole list is swapped
        }
    }

    public MainViewModel()
    {
        People = new ObservableCollection<Person>
        {
            new Person { Name = "Alice", ID = 10 },
            new Person { Name = "Bob", ID = 2 }
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class Person : INotifyPropertyChanged 
{
    private string? _name;
    public string? Name 
    {
        get => _name;
        set 
        {
            if (_name == value) return; // Optimization: Don't notify if value hasn't changed
            _name = value;
            OnPropertyChanged(); // [CallerMemberName] automatically picks up "Name"
        }
    }

    private int _id;
    public int ID 
    {
        get => _id;
        set 
        {
            if (_id == value) return;
            _id = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
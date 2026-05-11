using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;
using RadialLab.Controls; 

namespace RadialLab.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var mainStack = new StackPanel
            {
                Spacing = 30,
                Margin = new Thickness(20),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var title = new TextBlock
            {
                Text = "BARC Interface Prototype",
                FontSize = 20,
                FontWeight = FontWeight.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            };
     
            var actionButton = new Button
            {
                Content = "Execute Command",
                HorizontalAlignment = HorizontalAlignment.Left,
                Padding = new Thickness(15, 7)
            };

            // Custom Radial Menu
            var myRadialMenu = new RadialMenu();

            //  Visual Tree
            mainStack.Children.Add(title);
            mainStack.Children.Add(myRadialMenu);
            mainStack.Children.Add(actionButton);

            //  window content
            this.Content = mainStack;
        }
    }
}
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using System;

namespace RadialLab.Controls
{
    public class RadialMenu : Control
    {
        private readonly string[] _labels = { "+", "-", "*", "/" };
        private readonly IBrush[] _baseBrushes = { Brushes.Orange, Brushes.Crimson, Brushes.MediumSeaGreen, Brushes.DodgerBlue };
        
        // State tracking for the UI
        private int _hoveredSlice = -1;
        private int _pressedSlice = -1;

        public RadialMenu()
        {
            // Enable hit testing and hover effects
            ClipToBounds = false;
        }

        protected override Size MeasureOverride(Size availableSize) => new Size(300, 300);


        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);
            var pos = e.GetPosition(this);
            int newHover = GetSliceAtPoint(pos);

            if (newHover != _hoveredSlice)
            {
                _hoveredSlice = newHover;
                InvalidateVisual(); // Forces the Render() method to run again
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            _pressedSlice = _hoveredSlice;
            InvalidateVisual();
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            _pressedSlice = -1;
            InvalidateVisual();
            
            // Logic for a "Click"
            if (_hoveredSlice != -1)
            {
                Console.WriteLine($"Clicked operation: {_labels[_hoveredSlice]}");
            }
        }

        protected override void OnPointerExited(PointerEventArgs e)
        {
            base.OnPointerExited(e);
            _hoveredSlice = -1;
            InvalidateVisual();
        }

        // --- RENDERING LOGIC ---

        public override void Render(DrawingContext context)
        {
            if (Bounds.Width == 0 || Bounds.Height == 0) return;
            var center = new Point(Bounds.Width / 2, Bounds.Height / 2);
            double outerRadius = Math.Min(Bounds.Width, Bounds.Height) / 2 - 10;
            double innerRadius = 40;
            
            for (int i = 0; i < 4; i++)
            {
                double startAngle = i * (Math.PI / 2);
                double endAngle = (i + 1) * (Math.PI / 2);

                // Determine the color based on interaction state
                IBrush brush = _baseBrushes[i];
                if (i == _pressedSlice) brush = Brushes.Gray; // Darken on press
                else if (i == _hoveredSlice) brush = Lighten(_baseBrushes[i]); // Glow on hover

                var sliceGeometry = CreateSectorGeometry(center, startAngle, endAngle, outerRadius, innerRadius);
                context.DrawGeometry(brush, new Pen(Brushes.White, 2), sliceGeometry);

                DrawLabel(context, _labels[i], center, startAngle + (Math.PI / 4), outerRadius / 1.5);
            }
        }

        // Helper to find which slice the mouse is over
        private int GetSliceAtPoint(Point p)
        {
            var center = new Point(Bounds.Width / 2, Bounds.Height / 2);
            var diff = p - center;
            double dist = Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y);
            
            // Ignore if in the center hole or outside the menu
            if (dist < 40 || dist > (Bounds.Width / 2)) return -1;

            double angle = Math.Atan2(diff.Y, diff.X);
            if (angle < 0) angle += 2 * Math.PI;

            return (int)(angle / (Math.PI / 2)) % 4;
        }

        private IBrush Lighten(IBrush brush) => new SolidColorBrush(((SolidColorBrush)brush).Color, 0.7);

        // (CreateSectorGeometry and DrawLabel remain the same as your previous code)
        private StreamGeometry CreateSectorGeometry(Point center, double startAngle, double endAngle, double outerRadius, double innerRadius)
        {
            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                Point pt1 = center + new Point(Math.Cos(startAngle) * outerRadius, Math.Sin(startAngle) * outerRadius);
                Point pt2 = center + new Point(Math.Cos(endAngle) * outerRadius, Math.Sin(endAngle) * outerRadius);
                Point pt3 = center + new Point(Math.Cos(endAngle) * innerRadius, Math.Sin(endAngle) * innerRadius);
                Point pt4 = center + new Point(Math.Cos(startAngle) * innerRadius, Math.Sin(startAngle) * innerRadius);
                ctx.BeginFigure(pt1, true);
                ctx.ArcTo(pt2, new Size(outerRadius, outerRadius), 0, false, SweepDirection.Clockwise);
                ctx.LineTo(pt3);
                ctx.ArcTo(pt4, new Size(innerRadius, innerRadius), 0, false, SweepDirection.CounterClockwise);
                ctx.EndFigure(true);
            }
            return geometry;
        }

        private void DrawLabel(DrawingContext context, string text, Point center, double angle, double distance)
        {
            var labelPos = center + new Point(Math.Cos(angle) * distance, Math.Sin(angle) * distance);
            var ft = new FormattedText(text, System.Globalization.CultureInfo.InvariantCulture, 
                FlowDirection.LeftToRight, Typeface.Default, 24, Brushes.White);
            context.DrawText(ft, labelPos - new Point(ft.Width / 2, ft.Height / 2));
        }
    }
}
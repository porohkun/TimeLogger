using System;
using System.Windows;
using System.Windows.Controls;

namespace TimeLogger.Controls
{
    public class VerticalWrapContainer : Decorator
    {
        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        public static readonly DependencyProperty PaddingProperty
            = DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(VerticalWrapContainer),
                new FrameworkPropertyMetadata(
                    new Thickness(),
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));


        private double _fixedWidth;

        protected override Size MeasureOverride(Size constraint)
        {
            var padding = HelperCollapseThickness(this.Padding);

            if (Child == null)
                return new Size(padding.Width, padding.Height);

            Child.Measure(new Size(_fixedWidth,
                Math.Max(0.0, constraint.Height - padding.Height)));
            var childSize = Child.DesiredSize;

            return new Size(
                childSize.Width + padding.Width,
                childSize.Height + padding.Height);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            _fixedWidth = arrangeSize.Width;
            if (Child == null) return arrangeSize;

            var padding = HelperCollapseThickness(this.Padding);
            Child.Arrange(new Rect(Padding.Left, Padding.Top,
                                Math.Max(0, arrangeSize.Width - padding.Width),
                                Math.Max(0, arrangeSize.Height - padding.Height)));

            return arrangeSize;
        }

        private static Size HelperCollapseThickness(Thickness th)
        {
            return new Size(th.Left + th.Right, th.Top + th.Bottom);
        }
    }
}
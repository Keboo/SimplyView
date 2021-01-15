using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace SimplyView
{
    public class IsPressedBehavior : Behavior<ButtonBase>
    {
        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            set { SetCurrentValue(IsPressedProperty, value); }
        }

        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(IsPressedBehavior), 
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotMouseCapture += AssociatedObject_GotMouseCapture;
            AssociatedObject.LostMouseCapture += AssociatedObject_LostMouseCapture;
        }

        private void AssociatedObject_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            IsPressed = false;
        }

        private void AssociatedObject_GotMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            IsPressed = true;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.GotMouseCapture -= AssociatedObject_GotMouseCapture;
            AssociatedObject.LostMouseCapture -= AssociatedObject_LostMouseCapture;
            base.OnDetaching();
        }
    }
}

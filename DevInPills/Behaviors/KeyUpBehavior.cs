using Microsoft.Xaml.Interactivity;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace DevInPills.Behaviors
{
    [ContentProperty(Name = nameof(Actions))]
    [TypeConstraint(typeof(Control))]
    public class KeyUpBehavior : DependencyObject, IBehavior
    {
        private FrameworkElement AssociatedControl => AssociatedObject as FrameworkElement;
        public DependencyObject AssociatedObject { get; private set; }

        public VirtualKey Key { get; set; }

        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObject = associatedObject;
            AssociatedControl.KeyUp += AssociatedControl_KeyUp;
        }

        public void Detach()
        {
            AssociatedControl.KeyUp -= AssociatedControl_KeyUp;
        }

        private void AssociatedControl_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Key)
            {
                Interaction.ExecuteActions(AssociatedObject, Actions, e.Key);
                e.Handled = true;
            }
        }

        public ActionCollection Actions
        {
            get
            {
                var actions = (ActionCollection)GetValue(ActionsProperty);
                if (actions == null)
                {
                    SetValue(ActionsProperty, actions = new ActionCollection());
                }
                return actions;
            }
        }
        public static readonly DependencyProperty ActionsProperty =
            DependencyProperty.Register("Actions", typeof(ActionCollection),
                typeof(KeyUpBehavior), new PropertyMetadata(null));

    }
}
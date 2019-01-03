using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Pickaxe.View
{
    public class DragIgnoreSlider : Slider
    {
        public bool IsDragging { get; protected set; }
        protected override void OnThumbDragCompleted(System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            IsDragging = false;
            base.OnThumbDragCompleted(e);
        }

        protected override void OnThumbDragStarted(System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            IsDragging = true;
            base.OnThumbDragStarted(e);
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            if (!IsDragging)
            {
                base.OnValueChanged(oldValue, newValue);
            }
        }
    }
}

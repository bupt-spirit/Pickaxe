using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Pickaxe.Utility
{
    [Serializable]
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (propertyName != "Item[]")
                VerifyPropertyName(propertyName);
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged?.Invoke(this, e);
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real, 
            // public, instance property on this object. 
            if (TypeDescriptor.GetProperties(this)[propertyName] != null) return;
            var msg = "Invalid property name: " + propertyName;
            throw new Exception(msg);
        }
    }
}

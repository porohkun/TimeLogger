using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TimeLogger.MVVM
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual bool SetProperty<T>(ref T storage, T value, Action? onChanged = null, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);
            return true;
        }

        protected virtual bool SetProperty<TValue, TStorage>(
            TStorage storage,
            Expression<Func<TStorage, TValue>> propertyExpression,
            TValue value,
            Action? onChanged = null,
            [CallerMemberName] string? propertyName = null)
        {
            if (propertyExpression.Body is not MemberExpression memberExpression)
                throw new ArgumentException("Expression should be a property.");

            if (memberExpression.Member is not PropertyInfo propertyInfo)
                throw new ArgumentException("Expression should be a property.");

            if (propertyInfo.PropertyType != typeof(TValue))
                throw new ArgumentException($"Expression should be '{nameof(TValue)}' type.");

            var currentValue = (TValue)propertyInfo.GetValue(storage)!;

            if (EqualityComparer<TValue>.Default.Equals(currentValue, value))
            {
                return false;
            }

            propertyInfo.SetValue(storage, value);
            onChanged?.Invoke();
            RaisePropertyChanged(propertyInfo.Name);
            return true;
        }

        protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

using System;

namespace PreLaunchTaskr.GUI.WinUI3.Converters;

public partial class BoolToVisiblityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return (bool) value ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return (Visibility) value == Visibility.Visible;
    }
}

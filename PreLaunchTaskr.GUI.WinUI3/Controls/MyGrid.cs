﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace PreLaunchTaskr.GUI.WinUI3.Controls;

public partial class MyGrid : Grid
{
    public MyGrid() : base()
    {
        Loaded += (o, e) => RefreshLayout();
    }

    public void RefreshLayout()
    {
        foreach (UIElement element in Children)
        {
            FrameworkElement? frameworkElement = element as FrameworkElement;
            if (frameworkElement is not null)
            {
                frameworkElement.HorizontalAlignment = ContentHorizontalAlignment;
                frameworkElement.VerticalAlignment = ContentVerticalAlignment;
            }
        }
    }

    public HorizontalAlignment ContentHorizontalAlignment
    {
        get => (HorizontalAlignment) GetValue(ContentHorizontalAlignmentProperty);
        set => SetValue(ContentHorizontalAlignmentProperty, value);
    }

    public VerticalAlignment ContentVerticalAlignment
    {
        get => (VerticalAlignment) GetValue(ContentVerticalAlignmentProperty);
        set => SetValue(ContentVerticalAlignmentProperty, value);
    }

    public static readonly DependencyProperty ContentHorizontalAlignmentProperty = DependencyProperty.Register(
        nameof(ContentHorizontalAlignment),
        typeof(HorizontalAlignment),
        typeof(MyGrid),
        new PropertyMetadata(default));

    public static readonly DependencyProperty ContentVerticalAlignmentProperty = DependencyProperty.Register(
        nameof(ContentHorizontalAlignment),
        typeof(VerticalAlignment),
        typeof(MyGrid),
        new PropertyMetadata(default));
}

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using PreLaunchTaskr.GUI.WinUI3.ViewModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

/// <summary>
/// ��������Ϊ InstalledPackagedProgramListViewModel
/// </summary>
public sealed partial class InstalledPackagedProgramListPage : Page
{
    public InstalledPackagedProgramListPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        viewModel = (InstalledPackagedProgramListViewModel) e.Parameter;
        base.OnNavigatedTo(e);
    }

    private InstalledPackagedProgramListViewModel viewModel = null!;

    public InstalledPackagedProgramListItem? SelectedItem => viewModel.SelectedItem;

    private bool loaded;  // ��ʼΪ false

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (!loaded)
        {
            loaded = true;
            await viewModel.InitAsync(DispatcherQueue);
        }
    }

    private void Border_Loaded(object sender, RoutedEventArgs e)
    {
        Border border = (Border) sender;
        border.Shadow = shadow;
        border.Translation += new Vector3(0, 0, 16);
    }

    private static readonly ThemeShadow shadow = new();

    private async void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            return;

        await viewModel.SearchProgramAsync(sender.Text, DispatcherQueue);
    }

    private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        InstalledPackagedProgramListItem item = (InstalledPackagedProgramListItem) args.SelectedItem;
        sender.Text = item.Name;
        viewModel.SelectedItem = item;
    }

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0 || e.AddedItems[0] is null)
            return;

        ListView listView = (ListView) sender;
        listView.ScrollIntoView(e.AddedItems[0]);
    }
}

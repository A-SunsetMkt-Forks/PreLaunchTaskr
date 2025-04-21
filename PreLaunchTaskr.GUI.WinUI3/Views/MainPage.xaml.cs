using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

using PreLaunchTaskr.Common.Helpers;
using PreLaunchTaskr.GUI.WinUI3.Extensions;
using PreLaunchTaskr.GUI.WinUI3.Helpers;
using PreLaunchTaskr.GUI.WinUI3.Helpers.ForFilePicker;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();

        Navigation.IsPaneOpen = false;
        Navigation.ExpandedModeThresholdWidth = Math.E * Navigation.OpenPaneLength;
        Navigation.RegisterPropertyChangedCallback(
            NavigationView.IsPaneOpenProperty,
            (o, dp) => AddProgramButton.IsCompact = !Navigation.IsPaneOpen);
    }

    private readonly MainViewModel viewModel = new();

    private readonly TitleBarPassthroughHelper titleBarPassthroughHelper = new(App.Current.MainWindow);

    private void AddProgramButton_Click(object sender, RoutedEventArgs e)
    {
        AddProgramMenu.ShowAt(AddProgramButton);
    }

    private async void SelectProgramFile_Click(object sender, RoutedEventArgs e)
    {
        StorageFile? file = await FileOpenPickerHelper
            .OpenFileForWindow(App.Current.MainWindow)
            .AddFileTypeFilter(".exe")
            .PickSingleAsync();

        if (file is null || string.IsNullOrWhiteSpace(file.Path))
            return;

        if (!viewModel.AddProgram(file.Name, file.Path))
        {
            await ShowFileNameExistedMessageBox(file.Name);
        }
    }

    private async void InputProgramPath_Click(object sender, RoutedEventArgs e)
    {
        InputTextPage page = new();
        DialogResult result = await this.MessageBox(page, "��������λ��", MessageBoxButtons.OKCancel);
        if (result != DialogResult.OK || string.IsNullOrWhiteSpace(page.Text))
            return;

        string path = StringHelper.TrimQuotes(page.Text.Trim());

        if (!File.Exists(path))
        {
            await this.MessageBox(path, "û������ļ�");
            return;
        }

        string name = Path.GetFileName(path);

        if (!viewModel.AddProgram(name, path))
            await ShowFileNameExistedMessageBox(name);
    }

    private async void SelectInstalledProgram_Click(object sender, RoutedEventArgs e)
    {
        // �ݲ�֧���̵�Ӧ�ã���Ϊ�̵�Ӧ������·����Ȩ�����ļ������޷������������ӡ�
        //InstalledProgramListPage page = new();
        //DialogResult result;
        //result = await this.MessageBox(page, "�Ѱ�װ�ĳ���", MessageBoxButtons.OKCancel);
        //if (result != DialogResult.OK || page.SelectedPath is null || string.IsNullOrWhiteSpace(page.SelectedPath))
        //    return;

        //string path = page.SelectedPath;
        //if (!Directory.Exists(path) && !File.Exists(path))
        //    return;

        //result = await this.MessageBox("Ȼ��������ճ��·����ǰ����������Ӧ�ó���", "���ƿ������ڵ�·����", MessageBoxButtons.YesNo);
        //if (result != DialogResult.Yes)
        //    return;

        //ClipboardHelper.Copy(path);
        //StorageFile? file = await FileOpenPickerHelper
        //    .OpenFileForWindow(App.Current.MainWindow)
        //    .AddFileTypeFilter(".exe")
        //    .PickSingleAsync();
        //if (file is null || string.IsNullOrWhiteSpace(file.Path))
        //    return;

        //path = file.Path;
        //string name = Path.GetFileName(path);
        //if (!viewModel.AddProgram(name, path))
        //    await ShowFileNameExistedMessageBox(name);

        InstalledTraditionalProgramListPage page = new();
        DialogResult result;
        result = await this.MessageBox(page, "�Ѱ�װ�ĳ���", MessageBoxButtons.OKCancel);
        if (result != DialogResult.OK || page.SelectedItem is null || string.IsNullOrWhiteSpace(page.SelectedItem.PossiblePath))
            return;

        string path = page.SelectedItem.PossiblePath;
        if (!Directory.Exists(path) && !File.Exists(path))
            return;

        result = await this.MessageBox("Ȼ��������ճ��·����ǰ����������Ӧ�ó���", "���ƿ������ڵ�·����", MessageBoxButtons.YesNo);
        if (result != DialogResult.Yes)
            return;

        ClipboardHelper.Copy(path);
        StorageFile? file = await FileOpenPickerHelper
            .OpenFileForWindow(App.Current.MainWindow)
            .AddFileTypeFilter(".exe")
            .PickSingleAsync();
        if (file is null || string.IsNullOrWhiteSpace(file.Path))
            return;

        path = file.Path;
        string name = Path.GetFileName(path);
        if (!viewModel.AddProgram(name, path))
            await ShowFileNameExistedMessageBox(name);
    }

    private async Task ShowFileNameExistedMessageBox(string filename)
        => await this.MessageBox("ע����е�ӳ��ٳ��ǰ����ļ����������ļ�·������˼�ʹ��ͬλ��ͬ���ļ���Ҳ�ᱻӳ��ٳ֡�", $"����ӹ�ͬ���ļ� {filename}");

    private async Task ShowFileNameExistedMessageBox(IEnumerable<string> filename)
        => await this.MessageBox($"ע����е�ӳ��ٳ��ǰ����ļ����������ļ�·������˼�ʹ��ͬλ��ͬ���ļ���Ҳ�ᱻӳ��ٳ֡�\n�����ļ�������δ����ӣ�\n{new StringBuilder().AppendJoin('\n', filename)}", $"����ӹ�ͬ���ļ� ");

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        Navigation.IsPaneOpen = true;

        ProgramListProgressBar.Visibility = Visibility.Visible;
        await viewModel.InitAsync();
        ProgramListProgressBar.Visibility = Visibility.Collapsed;
        App.Current.MainWindow.ExtendsContentIntoTitleBar = true;
        App.Current.MainWindow.SetTitleBar(TitleBarBorder);
        titleBarPassthroughHelper.Passthrough(TitleBarToggleButton);
        AddProgramButton.MinWidth = Navigation.CompactPaneLength - 8;  // �۵�����ർ���˵���ȼ����ߵ� Margin
        // <Thickness x:Key="NavigationViewItemButtonMargin">4,2</Thickness>
        // https://github.com/microsoft/microsoft-ui-xaml/blob/main/src/controls/dev/NavigationView/NavigationView_themeresources.xaml

        ContentFrame.Navigate(typeof(ProgramUnselectedPage));
    }

    private void TitleBarToggleButton_Click(object sender, RoutedEventArgs e)
    {
        Navigation.IsPaneOpen = !Navigation.IsPaneOpen;
    }

    private void Navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is null)
        {
            ContentFrame.Navigate(typeof(ProgramUnselectedPage));
            return;
        }

        ContentFrame.Navigate(typeof(ProgramConfigPage), args.SelectedItem);
    }

    private void RemoveProgram_Click(object sender, RoutedEventArgs e)
    {
        ProgramListItem item = DataContextHelper.GetDataContext<ProgramListItem>(sender);
        viewModel.RemoveProgram(item);
    }

    private void CopyProgramPath_Click(object sender, RoutedEventArgs e)
    {
        ProgramListItem item = DataContextHelper.GetDataContext<ProgramListItem>(sender);
        ClipboardHelper.Copy(item.Path);
    }

    private void OpenProgramPath_Click(object sender, RoutedEventArgs e)
    {
        ProgramListItem item = DataContextHelper.GetDataContext<ProgramListItem>(sender);
        WindowsHelper.OpenPathInExplorer(item.Path);
    }

    private async void Navigation_Drop(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            IReadOnlyList<IStorageItem> items = await e.DataView.GetStorageItemsAsync();
            if (items.Count == 0)
                return;

            List<string> duplicatedFileNames = [];
            List<string> unsupportedFileNames = [];
            foreach (IStorageItem item in items)
            {
                if (Path.GetExtension(item.Name) == ".exe")
                {
                    if (!viewModel.AddProgram(item.Name, item.Path))
                    {
                        duplicatedFileNames.Add(item.Path);
                    }
                }
                else
                {
                    unsupportedFileNames.Add(item.Path);
                }
            }
            if (duplicatedFileNames.Count > 0)
            {
                await ShowFileNameExistedMessageBox(duplicatedFileNames);
            }
            if (unsupportedFileNames.Count > 0)
            {
                await this.MessageBox($"�����ļ����ļ����Ͳ�֧�ֶ�δ����ӣ�\n{new StringBuilder().AppendJoin('\n', unsupportedFileNames)}", "ֻ����� exe ���͵��ļ�");
            }
        }
    }

    private void Navigation_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
    }

    private async void DragDropToAdd_Click(object sender, RoutedEventArgs e)
    {
        await this.MessageBox("�ȹرմ���ʾ��Ȼ�������Ҫ��ӵĳ���� exe �ļ���ק�������ɡ�", "��ק�����");
    }
}

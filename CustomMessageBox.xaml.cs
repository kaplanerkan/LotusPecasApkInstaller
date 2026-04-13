using System.Windows;
using System.Windows.Input;

namespace LotusPecasApkInstaller;

public enum CustomBoxIcon { Info, Success, Warning, Error }

public partial class CustomMessageBox : Window
{
    public CustomMessageBox()
    {
        InitializeComponent();
    }

    public static void Show(Window? owner, string message, string title, CustomBoxIcon icon = CustomBoxIcon.Info, string okText = "OK")
    {
        var box = new CustomMessageBox
        {
            Owner = owner ?? Application.Current?.MainWindow
        };
        box.TitleText.Text = title;
        box.MessageText.Text = message;
        box.OkButton.Content = okText;
        box.IconText.Text = icon switch
        {
            CustomBoxIcon.Success => "✅",
            CustomBoxIcon.Warning => "⚠",
            CustomBoxIcon.Error => "⛔",
            _ => "ℹ"
        };
        box.ShowDialog();
    }

    private void Ok_Click(object sender, RoutedEventArgs e) => Close();
    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    private void Border_Drag(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left) DragMove();
    }
}

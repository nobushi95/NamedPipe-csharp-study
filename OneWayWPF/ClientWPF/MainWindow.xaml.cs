using Shared;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace ClientWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var msg = SendMessageTextBox.Text;
            // TODO: タイムアウトを考慮する
            Task.Run(async () =>
            {
                await new Pipe().SendAsync(msg, TimeSpan.FromSeconds(1));
            }).ConfigureAwait(false); // Waitにすると、Taskが終了するまでUIは触れない
            // NOTE: 待っている間くるくるを表示したい
        }
    }
}

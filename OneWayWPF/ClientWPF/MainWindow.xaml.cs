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
            SendMessageTextBox.Clear();
            // TODO: タイムアウトを考慮する
            // Wait()にすると、Taskが終了するまでUIは触れない
            // Taskを投げっぱなし(終了を感知しない)でもよい
            Task.Run(async () =>
            {
                await Pipe.SendAsync(msg, TimeSpan.FromSeconds(1));
            });
            // NOTE: 待っている間くるくるを表示したい
            // => Taskを投げる前にくるくる動作を開始して、TaskをWait()してUIスレッドに復帰後に、くるくるを停止すれば実現できそう？
        }
    }
}

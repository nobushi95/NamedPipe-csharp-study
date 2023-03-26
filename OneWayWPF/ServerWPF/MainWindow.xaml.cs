using Shared;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ServerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CancellationTokenSource _cancel = new();
        private readonly SynchronizationContext? _context;

        public MainWindow()
        {
            InitializeComponent();
            _context = SynchronizationContext.Current;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await new Pipe().Server(OnReceive, _cancel.Token);
            }).ConfigureAwait(false);
        }

        private void OnReceive(string? msg)
        {
            if (msg == "exit")
            {
                _cancel.Cancel();
                //Close(); // 終了できない(CloseはUIスレッドで呼ばなければいけない？)

                // DispatcherとSynchronizationContextで呼ぶ違いは？？
                //_context?.Send((object? _) => { Close(); }, null);
                Dispatcher.Invoke(() => Close());
            }

            // DispatcherとSynchronizationContextで呼ぶ違いは？？
            // どちらもUIが更新される

            //_context?.Send((object? _) =>
            //{
            //    ReceiveTextBlock.Text += $"{msg}\n";
            //}, null);

            Dispatcher.Invoke(() =>
            {
                ReceiveTextBlock.Text += $"{msg}\n";
            });
        }
    }
}

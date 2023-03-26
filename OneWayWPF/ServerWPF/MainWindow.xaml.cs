using Shared;
using System;
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
                await Pipe.Server(OnReceive, _cancel.Token);
            }).ConfigureAwait(false);
        }

        private void OnReceive(string? msg)
        {
            if (msg == "exit")
            {
                _cancel.Cancel();
                //Close(); // 終了できない(CloseはUIスレッドで呼ばなければいけない？)

                // SynchronizationContextとDispatcherで呼ぶ違いは？？
                // 1. SynchronizationContextで呼ぶ
                //_context?.Send((object? _) => { Close(); }, null);
                // 2. Dispatcherで呼ぶ
                Dispatcher.Invoke(() => Close());
            }

            var s1 = $"Thread ID: {Environment.CurrentManagedThreadId}"; // ワーカースレッドで実行

            // SynchronizationContextとDispatcherで呼ぶ違いは？？
            // どちらもUIが更新される

            // 1. SynchronizationContextで呼ぶ
            //_context?.Send((object? _) =>
            //{
            //    ReceiveTextBlock.Text += $"{msg}\n";
            //    var s2 = $"Thread ID: {Environment.CurrentManagedThreadId}"; // UIスレッドで実行
            //}, null);

            // 2. Dispatcherで呼ぶ
            Dispatcher.Invoke(() =>
            {
                ReceiveTextBlock.Text += $"{msg}\n";
                var s2 = $"Thread ID: {Environment.CurrentManagedThreadId}"; // UIスレッドで実行
            });

            var s3 = $"Thread ID: {Environment.CurrentManagedThreadId}"; // ワーカースレッドで実行(s1と同じだが偶然 or 仕様？？)
        }
    }
}

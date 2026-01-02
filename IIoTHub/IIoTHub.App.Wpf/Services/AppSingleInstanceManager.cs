using System.IO;
using System.IO.Pipes;

namespace IIoTHub.App.Wpf.Services
{
    public static class AppSingleInstanceManager
    {
        private const string _mutexName = "IIoTHub.App.Wpf.SingleInstance.Mutex";
        private const string _pipeName = "IIoTHub.App.Wpf.SingleInstance.Pipe";

        private static Mutex _mutex;
        private static CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// 判斷是否可以建立應用程式實例
        /// </summary>
        public static bool CanCreateInstance()
        {
            _mutex = new Mutex(true, _mutexName, out var isNewInstance);
            return isNewInstance;
        }

        /// <summary>
        /// 通知已存在的應用程式實例顯示主視窗
        /// </summary>
        public static void NotifyExistingInstanceToShowWindow()
        {
            try
            {
                using var client = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out);
                client.Connect(1000); // 最多等待 1 秒
                using var writer = new StreamWriter(client) { AutoFlush = true };
                writer.WriteLine("SHOW_WINDOW");
            }
            catch
            {
                // 若無法連線代表第一個實例尚未準備好
            }
        }

        /// <summary>
        /// 開始監聽來自其他執行個體的「顯示視窗」訊息 (僅應在主實例中呼叫)
        /// </summary>
        public static void StartListeningForShowWindowMessage(Action onShowWindowRequested)
        {
            if (_cancellationTokenSource != null)
                return; // already started

            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => ListenForShowWindowMessageAsync(onShowWindowRequested, _cancellationTokenSource.Token));
        }

        /// <summary>
        /// 監聽 Named Pipe 的訊息，接收到 "SHOW_WINDOW" 時觸發指定動作
        /// </summary>
        /// <param name="onShowWindowRequested"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private static async Task ListenForShowWindowMessageAsync(Action onShowWindowRequested, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    using var server = new NamedPipeServerStream(_pipeName, PipeDirection.In);
                    await server.WaitForConnectionAsync(token);

                    using var reader = new StreamReader(server);
                    var message = await reader.ReadLineAsync();

                    if (message == "SHOW_WINDOW")
                    {
                        onShowWindowRequested?.Invoke();
                    }
                }
                catch (OperationCanceledException)
                {
                    // 任務被取消，忽略
                }
                catch (Exception)
                {
                    
                }
            }
        }

        /// <summary>
        /// 釋放資源
        /// </summary>
        public static void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            _mutex = null;
        }
    }
}

namespace VRC_Game
{
  internal class BackgroundTimer
  {
    public delegate void HandleUpdate();
    private readonly HandleUpdate _updateFunction;
    private Task? _timerTask;
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    public BackgroundTimer(TimeSpan interval, HandleUpdate handleUpdate)
    {
      _timer = new PeriodicTimer(interval);
      _updateFunction = handleUpdate;
    }

    public void Start()
    {
      _timerTask = DoWorkAsync(_updateFunction);
    }

    private async Task DoWorkAsync(HandleUpdate handleUpdate)
    {
      try {
        while(await _timer.WaitForNextTickAsync(_cancellationTokenSource.Token)) {
          handleUpdate();
        }
      } catch (OperationCanceledException) { }
    }

    public async Task StopAsync()
    {
      if (_timerTask is null) return;

      _cancellationTokenSource.Cancel();
      await _timerTask;
      _cancellationTokenSource.Dispose();
    }
  }
}

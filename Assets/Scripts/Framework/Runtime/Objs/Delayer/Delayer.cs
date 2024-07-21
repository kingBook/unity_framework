using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 基于 async、await 实现的异步延时器
/// <code>
/// // 例：延时3秒
/// CancellationTokenSource cancellationToken = new CancellationTokenSource();
/// Delayer.Delay(3.0f, cancellationToken, () => {
///     Debug.Log("delayed");
/// });
/// 
/// // 延时过程中调用以下代码中断
/// // * 注意： cancellationToken 需要手动销毁
/// cancellationToken.Cancel();
/// cancellationToken.Dispose();
/// cancellationToken = null;
/// </code>
/// </summary>
public static class Delayer {

    public static async void Delay(float seconds, CancellationTokenSource cancellationTokenSource, System.Action onDelayed) {
        int ms = Mathf.FloorToInt(seconds * 1000);
        if (cancellationTokenSource != null) {
            try {
                await Task.Delay(ms, cancellationTokenSource.Token);
            } catch (System.Exception) {

            }
            if (!cancellationTokenSource.IsCancellationRequested) {
                onDelayed?.Invoke();
            }
        } else {
            await Task.Delay(ms);
            onDelayed?.Invoke();
        }
    }

    public static void Delay(float seconds, System.Action onDelayed) {
        Delay(seconds, null, onDelayed);
    }
}

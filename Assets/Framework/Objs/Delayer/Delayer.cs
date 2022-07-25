using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 基于 async、await 实现的异步延时器
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

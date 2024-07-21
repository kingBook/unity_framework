// 此脚本用于跳过Unity的启动Logo
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

[Preserve] // 此特性用于确保此脚本能被打包进程序
public class SkipSplashImage {

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void Run() {
        Task.Run(() => {
            SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
        });
    }
}
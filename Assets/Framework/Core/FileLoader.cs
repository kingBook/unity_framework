using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
#pragma warning disable 0649

/// <summary>
/// 文件加载器
/// </summary>
public class FileLoader : MonoBehaviour {
    [Tooltip("进度条"), SerializeField]
    private PanelProgressbar m_panelProgressbar;

    /// <summary>
    /// 文件加载进度事件（是假模拟的进度）
    /// <br>void(float progress)</br>
    /// <br>progress：表示加载进度，范围[0,1]</br>
    /// </summary>
    public event Action<float> onProgressEvent;
    /// <summary>
    /// 文件加载完成事件
    /// <br>void(byte[][] bytesList)</br>
    /// <br>bytesList：表示加载完成后各个文件的总字节(索引与加载时传递的参数对应)</br>
    /// </summary>
    public event Action<byte[][]> onCompleteEvent;

    private FileStream m_fileStream;
    private bool m_isLoading;
    private float m_progressValue;

    /// <summary>
    /// 异步加载一个或多个本地文件
    /// <br>如果文件不存在将在onComplete(byte[][] bytesList)事件参数bytesList添加一个null</br>
    /// </summary>
    /// <param name="progressbarVisible">是否显示进度条</param>
    /// <param name="filePaths">可变长度文件路径列表，如: @"C:\Users\Administrator\Desktop\views0.xml"</param>
    public async void LoadAsync(bool progressbarVisible, params string[] filePaths) {
        OnLoadStart(progressbarVisible);

        byte[][] outBytesList = new byte[filePaths.Length][];
        for (int i = 0; i < filePaths.Length; i++) {
            byte[] buffer = null;
            string filePath = filePaths[i];
            await Task.Run(() => {
                if (File.Exists(filePath)) {
                    m_fileStream = File.OpenRead(filePath);

                    int fileLength = (int)m_fileStream.Length;
                    buffer = new byte[fileLength];

                    m_fileStream.Read(buffer, 0, fileLength);
                }
            });
            if (!gameObject.activeSelf) {
                //加载过程中，删除该脚本绑定的对象时，打断
                break;
            }
            outBytesList[i] = buffer;
            Dispose();
        }

        //所有加载完成
        if (gameObject.activeSelf) {
            OnLoadCompleteAll(outBytesList);
        }
    }

    private void OnLoadStart(bool progressbarVisible) {
        m_isLoading = true;
        m_progressValue = 0.0f;
        if (m_panelProgressbar != null) {
            m_panelProgressbar.SetProgress(m_progressValue);
            m_panelProgressbar.SetText("loading 0%...");
            m_panelProgressbar.gameObject.SetActive(progressbarVisible);
        }
        gameObject.SetActive(true);
    }

    private void OnLoadCompleteAll(byte[][] outBytesList) {
        m_isLoading = false;
        m_progressValue = 1.0f;
        if (m_panelProgressbar != null) {
            m_panelProgressbar.SetProgress(m_progressValue);
            m_panelProgressbar.SetText("loading 100%...");
            m_panelProgressbar.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);

        onCompleteEvent?.Invoke(outBytesList);
    }

    private void Update() {
        if (m_isLoading) {
            //模拟假的加载进度
            m_progressValue = Mathf.Min(m_progressValue + 0.1f, 0.9f);
            m_panelProgressbar.SetProgress(m_progressValue);
            m_panelProgressbar.SetText("loading " + Mathf.FloorToInt(m_progressValue * 100) + "%...");
            onProgressEvent?.Invoke(m_progressValue);
        }
    }

    private void Dispose() {
        if (m_fileStream != null) {
            m_fileStream.Dispose();
            m_fileStream.Close();
            m_fileStream = null;
        }
    }

    private void OnDestroy() {
        Dispose();
    }

}

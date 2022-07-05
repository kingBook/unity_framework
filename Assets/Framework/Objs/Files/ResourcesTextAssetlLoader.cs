using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ResourcesTextAssetlLoader : MonoBehaviour {

    private string m_filePath;

    /// <summary>
    /// 加载完成事件，回调函数格式：<code> void OnLoaded(TextAsset textAsset)</code>
    /// </summary>
    public event System.Action<TextAsset> onLoadedEvent;

    /// <summary>
    /// 转换 <see cref="TextAsset"/>  到 <see cref="XmlDocument"/>
    /// </summary>
    /// <param name="textAsset"></param>
    /// <returns></returns>
    public static XmlDocument ConvertToXml(TextAsset textAsset) {
        var xmlDoc = new XmlDocument();

        // 添加 xml 声明 <?xml version="1.0" encoding="utf-8" ?>
        var xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
        xmlDoc.AppendChild(xmlDeclaration);

        // 使用 字符串创建 xml 文档
        xmlDoc.LoadXml(textAsset.text);

        return xmlDoc;
    }

    /// <summary>
    /// 同步加载 <see cref="TextAsset"/>
    /// </summary>
    /// <param name="filePath"> 文件相对于 Resources 文件夹的路径，不需要文件后缀名，如: WorkingMode/PhoneMessages </param>
    /// <returns> TextAsset </returns>
    public TextAsset Load(string filePath) {
        m_filePath = filePath;
        TextAsset textAsset = Resources.Load<TextAsset>(m_filePath);
        onLoadedEvent?.Invoke(textAsset);
        onLoadedEvent = null;
        return textAsset;
    }

    /// <summary>
    /// 异步加载 <see cref="TextAsset"/>
    /// </summary>
    /// <param name="filePath"> 文件相对于 Resources 文件夹的路径，不需要文件后缀名，如: WorkingMode/PhoneMessages </param>
    public void LoadAsync(string filePath) {
        m_filePath = filePath;
        StartCoroutine(LoadAsyncCoroutine());
    }


    private IEnumerator LoadAsyncCoroutine() {
        ResourceRequest resourceRequest = Resources.LoadAsync<TextAsset>(m_filePath);
        while (!resourceRequest.isDone) {
            yield return null;
        }
        TextAsset textAsset = (TextAsset)resourceRequest.asset;
        onLoadedEvent?.Invoke(textAsset);
        onLoadedEvent = null;
    }

}

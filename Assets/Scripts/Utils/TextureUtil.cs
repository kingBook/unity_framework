using System.Collections;
using UnityEngine;

public static class TextureUtil {

    public static Texture2D ConvertToTexture2D(RenderTexture renderTexture) {
        int width = renderTexture.width;
        int height = renderTexture.height;
        Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
        var activeRecord = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture2D.Apply();
        RenderTexture.active = activeRecord;
        return texture2D;
    }

    public static Sprite ConvertToSprite(Texture2D texture2D) {
        var rect = new Rect(0.0f, 0.0f, texture2D.width, texture2D.height);
        var pivot = new Vector2(0.5f, 0.5f);
        var sprite = Sprite.Create(texture2D, rect, pivot, 100.0f);
        return sprite;
    }

}

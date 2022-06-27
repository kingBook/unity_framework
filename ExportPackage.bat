rem 导出包工具
::注意 = 号两边不能有空格
::如果echo有中文需要以ANSI编码保存
@echo off
@set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\2019.4.28f1c1\Editor\Unity.exe"
@set PROJECT_PATH=.
@set ASSETS_PATH=Assets\Scripts\Game.cs
@set FILE_NAME=aa.unitypackage
echo 开始导出..

%UNITY_PATH% -batchmode -quit -nographics -projectPath %PROJECT_PATH% -exportPackage %ASSETS_PATH% %FILE_NAME%

echo 导出完成
Pause
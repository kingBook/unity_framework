rem ����������
::ע�� = �����߲����пո�
::���echo��������Ҫ��ANSI���뱣��
@echo off
@set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\2019.4.28f1c1\Editor\Unity.exe"
@set PROJECT_PATH=.
@set ASSETS_PATH=Assets\Scripts\Game.cs
@set FILE_NAME=aa.unitypackage
echo ��ʼ����..

%UNITY_PATH% -batchmode -quit -nographics -projectPath %PROJECT_PATH% -exportPackage %ASSETS_PATH% %FILE_NAME%

echo �������
Pause
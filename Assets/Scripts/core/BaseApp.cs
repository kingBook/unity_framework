using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum Language{AUTO,CN,EN}
	
/// <summary>
/// 整个应用程序的单例抽象类(基类)
/// <br>子类的以下方法：FixedUpdate、Update、LateUpdate、OnGUI、OnRenderObject，</br>
/// <br>将使用以下代替：FixedUpdate2、Update2、LateUpdate2、OnGUI2、OnRenderObject2。</br>
/// </summary>
public abstract class BaseApp<T>:BaseMonoBehaviour where T:class,new(){
	
	protected static T _instance;
	/// <summary>
	/// 应用程序的单例实例
	/// </summary>
	public static T instance{ get => _instance; }
	//用于取消delayDestroyInstance();函数
	private static CancellationTokenSource _delayDestroyTokenSource;

	[Tooltip("用于调试其它场景需要调用该脚本，" +
	 "\n在Hierarchy中拖入该脚本所在的.unity文件时，" +
	 "\n不执行载入标题、其他场景等，将在代码中判定实现" +
	 "\n发布项目时必须为false。")
	]
	[SerializeField]
	private bool _isDebug=false;

	/// <summary>
	/// 改变语言事件
	/// </summary>
	public event Action<Language> onChangeLanguage;
	[Tooltip("AUTO:运行时根据系统语言决定是CN/EN " +
	 "\nCN:中文 " +
	 "\nEN:英文")
	]
	[SerializeField,SetProperty("language")]//此处使用SetProperty序列化setter方法，用法： https://github.com/LMNRY/SetProperty
	protected Language _language=Language.AUTO;

	[Tooltip("场景加载器")]
	[SerializeField]
	private SceneLoader _sceneLoader=null;

	[Tooltip("更新管理器")]
	[SerializeField]
	private UpdateManager _updateManager=null;

	/// <summary>
	/// 暂停或恢复事件，在调用setPause(bool)时方法发出
	/// </summary>
	public event Action<bool> onPauseOrResume;
	private bool _isPause;

	protected override void Awake() {
		base.Awake();
		destroyInstance(false);
		//再次加载场景时，如果已有实例则删除
		if(_instance==null){
			_instance=this as T;
			if(_language==Language.AUTO){
				initLanguage();
			}
		}else{
			Destroy(gameObject);
		}
	}

	private void initLanguage(){
		bool isCN=Application.systemLanguage==SystemLanguage.Chinese;
		isCN=isCN||Application.systemLanguage==SystemLanguage.ChineseSimplified;
		isCN=isCN||Application.systemLanguage==SystemLanguage.ChineseTraditional;
		_language=isCN?Language.CN:Language.EN;
		//改变语言事件
		onChangeLanguage?.Invoke(_language);
	}

	/// <summary>
	/// 设置暂停/恢复更新、物理模拟
	/// </summary>
	/// <param name="value"></param>
	public void setPause(bool value){
		if(_isPause==value)return;
		_isPause=value;
		//暂停或恢复3D物理模拟
		Physics.autoSimulation=!_isPause;
		//暂停或恢复2D物理模拟
		Physics2D.autoSimulation=!_isPause;
		//发出事件
		onPauseOrResume?.Invoke(value);
	}
	
	protected override void OnDestroy(){
		_updateManager.remove(this);
		if(_instance!=null){
			if(_instance.Equals(this)){
				//_instance销毁前,为了避免调用instance出错，
				//销毁所有绑定BaseMonoBehaviour实例的GameObject。
				BaseMonoBehaviour[] baseMonos=Resources.FindObjectsOfTypeAll<BaseMonoBehaviour>();
				int i=baseMonos.Length;
				while(--i>=0){
					BaseMonoBehaviour mono=baseMonos[i];
					if(_instance.Equals(mono))continue;
					//注意：调用Destroy()时并不会立即销毁，
					//对象销毁操作始终延迟到当前Update循环结束、但始终在渲染前完成
					Destroy(mono.gameObject);
				}
				//延时销毁_instance,避免调用instance出错。
				_delayDestroyTokenSource=new CancellationTokenSource();
				delayDestroyInstance(5000,_delayDestroyTokenSource);
			}
		}
	}

	/// <summary>
	/// 延时销毁_instance,具体用法:https://www.cnblogs.com/kingBook/p/11225720.html
	/// </summary>
	/// <param name="ms">毫秒</param>
	/// <param name="tokenSource">用于取消的CancellationTokenSource</param>
	private async void delayDestroyInstance(int ms,CancellationTokenSource tokenSource){
		try{
            await Task.Delay(ms,tokenSource.Token);
        }catch (Exception){
        }
		if(!tokenSource.IsCancellationRequested){
			destroyInstance(true);
		}
	}
	/// <summary>
	/// 取消延时销毁_instance
	/// </summary>
	/// <param name="isLog">输出消息</param>
	private void destroyInstance(bool isLog){
		if(isLog)Debug2.Log("destroy BaseApp.instance.");
		_instance=null;
		if(_delayDestroyTokenSource!=null){
			_delayDestroyTokenSource.Cancel();
			_delayDestroyTokenSource.Dispose();
			_delayDestroyTokenSource=null;
		}
	}

	public bool isDebug{ get => _isDebug; }
	
	/// <summary>
	/// 应用程序的语言
	/// </summary>
	public Language language{
		get => _language;
		set{
			_language=value;
			onChangeLanguage?.Invoke(_language);
		}
	}

	/// <summary>
	/// 场景加载器(有进度条)
	/// </summary>
	public SceneLoader sceneLoader{ get => _sceneLoader; }

	/// <summary>
	/// 更新管理器
	/// </summary>
	public UpdateManager updateManager{ get => _updateManager; }

	/// <summary>
	/// 是否已暂停
	/// </summary>
	public bool isPause{ get => _isPause; }


}

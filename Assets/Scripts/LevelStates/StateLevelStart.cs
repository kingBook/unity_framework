using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class StateLevelStart : State {

    private FsmLevel m_fsm;
    private Level m_level;


    private void InitRenderSettings() {
        RenderSettings.skybox = m_level.skyboxMaterial; // 天空盒

        RenderSettings.ambientMode = AmbientMode.Flat; // 平面环境光照
        RenderSettings.ambientLight = new Color(0.5988371f, 0.5701693f, 0.5574281f); // 环境光颜色

        RenderSettings.fog = false; // 开启雾效果
        ColorUtility.TryParseHtmlString("#041F45", out Color fogColor);
        RenderSettings.fogColor = fogColor; // 雾颜色
        RenderSettings.fogDensity = 0.002f; // 雾密度
    }

    public override void OnStateEnter(Fsm fsm) {
        m_fsm = (FsmLevel)fsm;
        m_level = m_fsm.level;
        
        App.instance.GetGame<Game>().SetCurrentLevel(m_level);
        //InitRenderSettings();


        m_fsm.ChangeStateTo(m_fsm.stateLevelRunning);
    }

    public override void OnStateUpdate(Fsm fsm) {
        base.OnStateUpdate(fsm);
    }

    public override void OnStateExit(Fsm fsm) {
        base.OnStateExit(fsm);
    }
}
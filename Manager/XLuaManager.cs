using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using XLua;
/// <summary>
/// 说明：xLua管理类
/// 注意：
/// 1、整个Lua虚拟机执行的脚本分成3个模块：热修复、公共模块、逻辑模块
/// 2、公共模块：提供Lua语言级别的工具类支持，和游戏逻辑无关，最先被启动
/// 3、热修复模块：脚本全部放Lua/XLua目录下，随着游戏的启动而启动
/// 4、逻辑模块：资源热更完毕后启动
/// 5、资源热更以后，理论上所有被加载的Lua脚本都要重新执行加载，如果热更某个模块被删除，则可能导致Lua加载异常，这里的方案是释放掉旧的虚拟器另起一个
/// </summary>
public class XLuaManager : MonoSingleton<XLuaManager>
{
    //lua环境全局变量
    private LuaEnv m_kGobalLuaEnv;
    public const string luaScriptsFolder = "LuaScripts";
    public const string mainConfigScriptName = "Common.MainConfig";
    public const string gamestartLuaScriptName = "GameMain";

    protected override void Init()
    {
        base.Init();
    }
    public override void Dispose()
    {
        if (m_kGobalLuaEnv!=null)
        {
            try {
                m_kGobalLuaEnv.Dispose();
                m_kGobalLuaEnv = null;

            }
            catch (System.Exception ex)
            {
                string msg = string.Format("xLua exception : {0}\n {1}", ex.Message, ex.StackTrace);
                Logger.LogError(msg, null);
            }
        }
    }


    public void OnInitLua()
    {
        m_kGobalLuaEnv = new LuaEnv();
        if (m_kGobalLuaEnv != null)
        {
            m_kGobalLuaEnv.AddLoader(CustomLoader);

        }

    }

    public void OnInitLuaConfig()
    {
        //记载unity的配置
        if(m_kGobalLuaEnv!=null)
        {
            LoadLuaScript(mainConfigScriptName);
        }
    }

    public void  GameStart()
    {
        //启动lua主接口
        if (m_kGobalLuaEnv != null)
        {
            LoadLuaScript(gamestartLuaScriptName);
            SafeDoString("GameMain.OnStart()");
        }
    }

    /// <summary>
    ///重启lua环境
    /// </summary>
    public void ReStart()
    {
        Dispose();
        OnInitLua();
    }

    private void OnApplicationQuit()
    {

    }

    /// <summary>
    /// 再次加载lua脚本
    /// </summary>
    /// <param name="scriptName"></param>
    public void ReloadLuaScript(string scriptName)
    {
        SafeDoString(string.Format("package.loaded['{0}'] = nil", scriptName));
        LoadLuaScript(scriptName);
    }

    /// <summary>
    /// 加载lua脚本
    /// </summary>
    /// <param name="scriptName"></param>
    public void LoadLuaScript(string scriptName)
    {
        SafeDoString(string.Format("require('{0}')", scriptName));
    }

    /// <summary>
    /// 安全加载lua的content
    /// </summary>
    /// <param name="scriptContent"></param>
    public void SafeDoString(string scriptContent)
    {
        if (m_kGobalLuaEnv != null)
        {
            try
            {
                m_kGobalLuaEnv.DoString(scriptContent);
            }
            catch (System.Exception ex)
            {
                string msg = string.Format("xLua exception : {0}\n {1}", ex.Message, ex.StackTrace);
                Logger.LogError(msg, null);
            }
        }
    }

    private static byte[] CustomLoader(ref string filepath)
    {
        StringBuilder scriptPath = new StringBuilder();
        scriptPath.Append(filepath.Replace(".", "/")).Append(".lua");

#if UNITY_EDITOR
        var scriptDir = Path.Combine(Application.dataPath, luaScriptsFolder);
        var luaPath = Path.Combine(scriptDir, scriptPath.ToString());
        // Logger.Log("Load lua script : " + luaPath);
        return GameUtil.SafeReadAllBytes(luaPath);
#endif

//         var luaAddress = scriptPath.Append(".bytes").ToString();
// 
//         var asset = AddressablesManager.Instance.GetLuaCache(luaAddress);
//         if (asset != null)
//         {
//             Logger.Log("Load lua script : " + scriptPath);
//             return asset.bytes;
//         }
//         Logger.LogError("Load lua script failed : " + scriptPath + ", You should preload lua assetbundle first!!!");
        return null;
    }
    private void Update()
    {
        if (m_kGobalLuaEnv != null)
        {
            m_kGobalLuaEnv.Tick();

            if (Time.frameCount % 1000 == 0)
            {
                m_kGobalLuaEnv.FullGc();
            }
        }
    }
}

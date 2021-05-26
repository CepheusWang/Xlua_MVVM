using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 游戏的启动
/// 负责启动环境、配置表、lua环境等等操作
/// 1.启动整个lua环境
/// 2.启动需要的lua的相关require
/// 3.加载需要的公共图集
/// 4.对资源进行检测等
/// </summary>
public class GameLancher : MonoBehaviour
{
    /// <summary>
    /// 游戏使用协程启动，需要等待相关资源加载进入游戏中
    /// 
    /// </summary>
    /// <returns></returns>
   IEnumerator  Start()
    {
        var start = DateTime.Now;
        //启动打印环境
        LoggerHelper.Instance.Startup();

        //启动lua环境//
        XLuaManager.Instance.Startup();
        XLuaManager.Instance.OnInitLua();

        //加载Lua配置///
        XLuaManager.Instance.OnInitLuaConfig();

        //启动游戏//
        XLuaManager.Instance.GameStart();
        yield return null;
    }
}

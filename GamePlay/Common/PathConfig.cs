using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 静态路径配置
/// </summary>
public static class PathConfig 
{
    public static string m_LuaScriptPath = "LuaScripts";
    public static string LuaFrameWorkConfigPath = "{0}/FrameWork/UI/{1}";

    public static string LuaUILogicConfigPath = "{0}/UILogic/{1}";

    public static string GetLuaFrameConfigPath(string luaScripts)
    {
        return string.Format(LuaFrameWorkConfigPath,m_LuaScriptPath,luaScripts);
    }
}

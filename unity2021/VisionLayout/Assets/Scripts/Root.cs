
using UnityEngine;

/// <summary>
/// 根程序类
/// </summary>
/// <remarks>
/// 不参与模块编译，仅用于在编辑器中开发调试
/// </remarks>
public class Root : RootBase
{
    private void Awake()
    {
        doAwake();
    }

    private void Start()
    {
        entry_.__DebugPreload(exportRoot);
    }

    private void OnDestroy()
    {
        doDestroy();
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 60, 30), "Create"))
        {
            entry_.__DebugCreate("test", "default");
        }

        if (GUI.Button(new Rect(0, 30, 60, 30), "Open"))
        {
            entry_.__DebugOpen("test", "file", "", 0.5f);
        }

        if (GUI.Button(new Rect(0, 60, 60, 30), "Show"))
        {
            entry_.__DebugShow("test", 0.5f);
        }

        if (GUI.Button(new Rect(0, 90, 60, 30), "Hide"))
        {
            entry_.__DebugHide("test", 0.5f);
        }

        if (GUI.Button(new Rect(0, 120, 60, 30), "Close"))
        {
            entry_.__DebugClose("test", 0.5f);
        }

        if (GUI.Button(new Rect(0, 150, 60, 30), "Delete"))
        {
            entry_.__DebugDelete("test");
        }

        if (GUI.Button(new Rect(0, 180, 160, 30), "Popup ToolBar"))
        {
            entry_.__DebugPopupToolBar("test");
        }

        if (GUI.Button(new Rect(0, 210, 160, 30), "Open DummyBoard"))
        {
            entry_.__DebugOpenDummyBoard("test", "001", Random.Range(-Screen.width / 2, Screen.width / 2), Random.Range(-Screen.height / 2, Screen.height / 2));
        }

        if (GUI.Button(new Rect(0, 240, 160, 30), "Close DummyBoard"))
        {
            entry_.__DebugCloseDummyBoard("test", "001");
        }
    }
}


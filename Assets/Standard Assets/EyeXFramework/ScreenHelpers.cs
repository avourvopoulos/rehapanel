//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Provides utility functions related to screen and window handling.
/// </summary>
internal abstract class EyeXScreenHelpers
{
    protected IntPtr _hwnd;

    public EyeXScreenHelpers()
    {
        _hwnd = FindWindowWithThreadProcessId(Process.GetCurrentProcess().Id);
        GameWindowId = _hwnd.ToString();
    }

    /// <summary>
    /// Gets the Window ID for the game window.
    /// </summary>
    public string GameWindowId { get; private set; }

    /// <summary>
    /// Gets the position of the viewport in desktop coordinates (physical pixels).
    /// </summary>
    /// <returns>Position in physical desktop pixels.</returns>
    public Rect GetViewportPhysicalBounds()
    {
        return LogicalToPhysical(GetViewportLogicalBounds());
    }

    /// <summary>
    /// Gets the position of the viewport in logical pixels.
    /// </summary>
    /// <returns>Position in logical pixels.</returns>
    protected abstract Rect GetViewportLogicalBounds();

    /// <summary>
    /// Maps from logical pixels to physical desktop pixels.
    /// </summary>
    /// <param name="rect">Rectangle to be transformed.</param>
    /// <returns>Transformed rectangle.</returns>
    protected virtual Rect LogicalToPhysical(Rect rect)
    {
        var topLeft = new Win32Helpers.POINT { x = (int)rect.x, y = (int)rect.y };
        Win32Helpers.LogicalToPhysicalPoint(_hwnd, ref topLeft);

        var bottomRight = new Win32Helpers.POINT { x = (int)(rect.x + rect.width), y = (int)(rect.y + rect.height) };
        Win32Helpers.LogicalToPhysicalPoint(_hwnd, ref bottomRight);

        return new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
    }

    private static IntPtr FindWindowWithThreadProcessId(int processId)
    {
        var window = new IntPtr();

        Win32Helpers.EnumWindows(delegate(IntPtr wnd, IntPtr param)
        {
            int windowProcessId = 0;
            Win32Helpers.GetWindowThreadProcessId(wnd, out windowProcessId);
            if (windowProcessId == processId)
            {
                window = wnd;
                return false;
            }

            return true;
        },
        IntPtr.Zero);

        if (window.Equals(IntPtr.Zero))
        {
            UnityEngine.Debug.LogError("Could not find any window with process id " + processId);
        }

        return window;
    }
}

/// <summary>
/// Provides utility functions related to screen and window handling within the Unity Player.
/// </summary>
internal class UnityPlayerScreenHelpers : EyeXScreenHelpers
{
    protected override Rect GetViewportLogicalBounds()
    {
        var clientRect = new Win32Helpers.RECT();
        Win32Helpers.GetClientRect(_hwnd, ref clientRect);

        var topLeft = new Win32Helpers.POINT();
        Win32Helpers.ClientToScreen(_hwnd, ref topLeft);

        var bottomRight = new Win32Helpers.POINT { x = clientRect.right, y = clientRect.bottom };
        Win32Helpers.ClientToScreen(_hwnd, ref bottomRight);

        return new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
    }
}

#if UNITY_EDITOR
/// <summary>
/// Provides utility functions related to screen and window handling within the Unity Editor.
/// </summary>
internal class EditorScreenHelpers : EyeXScreenHelpers
{
    private readonly UnityEditor.EditorWindow _gameWindow;

    public EditorScreenHelpers()
    {
        _gameWindow = GetMainGameView();
    }

    protected override Rect GetViewportLogicalBounds()
    {
        var gameWindowBounds = _gameWindow.position;

        // Adjust for the toolbar
        var toolbarHeight = 0f;
        try
        {
            toolbarHeight = UnityEditor.EditorStyles.toolbar.fixedHeight;
        }
        catch (NullReferenceException)
        {
            // never mind
        }
        gameWindowBounds.y += toolbarHeight;
        gameWindowBounds.height -= toolbarHeight;

        // Adjust for unused areas caused by fixed aspect ratio or resolution vs game window size mismatch
        var viewportOffsetX = (gameWindowBounds.width - Screen.width) / 2.0f;
        var viewportOffsetY = (gameWindowBounds.height - Screen.height) / 2.0f;

        return new Rect(
            gameWindowBounds.x + viewportOffsetX, 
            gameWindowBounds.y + viewportOffsetY,
            Screen.width,
            Screen.height);
    }

    private static UnityEditor.EditorWindow GetMainGameView()
    {
        var unityEditorType = Type.GetType("UnityEditor.GameView,UnityEditor");
        var getMainGameViewMethod = unityEditorType.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        var result = getMainGameViewMethod.Invoke(null, null);
        return (UnityEditor.EditorWindow)result;
    }
}
#endif

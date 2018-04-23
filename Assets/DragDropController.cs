using JetBrains.Annotations;
using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using System;
#endif

public delegate void DragEnterDelegate(string filePath, int x, int y);
public delegate void DragMoveDelegate(string filePath, int x, int y);
public delegate void DragExitDelegate(string filePath, int x, int y);
public delegate void FileDroppedDelegate(string filePath, int x, int y);
public delegate void FileDataDelegate(string fileName, int x, int y, byte[] fileData);

public class DragDropController : MonoBehaviour
{
    public FileDroppedDelegate OnDropped;
    public FileDataDelegate OnDroppedData;
    public DragEnterDelegate OnDragEnter;
    public DragMoveDelegate OnDragMove;
    public DragExitDelegate OnDragExit;

    private IDragDropHandler _dragDropHandler;
    private bool _hooked;

    [UsedImplicitly]
    private void Awake()
    {
        InitialisePlatform();
        if (_dragDropHandler != null)
        {
            _dragDropHandler.Bind(this);
        }
    }

    [UsedImplicitly]
    private void OnDestroy()
    {
        Unregister();
    }
    
    public bool Register()
    {
        if (_dragDropHandler != null)
        {
            if (!_hooked)
            {
                _hooked = true;
                _dragDropHandler.Hook();
                return true;
            }
        }

        return false;
    }

    public bool Unregister()
    {
        if (_dragDropHandler != null)
        {
            if (_hooked)
            {
                _hooked = false;
                _dragDropHandler.Unhook();
                return true;
            }
        }
        
        return false;
    }

    private void InitialisePlatform()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        _dragDropHandler = new DragDropHandlerWindows();
#elif UNITY_WEBGL
        _dragDropHandler = new DragDropHandlerWebGL();
#else
#error Drag & drop functionality is not supported for the target platform via this plugin.
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    public void WebGL_OnDragEnter(string data)
    {
        ((DragDropHandlerWebGL)_dragDropHandler).OnDragEnter(data);
    }

    public void WebGL_OnDragOver(string data)
    {
        ((DragDropHandlerWebGL)_dragDropHandler).OnDragOver(data);
    }

    public void WebGL_OnDragLeave(string data)
    {
        ((DragDropHandlerWebGL)_dragDropHandler).OnDragLeave(data);
    }

    public void WebGL_OnDropData(string data)
    {
        ((DragDropHandlerWebGL)_dragDropHandler).OnDropData(data);
    }
#endif
}
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
using System.Text;
using System;

public class DragDropHandlerWebGL : IDragDropHandler
{
    [DllImport("__Internal")]
    private static extern void RegisterDragDrop(string controllerName);

    [DllImport("__Internal")]
    private static extern void UnregisterDragDrop();

    [DllImport("__Internal")]
    private static extern IntPtr GetFileData(string fileName);

    [DllImport("__Internal")]
    private static extern int GetFileDataLength(string fileName);

    [DllImport("__Internal")]
    private static extern void FreeFileData(string fileName);

    private DragDropController _controller;

    public void Bind(DragDropController controller)
    {
        _controller = controller;
    }

    public void Hook()
    {
        RegisterDragDrop(_controller.name);
    }

    public void Unhook()
    {
        UnregisterDragDrop();
    }

    private struct WebGLData
    {
        public string FileName;
        public int X;
        public int Y;
        public byte[] Bytes;
    }

    private bool GetData(string data, ref WebGLData output)
    {
        string[] bits = data.Split('|');
        if (bits.Length != 3 && bits.Length != 4)
        {
            return false;
        }

        output.FileName = bits[0];
        if (!int.TryParse(bits[1], out output.X) || !int.TryParse(bits[2], out output.Y))
        {
            return false;
        }

        if (bits.Length == 4)
        {
            int length = GetFileDataLength(output.FileName);
            IntPtr ptr = GetFileData(output.FileName);
            output.Bytes = new byte[length];
            Marshal.Copy(ptr, output.Bytes, 0, length);
            FreeFileData(output.FileName);
        }

        return true;
    }

    public void OnDragEnter(string data)
    {
        if (_controller.OnDragEnter == null)
        {
            return;
        }
    
        WebGLData webData = new WebGLData();
        if (!GetData(data, ref webData))
        {
            return;
        }

        _controller.OnDragEnter(webData.FileName, webData.X, webData.Y);
    }

    public void OnDragOver(string data)
    {
        if (_controller.OnDragMove == null)
        {
            return;
        }
        
        WebGLData webData = new WebGLData();
        if (!GetData(data, ref webData))
        {
            return;
        }

        _controller.OnDragMove(webData.FileName, webData.X, webData.Y);
    }

    public void OnDragLeave(string data)
    {
        if (_controller.OnDragExit == null)
        {
            return;
        }

        WebGLData webData = new WebGLData();
        if (!GetData(data, ref webData))
        {
            return;
        }

        _controller.OnDragExit(webData.FileName, webData.X, webData.Y);
    }

    public void OnDropData(string data)
    {
        if (_controller.OnDroppedData == null)
        {
            return;
        }
        
        WebGLData webData = new WebGLData();
        if (!GetData(data, ref webData))
        {
            return;
        }

        _controller.OnDroppedData(webData.FileName, webData.X, webData.Y, webData.Bytes);
    }
}
#endif
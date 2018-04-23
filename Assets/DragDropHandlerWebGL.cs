#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
using System.Text;

public class DragDropHandlerWebGL : IDragDropHandler
{
    [DllImport("__Internal")]
    private static extern void RegisterDragDrop(string unityCanvasId, string controllerName);

    [DllImport("__Internal")]
    private static extern void UnregisterDragDrop(string unityCanvasId);

    private DragDropController _controller;
    private string _canvasId = "#canvas";

    public void Bind(DragDropController controller)
    {
        _controller = controller;
    }

    public void Hook()
    {
        RegisterDragDrop(_canvasId, _controller.name);
    }

    public void Unhook()
    {
        UnregisterDragDrop(_canvasId);
    }

    private struct WebGLData
    {
        public string FileName;
        public int X;
        public int Y;
        public string Blob;
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
            output.Blob = bits[3];
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

        _controller.OnDroppedData(webData.FileName, webData.X, webData.Y, Encoding.UTF8.GetBytes(webData.Blob));
    }
}
#endif
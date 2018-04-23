using JetBrains.Annotations;
using UnityEngine;

public class Example : MonoBehaviour
{
    private DragDropController _dragDropController;

    [UsedImplicitly]
	private void Awake()
    {
        _dragDropController = gameObject.AddComponent<DragDropController>();
        if (!_dragDropController.Register())
        {
            Debug.Log("Failed to register drag & drop.");
        }

        _dragDropController.OnDropped += OnDrop;
        _dragDropController.OnDragEnter += OnDragEnter;
        _dragDropController.OnDragMove += OnDragMove;
        _dragDropController.OnDragExit += OnDragExit;
        _dragDropController.OnDroppedData += OnDropData;
	}
	
	[UsedImplicitly
	private void OnDestroy()
	{
		if (_dragDropController == null)
		{
			return;
		}
		
		_dragDropController.OnDropped = null;
        _dragDropController.OnDragEnter = null;
        _dragDropController.OnDragMove = null;
        _dragDropController.OnDragExit = null;
        _dragDropController.OnDroppedData = null;
		_dragDropController = null;
	}

    private void OnDragEnter(string fileName, int x, int y)
    {
        Debug.Log("OnDragEnter - File name: " + fileName);
        Debug.Log("X: " + x + ", Y: " + y);
    }

    private void OnDrop(string fileName, int x, int y)
    {
        Debug.Log("OnDrop - File name: " + fileName);
        Debug.Log("X: " + x + ", Y: " + y);
    }

    private void OnDragMove(string fileName, int x, int y)
    {
        Debug.Log("OnDragMove - File name: " + fileName);
        Debug.Log("X: " + x + ", Y: " + y);
    }

    private void OnDragExit(string fileName, int x, int y)
    {
        Debug.Log("OnDragExit - File name: " + fileName);
        Debug.Log("X: " + x + ", Y: " + y);
    }

    private void OnDropData(string fileName, int x, int y, byte[] data)
    {
        Debug.Log("OnDropData - File name: " + fileName);
        Debug.Log("Data: " + System.Text.Encoding.UTF8.GetString(data));
    }
}

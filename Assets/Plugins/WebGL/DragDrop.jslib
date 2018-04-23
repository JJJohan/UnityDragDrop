mergeInto(LibraryManager.library, 
{
	RegisterDragDrop : function(canvasIdPtr, controllerNamePtr)
	{			
		var canvasId = Pointer_stringify(canvasIdPtr);
		var controller = Pointer_stringify(controllerNamePtr);
		
		var canvas = document.getElementById(canvasId);
		if (canvas !== undefined)
		{		
			function iterateFiles(ev, eventType, grabData)
			{
				var x = ev.clientX;
				var y = ev.clientY;
			
				if (ev.dataTransfer.items)
				{
					for (var i = 0; i < ev.dataTransfer.items.length; i++) 
					{
						if (ev.dataTransfer.items[i].kind === 'file') 
						{
							var file = ev.dataTransfer.items[i].getAsFile();
							sendFileEvent(file, eventType, grabData, x, y);
						}
					}
				} 
				else if (ev.dataTransfer.files)
				{
					for (var i = 0; i < ev.dataTransfer.files.length; i++)
					{
						var file = ev.dataTransfer.files[i];
						sendFileEvent(file, eventType, grabData, x, y);
					}
				} 
			}
			
			function sendFileEvent(file, eventType, grabData, x, y)
			{
				if (file === null || file === undefined)
				{
					return;
				}
			
				if (grabData)
				{
					var reader = new FileReader();
					reader.readAsDataURL(file);
					reader.onload = function() 
					{
						SendMessage(controller, 'WebGL_OnDropData', file.name + '|' + x + '|' + y + '|' + reader.result);
					};
				}
				else
				{
					SendMessage(controller, eventType, file.name + '|' + x + '|' + y);
				}
			}
		
			canvas.ondragover = function(e)
			{
				e.preventDefault();
				iterateFiles(e, 'WebGL_OnDragOver', false);
			}
			
			canvas.ondragenter = function(e)
			{
				iterateFiles(e, 'WebGL_OnDragEnter', false);
			}
			
			canvas.ondragleave = function(e)
			{
				iterateFiles(e, 'WebGL_OnDragLeave', false);
			}
		
			canvas.ondrop = function(e)
			{
				e.stopPropagation();
				e.preventDefault();
				iterateFiles(e, 'WebGL_OnDropData', true);
			}
		}
	},
  
	UnregisterDragDrop : function(canvasIdPtr)
	{
		var canvasId = Pointer_stringify(canvasIdPtr);
		
		var canvas = document.getElementById(canvasId);
		if (canvas !== undefined)
		{
			canvas.ondragstart = null;
			canvas.ondragover = null;
			canvas.ondrop = null;
		}
	}
});
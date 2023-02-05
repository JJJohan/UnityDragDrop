mergeInto(LibraryManager.library, 
{
	RegisterDragDrop : function(controllerNamePtr)
	{			
		var controller = UTF8ToString(controllerNamePtr);
		
		var canvas = GetCanvasElement();
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
					reader.onload = function() 
					{
						(window.filedata = window.filedata ? window.filedata : {})[file.name] = reader.result;
						SendMessage(controller, 'WebGL_OnDropData', file.name + '|' + x + '|' + y + '|true');
					};
					reader.readAsArrayBuffer(file);
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
  
	UnregisterDragDrop : function()
	{		
		var canvas = GetCanvasElement();
		if (canvas !== undefined)
		{
			canvas.ondragstart = null;
			canvas.ondragover = null;
			canvas.ondrop = null;
		}
	},
	
	GetFileData: function(fileNamePtr) 
	{
     var filename = UTF8ToString(fileNamePtr);
     var filedata = window.filedata[filename];
     var ptr = (window.fileptr = window.fileptr ? window.fileptr : {})[filename] = _malloc(filedata.byteLength);
     var dataHeap = new Uint8Array(HEAPU8.buffer, ptr, filedata.byteLength);
     dataHeap.set(new Uint8Array(filedata));
     return ptr;
   },
   
   GetFileDataLength: function(fileNamePtr) 
   {
     var filename = UTF8ToString(fileNamePtr);
     console.log("GetFileDataLength");
     console.log(filename);
     console.log(window);
     console.log(window.filedata);
     return window.filedata[filename].byteLength;
   },
   
   FreeFileData: function(fileNamePtr) 
   {
     var filename = UTF8ToString(fileNamePtr);
     _free(window.fileptr[filename]);
     delete window.fileptr[filename];
     delete window.filedata[filename];
   }
});
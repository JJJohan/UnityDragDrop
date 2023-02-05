function GetCanvasElement()
{
	// Get the canvas by the default canvas name. This may need to be changed for custom templates.
	var canvas = document.getElementById('unity-canvas');
	if (canvas !== undefined)
	{
		return canvas;
	}
	
	// As a fallback, return the first canvas in the DOM.
	var canvasElements = document.getElementsByClassName('canvas');
	return canvasElements[0];		
}
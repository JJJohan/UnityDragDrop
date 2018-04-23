# UnityDragDrop

Adds simple Drag & Drop support for Unity3D.
Supports Windows (both in the editor and standalone builds) and WebGL builds.

For Windows, Drag and drop is implemented using simple OLE interface, therefore extended functionality such as OnDragEnter, OnDragOver and OnDragExit events that rely on IDropTarget interface are unavailable. It is entirely possible to do this, but not without a dedicated native plugin that implementes IDropTarget. While I did get this working at some stage, sadly I did not back up the code for the native plugin before reinstalling Windows.

WebGL uses the browser's ondragenter, ondragover, ondragleave and ondrop events on the canvas which is much easier to handle. The actual file upload event is not handled in this repository as it was intended to be hooked to proprietary code. However I would recommend using the following method for such an implementation:

https://forum.unity.com/threads/solved-webgl-returning-a-byte-from-jslib-to-unity.334222/#post-2163749

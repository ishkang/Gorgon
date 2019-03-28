﻿
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA2213:'Form' contains field '_cube' that is of IDisposable type 'Cube', but it is never disposed. Change the Dispose method on 'Form' to call Close or Dispose on this field.", Justification = "<Pending>", Scope = "member", Target = "~F:Gorgon.Examples.Form._cube")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA2213:'Form' contains field '_graphics' that is of IDisposable type 'GorgonGraphics', but it is never disposed. Change the Dispose method on 'Form' to call Close or Dispose on this field.", Justification = "<Pending>", Scope = "member", Target = "~F:Gorgon.Examples.Form._graphics")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA2213:'Form' contains field '_inputLayout' that is of IDisposable type 'GorgonInputLayout', but it is never disposed. Change the Dispose method on 'Form' to call Close or Dispose on this field.", Justification = "<Pending>", Scope = "member", Target = "~F:Gorgon.Examples.Form._inputLayout")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA2213:'Form' contains field '_pixelShader' that is of IDisposable type 'GorgonPixelShader', but it is never disposed. Change the Dispose method on 'Form' to call Close or Dispose on this field.", Justification = "<Pending>", Scope = "member", Target = "~F:Gorgon.Examples.Form._pixelShader")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA2213:'Form' contains field '_swap' that is of IDisposable type 'GorgonSwapChain', but it is never disposed. Change the Dispose method on 'Form' to call Close or Dispose on this field.", Justification = "<Pending>", Scope = "member", Target = "~F:Gorgon.Examples.Form._swap")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA2213:'Form' contains field '_texture' that is of IDisposable type 'GorgonTexture2DView', but it is never disposed. Change the Dispose method on 'Form' to call Close or Dispose on this field.", Justification = "<Pending>", Scope = "member", Target = "~F:Gorgon.Examples.Form._texture")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA2213:'Form' contains field '_vertexShader' that is of IDisposable type 'GorgonVertexShader', but it is never disposed. Change the Dispose method on 'Form' to call Close or Dispose on this field.", Justification = "<Pending>", Scope = "member", Target = "~F:Gorgon.Examples.Form._vertexShader")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA2213:'Form' contains field '_wvpBuffer' that is of IDisposable type 'GorgonConstantBufferView', but it is never disposed. Change the Dispose method on 'Form' to call Close or Dispose on this field.", Justification = "<Pending>", Scope = "member", Target = "~F:Gorgon.Examples.Form._wvpBuffer")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA1063:Modify 'Cube.Dispose' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.", Justification = "<Pending>", Scope = "member", Target = "~M:Gorgon.Examples.Cube.Dispose")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA1816:Change Cube.Dispose() to call GC.SuppressFinalize(object). This will prevent derived types that introduce a finalizer from needing to re-implement 'IDisposable' to call it.", Justification = "<Pending>", Scope = "member", Target = "~M:Gorgon.Examples.Cube.Dispose")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA1031:Modify 'OnLoad' to catch a more specific exception type, or rethrow the exception.", Justification = "<Pending>", Scope = "member", Target = "~M:Gorgon.Examples.Form.OnLoad(System.EventArgs)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA1031:Modify 'Main' to catch a more specific exception type, or rethrow the exception.", Justification = "<Pending>", Scope = "member", Target = "~M:Gorgon.Examples.Program.Main")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA1063:Provide an overridable implementation of Dispose(bool) on 'Cube' or mark the type as sealed. A call to Dispose(false) should only clean up native resources. A call to Dispose(true) should clean up both managed and native resources.", Justification = "<Pending>", Scope = "type", Target = "~T:Gorgon.Examples.Cube")]
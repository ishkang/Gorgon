﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GorgonLibrary;
using GorgonLibrary.UI;
using GorgonLibrary.Diagnostics;
using GorgonLibrary.PlugIns;
using GorgonLibrary.Graphics;
using GorgonLibrary.HID;
using GorgonLibrary.FileSystem;

namespace Tester
{
	public partial class Form1 : Form
	{
		GorgonInputDeviceFactory input = null;
		GorgonPointingDevice mouse = null;
		GorgonKeyboard keyboard = null;
		GorgonFileSystem fileSystem = null;
		GorgonCustomHID joystick = null;

		private bool Idle(GorgonFrameRate timing)
		{
			labelMouse.Text = mouse.Position.X.ToString() + "x" + mouse.Position.Y.ToString();

			if ((joystick != null) && (joystick.Data.Count > 0))
			{
				byte[] data = joystick.Data[0].GetValue<byte[]>();
				
				labelMouse.Text += "\r\n| ";
				for (int i = 0; i < data.Length; i++)
					labelMouse.Text += data[i].ToString() + " | ";
			}

			if (keyboard.KeyStates[KeyboardKeys.A] == KeyState.Down)
				labelMouse.Text += "\r\nAAAAAAAAAAAAAAAAAAAAAAAAAAA!!";

			return true;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			try
			{
				Gorgon.Initialize(this);				
				GorgonPlugInFactory.LoadPlugInAssembly(@"..\..\..\..\PlugIns\Gorgon.HID.RawInput\bin\Debug\Gorgon.HID.RawInput.dll");
				GorgonPlugInFactory.LoadPlugInAssembly(@"..\..\..\..\PlugIns\Gorgon.FileSystem.Zip\bin\Debug\Gorgon.FileSystem.zip.dll");
				input = GorgonHIDFactory.CreateInputDeviceFactory("GorgonLibrary.HID.GorgonRawInput");
				mouse = input.CreatePointingDevice();
				keyboard = input.CreateKeyboard();

				fileSystem = new GorgonFileSystem();
				fileSystem.AddProvider("GorgonLibrary.FileSystem.GorgonZipFileSystemProvider");
				fileSystem.Mount(System.IO.Path.GetPathRoot(Application.ExecutablePath) + @"unpak\", "/FS");
				fileSystem.Mount(System.IO.Path.GetPathRoot(Application.ExecutablePath) + @"unpak\ParilTest.zip", "/Zip");

				System.IO.Stream stream = fileSystem.GetFile("/Zip/Grid.png").OpenStream(false);
				byte[] file = fileSystem.GetFile("/FS/ParilTest.zip").Read();

				joystick = input.CreateCustomHID(input.CustomHIDs[5].Name);
				
				Gorgon.Go(Idle);
			}
			catch (Exception ex)
			{
				GorgonException.Catch(ex, () => GorgonDialogs.ErrorBox(this, ex));				
			}
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);

			input.Dispose();
			Gorgon.Terminate();
		}

		public Form1()
		{
			InitializeComponent();
		}
	}
}

#region LGPL.
// 
// Gorgon.
// Copyright (C) 2007 Michael Winsor
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Created: Saturday, May 12, 2007 1:24:04 AM
// 
#endregion

namespace GorgonLibrary.Framework
{
	internal partial class DeviceInformationDialog
	{
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}

				_groupHeaderFont.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeviceInformationDialog));
			this.panel1 = new System.Windows.Forms.Panel();
			this.listCaps = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader("(none)");
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.labelCardName = new SharpUtilities.Controls.GradientLabel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.OK = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.listCaps);
			this.panel1.Controls.Add(this.labelCardName);
			this.panel1.Location = new System.Drawing.Point(-1, -1);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(484, 312);
			this.panel1.TabIndex = 11;
			// 
			// listCaps
			// 
			this.listCaps.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listCaps.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.listCaps.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listCaps.FullRowSelect = true;
			this.listCaps.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listCaps.Location = new System.Drawing.Point(0, 24);
			this.listCaps.MultiSelect = false;
			this.listCaps.Name = "listCaps";
			this.listCaps.OwnerDraw = true;
			this.listCaps.Size = new System.Drawing.Size(482, 286);
			this.listCaps.TabIndex = 11;
			this.listCaps.UseCompatibleStateImageBehavior = false;
			this.listCaps.View = System.Windows.Forms.View.Details;
			this.listCaps.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listCaps_DrawSubItem);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Capability";
			this.columnHeader1.Width = 353;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Value";
			this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.columnHeader2.Width = 96;
			// 
			// labelCardName
			// 
			this.labelCardName.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.labelCardName.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelCardName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelCardName.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.labelCardName.GradientAngle = 45F;
			this.labelCardName.Location = new System.Drawing.Point(0, 0);
			this.labelCardName.Name = "labelCardName";
			this.labelCardName.Size = new System.Drawing.Size(482, 24);
			this.labelCardName.TabIndex = 10;
			this.labelCardName.Text = "Bitchin\' Fast 3D 2000 capabilities";
			this.labelCardName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.OK);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 309);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(482, 37);
			this.panel2.TabIndex = 12;
			// 
			// OK
			// 
			this.OK.Cursor = System.Windows.Forms.Cursors.Hand;
			this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OK.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.OK.ForeColor = System.Drawing.Color.Black;
			this.OK.Image = global::GorgonLibrary.Properties.Resources.check;
			this.OK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.OK.Location = new System.Drawing.Point(413, 7);
			this.OK.Name = "OK";
			this.OK.Size = new System.Drawing.Size(57, 24);
			this.OK.TabIndex = 9;
			this.OK.Text = "&OK";
			this.OK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.OK.UseVisualStyleBackColor = true;
			// 
			// DeviceInformationDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(482, 346);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel2);
			this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(6)))));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DeviceInformationDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Device Info";
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ListView listCaps;
		private SharpUtilities.Controls.GradientLabel labelCardName;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button OK;


	}
}

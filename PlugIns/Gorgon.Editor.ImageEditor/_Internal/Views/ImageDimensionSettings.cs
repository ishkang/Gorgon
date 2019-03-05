﻿#region MIT
// 
// Gorgon.
// Copyright (C) 2019 Michael Winsor
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// Created: January 15, 2019 9:04:41 PM
// 
#endregion

using System;
using System.ComponentModel;
using Gorgon.Editor.UI;
using Gorgon.Editor.UI.Views;
using Gorgon.Editor.ImageEditor.Properties;
using Gorgon.Editor.ImageEditor.ViewModels;
using Gorgon.Graphics.Imaging;
using System.Windows.Forms;

namespace Gorgon.Editor.ImageEditor
{
    /// <summary>
    /// The panel used to provide settings for image import resizing.
    /// </summary>
    internal partial class ImageDimensionSettings 
        : EditorBaseControl, IDataContext<IDimensionSettings>
    {
        #region Properties.
        /// <summary>Property to return the data context assigned to this view.</summary>
        /// <value>The data context.</value>
        [Browsable(false)]
        public IDimensionSettings DataContext
        {
            get;
            private set;
        }
        #endregion

        #region Methods.
        /// <summary>
        /// Function to validate the state of the controls in the view.
        /// </summary>
        private void ValidateControls()
        {
            ButtonOK.Enabled = (DataContext?.OkCommand != null) && (DataContext.OkCommand.CanExecute(null));

            if (DataContext == null)
            {
                RadioCrop.Enabled = RadioResize.Enabled = LabelImageFilter.Enabled =
                    ComboImageFilter.Enabled = LabelAnchor.Enabled = false;
                return;
            }
            
            LabelAnchor.Enabled = AlignmentPicker.Enabled = RadioCrop.Checked;                        
            LabelImageFilter.Enabled = ComboImageFilter.Enabled = RadioResize.Checked;
        }

        /// <summary>
        /// Function to update whether mip maps are supported or not.
        /// </summary>
        /// <param name="dataContext">The current data context.</param>
        private void UpdateMipSupport(IDimensionSettings dataContext)
        {
            if (dataContext == null)
            {
                LabelMipLevels.Visible = NumericMipLevels.Visible = true;
                LabelMipLevels.Enabled = NumericMipLevels.Enabled = false;
                return;
            }

            LabelMipLevels.Enabled = NumericMipLevels.Enabled = 
            LabelMipLevels.Visible = NumericMipLevels.Visible = dataContext.MipSupport;
        }

        /// <summary>
        /// Function to update the label(s) on the view.
        /// </summary>
        /// <param name="dataContext">The current data context.</param>
        private void UpdateLabels(IDimensionSettings dataContext)
        {
            if (dataContext == null)
            {
                // Default this to a hard coded message so we know that we messed up.
                LabelDepthOrArray.Text = @"No image";
                return;
            }

            LabelDepthOrArray.Text = dataContext.HasDepth ? Resources.GORIMG_TEXT_DEPTH_SLICE_COUNT : Resources.GORIMG_TEXT_ARRAY_INDEX_COUNT;
        }

        /// <summary>Handles the SelectedValueChanged event of the ComboImageFilter control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ComboImageFilter_SelectedValueChanged(object sender, EventArgs e)
        {
            if (DataContext == null)
            {
                return;
            }

            DataContext.ImageFilter = (ImageFilter)ComboImageFilter.SelectedItem;
        }

        /// <summary>Handles the Click event of the RadioCrop control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void RadioCrop_Click(object sender, EventArgs e)
        {
            if (DataContext == null)
            {
                return;
            }

            DataContext.CurrentMode = CropResizeMode.Crop;

            ValidateControls();
        }

        /// <summary>Handles the Click event of the RadioResize control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void RadioResize_Click(object sender, EventArgs e)
        {
            if (DataContext == null)
            {
                return;
            }

            DataContext.CurrentMode = CropResizeMode.Resize;

            ValidateControls();
        }

        /// <summary>Handles the PropertyChanged event of the DataContext control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void DataContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IDimensionSettings.CurrentMode):
                    RadioCrop.Checked = (DataContext.CurrentMode == CropResizeMode.Crop) 
                                    || (DataContext.CurrentMode == CropResizeMode.None);
                    RadioResize.Checked = !RadioCrop.Checked;
                    break;
                case nameof(IDimensionSettings.HasDepth):
                    UpdateLabels(DataContext);
                    break;
                case nameof(IDimensionSettings.MipSupport):
                    UpdateMipSupport(DataContext);
                    break;
                case nameof(IDimensionSettings.Width):
                case nameof(IDimensionSettings.MaxWidth):
                    UpdateNumericUpDown(NumericWidth, DataContext.MaxWidth, DataContext.Width);
                    break;
                case nameof(IDimensionSettings.Height):
                case nameof(IDimensionSettings.MaxHeight):
                    UpdateNumericUpDown(NumericHeight, DataContext.MaxHeight, DataContext.Height);
                    break;
                case nameof(IDimensionSettings.MipLevels):
                case nameof(IDimensionSettings.MaxMipLevels):
                    UpdateNumericUpDown(NumericMipLevels, DataContext.MaxMipLevels, DataContext.MipLevels);
                    break;
                case nameof(IDimensionSettings.DepthSlicesOrArrayIndices):
                case nameof(IDimensionSettings.MaxDepthOrArrayIndices):
                    UpdateNumericUpDown(NumericDepthOrArray, DataContext.MaxDepthOrArrayIndices, DataContext.DepthSlicesOrArrayIndices);
                    break;
            }

            ValidateControls();
        }


        /// <summary>Handles the Click event of the ButtonCancel control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            if ((DataContext?.CancelCommand == null) || (!DataContext.CancelCommand.CanExecute(null)))
            {
                return;
            }

            DataContext.CancelCommand.Execute(null);
        }


        /// <summary>Handles the Click event of the ButtonOK control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ButtonOK_Click(object sender, EventArgs e)
        {
            if ((DataContext?.OkCommand == null) || (!DataContext.OkCommand.CanExecute(null)))
            {
                return;
            }

            DataContext.OkCommand.Execute(null);
        }

        /// <summary>Handles the AlignmentChanged event of the AlignmentPicker control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void AlignmentPicker_AlignmentChanged(object sender, EventArgs e)
        {
            if (DataContext == null)
            {
                return;
            }

            DataContext.CropAlignment = AlignmentPicker.Alignment;
        }

        /// <summary>
        /// Function to update a numeric up/down control.
        /// </summary>
        /// <param name="control">The control to update.</param>
        /// <param name="maxValue">The maximum value for the control.</param>
        /// <param name="currentValue">The current value for the control.</param>
        /// <param name="steps">[Optional] The number of steps used to increment the value.</param>
        private void UpdateNumericUpDown(NumericUpDown control, int maxValue, int currentValue, int steps = 1)
        {
            control.Maximum = maxValue;
            control.Value = currentValue;
            control.Increment = steps;
        }


        /// <summary>Handles the ValueChanged event of the NumericWidth control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void NumericWidth_ValueChanged(object sender, EventArgs e)
        {
            if (DataContext == null)
            {
                return;
            }

            DataContext.Width = (int)NumericWidth.Value;
        }

        /// <summary>Handles the ValueChanged event of the NumericHeight control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void NumericHeight_ValueChanged(object sender, EventArgs e)
        {
            if (DataContext == null)
            {
                return;
            }

            DataContext.Height = (int)NumericHeight.Value;
        }

        /// <summary>Handles the ValueChanged event of the NumericDepthOrArray control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void NumericDepthOrArray_ValueChanged(object sender, EventArgs e)
        {
            if (DataContext == null)
            {
                return;
            }

            DataContext.DepthSlicesOrArrayIndices = (int)NumericDepthOrArray.Value;
        }

        /// <summary>Handles the ValueChanged event of the NumericMipLevels control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void NumericMipLevels_ValueChanged(object sender, EventArgs e)
        {
            if (DataContext == null)
            {
                return;
            }

            DataContext.MipLevels = (int)NumericMipLevels.Value;
        }

        /// <summary>
        /// Function to unassign the events assigned to the datacontext.
        /// </summary>
        private void UnassignEvents()
        {
            if (DataContext == null)
            {
                return;
            }

            DataContext.PropertyChanged -= DataContext_PropertyChanged;
        }


        /// <summary>
        /// Function called when the view should be reset by a <b>null</b> data context.
        /// </summary>
        private void ResetDataContext()
        {
            RadioCrop.Enabled = RadioResize.Enabled = RadioCrop.Checked = RadioResize.Checked = false;
            AlignmentPicker.Alignment = Gorgon.UI.Alignment.Center;
            AlignmentPicker.Enabled = false;            
            ComboImageFilter.Text = string.Empty;
            ComboImageFilter.Items.Clear();
            UpdateLabels(null);
            UpdateMipSupport(null);
        }

        /// <summary>
        /// Function to initialize the view from the current data context.
        /// </summary>
        /// <param name="dataContext">The data context being assigned.</param>
        private void InitializeFromDataContext(IDimensionSettings dataContext)
        {
            if (dataContext == null)
            {
                ResetDataContext();
                return;
            }

            RadioResize.Enabled = RadioCrop.Enabled = true;
            
            switch (dataContext.CurrentMode)
            {
                case CropResizeMode.Resize:
                    RadioResize.Checked = true;
                    break;
                default:
                    RadioCrop.Checked = true;
                    break;
            }
            
            AlignmentPicker.Alignment = dataContext.CropAlignment;
            ComboImageFilter.SelectedItem = dataContext.ImageFilter;
            UpdateLabels(dataContext);
            UpdateMipSupport(dataContext);
            UpdateNumericUpDown(NumericWidth, dataContext.MaxWidth, dataContext.Width);
            UpdateNumericUpDown(NumericHeight, dataContext.MaxHeight, dataContext.Height);
            UpdateNumericUpDown(NumericMipLevels, dataContext.MaxMipLevels, dataContext.MipLevels);
            UpdateNumericUpDown(NumericDepthOrArray, dataContext.MaxDepthOrArrayIndices, dataContext.DepthSlicesOrArrayIndices);
        }

        /// <summary>Raises the <see cref="E:System.Windows.Forms.UserControl.Load"/> event.</summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsDesignTime)
            {
                return;
            }

            ValidateControls();
        }

        /// <summary>Function to assign a data context to the view as a view model.</summary>
        /// <param name="dataContext">The data context to assign.</param>
        /// <remarks>Data contexts should be nullable, in that, they should reset the view back to its original state when the context is null.</remarks>
        public void SetDataContext(IDimensionSettings dataContext)
        {
            UnassignEvents();

            InitializeFromDataContext(dataContext);
            DataContext = dataContext;

            if (DataContext == null)
            {
                return;
            }

            DataContext.PropertyChanged += DataContext_PropertyChanged;
        }
        #endregion

        #region Constructor/Finalizer.
        /// <summary>Initializes a new instance of the <see cref="T:Gorgon.Editor.Views.ImageDimensionSettings"/> class.</summary>
        public ImageDimensionSettings()
        {
            InitializeComponent();

            // Populate the image filter drop down.
            var filters = (ImageFilter[])Enum.GetValues(typeof(ImageFilter));

            foreach (ImageFilter filter in filters)
            {
                ComboImageFilter.Items.Add(filter);
            }
        }
        #endregion
    }
}
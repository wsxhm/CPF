using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using CPF.Input;
using System.Diagnostics;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 表示重新分布 Grid 控件的列间距或行间距的控件。
    /// </summary>
    [Description("表示重新分布 Grid 控件的列间距或行间距的控件。")]
    public class GridSplitter : Thumb
    {
        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Width), new UIPropertyMetadataAttribute(typeof(FloatField), "5", UIPropertyOptions.AffectsMeasure));
            overridePropertys.Override(nameof(Height), new UIPropertyMetadataAttribute(typeof(FloatField), "100", UIPropertyOptions.AffectsMeasure));
        }
        public GridResizeDirection ResizeDirection
        {
            get { return (GridResizeDirection)GetValue(); }
            set { SetValue(value); }
        }

        protected override object OnGetDefaultValue(PropertyMetadataAttribute pm)
        {
            if (pm.PropertyName == nameof(Cursor))
            {
                switch (GetEffectiveResizeDirection())
                {
                    case GridResizeDirection.Columns:
                        return (Cursor)Cursors.SizeWestEast;
                    case GridResizeDirection.Rows:
                        return (Cursor)Cursors.SizeNorthSouth;
                }
            }
            return base.OnGetDefaultValue(pm);
        }

        public GridResizeBehavior ResizeBehavior
        {
            get { return (GridResizeBehavior)GetValue(); }
            set { SetValue(value); }
        }
        public bool ShowsPreview
        {
            get { return (bool)GetValue(); }
            set { SetValue(value); }
        }

        public float KeyboardIncrement
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }
        [PropertyMetadata(1f)]
        public float DragIncrement
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 预览拖拽的颜色
        /// </summary>
        [PropertyMetadata(typeof(Color), "10,10,10,50")]
        public Color PreviewColor
        {
            get { return (Color)GetValue(); }
            set { SetValue(value); }
        }

        private ResizeData _resizeData;
        // Initialize the data needed for resizing
        private void InitializeData(bool ShowsPreview)
        {
            Grid grid = Parent as Grid;

            // If not in a grid or can't resize, do nothing
            if (grid != null)
            {
                // Setup data used for resizing
                _resizeData = new ResizeData();
                _resizeData.Grid = grid;
                _resizeData.ShowsPreview = ShowsPreview;
                _resizeData.ResizeDirection = GetEffectiveResizeDirection();
                _resizeData.ResizeBehavior = GetEffectiveResizeBehavior(_resizeData.ResizeDirection);
                _resizeData.SplitterLength = Math.Min(ActualSize.Width, ActualSize.Height);

                // Store the rows and columns to resize on drag events
                if (!SetupDefinitionsToResize())
                {
                    // Unable to resize, clear data
                    _resizeData = null;
                    return;
                }

                // Setup the preview in the adorner if ShowsPreview is true
                SetupPreview();
            }
        }
        private GridResizeDirection GetEffectiveResizeDirection()
        {
            GridResizeDirection direction = ResizeDirection;

            if (direction == GridResizeDirection.Auto)
            {
                // When HorizontalAlignment is Left, Right or Center, resize Columns
                if (MarginLeft.IsAuto || MarginRight.IsAuto)
                {
                    direction = GridResizeDirection.Columns;
                }
                else if (MarginTop.IsAuto || MarginBottom.IsAuto)
                {
                    direction = GridResizeDirection.Rows;
                }
                else if (ActualSize.Width <= ActualSize.Height)// Fall back to Width vs Height
                {
                    direction = GridResizeDirection.Columns;
                }
                else
                {
                    direction = GridResizeDirection.Rows;
                }

            }
            return direction;
        }

        // Convert BasedOnAlignment to Next/Prev/Both depending on alignment and Direction
        private GridResizeBehavior GetEffectiveResizeBehavior(GridResizeDirection direction)
        {
            GridResizeBehavior resizeBehavior = ResizeBehavior;

            if (resizeBehavior == GridResizeBehavior.BasedOnAlignment)
            {
                if (direction == GridResizeDirection.Columns)
                {
                    if (!MarginLeft.IsAuto)
                    {
                        resizeBehavior = GridResizeBehavior.PreviousAndCurrent;
                    }
                    else if (!MarginRight.IsAuto)
                    {
                        resizeBehavior = GridResizeBehavior.CurrentAndNext;
                    }
                    else
                    {
                        resizeBehavior = GridResizeBehavior.PreviousAndNext;
                    }
                }
                else
                {
                    if (!MarginTop.IsAuto)
                    {
                        resizeBehavior = GridResizeBehavior.PreviousAndCurrent;
                    }
                    else if (!MarginBottom.IsAuto)
                    {
                        resizeBehavior = GridResizeBehavior.CurrentAndNext;
                    }
                    else
                    {
                        resizeBehavior = GridResizeBehavior.PreviousAndNext;
                    }
                }
            }
            return resizeBehavior;
        }
        // Returns true if GridSplitter can resize rows/columns
        private bool SetupDefinitionsToResize()
        {
            int splitterIndex, index1, index2;

            int gridSpan = _resizeData.ResizeDirection == GridResizeDirection.Columns ? Grid.ColumnSpan(this) : Grid.RowSpan(this);

            if (gridSpan == 1)
            {
                splitterIndex = _resizeData.ResizeDirection == GridResizeDirection.Columns ? Grid.ColumnIndex(this) : Grid.RowIndex(this);

                // Select the columns based on Behavior
                switch (_resizeData.ResizeBehavior)
                {
                    case GridResizeBehavior.PreviousAndCurrent:
                        // get current and previous
                        index1 = splitterIndex - 1;
                        index2 = splitterIndex;
                        break;
                    case GridResizeBehavior.CurrentAndNext:
                        // get current and next
                        index1 = splitterIndex;
                        index2 = splitterIndex + 1;
                        break;
                    default: // GridResizeBehavior.PreviousAndNext
                        // get previous and next
                        index1 = splitterIndex - 1;
                        index2 = splitterIndex + 1;
                        break;
                }

                // Get # of rows/columns in the resize direction
                int count = (_resizeData.ResizeDirection == GridResizeDirection.Columns) ? _resizeData.Grid.ColumnDefinitions.Count : _resizeData.Grid.RowDefinitions.Count;

                if (index1 >= 0 && index2 < count)
                {
                    _resizeData.SplitterIndex = splitterIndex;

                    _resizeData.Definition1Index = index1;
                    _resizeData.Definition1 = GetGridDefinition(_resizeData.Grid, index1, _resizeData.ResizeDirection);
                    _resizeData.OriginalDefinition1Length = _resizeData.Definition1.UserSizeValueCache;  //save Size if user cancels
                    _resizeData.OriginalDefinition1ActualLength = GetActualLength(_resizeData.Definition1);

                    _resizeData.Definition2Index = index2;
                    _resizeData.Definition2 = GetGridDefinition(_resizeData.Grid, index2, _resizeData.ResizeDirection);
                    _resizeData.OriginalDefinition2Length = _resizeData.Definition2.UserSizeValueCache;  //save Size if user cancels
                    _resizeData.OriginalDefinition2ActualLength = GetActualLength(_resizeData.Definition2);

                    // Determine how to resize the columns 
                    bool isStar1 = IsStar(_resizeData.Definition1);
                    bool isStar2 = IsStar(_resizeData.Definition2);
                    if (isStar1 && isStar2)
                    {
                        // If they are both stars, resize both
                        _resizeData.SplitBehavior = SplitBehavior.Split;
                    }
                    else
                    {
                        // One column is fixed width, resize the first one that is fixed
                        _resizeData.SplitBehavior = !isStar1 ? SplitBehavior.Resize1 : SplitBehavior.Resize2;
                    }

                    return true;
                }
            }
            return false;
        }
        private void RemovePreviewAdorner()
        {
            // Remove the preview grid from the adorner
            if (_resizeData.Adorner != null)
            {
                //AdornerLayer layer = VisualTreeHelper.GetParent(_resizeData.Adorner) as AdornerLayer;
                //layer.Remove(_resizeData.Adorner);
                Root.Children.Remove(_resizeData.Adorner);
                _resizeData.Adorner.Dispose();
                _resizeData.Adorner = null;
            }
        }
        // Create the Preview adorner and add it to the adorner layer
        private void SetupPreview()
        {
            if (_resizeData.ShowsPreview)
            {
                // Get the adorner layer and add an adorner to it
                //AdornerLayer adornerlayer = AdornerLayer.GetAdornerLayer(_resizeData.Grid);

                //// Can't display preview
                //if (adornerlayer == null)
                //{
                //    return;
                //}

                _resizeData.Adorner = new PreviewAdorner(this);
                var b = _resizeData.Adorner[nameof(Color)] <= this[nameof(PreviewColor)];
                _resizeData.Adorner.Width = ActualSize.Width;
                _resizeData.Adorner.Height = ActualSize.Height;
                _resizeData.Adorner.IsHitTestVisible = false;
                //adornerlayer.Add(_resizeData.Adorner);
                Root.Children.Add(_resizeData.Adorner);

                // Get constraints on preview's translation
                GetDeltaConstraints(out _resizeData.MinChange, out _resizeData.MaxChange);
            }
        }


        /// <summary>
        ///     An event announcing that the splitter is no longer focused
        /// </summary>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (_resizeData != null)
            {
                CancelResize();
            }
        }

        //private static void OnDragStarted(object sender, DragStartedEventArgs e)
        //{
        //    GridSplitter splitter = sender as GridSplitter;
        //    splitter.OnDragStarted(e);
        //}

        // Thumb Mouse Down
        protected override void OnDragStarted(DragStartedEventArgs e)
        {
            //Debug.Assert(_resizeData == null, "_resizeData is not null, DragCompleted was not called");

            InitializeData(ShowsPreview);
        }

        // Thumb dragged
        protected override void OnDragDelta(DragDeltaEventArgs e)
        {
            if (_resizeData != null)
            {
                float horizontalChange = e.HorizontalChange;
                float verticalChange = e.VerticalChange;

                // Round change to nearest multiple of DragIncrement
                float dragIncrement = DragIncrement;
                horizontalChange = (float)Math.Round(horizontalChange / dragIncrement) * dragIncrement;
                verticalChange = (float)Math.Round(verticalChange / dragIncrement) * dragIncrement;

                if (_resizeData.ShowsPreview)
                {
                    //Set the Translation of the Adorner to the distance from the thumb
                    if (_resizeData.ResizeDirection == GridResizeDirection.Columns)
                    {
                        _resizeData.Adorner.OffsetX = Math.Min(Math.Max(horizontalChange, _resizeData.MinChange), _resizeData.MaxChange);
                    }
                    else
                    {
                        _resizeData.Adorner.OffsetY = Math.Min(Math.Max(verticalChange, _resizeData.MinChange), _resizeData.MaxChange);
                    }
                }
                else
                {
                    // Directly update the grid
                    MoveSplitter(horizontalChange, verticalChange);
                }
            }
        }

        //private static void OnDragCompleted(object sender, DragCompletedEventArgs e)
        //{
        //    GridSplitter splitter = sender as GridSplitter;
        //    splitter.OnDragCompleted(e);
        //}

        // Thumb dragging finished
        protected override void OnDragCompleted(DragCompletedEventArgs e)
        {
            if (_resizeData != null)
            {
                if (_resizeData.ShowsPreview)
                {
                    // Update the grid
                    MoveSplitter(_resizeData.Adorner.OffsetX, _resizeData.Adorner.OffsetY);
                    RemovePreviewAdorner();
                }

                _resizeData = null;
            }
        }

        /// <summary>
        ///     This is the method that responds to the KeyDown event.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            Keys key = e.Key;
            switch (key)
            {
                case Keys.Escape:
                    if (_resizeData != null)
                    {
                        CancelResize();
                        e.Handled = true;
                    }
                    break;

                case Keys.Left:
                    e.Handled = KeyboardMoveSplitter(-KeyboardIncrement, 0);
                    break;
                case Keys.Right:
                    e.Handled = KeyboardMoveSplitter(KeyboardIncrement, 0);
                    break;
                case Keys.Up:
                    e.Handled = KeyboardMoveSplitter(0, -KeyboardIncrement);
                    break;
                case Keys.Down:
                    e.Handled = KeyboardMoveSplitter(0, KeyboardIncrement);
                    break;
            }
        }

        // Cancels the Resize when the user hits Escape
        private void CancelResize()
        {
            // Restore original column/row lengths
            Grid grid = Parent as Grid;

            if (_resizeData.ShowsPreview)
            {
                RemovePreviewAdorner();
            }
            else // Reset the columns'/rows' lengths to the saved values 
            {
                SetDefinitionLength(_resizeData.Definition1, _resizeData.OriginalDefinition1Length);
                SetDefinitionLength(_resizeData.Definition2, _resizeData.OriginalDefinition2Length);
            }

            _resizeData = null;
        }
        #region Row/Column Abstractions 
        // These methods are to help abstract dealing with rows and columns.  
        // DefinitionBase already has internal helpers for getting Width/Height, MinWidth/MinHeight, and MaxWidth/MaxHeight

        // Returns true if the row/column has a Star length
        private static bool IsStar(DefinitionBase definition)
        {
            return definition.UserSizeValueCache.IsStar;
        }

        // Gets Column or Row definition at index from grid based on resize direction
        private static DefinitionBase GetGridDefinition(Grid grid, int index, GridResizeDirection direction)
        {
            return direction == GridResizeDirection.Columns ? (DefinitionBase)grid.ColumnDefinitions[index] : (DefinitionBase)grid.RowDefinitions[index];
        }

        // Retrieves the ActualWidth or ActualHeight of the definition depending on its type Column or Row
        private float GetActualLength(DefinitionBase definition)
        {
            ColumnDefinition column = definition as ColumnDefinition;

            return column == null ? ((RowDefinition)definition).ActualHeight : column.ActualWidth;
        }

        // Gets Column or Row definition at index from grid based on resize direction
        private static void SetDefinitionLength(DefinitionBase definition, GridLength length)
        {
            definition.SetValue(length, definition is ColumnDefinition ? nameof(ColumnDefinition.Width) : nameof(RowDefinition.Height));
        }

        #endregion

        // Get the minimum and maximum Delta can be given definition constraints (MinWidth/MaxWidth)
        private void GetDeltaConstraints(out float minDelta, out float maxDelta)
        {
            float definition1Len = GetActualLength(_resizeData.Definition1);
            float definition1Min = GetMinValue(_resizeData.Definition1);
            float definition1Max = GetMaxValue(_resizeData.Definition1);

            float definition2Len = GetActualLength(_resizeData.Definition2);
            float definition2Min = GetMinValue(_resizeData.Definition2);
            float definition2Max = GetMaxValue(_resizeData.Definition2);

            //Set MinWidths to be greater than width of splitter
            if (_resizeData.SplitterIndex == _resizeData.Definition1Index)
            {
                definition1Min = Math.Max(definition1Min, _resizeData.SplitterLength);
            }
            else if (_resizeData.SplitterIndex == _resizeData.Definition2Index)
            {
                definition2Min = Math.Max(definition2Min, _resizeData.SplitterLength);
            }

            if (_resizeData.SplitBehavior == SplitBehavior.Split)
            {
                // Determine the minimum and maximum the columns can be resized
                minDelta = -Math.Min(definition1Len - definition1Min, definition2Max - definition2Len);
                maxDelta = Math.Min(definition1Max - definition1Len, definition2Len - definition2Min);
            }
            else if (_resizeData.SplitBehavior == SplitBehavior.Resize1)
            {
                minDelta = definition1Min - definition1Len;
                maxDelta = definition1Max - definition1Len;
            }
            else
            {
                minDelta = definition2Len - definition2Max;
                maxDelta = definition2Len - definition2Min;
            }
        }

        float GetMaxValue(DefinitionBase definitionBase)
        {
            if (definitionBase is ColumnDefinition column)
            {
                return column.MaxWidth.IsAuto ? float.PositiveInfinity : column.MaxWidth.GetActualValue(Parent.ActualSize.Width);
            }
            return (definitionBase as RowDefinition).MaxHeight.IsAuto ? float.PositiveInfinity : (definitionBase as RowDefinition).MaxHeight.GetActualValue(Parent.ActualSize.Height);
        }
        float GetMinValue(DefinitionBase definitionBase)
        {
            if (definitionBase is ColumnDefinition column)
            {
                return column.MinWidth.GetActualValue(Parent.ActualSize.Width);
            }
            return (definitionBase as RowDefinition).MinHeight.GetActualValue(Parent.ActualSize.Height);
        }

        //Sets the length of definition1 and definition2 
        private void SetLengths(float definition1Pixels, float definition2Pixels)
        {
            // For the case where both definition1 and 2 are stars, update all star values to match their current pixel values
            if (_resizeData.SplitBehavior == SplitBehavior.Split)
            {
                IEnumerable definitions = _resizeData.ResizeDirection == GridResizeDirection.Columns ? (IEnumerable)_resizeData.Grid.ColumnDefinitions : (IEnumerable)_resizeData.Grid.RowDefinitions;

                int i = 0;
                foreach (DefinitionBase definition in definitions)
                {
                    // For each definition, if it is a star, set is value to ActualLength in stars
                    // This makes 1 star == 1 pixel in length
                    if (i == _resizeData.Definition1Index)
                    {
                        SetDefinitionLength(definition, new GridLength(definition1Pixels, GridUnitType.Star));
                    }
                    else if (i == _resizeData.Definition2Index)
                    {
                        SetDefinitionLength(definition, new GridLength(definition2Pixels, GridUnitType.Star));
                    }
                    else if (IsStar(definition))
                    {
                        SetDefinitionLength(definition, new GridLength(GetActualLength(definition), GridUnitType.Star));
                    }

                    i++;
                }
            }
            else if (_resizeData.SplitBehavior == SplitBehavior.Resize1)
            {
                SetDefinitionLength(_resizeData.Definition1, new GridLength(definition1Pixels));
            }
            else
            {
                SetDefinitionLength(_resizeData.Definition2, new GridLength(definition2Pixels));
            }
        }

        // Move the splitter by the given Delta's in the horizontal and vertical directions
        private void MoveSplitter(float horizontalChange, float verticalChange)
        {
            Debug.Assert(_resizeData != null, "_resizeData should not be null when calling MoveSplitter");

            float delta;
            var dpi = Root.LayoutScaling;

            // Calculate the offset to adjust the splitter.  If layout rounding is enabled, we
            // need to round to an integer physical pixel value to avoid round-ups of children that
            // expand the bounds of the Grid.  In practice this only happens in high dpi because
            // horizontal/vertical offsets here are never fractional (they correspond to mouse movement
            // across logical pixels).  Rounding error only creeps in when converting to a physical
            // display with something other than the logical 96 dpi.
            if (_resizeData.ResizeDirection == GridResizeDirection.Columns)
            {
                delta = horizontalChange;
                if (this.UseLayoutRounding)
                {
                    delta = UIElement.RoundLayoutValue(delta, dpi);
                }
            }
            else
            {
                delta = verticalChange;
                if (this.UseLayoutRounding)
                {
                    delta = UIElement.RoundLayoutValue(delta, dpi);
                }
            }

            var definition1 = _resizeData.Definition1;
            var definition2 = _resizeData.Definition2;
            if (definition1 != null && definition2 != null)
            {
                var actualLength1 = GetActualLength(definition1);
                var actualLength2 = GetActualLength(definition2);

                // When splitting, Check to see if the total pixels spanned by the definitions 
                // is the same asbefore starting resize. If not cancel the drag
                if (_resizeData.SplitBehavior == SplitBehavior.Split &&
                    !FloatUtil.AreClose(actualLength1 + actualLength2, _resizeData.OriginalDefinition1ActualLength + _resizeData.OriginalDefinition2ActualLength))
                {
                    CancelResize();
                    return;
                }

                float min, max;
                GetDeltaConstraints(out min, out max);

                // Flip when the splitter's flow direction isn't the same as the grid's
                //if (FlowDirection != _resizeData.Grid.FlowDirection)
                //    delta = -delta;

                // Constrain Delta to Min/MaxWidth of columns
                delta = Math.Min(Math.Max(delta, min), max);

                // With floating point operations there may be loss of precision to some degree. Eg. Adding a very 
                // small value to a very large one might result in the small value being ignored. In the following 
                // steps there are two floating point operations viz. actualLength1+delta and actualLength2-delta. 
                // It is possible that the addition resulted in loss of precision and the delta value was ignored, whereas 
                // the subtraction actual absorbed the delta value. This now means that 
                // (definition1LengthNew + definition2LengthNewis) 2 factors of precision away from 
                // (actualLength1 + actualLength2). This can cause a problem in the subsequent drag iteration where 
                // this will be interpreted as the cancellation of the resize operation. To avoid this imprecision we use 
                // make definition2LengthNew be a function of definition1LengthNew so that the precision or the loss 
                // thereof can be counterbalanced. See DevDiv bug#140228 for a manifestation of this problem.

                float definition1LengthNew = actualLength1 + delta;
                //float definition2LengthNew = actualLength2 - delta;
                float definition2LengthNew = actualLength1 + actualLength2 - definition1LengthNew;

                SetLengths(definition1LengthNew, definition2LengthNew);
            }
        }

        // Move the splitter using the Keyboard (Don't show preview)
        internal bool KeyboardMoveSplitter(float horizontalChange, float verticalChange)
        {
            // If moving with the mouse, ignore keyboard motion
            if (_resizeData != null)
            {
                return false;  // don't handle the event
            }

            InitializeData(false);  // don't show preview

            // Check that we are actually able to resize
            if (_resizeData == null)
            {
                return false;  // don't handle the event
            }

            // Keyboard keys are unaffected by FlowDirection.
            //if (FlowDirection == FlowDirection.RightToLeft)
            //{
            //    horizontalChange = -horizontalChange;
            //}

            MoveSplitter(horizontalChange, verticalChange);

            _resizeData = null;

            return true;
        }


        private enum SplitBehavior : byte
        {
            Split, // Both columns/rows are star lengths
            Resize1, // resize 1 only
            Resize2, // resize 2 only
        }

        private class ResizeData
        {
            public bool ShowsPreview;
            public PreviewAdorner Adorner;

            // The constraints to keep the Preview within valid ranges
            public float MinChange;
            public float MaxChange;

            // The grid to Resize
            public Grid Grid;

            // cache of Resize Direction and Behavior
            public GridResizeDirection ResizeDirection;
            public GridResizeBehavior ResizeBehavior;

            // The columns/rows to resize
            public DefinitionBase Definition1;
            public DefinitionBase Definition2;

            // Are the columns/rows star lengths
            public SplitBehavior SplitBehavior;

            // The index of the splitter
            public int SplitterIndex;

            // The indices of the columns/rows
            public int Definition1Index;
            public int Definition2Index;

            // The original lengths of Definition1 and Definition2 (to restore lengths if user cancels resize)
            public GridLength OriginalDefinition1Length;
            public GridLength OriginalDefinition2Length;
            public float OriginalDefinition1ActualLength;
            public float OriginalDefinition2ActualLength;

            // The minimum of Width/Height of Splitter.  Used to ensure splitter 
            //isn't hidden by resizing a row/column smaller than the splitter
            public float SplitterLength;
        }

    }

    class PreviewAdorner : UIElement
    {
        public PreviewAdorner(UIElement owner)
        {
            this.owner = owner; 
            var point = owner.PointToView(new Point(OffsetX, OffsetY));
            MarginLeft = point.X;
            MarginTop = point.Y;
        }
        UIElement owner;

        [UIPropertyMetadata(typeof(Color), "0,0,0", UIPropertyOptions.AffectsRender)]
        public Color Color
        {
            get { return (Color)GetValue(); }
            set { SetValue(value); }
        }

        public float OffsetX
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }
        public float OffsetY
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }
        [PropertyChanged(nameof(OffsetX))]
        [PropertyChanged(nameof(OffsetY))]
        void OnOffset(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var point = owner.PointToView(new Point(OffsetX, OffsetY));
            MarginLeft = point.X;
            MarginTop = point.Y;
        }

        protected override void OnRender(DrawingContext dc)
        {
            using (SolidColorBrush sb = new SolidColorBrush(Color))
            {
                dc.FillRectangle(sb, new Rect(0, 0, ActualSize.Width, ActualSize.Height));
            }
        }
    }

    /// <summary>
    /// Enum to indicate whether GridSplitter resizes Columns or Rows
    /// </summary>
    public enum GridResizeDirection : byte
    {
        /// <summary>
        /// Determines whether to resize rows or columns based on its Alignment and 
        /// width compared to height
        /// </summary>
        Auto,

        /// <summary>
        /// Resize columns when dragging Splitter.
        /// </summary>
        Columns,

        /// <summary>
        /// Resize rows when dragging Splitter.
        /// </summary>
        Rows,

        // NOTE: if you add or remove any values in this enum, be sure to update GridSplitter.IsValidResizeDirection()
    }


    /// <summary>
    /// Enum to indicate what Columns or Rows the GridSplitter resizes
    /// </summary>
    public enum GridResizeBehavior : byte
    {
        /// <summary>
        /// Determine which columns or rows to resize based on its Alignment.
        /// </summary>
        BasedOnAlignment,

        /// <summary>
        /// Resize the current and next Columns or Rows.
        /// </summary>
        CurrentAndNext,

        /// <summary>
        /// Resize the previous and current Columns or Rows.
        /// </summary>
        PreviousAndCurrent,

        /// <summary>
        /// Resize the previous and next Columns or Rows.
        /// </summary>
        PreviousAndNext,

        // NOTE: if you add or remove any values in this enum, be sure to update GridSplitter.IsValidResizeBehavior()
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using CPF.Input;
//using IOleDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace CPF.Windows
{
    public class OleDropTarget : IDropTarget
    {
        private readonly WindowImpl _target;

        Input.IDataObject _currentDrag;

        public OleDropTarget(WindowImpl target)
        {
            _target = target;
        }

        public static uint ConvertDropEffect(DragDropEffects operation)
        {
            DropEffect result = DropEffect.None;
            if (operation.HasFlag(DragDropEffects.Copy))
                result |= DropEffect.Copy;
            if (operation.HasFlag(DragDropEffects.Move))
                result |= DropEffect.Move;
            if (operation.HasFlag(DragDropEffects.Link))
                result |= DropEffect.Link;
            return (uint)result;
        }

        public static DragDropEffects ConvertDropEffect(DropEffect effect)
        {
            DragDropEffects result = DragDropEffects.None;
            if (effect.HasFlag(DropEffect.Copy))
                result |= DragDropEffects.Copy;
            if (effect.HasFlag(DropEffect.Move))
                result |= DragDropEffects.Move;
            if (effect.HasFlag(DropEffect.Link))
                result |= DragDropEffects.Link;
            return result;
        }

        //private static InputModifiers ConvertKeyState(int grfKeyState)
        //{
        //    InputModifiers modifiers = InputModifiers.None;
        //    var state = (UnmanagedMethods.ModifierKeys)grfKeyState;

        //    if (state.HasFlag(UnmanagedMethods.ModifierKeys.MK_LBUTTON))
        //        modifiers |= InputModifiers.LeftMouseButton;
        //    if (state.HasFlag(UnmanagedMethods.ModifierKeys.MK_MBUTTON))
        //        modifiers |= InputModifiers.MiddleMouseButton;
        //    if (state.HasFlag(UnmanagedMethods.ModifierKeys.MK_RBUTTON))
        //        modifiers |= InputModifiers.RightMouseButton;
        //    if (state.HasFlag(UnmanagedMethods.ModifierKeys.MK_SHIFT))
        //        modifiers |= InputModifiers.Shift;
        //    if (state.HasFlag(UnmanagedMethods.ModifierKeys.MK_CONTROL))
        //        modifiers |= InputModifiers.Control;
        //    if (state.HasFlag(UnmanagedMethods.ModifierKeys.MK_ALT))
        //        modifiers |= InputModifiers.Alt;
        //    return modifiers;
        //}
        HRESULT IDropTarget.DragEnter(IOleDataObject pDataObj, uint grfKeyState, POINT pt, ref uint pdwEffect)
        {
            _currentDrag = pDataObj as Input.IDataObject;
            if (_currentDrag == null)
                _currentDrag = new OleDataObject(pDataObj);

            //var args = new RawDragEvent(
            //    _dragDevice,
            //    RawDragEventType.DragEnter,
            //    _target,
            //    GetDragLocation(pt),
            //    _currentDrag,
            //    ConvertDropEffect(pdwEffect),
            //    //ConvertKeyState(grfKeyState)
            //);
            //dispatch(args);
            //pdwEffect = ConvertDropEffect(args.Effects);
            //Debug.WriteLine(pdwEffect + " " + _currentDrag.Get(DataFormat.Text));

            //var etc = pDataObj.EnumFormatEtc(DATADIR.DATADIR_GET);
            //FORMATETC[] fs = new FORMATETC[1];
            //while (etc.Next(1, fs, null) == 0)
            //{
            //    Debug.WriteLine(fs[0].cfFormat);
            //}
            //Debug.WriteLine(pdwEffect);
            pdwEffect = ConvertDropEffect(_target.Root.InputManager.DragDropDevice.DragEnter(new DragEventArgs(_currentDrag, GetDragLocation(pt), _target.Root) { DragEffects = ConvertDropEffect((DropEffect)pdwEffect) }, _target.Root.LayoutManager.VisibleUIElements));
            return HRESULT.S_OK;
        }

        HRESULT IDropTarget.DragOver(uint grfKeyState, POINT pt, ref uint pdwEffect)
        {
            pdwEffect = ConvertDropEffect(_target.Root.InputManager.DragDropDevice.DragOver(new DragEventArgs(_currentDrag, GetDragLocation(pt), _target.Root) { DragEffects = ConvertDropEffect((DropEffect)pdwEffect) }, _target.Root.LayoutManager.VisibleUIElements));
            return HRESULT.S_OK;
        }

        HRESULT IDropTarget.DragLeave()
        {
            try
            {
                _target.Root.InputManager.DragDropDevice.DragLeave(_target.Root.LayoutManager.VisibleUIElements);
                return HRESULT.S_OK;
            }
            finally
            {
                _currentDrag = null;
            }
        }

        HRESULT IDropTarget.Drop(IOleDataObject pDataObj, uint grfKeyState, POINT pt, ref uint pdwEffect)
        {
            try
            {
                _currentDrag = pDataObj as Input.IDataObject;
                if (_currentDrag == null)
                    _currentDrag = new OleDataObject(pDataObj);

                pdwEffect = ConvertDropEffect(_target.Root.InputManager.DragDropDevice.Drop(new DragEventArgs(_currentDrag, GetDragLocation(pt), _target.Root) { DragEffects = ConvertDropEffect((DropEffect)pdwEffect) }, _target.Root.LayoutManager.VisibleUIElements));

                return HRESULT.S_OK;
            }
            finally
            {
                _currentDrag = null;
            }
        }

        private Point GetDragLocation(POINT dragPoint)
        {
            //int x = (int)dragPoint;
            //int y = (int)(dragPoint >> 32);

            var screenPt = new Point(dragPoint.X, dragPoint.Y);
            return _target.PointToClient(screenPt);
            //return screenPt;
        }


    }
}

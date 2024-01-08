// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

//using Microsoft.MobileBlazorBindings.Core;
using System;
using System.Diagnostics;

namespace CPF.Razor
{
    internal class CpfElementManager : ElementManager<ICpfElementHandler>
    {
        protected override bool IsParented(ICpfElementHandler handler)
        {
            return handler.Element.Parent != null;
        }

        protected override void AddChildElement(
            ICpfElementHandler parentHandler,
            ICpfElementHandler childHandler,
            int physicalSiblingIndex)
        {
            if (parentHandler.Element is CPF.Controls.Panel panel)
            {
                if (physicalSiblingIndex <= panel.Children.Count)
                {
                    panel.Children.Insert(physicalSiblingIndex, childHandler.Element);
                }
                else
                {
                    //Debug.WriteLine($"WARNING: {nameof(AddChildElement)} called with {nameof(physicalSiblingIndex)}={physicalSiblingIndex}, but parentControl.Controls.Count={parentHandler.Control.Controls.Count}");
                    panel.Children.Add(childHandler.Element);
                }
            }
            else if (parentHandler.Element is CPF.Controls.Window win)
            {
                if (physicalSiblingIndex <= win.Children.Count)
                {
                    win.Children.Insert(physicalSiblingIndex, childHandler.Element);
                }
                else
                {
                    win.Children.Add(childHandler.Element);
                }
            }
            else if (parentHandler.Element is CPF.Controls.ContentControl contentControl)
            {
                contentControl.Content = childHandler.Element;
            }
            else
            {
                Debug.Fail("未实现添加控件");
            }
        }

        protected override int GetPhysicalSiblingIndex(
            ICpfElementHandler handler)
        {
            return (handler.Element.Parent as CPF.Controls.Panel).Children.IndexOf(handler.Element);
        }

        protected override void RemoveElement(ICpfElementHandler handler)
        {
            if (handler.Element.Parent is CPF.Controls.Panel panel)
            {
                panel.Children.Remove(handler.Element);
            }
            else
            {
                Debug.Fail("未实现移除控件");
            }
        }

        protected override bool IsParentOfChild(ICpfElementHandler parentHandler, ICpfElementHandler childHandler)
        {
            return childHandler.Element.Parent == parentHandler.Element;
        }
    }
}

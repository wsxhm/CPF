﻿// <auto-generated>
// Code generated by COM Proxy Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
using System;
using ComInterfaceDispatch = System.Runtime.InteropServices.ComWrappers.ComInterfaceDispatch;
using Marshal = System.Runtime.InteropServices.Marshal;

namespace CPF.Windows.ComWrapper
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    unsafe partial class IDropSourceProxy
    {
        [System.Runtime.InteropServices.UnmanagedCallersOnly]
        public static int QueryContinueDrag(System.IntPtr thisPtr, int fEscapePressed, int grfKeyState)
        {
            try
            {
                var inst = ComInterfaceDispatch.GetInstance<CPF.Windows.IDropSource>((ComInterfaceDispatch*)thisPtr);
                return (int)inst.QueryContinueDrag(fEscapePressed, grfKeyState);
            }
            catch (System.Exception __e)
            {
                Console.WriteLine(__e);
                return __e.HResult;
            }
        }
        [System.Runtime.InteropServices.UnmanagedCallersOnly]
        public static int GiveFeedback(System.IntPtr thisPtr, int dwEffect)
        {
            try
            {
                var inst = ComInterfaceDispatch.GetInstance<CPF.Windows.IDropSource>((ComInterfaceDispatch*)thisPtr);
                return (int)inst.GiveFeedback(dwEffect);
            }
            catch (System.Exception __e)
            {
                Console.WriteLine(__e);
                return __e.HResult;
            }
        }
    }
}
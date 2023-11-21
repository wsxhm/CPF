using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using static CPF.Windows.UnmanagedMethods;
using CPF;
using System.Threading;
using System.IO;
using CPF.Threading;
using System.Collections.Concurrent;
using CPF.Drawing;
using CPF.Input;
using System.Linq;
using CPF.Controls;
using CPF.Platform;
using CPF.Shapes;
using CPF.Animation;
using System.Diagnostics;
namespace CPF.Windows
{

    internal unsafe class GPStream : IStream
    {
        protected Stream dataStream;
        public const int STREAM_SEEK_SET = 0x0;
        public const int STREAM_SEEK_CUR = 0x1;
        public const int STREAM_SEEK_END = 0x2;
        // to support seeking ahead of the stream length...
        long virtualPosition = -1;

        internal GPStream(Stream stream)
        {
            if (!stream.CanSeek)
            {
                const int ReadBlock = 256;
                byte[] bytes = new byte[ReadBlock];
                int readLen;
                int current = 0;
                do
                {
                    if (bytes.Length < current + ReadBlock)
                    {
                        byte[] newData = new byte[bytes.Length * 2];
                        Array.Copy(bytes, newData, bytes.Length);
                        bytes = newData;
                    }
                    readLen = stream.Read(bytes, current, ReadBlock);
                    current += readLen;
                } while (readLen != 0);

                dataStream = new MemoryStream(bytes);
            }
            else
            {
                dataStream = stream;
            }
        }

        private void ActualizeVirtualPosition()
        {
            if (virtualPosition == -1) return;

            if (virtualPosition > dataStream.Length)
                dataStream.SetLength(virtualPosition);

            dataStream.Position = virtualPosition;

            virtualPosition = -1;
        }

        public virtual HRESULT Clone(IntPtr* o)
        {
            NotImplemented();
            return HRESULT.E_NOTIMPL;
        }

        public virtual void Commit(uint grfCommitFlags)
        {
            dataStream.Flush();
            // Extend the length of the file if needed.
            ActualizeVirtualPosition();
        }

        public virtual HRESULT CopyTo(IntPtr pstm, ulong cb, ulong* pcbRead, ulong* c)
        {
            return HRESULT.E_NOTIMPL;
            //int bufsize = 4096; // one page
            //IntPtr buffer = Marshal.AllocHGlobal(bufsize);
            //if (buffer == IntPtr.Zero) throw new OutOfMemoryException();
            //ulong written = 0;
            //try
            //{
            //    while (written < cb)
            //    {
            //        ulong toRead = (ulong)bufsize;
            //        if (written + toRead > cb) toRead = cb - written;
            //        uint read;
            //        uint* r = &read;
            //        Read((byte*)buffer, (uint)toRead, r);
            //        if (r[0] == 0) break;
            //        r[0] = 0;
            //        pstm.Write((byte*)buffer, read, r);
            //        if (r[0] != read)
            //        {
            //            throw EFail("Wrote an incorrect number of bytes");
            //        }
            //        written += read;
            //    }
            //}
            //finally
            //{
            //    Marshal.FreeHGlobal(buffer);
            //}
            ////if (pcbRead != null && pcbRead.Length > 0)
            ////{
            ////    pcbRead[0] = written;
            ////}

            //*c = written;
            //return HRESULT.S_OK;
        }

        public virtual Stream GetDataStream()
        {
            return dataStream;
        }

        public HRESULT LockRegion(
            ulong libOffset,
            ulong cb,
            uint dwLockType)
        {
            return HRESULT.E_NOTIMPL;
        }

        protected static ExternalException EFail(string msg)
        {
            throw new ExternalException(msg);
        }

        protected static void NotImplemented()
        {
            throw new NotImplementedException();
        }

        public void Read(byte* pv, uint cb, uint* c)
        {
            byte[] buffer = new byte[cb];
            int count = Read(buffer, (int)cb);
            Marshal.Copy(buffer, 0, (IntPtr)pv, (int)cb);
            if (c != null)
            {
                *c = (uint)count;
            }
        }

        public int Read(byte[] buffer, int length)
        {
            ActualizeVirtualPosition();
            return dataStream.Read(buffer, 0, length);
        }

        public virtual void Revert()
        {
            NotImplemented();
        }

        public virtual void Seek(long offset, SeekOrigin dwOrigin, ulong* l)
        {
            // Console.WriteLine("IStream::Seek("+ offset + ", " + origin + ")");
            long pos = virtualPosition;
            if (virtualPosition == -1)
            {
                pos = dataStream.Position;
            }
            long len = dataStream.Length;
            switch (dwOrigin)
            {
                case SeekOrigin.Begin:
                    if (offset <= len)
                    {
                        dataStream.Position = offset;
                        virtualPosition = -1;
                    }
                    else
                    {
                        virtualPosition = offset;
                    }
                    break;
                case SeekOrigin.End:
                    if (offset <= 0)
                    {
                        dataStream.Position = len + offset;
                        virtualPosition = -1;
                    }
                    else
                    {
                        virtualPosition = len + offset;
                    }
                    break;
                case SeekOrigin.Current:
                    if (offset + pos <= len)
                    {
                        dataStream.Position = pos + offset;
                        virtualPosition = -1;
                    }
                    else
                    {
                        virtualPosition = offset + pos;
                    }
                    break;
            }
            if (l == null)
            {
                return;
            }
            if (virtualPosition != -1)
            {
                *l = (uint)virtualPosition;
            }
            else
            {
                *l = (uint)dataStream.Position;
            }
        }

        public void SetSize(ulong value)
        {
            dataStream.SetLength((long)value);
        }

        public void Stat(
             STATSTG* pstatstg,
            STATFLAG grfStatFlag)
        {
            STATSTG stats = new STATSTG();
            stats.cbSize = (ulong)dataStream.Length;
            //Marshal.StructureToPtr(stats, pstatstg, true);
            *pstatstg = stats;
        }

        public HRESULT UnlockRegion(
            ulong libOffset,
            ulong cb,
            uint dwLockType)
        {
            return HRESULT.E_NOTIMPL;
        }

        public void Write(byte* pv, uint cb, uint* c)
        {
            byte[] buffer = new byte[cb];
            Marshal.Copy((IntPtr)pv, buffer, 0, (int)cb);
            *c = (uint)Write(buffer, (int)cb);
        }

        public int Write(byte[] buffer, /* cpr: int offset,*/ int length)
        {
            ActualizeVirtualPosition();
            dataStream.Write(buffer, 0, length);
            return length;
        }

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct STATSTG
    {
        /// <summary>
        /// Pointer to the name.
        /// </summary>
        private IntPtr pwcsName;
        public STGTY type;

        /// <summary>
        /// Size of the stream in bytes.
        /// </summary>
        public ulong cbSize;

        public FILETIME mtime;
        public FILETIME ctime;
        public FILETIME atime;

        /// <summary>
        /// The stream mode.
        /// </summary>
        public STGM grfMode;

        /// <summary>
        /// Supported locking modes.
        /// <see href="https://docs.microsoft.com/en-us/windows/desktop/api/objidl/ne-objidl-taglocktype"/>
        /// </summary>
        /// <remarks>
        /// '0' means does not support lock modes.
        /// </remarks>
        public uint grfLocksSupported;

        /// <remarks>
        /// Only for IStorage objects
        /// </remarks>
        public Guid clsid;

        /// <remarks>
        /// Only valid for IStorage objects.
        /// </remarks>
        public uint grfStateBits;
        public uint reserved;

        public string GetName() => Marshal.PtrToStringUni(pwcsName);

        /// <summary>
        /// Caller is responsible for freeing the name memory.
        /// </summary>
        public void FreeName()
        {
            if (pwcsName != IntPtr.Zero)
                Marshal.FreeCoTaskMem(pwcsName);

            pwcsName = IntPtr.Zero;
        }

        /// <summary>
        /// Callee is repsonsible for allocating the name memory.
        /// </summary>
        public void AllocName(string name)
        {
            pwcsName = Marshal.StringToCoTaskMemUni(name);
        }
    }

    public enum STGTY : uint
    {
        STGTY_STORAGE = 1,
        STGTY_STREAM = 2,
        STGTY_LOCKBYTES = 3,
        STGTY_PROPERTY = 4
    }

    [Flags]
    public enum STGM : uint
    {
        /// <summary>
        /// Read only, and each change to a storage or stream element is written as it occurs.
        /// Fails if the given storage object already exists.
        /// [STGM_DIRECT] [STGM_READ] [STGM_FAILIFTHERE] [STGM_SHARE_DENY_WRITE]
        /// </summary>
        Default = 0x00000000,

        STGM_TRANSACTED = 0x00010000,
        STGM_SIMPLE = 0x08000000,
        STGM_WRITE = 0x00000001,
        STGM_READWRITE = 0x00000002,
        STGM_SHARE_DENY_NONE = 0x00000040,
        STGM_SHARE_DENY_READ = 0x00000030,
        STGM_SHARE_DENY_WRITE = 0x00000020,
        STGM_SHARE_EXCLUSIVE = 0x00000010,
        STGM_PRIORITY = 0x00040000,
        STGM_DELETEONRELEASE = 0x04000000,
        STGM_NOSCRATCH = 0x00100000,
        STGM_CREATE = 0x00001000,
        STGM_CONVERT = 0x00020000,
        STGM_NOSNAPSHOT = 0x00200000,
        STGM_DIRECT_SWMR = 0x00400000
    }
}

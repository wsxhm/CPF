using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Platform
{
    //public class RuntimePlatformInfo
    //{
    //    public OperatingSystemType OperatingSystem { get; set; }

    //    public bool IsDesktop { get; set; }
    //    public bool IsMobile { get; set; }
    //    public bool IsCoreClr { get; set; }
    //    public bool IsMono { get; set; }
    //    public bool IsDotNetFramework { get; set; }
    //    public bool IsUnix { get; set; }
    //}

    public enum OperatingSystemType:byte
    {
        Unknown,
        Windows,
        Linux,
        OSX,
        Android,
        iOS
    }
}

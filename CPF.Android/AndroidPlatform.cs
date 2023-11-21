using Android.Content;
using CPF.Controls;
using CPF.Input;
using CPF.Platform;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Linq;
using Android.OS;

namespace CPF.Android
{
    public class AndroidPlatform : RuntimePlatform
    {
        internal PixelPoint mousePosition;
        public override PixelPoint MousePosition => mousePosition;

        public override TimeSpan DoubleClickTime => TimeSpan.FromSeconds(0.5);

        public override IPopupImpl CreatePopup()
        {
            return new PopupImpl();
        }

        public override IWindowImpl CreateWindow()
        {
            return new WindowImpl();
        }

        public override DragDropEffects DoDragDrop(DragDropEffects allowedEffects, params (DataFormat, object)[] data)
        {
            throw new NotImplementedException();
        }

        public override IReadOnlyList<Screen> GetAllScreen()
        {
            var display = CpfActivity.CurrentActivity.WindowManager.DefaultDisplay;
            global::Android.Graphics.Rect rect = new global::Android.Graphics.Rect();
            display.GetRectSize(rect);

            global::Android.Graphics.Point point1 = new global::Android.Graphics.Point();
            display.GetSize(point1);

            var screen = new Screen(new Drawing.Rect(0, 0, point1.X, point1.Y), new Drawing.Rect(rect.Left, rect.Top, rect.Right, rect.Bottom), true);
            return new Screen[] { screen };
        }

        public override IClipboard GetClipboard()
        {
            return new ClipboardImpl();
        }

        public override object GetCursor(Cursors cursorType)
        {
            return cursorType;
        }

        public override SynchronizationContext GetSynchronizationContext()
        {
            return new AndroidSynchronizationContext();
        }
        static Dictionary<KeyGesture, PlatformHotkey> keyValuePairs = new Dictionary<KeyGesture, PlatformHotkey>() {
            { new KeyGesture(Keys.C,InputModifiers.Control),PlatformHotkey.Copy},
            { new KeyGesture(Keys.X,InputModifiers.Control),PlatformHotkey.Cut},
            { new KeyGesture(Keys.V,InputModifiers.Control),PlatformHotkey.Paste},
            { new KeyGesture(Keys.Y,InputModifiers.Control),PlatformHotkey.Redo},
            { new KeyGesture(Keys.A,InputModifiers.Control),PlatformHotkey.SelectAll},
            { new KeyGesture(Keys.Z,InputModifiers.Control),PlatformHotkey.Undo},
        };
        public override PlatformHotkey Hotkey(KeyGesture keyGesture)
        {
            keyValuePairs.TryGetValue(keyGesture, out PlatformHotkey platformHotkey);
            return platformHotkey;
        }

        public override void Run()
        {

        }


        public override Task<string[]> ShowFileDialogAsync(FileDialog dialog, IWindowImpl parent)
        {
            //var activity = CpfActivity.CurrentActivity as CpfActivity;
            //if (activity != null && activity.Main != null)
            //{
            //    //activity.fileName = new TaskCompletionSource<string[]>();
            //    //FileSaveFragment fileSaveFragment = FileSaveFragment.newInstance("*/*", 500, Resource.String.btn_ok, Resource.String.btn_cancel, Resource.String.tag_title_SaveFile, Resource.String.tag_save_hint, Resource.Drawable.filedialog_root_l, Resource.Drawable.filedialog_folder_l, Resource.Drawable.filedialog_folder_up_l, Resource.Drawable.filedialog_xlsfile_l);
            //    //fileSaveFragment.Show(activity.FragmentManager, fileSaveFragment.Tag);
            //    //return activity.fileName.Task;
            //    var f = new OpenFileDialogView();
            //    activity.Main.Root.Children.Add(f);
            //}
            //return Task.Run(() => new string[0]);

            return Task.Run(() =>
            {
                if (dialog is OpenFileDialog open)
                {
                    var p = new PickOptions { PickerTitle = dialog.Title };
                    if (open.Filters != null && open.Filters.Count > 0)
                    {
                        HashSet<string> ex = new HashSet<string>();
                        foreach (var item in open.Filters)
                        {
                            if (!string.IsNullOrWhiteSpace(item.Extensions))
                            {
                                var es = item.Extensions.Split(',');
                                foreach (var e in es)
                                {
                                    if (e == null)
                                    {
                                        continue;
                                    }
                                    if (MIME_Map.TryGetValue(e, out var ee))
                                    {
                                        ex.Add(ee);
                                    }
                                }
                            }
                        }
                        p.FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                            { DevicePlatform.Android, ex }
                        });
                    }
                    if (!open.AllowMultiple)
                    {
                        Task<FileResult> fileTask = null;
                        CPF.Threading.Dispatcher.MainThread.Invoke(() =>
                        {
                            fileTask = FilePicker.PickAsync(p);
                        });
                        var file = fileTask.Result;
                        if (file != null)
                        {
                            return new string[] { file.FullPath };
                        }
                    }
                    else
                    {
                        Task<IEnumerable<FileResult>> fileTask = null;
                        CPF.Threading.Dispatcher.MainThread.Invoke(() =>
                        {
                            fileTask = FilePicker.PickMultipleAsync(p);
                        });
                        var files = fileTask.Result;
                        if (files != null)
                        {
                            return files.Select(a => a.FullPath).ToArray();
                        }
                    }
                }
                else if (dialog is SaveFileDialog save)
                {
                    throw new NotImplementedException();
                }
                return new string[0];
            });
        }

        public override Task<string> ShowFolderDialogAsync(OpenFolderDialog dialog, IWindowImpl parent)
        {
            throw new NotImplementedException();
        }

        public override INativeImpl CreateNative()
        {
            return new NativeImpl();
        }

        public override INotifyIconImpl CreateNotifyIcon()
        {
            return null;
        }

        public override void Run(CancellationToken cancellation)
        {
            throw new NotSupportedException("安卓不能支持主线程的消息循环控制");

            //var queue = Looper.MyLooper().Queue;
            //var next = queue.Class.GetDeclaredMethod("next");
            //next.Accessible = true;
            //while (!cancellation.IsCancellationRequested)
            //{
            //    var message = (Message)next.Invoke(queue);
            //    if (message == null)
            //    {
            //        break;
            //    }
            //    try
            //    {
            //        message.Target.DispatchMessage(message);
            //    }
            //    catch (Exception e)
            //    {
            //        System.Diagnostics.Debug.WriteLine(e);
            //    }

            //    //Binder.ClearCallingIdentity();
            //    //var recycleUnchecked = message.Class.GetDeclaredMethod("recycleUnchecked");
            //    //recycleUnchecked.Accessible = true;
            //    //recycleUnchecked.Invoke(message);
            //    //message.Recycle();
            //}

            //cancellation.Register(() =>
            //{
            //    Looper.MyLooper().Quit();
            //});
            //cancellation.ThrowIfCancellationRequested();
            cancellation.Register(() =>
            {
                throw new Java.Lang.RuntimeException();
            });
            try
            {
                Looper.Loop();
            }
            catch (Exception e)
            {
            }
        }

        static Dictionary<string, string> MIME_Map = new Dictionary<string, string>
        {
            //{后缀名，MIME类型}
            {"pbm", "image/x-portable-bitmap"},
            {"pcx", "image/x-pcx"},
            {"nbmp", "image/nbmp"},
            {"pda", "image/x-pda"},
            {"pgm", "image/x-portable-graymap"},
            {"pict", "image/x-pict"},
            {"png", "image/png"},
            {"pnm", "image/x-portable-anymap"},
            {"pnz", "image/png"},
            {"ppm", "image/x-portable-pixmap"},
            {"nokia-op-logo", "image/vnd.nok-oplogo-color"},
            {"qti", "image/x-quicktime"},
            {"qtif", "image/x-quicktime"},
            {"ras", "image/x-cmu-raster"},
            {"rf", "image/vnd.rn-realflash"},
            {"rp", "image/vnd.rn-realpix"},
            {"rgb", "image/x-rgb"},
            {"si9", "image/vnd.lgtwap.sis"},
            {"si7", "image/vnd.stiwap.sis"},
            {"svf", "image/vnd"},
            {"svg", "image/svg-xml"},
            {"svh", "image/svh"},
            {"si6", "image/si6"},
            {"tif", "image/tiff"},
            {"tiff", "image/tiff"},
            {"toy", "image/toy"},
            {"wbmp", "image/vnd.wap.wbmp"},
            {"wi", "image/wavelet"},
            {"wpng", "image/x-up-wpng"},
            {"xbm", "image/x-xbitmap"},
            {"xpm", "image/x-xpixmap"},
            {"xwd", "image/x-xwindowdump"},
            {"fh4", "image/x-freehand"},
            {"fh5", "image/x-freehand"},
            {"fhc", "image/x-freehand"},
            {"fif", "image/fif"},
            {"bmp", "image/bmp"},
            {"cal", "image/x-cals"},
            {"cod", "image/cis-cod"},
            {"fpx", "image/x-fpx"},
            {"dcx", "image/x-dcx"},
            {"eri", "image/x-eri"},
            {"gif", "image/gif"},
            {"ief", "image/ief"},
            {"ifm", "image/gif"},
            {"ifs", "image/ifs"},
            {"j2k", "image/j2k"},
            {"jpe", "image/jpeg"},
            {"jpeg", "image/jpeg"},
            {"jpg", "image/jpeg"},
            {"jpz", "image/jpeg"},
            {"mil", "image/x-cals"},

            {"3gp", "video/3gpp"},
            {"asf", "video/x-ms-asf"},
            {"asx", "video/x-ms-asf"},
            {"avi", "video/x-msvideo"},
            {"fvi", "video/isivideo"},
            {"lsf", "video/x-ms-asf"},
            {"lsx", "video/x-ms-asf"},
            {"m4u", "video/vnd.mpegurl"},
            {"m4v", "video/x-m4v"},
            {"pvx", "video/x-pv-pvx"},
            {"qt", "video/quicktime"},
            {"rv", "video/vnd.rn-realvideo"},
            {"viv", "video/vivo"},
            {"vivo", "video/vivo"},
            {"vdo", "video/vdo"},
            {"wm", "video/x-ms-wm"},
            {"wmx", "video/x-ms-wmx"},
            {"wv", "video/wavelet"},
            {"wvx", "video/x-ms-wvx"},
            {"mov", "video/quicktime"},
            {"movie", "video/x-sgi-movie"},
            {"mp4", "video/mp4"},
            {"mng", "video/x-mng"},
            {"mpe", "video/mpeg"},
            {"mpeg", "video/mpeg"},
            {"mpg", "video/mpeg"},
            {"mpg4", "video/mp4"},

            {"aif", "audio/x-aiff"},
            {"aifc", "audio/x-aiff"},
            {"aiff", "audio/x-aiff"},
            {"als", "audio/X-Alpha5"},
            {"au", "audio/basic"},
            {"es", "audio/echospeech"},
            {"esl", "audio/echospeech"},
            {"awb", "audio/amr-wb"},
            {"imy", "audio/melody"},
            {"it", "audio/x-mod"},
            {"itz", "audio/x-mod"},
            {"tsi", "audio/tsplayer"},
            {"ult", "audio/x-mod"},
            {"vib", "audio/vib"},
            {"vox", "audio/voxware"},
            {"vqe", "audio/x-twinvq-plugin"},
            {"vqf", "audio/x-twinvq"},
            {"vql", "audio/x-twinvq"},
            {"wav", "audio/x-wav"},
            {"wax", "audio/x-ms-wax"},
            {"wmv", "audio/x-ms-wmv"},
            {"wma", "audio/x-ms-wma"},
            {"xmz", "audio/x-mod"},
            {"m15", "audio/x-mod"},
            {"m3u", "audio/x-mpegurl"},
            {"m3url", "audio/x-mpegurl"},
            {"m4a", "audio/mp4a-latm"},
            {"m4b", "audio/mp4a-latm"},
            {"m4p", "audio/mp4a-latm"},
            {"ma1", "audio/ma1"},
            {"ma2", "audio/ma2"},
            {"ma3", "audio/ma3"},
            {"ma5", "audio/ma5"},
            {"mdz", "audio/x-mod"},
            {"mid", "audio/midi"},
            {"midi", "audio/midi"},
            {"mio", "audio/x-mio"},
            {"mod", "audio/x-mod"},
            {"mp2", "audio/x-mpeg"},
            {"mp3", "audio/x-mpeg"},
            {"mpga", "audio/mpeg"},
            {"ogg", "audio/ogg"},
            {"nsnd", "audio/nsnd"},
            {"pae", "audio/x-epac"},
            {"pac", "audio/x-pac"},
            {"qcp", "audio/vnd.qcelp"},
            {"ra", "audio/x-pn-realaudio"},
            {"ram", "audio/x-pn-realaudio"},
            {"rm", "audio/x-pn-realaudio"},
            {"rmf", "audio/x-rmf"},
            {"rmm", "audio/x-pn-realaudio"},
            {"rmvb", "audio/x-pn-realaudio"},
            {"rpm", "audio/x-pn-realaudio-plugin"},
            {"s3m", "audio/x-mod"},
            {"s3z", "audio/x-mod"},
            {"stm", "audio/x-mod"},
            {"smz", "audio/x-smd"},
            {"snd", "audio/basic"},
            {"smd", "audio/x-smd"},
            {"xm", "audio/x-mod"},

            {"c", "text/plain"},
            {"asc", "text/plain"},
            {"conf", "text/plain"},
            {"cpp", "text/plain"},
            {"css", "text/css"},
            {"dhtml", "text/html"},
            {"etx", "text/x-setext"},
            {"h", "text/plain"},
            {"hdm", "text/x-hdml"},
            {"hdml", "text/x-hdml"},
            {"htm", "text/html"},
            {"html", "text/html"},
            {"hts", "text/html"},
            {"jad", "text/vnd.sun.j2me.app-descriptor"},
            {"java", "text/plain"},
            {"log", "text/plain"},
            {"mel", "text/x-vmel"},
            {"mrl", "text/x-mrml"},
            {"prop", "text/plain"},
            {"r3t", "text/vnd.rn-realtext3d"},
            {"sgm", "text/x-sgml"},
            {"rc", "text/plain"},
            {"rtx", "text/richtext"},
            {"rt", "text/vnd.rn-realtext"},
            {"sgml", "text/x-sgml"},
            {"spc", "text/x-speech"},
            {"txt", "text/plain"},
            {"tsv", "text/tab-separated-values"},
            {"talk", "text/x-speech"},
            {"vcf", "text/x-vcard"},
            {"wml", "text/vnd.wap.wml"},
            {"wmls", "text/vnd.wap.wmlscript"},
            {"wmlscript", "text/vnd.wap.wmlscript"},
            {"ws", "text/vnd.wap.wmlscript"},
            {"xml", "text/xml"},
            {"xsit", "text/xml"},
            {"xsl", "text/xml"},
            {"xul", "text/xul"},

            {"apk", "application/vnd.android.package-archive"},

            {"aab", "application/x-authoware-bin"},
            {"aam", "application/x-authoware-map"},
            {"aas", "application/x-authoware-seg"},
            {"ai", "application/postscript"},
            {"amc", "application/x-mpeg"},
            {"ani", "application/octet-stream"},
            {"asd", "application/astound"},
            {"asn", "application/astound"},
            {"asp", "application/x-asap"},
            {"avb", "application/octet-stream"},
            {"bcpio", "application/x-bcpio"},
            {"bin", "application/octet-stream"},
            {"bld", "application/bld"},
            {"bld2", "application/bld2"},
            {"bpk", "application/octet-stream"},
            {"bz2", "application/x-bzip2"},
            {"ccn", "application/x-cnc"},
            {"cco", "application/x-cocoa"},
            {"cdf", "application/x-netcdf"},
            {"chat", "application/x-chat"},
            {"class", "application/octet-stream"},
            {"clp", "application/x-msclip"},
            {"cmx", "application/x-cmx"},
            {"co", "application/x-cult3d-object"},
            {"cpio", "application/x-cpio"},
            {"cpt", "application/mac-compactpro"},
            {"crd", "application/x-mscardfile"},
            {"csh", "application/x-csh"},
            {"cur", "application/octet-stream"},
            {"dcr", "application/x-director"},
            {"dir", "application/x-director"},
            {"dll", "application/octet-stream"},
            {"dmg", "application/octet-stream"},
            {"dms", "application/octet-stream"},
            {"doc", "application/msword"},
            {"docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            {"dot", "application/x-dot"},
            {"dvi", "application/x-dvi"},
            {"dwg", "application/x-autocad"},
            {"dxf", "application/x-autocad"},
            {"dxr", "application/x-director"},
            {"ebk", "application/x-expandedbook"},
            {"eps", "application/postscript"},
            {"evy", "application/x-envoy"},
            {"exe", "application/octet-stream"},
            {"etc", "application/x-earthtime"},
            {"fm", "application/x-maker"},
            {"gps", "application/x-gps"},
            {"gtar", "application/x-gtar"},
            {"gz", "application/x-gzip"},
            {"gca", "application/x-gca-compressed"},
            {"hdf", "application/x-hdf"},
            {"hlp", "application/winhlp"},
            {"hqx", "application/mac-binhex40"},
            {"ico", "application/octet-stream"},
            {"ins", "application/x-NET-Install"},
            {"ips", "application/x-ipscript"},
            {"ipx", "application/x-ipix"},
            {"jam", "application/x-jam"},
            {"jar", "application/java-archive"},
            {"jnlp", "application/x-java-jnlp-file"},
            {"latex", "application/x-latex"},
            {"lcc", "application/fastman"},
            {"lcl", "application/x-digitalloca"},
            {"lcr", "application/x-digitalloca"},
            {"lgh", "application/lgh"},
            {"lha", "application/octet-stream"},
            {"js", "application/x-javascript"},
            {"jwc", "application/jwc"},
            {"kjx", "application/x-kjx"},
            {"lzh", "application/x-lzh"},
            {"m13", "application/x-msmediaview"},
            {"m14", "application/x-msmediaview"},
            {"man", "application/x-troff-man"},
            {"mbd", "application/mbedlet"},
            {"mct", "application/x-mascot"},
            {"mdb", "application/x-msaccess"},
            {"me", "application/x-troff-me"},
            {"mi", "application/x-mif"},
            {"mif", "application/x-mif"},
            {"mmf", "application/x-skt-lbs"},
            {"mny", "application/x-msmoney"},
            {"moc", "application/x-mocha"},
            {"mocha", "application/x-mocha"},
            {"mpn", "application/vnd.mophun.application"},
            {"mpc", "application/vnd.mpohun.certificate"},
            {"mof", "application/x-yumekara"},
            {"mpp", "application/vnd.ms-project"},
            {"mps", "application/x-mapserver"},
            {"mrm", "application/x-mrm"},
            {"ms", "application/x-troff-ms"},
            {"msg", "application/vnd.ms-outlook"},
            {"mts", "application/metastream"},
            {"mtx", "application/metastream"},
            {"mtz", "application/metastream"},
            {"mzv", "application/metastream"},
            {"nar", "application/zip"},
            {"nc", "application/x-netcdf"},
            {"ndwn", "application/ndwn"},
            {"nif", "application/x-nif"},
            {"nmz", "application/x-scream"},
            {"npx", "application/x-netfpx"},
            {"nva", "application/x-neva1"},
            {"oda", "application/oda"},
            {"oom", "application/x-AtlasMate-Plugin"},
            {"pan", "application/x-pan"},
            {"pdf", "application/pdf"},
            {"pfr", "application/font-tdpfr"},
            {"pm", "application/x-perl"},
            {"pmd", "application/x-pmd"},
            {"pot", "application/vnd.ms-powerpoint"},
            {"pps", "application/vnd.ms-powerpoint"},
            {"ppt", "application/vnd.ms-powerpoint"},
            {"pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
            {"pqf", "application/x-cprplayer"},
            {"pqi", "application/cprplayer"},
            {"proxy", "application/x-ns-proxy-autoconfig"},
            {"ps", "application/postscript"},
            {"ptlk", "application/listenup"},
            {"pub", "application/x-mspublisher"},
            {"prc", "application/x-prc"},
            {"rar", "application/x-rar-compressed"},
            {"rdf", "application/rdf+xml"},
            {"rlf", "application/x-richlink"},
            {"rnx", "application/vnd.rn-realplayer"},
            {"roff", "application/x-troff"},
            {"rtf", "application/rtf"},
            {"rtg", "application/metastream"},
            {"rwc", "application/x-rogerwilco"},
            {"sca", "application/x-supercard"},
            {"scd", "application/x-msschedule"},
            {"sdf", "application/e-score"},
            {"sea", "application/x-stuffit"},
            {"sh", "application/x-sh"},
            {"shw", "application/presentations"},
            {"shar", "application/x-shar"},
            {"sis", "application/vnd.symbian.install"},
            {"sit", "application/x-stuffit"},
            {"skd", "application/x-Koan"},
            {"skm", "application/x-Koan"},
            {"skp", "application/x-Koan"},
            {"skt", "application/x-Koan"},
            {"slc", "application/x-salsa"},
            {"smi", "application/smil"},
            {"smil", "application/smil"},
            {"smp", "application/studiom"},
            {"spl", "application/futuresplash"},
            {"spr", "application/x-sprite"},
            {"sprite", "application/x-sprite"},
            {"spt", "application/x-spt"},
            {"src", "application/x-wais-source"},
            {"stk", "application/hyperstudio"},
            {"sv4cpio", "application/x-sv4cpio"},
            {"sv4crc", "application/x-sv4crc"},
            {"swf", "application/x-shockwave-flash"},
            {"swfl", "application/x-shockwave-flash"},
            {"t", "application/x-troff"},
            {"tad", "application/octet-stream"},
            {"tar", "application/x-tar"},
            {"taz", "application/x-tar"},
            {"tbp", "application/x-timbuktu"},
            {"tbt", "application/x-timbuktu"},
            {"tcl", "application/x-tcl"},
            {"tex", "application/x-tex"},
            {"texi", "application/x-texinfo"},
            {"texinfo", "application/x-texinfo"},
            {"tgz", "application/x-tar"},
            {"thm", "application/vnd.eri.thm"},
            {"tki", "application/x-tkined"},
            {"tkined", "application/x-tkined"},
            {"toc", "application/toc"},
            {"tr", "application/x-troff"},
            {"trm", "application/x-msterminal"},
            {"tsp", "application/dsptype"},
            {"ttf", "application/octet-stream"},
            {"ttz", "application/t-time"},
            {"ustar", "application/x-ustar"},
            {"uu", "application/x-uuencode"},
            {"uue", "application/x-uuencode"},
            {"vcd", "application/x-cdlink"},
            {"vmd", "application/vocaltec-media-desc"},
            {"vmf", "application/vocaltec-media-file"},
            {"vmi", "application/x-dreamcast-vms-info"},
            {"vms", "application/x-dreamcast-vms"},
            {"wis", "application/x-InstallShield"},
            {"wmd", "application/x-ms-wmd"},
            {"wmf", "application/x-msmetafile"},
            {"wmlc", "application/vnd.wap.wmlc"},
            {"wmlsc", "application/vnd.wap.wmlscriptc"},
            {"wps", "application/vnd.ms-works"},
            {"wmz", "application/x-ms-wmz"},
            {"wri", "application/x-mswrite"},
            {"web", "application/vnd.xara"},
            {"wsc", "application/vnd.wap.wmlscriptc"},
            {"wxl", "application/x-wxl"},
            {"x-gzip", "application/x-gzip"},
            {"xar", "application/vnd.xara"},
            {"xdm", "application/x-xdma"},
            {"xdma", "application/x-xdma"},
            {"xdw", "application/vnd.fujixerox.docuworks"},
            {"xht", "application/xhtml+xml"},
            {"xhtm", "application/xhtml+xml"},
            {"xhtml", "application/xhtml+xml"},
            {"xla", "application/vnd.ms-excel"},
            {"xlc", "application/vnd.ms-excel"},
            {"xll", "application/x-excel"},
            {"xlm", "application/vnd.ms-excel"},
            {"xls", "application/vnd.ms-excel"},
            {"xlt", "application/vnd.ms-excel"},
            {"xlw", "application/vnd.ms-excel"},
            {"xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {"xpi", "application/x-xpinstall"},
            {"yz1", "application/x-yz1"},
            {"z", "application/x-compress"},
            {"zac", "application/x-zaurus-zac"},
            {"zip", "application/zip"},

            {"gau", "chemical/x-gaussian-input"},
            {"csm", "chemical/x-csml"},
            {"csml", "chemical/x-csml"},
            {"emb", "chemical/x-embl-dl-nucleotide"},
            {"embl", "chemical/x-embl-dl-nucleotide"},
            {"mol", "chemical/x-mdl-molfile"},
            {"pdb", "chemical/x-pdb"},
            {"xyz", "chemical/x-pdb"},
            {"mop", "chemical/x-mopac-input"},

            {"dcm", "x-lml/x-evm"},
            {"evm", "x-lml/x-evm"},
            {"gdb", "x-lml/x-gdb"},
            {"lak", "x-lml/x-lak"},
            {"lml", "x-lml/x-lml"},
            {"lmlpack", "x-lml/x-lmlpack"},
            {"ndb", "x-lml/x-ndb"},
            {"rte", "x-lml/x-gps"},
            {"wpt", "x-lml/x-gps"},
            {"trk", "x-lml/x-gps"},

            {"svr", "x-world/x-svr"},
            {"ivr", "i-world/i-vrml"},
            {"vre", "x-world/x-vream"},
            {"vrml", "x-world/x-vrml"},
            {"vrt", "x-world/x-vrt"},
            {"vrw", "x-world/x-vream"},
            {"vts", "workbook/formulaone"},
            {"wrl", "x-world/x-vrml"},
            {"wrz", "x-world/x-vrml"},

            {"dwf", "drawing/x-dwf"},
            {"ice", "x-conference/x-cooltalk"},
            {"map", "magnus-internal/imagemap"},
            {"shtml", "magnus-internal/parsed-html"},
            {"cgi", "magnus-internal/cgi"},

            {"", "*/*"}
        };
    }

}

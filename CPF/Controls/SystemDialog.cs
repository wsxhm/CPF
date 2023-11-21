using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CPF.Platform;

namespace CPF.Controls
{
    public abstract class FileDialog : FileSystemDialog
    {
        [NotCpfProperty]
        public List<FileDialogFilter> Filters { get; set; } = new List<FileDialogFilter>();
        public string InitialFileName { get { return GetValue<string>(); } set { SetValue(value); } }
    }

    public abstract class FileSystemDialog : SystemDialog
    {
        /// <summary>
        /// 默认路径
        /// </summary>
        public string Directory { get { return GetValue<string>(); } set { SetValue(value); } }
    }

    /// <summary>
    /// 保存文件对话框
    /// </summary>
    public class SaveFileDialog : FileDialog
    {
        /// <summary>
        /// 保存文件对话框
        /// </summary>
        public SaveFileDialog()
        {

        }
        public string DefaultExtension { get { return GetValue<string>(); } set { SetValue(value); } }

        public async Task<string> ShowAsync(Window parent)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            var r = await Application.GetRuntimePlatform()
                 .ShowFileDialogAsync(this, parent.ViewImpl as IWindowImpl);

            if (r != null && r.Length > 0)
            {
                return r[0];
            }
            return string.Empty;
        }
    }
    /// <summary>
    /// 打开文件对话框
    /// </summary>
    public class OpenFileDialog : FileDialog
    {
        /// <summary>
        /// 打开文件对话框
        /// </summary>
        public OpenFileDialog()
        {

        }
        /// <summary>
        /// 多选
        /// </summary>
        public bool AllowMultiple { get { return GetValue<bool>(); } set { SetValue(value); } }

        public Task<string[]> ShowAsync(Window parent)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            return Application.GetRuntimePlatform().ShowFileDialogAsync(this, parent?.ViewImpl as IWindowImpl);
        }
    }

    /// <summary>
    /// 打开目录对话框
    /// </summary>
    public class OpenFolderDialog : FileSystemDialog
    {
        /// <summary>
        /// 打开目录对话框
        /// </summary>
        public OpenFolderDialog()
        {

        }
        //public string DefaultDirectory { get; set; }
        /// <summary>
        /// 如果按取消按钮是返回空字符串
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public Task<string> ShowAsync(Window parent)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            return Application.GetRuntimePlatform().ShowFolderDialogAsync(this, parent.ViewImpl as IWindowImpl);
        }
    }

    public abstract class SystemDialog : CpfObject
    {
        public string Title { get { return GetValue<string>(); } set { SetValue(value); } }
    }

    /// <summary>
    /// 文件筛选描述
    /// </summary>
    public class FileDialogFilter
    {
        /// <summary>
        /// 筛选分类名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///png,jpg
        /// </summary>
        public string Extensions { get; set; }
    }
}

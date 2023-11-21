using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Java.IO;
using Android.Graphics;
using Android.Graphics.Drawables;
using static Android.Widget.AdapterView;

namespace CPF.Android
{
    public class FileSaveFragment : DialogFragment, IOnItemClickListener, IDialogInterfaceOnClickListener, View.IOnClickListener
    {
        private static string TAG = "FileSaveFragment";

        /*
         * Use the unicode "back" triangle to indicate there is a parent directory
         * rather than an icon to minimise file dependencies.
         * 
         * You may have to find an alternative symbol if the font in use doesn't
         * support this character.
         */
        static string PARENT = "\u25C0";

        private FileSaveCallbacks mCallbacks;
        private List<File> directoryList;
        private string defaultExtension;

        // The widgets required to provide the UI.
        private TextView currentPath;
        private EditText fileName;
        private LinearLayout root;
        private ListView directoryView;

        // The directory the user has selected.
        private File currentDirectory;

        // Resource IDs
        private int resourceID_OK;
        private int resourceID_Cancel;
        private int resourceID_Title;
        private int resourceID_EditHint;
        private int resourceID_Icon;

        private int dialog_Height;
        private int resourceID_Dir;
        private int resourceID_UpDir;
        private int resourceID_File;

        /**
		 * Does file already exist?
		 * */
        public static bool FileExists(string absolutePath, string fileName)
        {
            File checkFile = new File(absolutePath, fileName);
            return checkFile.Exists();
        }

        /**
		 * Restrict valid filenames to alpha-numeric (word chars) only. Simplifies
		 * reserved path character validation at cost of forbidding spaces, hyphens
		 * and underscores.
		 * 
		 * @param fileName
		 *            - filename without extension or path information.
		 * 
		 * */
        public static bool IsAlphaNumeric(string fileName)
        {
            fileName = NameNoExtension(fileName);
            return (!Regex.IsMatch(fileName, ".*\\W{1,}.*"));
        }

        /**
		 * Return the characters following the final full stop in the filename.
		 * */
        public static string Extension(string fileName)
        {

            string extension = "";

            if (fileName.Contains("."))
            {
                string[] tokens = Regex.Split(fileName, "\\.(?=[^\\.]+$)");
                extension = tokens[1];
            }

            return extension;
        }

        /**
		 * Return the filename without any extension. Extension is taken to be the
		 * characters following the final full stop (if any) in the filename.
		 * 
		 * @param fileName
		 *            - File name with or without extension.
		 * */
        public static string NameNoExtension(string fileName)
        {

            if (fileName.Contains("."))
            {
                String[] tokens = Regex.Split(fileName, "\\.(?=[^\\.]+$)");
                fileName = tokens[0];
            }

            return fileName;
        }

        /**
		 * Signal to / request action of host activity.
		 * 
		 * */
        public interface FileSaveCallbacks
        {

            /**
			 * Hand potential file details to context for validation.
			 * 
			 * @param absolutePath
			 *            - Absolute path to target directory.
			 * @param fileName
			 *            - Filename. Not guaranteed to have a type extension.
			 * 
			 * */
            public bool onCanSave(string absolutePath, string fileName);

            /**
			 * Hand validated path and name to context for use. If user cancels
			 * absolutePath and filename are handed out as null.
			 * 
			 * @param absolutePath
			 *            - Absolute path to target directory.
			 * @param fileName
			 *            - Filename. Not guaranteed to have a type extension.
			 * */
            public void onConfirmSave(string absolutePath, string fileName);
        }

        /**
		 * Create new instance of a file save popup.
		 * 
		 * @param defaultExtension
		 *            - Display a default extension for file to be created. Can be
		 *            null.
		 * @param resourceID_OK
		 *            - string resource ID for the positive (OK) button.
		 * @param resourceID_Cancel
		 *            - string resource ID for the negative (Cancel) button.
		 * @param resourceID_Title
		 *            - string resource ID for the dialogue's title.
		 * @param resourceID_EditHint
		 *            - string resource ID for the filename edit widget.
		 * @param resourceID_Icon
		 *            - Drawable resource ID for the dialogue's title bar icon.
		 * */
        public static FileSaveFragment newInstance(string defaultExtension,
                int resource_DialogHeight, int resourceID_OK,
                int resourceID_Cancel, int resourceID_Title,
                int resourceID_EditHint, int resourceID_Icon,
                int resourceID_Directory, int resourceID_UpDirectory,
                int resourceID_File)
        {
            FileSaveFragment frag = new FileSaveFragment();

            Bundle args = new Bundle();
            args.PutString("extensionList", defaultExtension);
            args.PutInt("captionOK", resourceID_OK);
            args.PutInt("captionCancel", resourceID_Cancel);
            args.PutInt("popupTitle", resourceID_Title);
            args.PutInt("editHint", resourceID_EditHint);
            args.PutInt("popupIcon", resourceID_Icon);

            args.PutInt("dialogHeight", resource_DialogHeight);
            args.PutInt("iconDirectory", resourceID_Directory);
            args.PutInt("iconUpDirectory", resourceID_UpDirectory);
            args.PutInt("iconFile", resourceID_File);
            frag.Arguments = args;
            return frag;
        }

        /**
		 * Note the parent activity for callback purposes.
		 * 
		 * @param activity
		 *            - parent activity
		 * */

        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
            // The containing activity is expected to implement the fragment's
            // callbacks otherwise it can't react to item changes.
            if (!(activity is FileSaveCallbacks))
            {
                throw new Exception(
                        "Activity must implement fragment's callbacks.");
            }

            mCallbacks = (FileSaveCallbacks)activity;
            directoryList = new List<File>();
            defaultExtension = Arguments.GetString("extensionList");
            resourceID_OK = Arguments.GetInt("captionOK");
            resourceID_Cancel = Arguments.GetInt("captionCancel");
            resourceID_Title = Arguments.GetInt("popupTitle");
            resourceID_EditHint = Arguments.GetInt("editHint");
            resourceID_Icon = Arguments.GetInt("popupIcon");

            dialog_Height = Arguments.GetInt("dialogHeight");
            resourceID_File = Arguments.GetInt("iconFile");
            resourceID_Dir = Arguments.GetInt("iconDirectory");
            resourceID_UpDir = Arguments.GetInt("iconUpDirectory");
        }

        /**
		 * Build the popup.
		 * */
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {

            /*
			 * Use the same callback for [OK] & [Cancel]. Hand out nulls to indicate
			 * abandonment.
			 */

            /*
			 * We want to make this a transportable piece of code so don't want an
			 * XML layout dependency so layout is set up in code.
			 * 
			 * [ListView of directory names ] [ ] [ ] [ ]
			 * ------------------------------------------------------ {current
			 * path}/ [ Enter Filename ] {default extension}
			 */

            // Set up the container view.
            LinearLayout.LayoutParams rootLayout = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent, 0.0F);
            root = new LinearLayout(Activity);
            root.Orientation = Orientation.Vertical;
            root.LayoutParameters = rootLayout;

            /*
			 * Set up initial sub-directory list.
			 */
            currentDirectory = global::Android.OS.Environment.ExternalStorageDirectory;
            directoryList = getSubDirectories(currentDirectory);
            DirectoryDisplay displayFormat = new DirectoryDisplay(Activity,
                    directoryList, this);

            /*
			 * Fix the height of the listview at 150px, enough to show 3 or 4
			 * entries at a time. Don't want the popup shrinking and growing all the
			 * time. Tried it. Most disconcerting.
			 */
            LinearLayout.LayoutParams listViewLayout = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent, dialog_Height, 0.0F);
            directoryView = new ListView(Activity);
            directoryView.LayoutParameters = listViewLayout;
            directoryView.Adapter = displayFormat;
            directoryView.OnItemClickListener = this;
            root.AddView(directoryView);

            View horizDivider = new View(Activity);
            horizDivider.SetBackgroundColor(Color.Cyan);
            root.AddView(horizDivider, new ViewGroup.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent, 2));

            /*
			 * Now set up the filename entry area.
			 * 
			 * {current path}/ [Enter Filename ] {default extension}
			 */
            LinearLayout nameArea = new LinearLayout(Activity);
            nameArea.Orientation = Orientation.Horizontal;
            nameArea.LayoutParameters = rootLayout;
            root.AddView(nameArea);

            currentPath = new TextView(Activity);
            currentPath.SetText(currentDirectory.AbsolutePath + "/", TextView.BufferType.Normal);
            nameArea.AddView(currentPath);

            /*
			 * We want the filename input area to be as large as possible, but still
			 * leave enough room to show the path and any default extension that may
			 * be supplied so we give it a weight of 1.
			 */
            LinearLayout.LayoutParams fileNameLayout = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent, 1.0F);
            fileName = new EditText(Activity);
            fileName.Hint = resourceID_EditHint.ToString();
            fileName.Gravity = GravityFlags.Left;
            fileName.LayoutParameters = fileNameLayout;
            fileName.InputType = global::Android.Text.InputTypes.TextFlagNoSuggestions;
            nameArea.AddView(fileName);

            /*
			 * We only display the default extension if one has been supplied.
			 */
            if (defaultExtension != null)
            {
                TextView defaultExt = new TextView(Activity);
                defaultExt.Text = defaultExtension;
                defaultExt.Gravity = GravityFlags.Left;
                defaultExt.SetPadding(2, 0, 6, 0);
                nameArea.AddView(defaultExt);
            }

            // Use the standard AlertDialog builder to create the popup.
            // Android custom and practice is normally to chain calls from the
            // builder, but
            // it can become an unreadable and unmaintainable mess very quickly so I
            // don't.
            var popupBuilder = new AlertDialog.Builder(Activity);
            popupBuilder.SetView(root);
            popupBuilder.SetIcon(resourceID_Icon);
            popupBuilder.SetTitle(resourceID_Title);

            //    // Set up anonymous methods to handle [OK] & [Cancel] click.
            //    popupBuilder.SetPositiveButton(resourceID_OK,
            //            new DialogInterface.OnClickListener()
            //            {

            //                public void onClick(DialogInterface dialog, int whichButton)
            //    {
            //        // Empty method. Method defined in onStart();
            //    }
            //});

            popupBuilder.SetPositiveButton(resourceID_OK, this);
            popupBuilder.SetNegativeButton(resourceID_Cancel, this);

            return popupBuilder.Create();
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            mCallbacks.onConfirmSave(null, null);
        }


        /**
         * Provide the [PositiveButton] with a click listener that doesn't dismiss
         * the popup if the user has entered an invalid filename.
         * 
         * */

        public override void OnStart()
        {
            base.OnStart();
            AlertDialog d = (AlertDialog)Dialog;
            if (d != null)
            {
                Button positiveButton = d
                        .GetButton((int)DialogButtonType.Positive);
                positiveButton.SetOnClickListener(this);
            }
        }

        public void OnClick(View v)
        {
            string absolutePath = currentDirectory.AbsolutePath;
            string filename = fileName.Text + defaultExtension;
            if (mCallbacks.onCanSave(absolutePath, filename))
            {
                Dismiss();
                mCallbacks.onConfirmSave(absolutePath, filename);
            }
        }
        /**
         * Identify all sub-directories within a directory.
         * 
         * @param directory
         *            The directory to walk.
         * */
        private List<File> getSubDirectories(File directory)
        {

            List<File> directories = new List<File>();
            File[] files = directory.ListFiles();

            // Allow navigation back up the tree when the directory is a
            // sub-directory.
            if (directory.Parent != null)
            {
                directories.Add(new File(PARENT));
            }

            // Enumerate any sub-directories in this directory.
            if (files != null)
            {
                foreach (File f in files)
                {
                    if (f.IsDirectory && !f.IsHidden)
                    {
                        directories.Add(f);
                    }
                }
            }

            return directories;

        }

        /**
         * Refresh the listview's display adapter using the content of the
         * identified directory.
         * 
         * */

        public void OnItemClick(AdapterView parent, View view, int pos, long id)
        {

            File selected = null;

            if (pos >= 0 || pos < directoryList.Count)
            {
                selected = directoryList[pos];
                string name = selected.Name;

                // Are we going up or down?
                if (name.Equals(PARENT))
                {
                    currentDirectory = currentDirectory.ParentFile;
                }
                else
                {
                    currentDirectory = selected;
                }

                // Refresh the listview display for the newly selected directory.
                directoryList = getSubDirectories(currentDirectory);
                DirectoryDisplay displayFormatter = new DirectoryDisplay(
                        Activity, directoryList, this);
                directoryView.Adapter = displayFormatter;

                // Update the path TextView widget. Tell the user where he or she
                // is.
                string path = currentDirectory.AbsolutePath;
                if (currentDirectory.Parent != null)
                {
                    path += "/";
                }
                currentPath.Text = path;
            }
        }


        /**
         * Display the sub-directories in a selected directory.
         * 
         * */
        private class DirectoryDisplay : ArrayAdapter<File>
        {
            FileSaveFragment fragment;
            public DirectoryDisplay(Context context, List<File> displayContent, FileSaveFragment fragment) : base(context, global::Android.Resource.Layout.SimpleListItem1, displayContent)
            {
                this.fragment = fragment;
            }

            /**
             * Display the name of each sub-directory.
             * */
            public override View GetView(int position, View convertView, ViewGroup parent)
            {

                int iconID = fragment.resourceID_File;
                // We assume that we've got a parent directory...
                TextView textview = (TextView)base.GetView(position, convertView,
                        parent);

                // If we've got a directory then get its name.
                if (fragment.directoryList[position] != null)
                {
                    textview.Text = fragment.directoryList[position].Name;

                    if (fragment.directoryList[position].IsDirectory)
                    {
                        iconID = fragment.resourceID_Dir;
                    }

                    string name = fragment.directoryList[position].Name;
                    if (name.Equals(PARENT))
                    {
                        // iconID = -1;
                        iconID = fragment.resourceID_UpDir;
                    }

                    // Icon to the left of the text.
                    if (iconID > 0)
                    {
                        Drawable icon = fragment.Activity.Resources.GetDrawable(
                                iconID);
                        textview.SetCompoundDrawablesWithIntrinsicBounds(icon,
                                null, null, null);
                    }

                }

                return textview;
            }

        }
    }
}
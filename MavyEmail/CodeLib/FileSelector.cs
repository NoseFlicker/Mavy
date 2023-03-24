using System.Drawing.Imaging;
using System.Windows.Forms;

namespace QiHe.CodeLib
{
    /// <summary>
    /// Represents the file type.
    /// </summary>
    public enum FileType 
    {
        /// <summary>
        /// Txt file
        /// </summary>
        Txt,
        /// <summary>
        /// Rtf file
        /// </summary>
        Rtf,
        /// <summary>
        /// Html file
        /// </summary>
        Html,
        /// <summary>
        /// Xml file
        /// </summary>
        Xml,
        /// <summary>
        /// PDF file
        /// </summary>
        PDF,
        /// <summary>
        /// Bin file
        /// </summary>
        Bin,
        /// <summary>
        /// Zip file
        /// </summary>
        Zip,
        /// <summary>
        /// Image file
        /// </summary>
        Img,
        /// <summary>
        /// Excel 97 xls
        /// </summary>
        Excel97,
        /// <summary>
        /// Excel 2007 Open Xml Format
        /// </summary>
        Excel2007,
        /// <summary>
        /// All file type
        /// </summary>
        All
    }
    /// <summary>
    /// FileSelector
    /// </summary>
    public static class FileSelector
    {
        /// <summary>
        /// The Title of FileDialog for select a Single File
        /// </summary>
        private const string TitleSingleFile = "Please choose a file";

        /// <summary>
        /// The Title of FileDialog for select Multiple Files
        /// </summary>
        private const string TitleMultiFile = "Please choose files";

        /// <summary>
        /// Filter
        /// </summary>
        private static string Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
        
        /// <summary>
        /// Sets the file extension.
        /// </summary>
        /// <value>The file extension.</value>
        private static FileType FileExtension
        {
            set
            {
                Filter = value switch
                {
                    FileType.Txt => "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    FileType.Rtf => "Rtf files (*.rtf)|*.rtf|All files (*.*)|*.*",
                    FileType.Html => "Html files (*.htm;*.html)|*.htm;*.html|All files (*.*)|*.*",
                    FileType.Xml => "XML files (*.xml)|*.xml|Config files (*.config)|*.config|All files (*.*)|*.*",
                    FileType.PDF => "PDF files (*.pdf)|*.pdf|PDF form files (*.fdf)|*.fdf|All files (*.*)|*.*",
                    FileType.Bin =>
                    "Application files(*.exe;*.dll)|*.exe;*.dll|Binary files (*.bin)|*.bin|All files (*.*)|*.*",
                    FileType.Zip => "Zip files (*.zip)|*.zip|All files (*.*)|*.*",
                    FileType.Img =>
                    "Gif(*.gif)|*.gif|Jpeg(*.jpg)|*.jpg|Emf(*.emf)|*.emf|Bmp(*.bmp)|*.bmp|Png(*.png)|*.png",
                    FileType.Excel97 => "Excel files (*.xls)|*.xls|All files (*.*)|*.*",
                    FileType.Excel2007 => "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                    FileType.All => "All files (*.*)|*.*",
                    _ => Filter
                };
            }
        }

        /// <summary>
        /// Gets the image format.
        /// </summary>
        /// <value>The image format.</value>
        public static ImageFormat ImageFormat
        {
            get
            {
                return SFD.FilterIndex switch
                {
                    1 => ImageFormat.Gif,
                    2 => ImageFormat.Jpeg,
                    3 => ImageFormat.Emf,
                    4 => ImageFormat.Bmp,
                    5 => ImageFormat.Png,
                    _ => ImageFormat.Png
                };
            }
        }

        private static readonly OpenFileDialog OFD = new OpenFileDialog();
        private static readonly SaveFileDialog SFD = new SaveFileDialog();

        /// <summary>
        /// Gets or sets the initial directory displayed by the file dialog box.
        /// </summary>
        public static string InitialPath
        {
            set
            {
                OFD.InitialDirectory = value;
                SFD.InitialDirectory = value;
            }
        }

        /// <summary>
        /// Initializes the <see cref="FileSelector"/> class.
        /// </summary>
        static FileSelector() => OFD.RestoreDirectory = false;

        /// <summary>
        /// Browses the file.
        /// </summary>
        /// <returns></returns>
        private static string BrowseFile()
        {
            OFD.Title = TitleSingleFile;
            OFD.Filter = Filter;
            OFD.Multiselect = false;
            return OFD.ShowDialog() == DialogResult.OK ? OFD.FileName : null;
        }

        /// <summary>
        /// Browses the files.
        /// </summary>
        /// <returns></returns>
        private static string[] BrowseFiles()
        {
            OFD.Title = TitleMultiFile;
            OFD.Filter = Filter;
            OFD.Multiselect = true;
            return OFD.ShowDialog() == DialogResult.OK ? OFD.FileNames : null;
        }

        /// <summary>
        /// Browses the file for save.
        /// </summary>
        /// <returns></returns>
        private static string BrowseFileForSave()
        {
            SFD.Title = TitleSingleFile;
            SFD.Filter = Filter;
            return SFD.ShowDialog() == DialogResult.OK ? SFD.FileName : null;
        }
        /// <summary>
        /// Browses the file.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string BrowseFile(FileType type)
        {
            FileExtension = type;
            return BrowseFile();
        }
        /// <summary>
        /// Browses the file.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static string BrowseFile(string filter)
        {
            Filter = filter;
            return BrowseFile();
        }

        /// <summary>
        /// Browses the files.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string[] BrowseFiles(FileType type)
        {
            FileExtension = type;
            return BrowseFiles();
        }

        /// <summary>
        /// Browses the files.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static string[] BrowseFiles(string filter)
        {
            Filter = filter;
            return BrowseFiles();
        }

        /// <summary>
        /// Browses the file for save.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string BrowseFileForSave(FileType type)
        {
            FileExtension = type;
            return BrowseFileForSave();
        }

        /// <summary>
        /// Browses the file for save.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static string BrowseFileForSave(string filter)
        {
            Filter = filter;
            return BrowseFileForSave();
        }
    }
}
using System.IO.Compression;

namespace SevenTiny.Bantina.IO
{
    public class ZipHelper
    {
        /// <summary>
        /// zip file
        /// </summary>
        /// <param name="sourcePath">file path to zip</param>
        /// <param name="zipPath">zip file path</param>
        public static void Zip(string sourcePath, string zipPath)
        {
            ZipFile.CreateFromDirectory(sourcePath, zipPath);
        }
        /// <summary>
        /// exact file
        /// </summary>
        /// <param name="zipPath">zip file path</param>
        /// <param name="exactPath">exact file path</param>
        public static void UnZip(string zipPath, string exactPath)
        {
            ZipFile.ExtractToDirectory(zipPath, exactPath);
        }
    }
}

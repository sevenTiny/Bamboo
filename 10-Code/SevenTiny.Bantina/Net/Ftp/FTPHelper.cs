using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Net.Ftp
{
    public class FTPHelper
    {
        /// <summary>
        /// ftp upload file
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <param name="ftpUrl">ftp address</param>
        /// <param name="ftpUser">ftp user</param>
        /// <param name="ftpPwd">ftp password</param>
        public static void Upload(string filePath, string ftpUrl, string ftpUser, string ftpPwd)
        {
            FtpWebRequest request;
            request = WebRequest.Create(new Uri(ftpUrl)) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = true;
            request.Credentials = new NetworkCredential(ftpUser, ftpPwd);
            using (var inputStream = File.OpenRead(filePath))
            using (var outputStream = request.GetRequestStream())
            {
                var buffer = new byte[10240];
                int totalReadBytesCount = 0;
                int readBytesCount;
                while ((readBytesCount = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outputStream.Write(buffer, 0, readBytesCount);
                    //totalReadBytesCount += readBytesCount;
                    //var progress = Math.Round(totalReadBytesCount * 100.0 / inputStream.Length, 2);
                    //Console.Write($"\r{progress}%");
                }
            }
        }
        /// <summary>
        /// ftp upload file async
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <param name="ftpUrl">ftp url</param>
        /// <param name="ftpUser">ftp user</param>
        /// <param name="ftpPwd">ftp password</param>
        public static async Task UploadAsync(string filePath, string ftpUrl, string ftpUser, string ftpPwd)
        {
            FtpWebRequest request;
            request = WebRequest.Create(new Uri(ftpUrl)) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = true;
            request.Credentials = new NetworkCredential(ftpUser, ftpPwd);
            var inputStream = File.OpenRead(filePath);
            var outputStream = await request.GetRequestStreamAsync();
            var buffer = new byte[10240];
            int totalReadBytesCount = 0;
            int readBytesCount;
            while ((readBytesCount = await inputStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await outputStream.WriteAsync(buffer, 0, readBytesCount);
                // totalReadBytesCount += readBytesCount;
                //var progress = Math.Round(totalReadBytesCount * 100.0 / inputStream.Length, 2);
                //Console.Write($"\r{progress}%");
            }
        }
        /// <summary>
        /// ftp download file
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <param name="ftpUrl">ftp url</param>
        /// <param name="ftpUser">ftp user</param>
        /// <param name="ftpPwd">ftp password</param>
        public static void Download(string filePath, string ftpUrl, string ftpUser, string ftpPwd)
        {
            FtpWebRequest request;
            request = WebRequest.Create(new Uri(ftpUrl)) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = true;
            request.Credentials = new NetworkCredential(ftpUser, ftpPwd);
            using (Stream ftpStream = request.GetResponse().GetResponseStream())
            using (Stream fileStream = File.Create(filePath))
            {
                byte[] buffer = new byte[10240];
                int read;
                while ((read = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, read);
                    // int position = (int)fileStream.Position;
                    //Console.Write($"\r{Math.Round(position * 100.0 / totalSize, 2)}%");
                }
            }
        }
        /// <summary>
        /// ftp download file async
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <param name="ftpUrl">ftp url</param>
        /// <param name="ftpUser">ftp user</param>
        /// <param name="ftpPwd">ftp password</param>
        public static async Task DownloadAsync(string filePath, string ftpUrl, string ftpUser, string ftpPwd)
        {
            FtpWebRequest request;
            request = WebRequest.Create(new Uri(ftpUrl)) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = true;
            request.Credentials = new NetworkCredential(ftpUser, ftpPwd);
            Stream ftpStream = (await request.GetResponseAsync()).GetResponseStream();
            Stream fileStream = File.Create(filePath);
            byte[] buffer = new byte[10240];
            int read;
            while ((read = await ftpStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, read);
                int position = (int)fileStream.Position;
                //Console.Write($"\r{Math.Round(position * 100.0 / totalSize, 2)}%");
            }
        }
    }
}

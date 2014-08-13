using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Checksums;

namespace FileUpload.Models
{
    public class zipFileApi
    {


        public static void CreateZip(string sourceFilePath, string destinationZipFilePath)
        {
            if (sourceFilePath[sourceFilePath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                sourceFilePath += System.IO.Path.DirectorySeparatorChar;

            ZipOutputStream zipStream = new ZipOutputStream(File.Create(destinationZipFilePath));
            zipStream.SetLevel(6);  // 压缩级别 0-9
            CreateZipFiles(sourceFilePath, zipStream, sourceFilePath);

            zipStream.Finish();
            zipStream.Close();
        }


        private static void CreateZipFiles(string sourceFilePath, ZipOutputStream zipStream, string staticFile)
        {
            Crc32 crc = new Crc32();
            string[] filesArray = Directory.GetFileSystemEntries(sourceFilePath);
            foreach (string file in filesArray)
            {
                if (Directory.Exists(file))                     //如果当前是文件夹，递归
                {
                    CreateZipFiles(file, zipStream, staticFile);
                }

                else                                            //如果是文件，开始压缩
                {
                    FileStream fileStream = File.OpenRead(file);

                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                    string tempFile = file.Substring(staticFile.LastIndexOf("\\") + 1);
                    ZipEntry entry = new ZipEntry(tempFile);

                    entry.DateTime = DateTime.Now;
                    entry.Size = fileStream.Length;
                    fileStream.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    zipStream.PutNextEntry(entry);

                    zipStream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        #region 解压缩包（将压缩包解压到指定目录）
        /// <summary>
        /// 解压缩包（将压缩包解压到指定目录）
        /// </summary>
        /// <param name="zipedFileName">压缩包名称</param>
        /// <param name="unZipDirectory">解压缩目录</param>
        /// <param name="password">密码</param>
        public static void UnZipFiles(string zipedFileName, string unZipDirectory, string password = "")
        {
            using (ZipInputStream zis = new ZipInputStream(File.Open(zipedFileName, FileMode.OpenOrCreate)))
            {
                if (!string.IsNullOrEmpty(password))
                {
                    zis.Password = password;//有加密文件的，可以设置密码解压
                }

                ZipEntry zipEntry;
                while ((zipEntry = zis.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(unZipDirectory);
                    string pathName = Path.GetDirectoryName(zipEntry.Name);
                    string fileName = Path.GetFileName(zipEntry.Name);

                    pathName = pathName.Replace(".", "$");
                    directoryName += "\\" + pathName;

                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        FileStream fs = File.Create(Path.Combine(directoryName, fileName));
                        int size = 2048;
                        byte[] bytes = new byte[2048];
                        while (true)
                        {
                            size = zis.Read(bytes, 0, bytes.Length);
                            if (size > 0)
                            {
                                fs.Write(bytes, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        fs.Close();
                    }
                }
            }
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using GDG.Utils;

namespace GDG.Base
{
	public class FileBuilder
	{
		public static void WriteIntoFile(string text)
		{
			
		}
        public static bool CopyFolder(string sourceFolderPath, string targetFolderPath)
        {
            try
            {
                //如果目标路径不存在,则创建目标路径
                if (!System.IO.Directory.Exists(targetFolderPath))
                {
                    System.IO.Directory.CreateDirectory(targetFolderPath);
                }
                //遍历目录下文件
                string[] files = System.IO.Directory.GetFiles(sourceFolderPath);
                foreach (string file in files)
                {
                    file.GetFileName(out string extension);
                    if(extension.Equals("meta"))
                        continue;

                    string name = System.IO.Path.GetFileName(file);
                    string fullPath = System.IO.Path.Combine(targetFolderPath, name);
                    System.IO.File.Copy(file, fullPath);//复制文件
                }
                //遍历目录下文件夹
                string[] folders = System.IO.Directory.GetDirectories(sourceFolderPath);
                foreach (string folder in folders)
                {
                    string name = System.IO.Path.GetFileName(folder);
                    string fullPath = System.IO.Path.Combine(targetFolderPath, name);
                    CopyFolder(folder, fullPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new CustomErrorException(ex.Message);
            }
        }
	}
}
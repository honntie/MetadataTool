using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConsoleApp1
{
    class PathManager
    {
        SaveFileDialog saveDialog = new SaveFileDialog();    // 导出文件的对话框
        OpenFileDialog openDialog = new OpenFileDialog();    // 打开文件的对话框

        // 文件消息
        private FileInfo fileInfo = null;
        private FileInfo openFileInfo = null;
        public string FilePath => fileInfo.FullName;

        // json消息
        private FileInfo jsonInfo = null;
        public string JsonPath => jsonInfo.FullName;

        public PathManager()
        {
        }

        public bool FindOpenPath(params string[] paths)
        {
            SetFileDisplay(openDialog, "读取global-metadata.dat");

            if (paths.Length > 0) openFileInfo = fileInfo = new FileInfo(paths[0]);
            else 
            {
                if (openDialog.ShowDialog() != DialogResult.OK) return false;
                openFileInfo = fileInfo = new FileInfo(openDialog.FileName);
            }
            return true;
        }

        public bool FindDumpPath()
        {
            SetFileDisplay(saveDialog, "另存为global-metadata.dat");
            if (saveDialog.ShowDialog() != DialogResult.OK) return false;

            if (openFileInfo.FullName == saveDialog.FileName)
            {
                Logger.I("另存为不能覆盖当前加载的文件");
                return false;
            }

            fileInfo = new FileInfo(saveDialog.FileName);
            return true;
        }

        public bool FindJsonSavePath()
        {
            SetJsonDisplay(saveDialog, "导出字符串");
            if (saveDialog.ShowDialog() != DialogResult.OK) return false;
            jsonInfo = new FileInfo(saveDialog.FileName);
            return true;
        }

        public bool FindJsonOpenPath()
        {
            SetJsonDisplay(openDialog, "读取字符串");
            if (openDialog.ShowDialog() != DialogResult.OK) return false;
            jsonInfo = new FileInfo(openDialog.FileName);
            return true;
        }

        private void SetJsonDisplay(FileDialog dialog, string title)
        {
            dialog.Title = title;
            dialog.InitialDirectory = jsonInfo == null ? (fileInfo == null ? Environment.CurrentDirectory : fileInfo.Directory.FullName) : jsonInfo.Directory.FullName;
            dialog.FileName = jsonInfo == null ? "string.json" : jsonInfo.Name;
            dialog.Filter = "string.json|*.json|所有文件|*.*";

        }

        private void SetFileDisplay(FileDialog dialog, string title)
        {
            dialog.Title = title;
            dialog.InitialDirectory = fileInfo == null ? (jsonInfo == null ? Environment.CurrentDirectory : jsonInfo.Directory.FullName) : fileInfo.Directory.FullName;
            dialog.FileName = fileInfo == null ? "global-metadata.dat" : fileInfo.Name;
            dialog.Filter = "global-metadata.dat|*.dat|所有文件|*.*";

        }

    }
}

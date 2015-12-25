using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UltimateFilesHurricaneManagerClassLibrary;

namespace MainForm
{
    public class Folder : AbstractFolder
    {
        public Folder(string path)
        {
            Path = path;
            var currentDirInfo = new DirectoryInfo(Path);
            Name = currentDirInfo.Name;
            DateOfCreation = currentDirInfo.CreationTime.ToShortDateString();
            FilesList = new List<AbstractFile>();
            DirectoriesList = new List<AbstractFolder>();
        }        

        public override void Open()
        {
            var currentDirInfo = new DirectoryInfo(Path);

            foreach (var item in currentDirInfo.GetFiles())
            {
                var newFile = new File(System.IO.Path.Combine(Path, item.Name));
                FilesList.Add(newFile);
            }
            foreach (var item in currentDirInfo.GetDirectories())
            {
                var newDirectory = new Folder(System.IO.Path.Combine(Path, item.Name));
                DirectoriesList.Add(newDirectory);
            }
        }

        public override void Remove()
        {
            Directory.Delete(Path, true);
        }

        public override void Replace(AbstractFolder nodeElement)
        {
            if (nodeElement is Folder)
                Directory.Move(Path, nodeElement.Path);
            else
            {
                foreach (ProgressInfo progressInfo in Copy(nodeElement))
                {

                }
                Remove();
            }
        }

        public override AbstractFile CreateFile(string fileName)
        {
            return new File(System.IO.Path.Combine(Path, fileName));
        }

        public override AbstractFolder CreateFolder(string folderName)
        {
            string newFolderPath = System.IO.Path.Combine(Path, folderName);
            Directory.CreateDirectory(newFolderPath);
            return new Folder(newFolderPath);
        }
    }
}
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UltimateFilesHurricaneManagerClassLibrary;

namespace UltimateFilesHurricaneManagerClassLibrary
{
    public abstract class AbstractFile
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public long Size { get; set; }
        public string DateOfCreation { get; set; }
        public string DateOfChange { get; set; }
        public string DateOfLastAppeal { get; set; }

        public abstract void Copy(AbstractFile file);
        public abstract void Write(byte[] bytesArr);
        public abstract void Replace(AbstractFolder inDirectory);
        public abstract void Remove();
        public abstract void Open();
    }

    public abstract class AbstractFolder
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string DateOfCreation { get; set; }
        public List<AbstractFile> FilesList { get; set; }
        public List<AbstractFolder> DirectoriesList { get; set; }

        public abstract void Open();
        public abstract void Remove();
        public abstract void Replace(AbstractFolder nodeElement);
        public abstract AbstractFile CreateFile(string fileName);
        public abstract AbstractFolder CreateFolder(string folderName);

        public IEnumerable<ProgressInfo> Copy(AbstractFolder nodeElement)
        {
            int count = h_countFiles(this);

            var progressInfo = new ProgressInfo
            {
                All = count
            };

            Directory.CreateDirectory(nodeElement.Path);

            foreach (AbstractFile item in FilesList)
            {
                AbstractFile destination = nodeElement.CreateFile(item.Name);
                item.Copy(destination);
                progressInfo.Current++;

                yield return progressInfo;
            }
            foreach (AbstractFolder item in DirectoriesList)
            {
                AbstractFolder createdFolder = nodeElement.CreateFolder(item.Name);
                foreach (ProgressInfo innerItem in item.Copy(createdFolder))
                {
                    yield return innerItem;
                }
                progressInfo.Current++;

                yield return progressInfo;
            }
        }

        private int h_countFiles(AbstractFolder abstractFolder)
        {
            abstractFolder.Open();

            return abstractFolder.FilesList.Count +
              abstractFolder.DirectoriesList.Sum(innerFolder => h_countFiles(innerFolder));
        }
    }
}

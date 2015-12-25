using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO.Compression;

namespace UltimateFilesHurricaneManagerClassLibrary
{
    public class ArchiveFolder : AbstractFolder
    {
        public string innerPath { get; set; }

        public ArchiveFolder(string path, string innerPath)
        {
            Path = path;
            this.innerPath = innerPath;
            FilesList = new List<AbstractFile>();
            DirectoriesList = new List<AbstractFolder>();

            if (innerPath == @"")
            {
                Name = Path.Substring(0, Path.Length - Path.IndexOf(@"/") - 1);
            }
            else
            {
                Name = innerPath.Substring(innerPath.LastIndexOf(@"/", innerPath.Length - 2) + 1,
                    innerPath.Length - innerPath.LastIndexOf(@"/", innerPath.Length - 2) - 2);
            }
        }
        
        public override void Open()
        {
            DirectoriesList.Clear();
            FilesList.Clear();

            using (var arc = ZipFile.OpenRead(Path))
            {
                List<string> existingFolders = new List<string>();

                foreach (var item in arc.Entries)
                {
                    if (item.FullName.LastIndexOf(@"/") == item.FullName.Length - 1)
                    {
                        if (innerPath == "")
                        {
                            if (!existingFolders.Contains(item.FullName.Substring(0, item.FullName.IndexOf(@"/") + 1)) &&
                                item.FullName.Substring(0, item.FullName.IndexOf(@"/") + 1) != "")
                            {
                                var newFolder = new ArchiveFolder(Path, item.FullName.Substring(0, item.FullName.IndexOf(@"/") + 1));
                                DirectoriesList.Add(newFolder);
                                existingFolders.Add(item.FullName.Substring(0, item.FullName.IndexOf(@"/") + 1));
                            }
                        }
                        else
                        {
                            if (item.FullName.IndexOf(innerPath) >= 0)
                            {
                                var inPath = item.FullName.Replace(innerPath, "");
                                if (!existingFolders.Contains(inPath.Substring(0, inPath.IndexOf(@"/") + 1)) &&
                                    inPath.Substring(0, inPath.IndexOf(@"/") + 1) != "")
                                {
                                    var newFolder = new ArchiveFolder(Path, item.FullName);
                                    DirectoriesList.Add(newFolder);
                                    existingFolders.Add(inPath.Substring(0, inPath.IndexOf(@"/") + 1));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (innerPath == "")
                        {
                            if (item.FullName.IndexOf(@"/") < 0)
                            {                               
                                var newFile = new ArchiveFile(item.FullName, item);
                                FilesList.Add(newFile);
                            }
                        }
                        else
                        {
                            if (item.FullName.IndexOf(innerPath) >= 0)
                            {
                                var inPath = item.FullName.Replace(innerPath, "");
                                if (inPath.IndexOf(@"/") < 0)
                                {
                                    var newFile = new ArchiveFile(item.FullName, item);
                                    FilesList.Add(newFile);
                                }
                            }
                        }
                    }
                }
                                
            }


            foreach (var item in DirectoriesList)
            {
                Console.WriteLine(item.Name);
            }
            Console.WriteLine("----------------------");
            foreach (var item in FilesList)
            {
                Console.WriteLine(item.Path);
            }


        }

        public override void Remove()
        {
            using (var arc = ZipFile.Open(Path, ZipArchiveMode.Update))
            {
                if (innerPath.Equals(""))
                {
                    while (arc.Entries.Count > 0)
                    {
                        arc.Entries.First().Delete();
                    }
                }
                else
                {
                    while (arc.Entries.Count > 0)
                    {
                        if (arc.Entries.Where(o => o.FullName.IndexOf(innerPath) >= 0).Count() > 0)
                        {
                            arc.Entries.Where(o => o.FullName.IndexOf(innerPath) >= 0).First().Delete();
                        }
                    }
                }
            }
        }

        public override void Replace(AbstractFolder inDirectory)
        {
            AbstractFolder abstractFolder = inDirectory.CreateFolder(Name);
            Copy(abstractFolder);
            Remove();
        }

        public override AbstractFile CreateFile(string fileName)
        {
            using (var arc = ZipFile.Open(Path, ZipArchiveMode.Update))
            {
                var newEntry = arc.CreateEntry(innerPath + fileName);
                var newFile = new ArchiveFile(newEntry.FullName, newEntry);
                return newFile;
            }            
        }

        public override AbstractFolder CreateFolder(string folderName)
        {           
            var newFolder = new ArchiveFolder(Path, innerPath + folderName);
            return newFolder;
        }
    }
}

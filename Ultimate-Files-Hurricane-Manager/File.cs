using System.Diagnostics;
using System.IO;
using UltimateFilesHurricaneManagerClassLibrary;

namespace MainForm
{
    public class File : AbstractFile
    {
        public File(string path)
        {
            Path = path;
            var disk = new DriveInfo(Path);
            if (!disk.IsReady)
                return;

            var fileInf = new FileInfo(Path);
            if (!fileInf.Exists) return;

            var currentFileInfo = new FileInfo(Path);
            Name = currentFileInfo.Name;
            Size = currentFileInfo.Length;
            DateOfCreation = System.IO.File.GetCreationTime(Path).ToShortDateString();
            DateOfChange = System.IO.File.GetLastAccessTimeUtc(path).ToShortDateString();
            DateOfLastAppeal = System.IO.File.GetLastAccessTime(path).ToShortDateString();
        }

        public override void Copy(AbstractFile newFile)
        {
            byte[] buffer = new byte[1024 * 1024]; //мегабайтный буфер
            using (FileStream file = System.IO.File.Open(Path, FileMode.Open))
            {
                file.Read(buffer, 0, buffer.Length);
                // добавить обработку, если прочиталось меньше мегабайта
                newFile.Write(buffer);
            }
        }

        public override void Write(byte[] bytesArr)
        {
            using (var fstream = new FileStream(Path, FileMode.OpenOrCreate))
            {
                fstream.Write(bytesArr, 0, bytesArr.Length);
            }
        }

        public override void Replace(AbstractFolder inDirectory)
        {
            if (inDirectory is Folder)
                System.IO.File.Move(Path, System.IO.Path.Combine(inDirectory.Path, Name));
            else
            {
                AbstractFile abstractFile = inDirectory.CreateFile(Name);
                Copy(abstractFile);
                Remove();
            }
        }

        public override void Remove()
        {
            if (!System.IO.File.Exists(Path)) return;
            System.IO.File.Delete(Path);
        }

        public override void Open()
        {
            //Проверка на существование файла
            if (!System.IO.File.Exists(Path)) return;
            //Открываем файл внешней программой
            var p1 = new Process { StartInfo = { FileName = Path } };
            p1.Start();
        }
    }
}
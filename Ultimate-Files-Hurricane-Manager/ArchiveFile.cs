using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using MainForm;


namespace UltimateFilesHurricaneManagerClassLibrary
{
    public class ArchiveFile : AbstractFile
    {
        private readonly ZipArchiveEntry _entry;

        public ArchiveFile(string path, ZipArchiveEntry entry)
        {
            // для работы с entry архив должен быть открыт в ArchiveDirectory в режиме Update
            _entry = entry;
            // path внутри архива
            Path = path;

            Name = "";
            Size = 0;
            DateOfCreation = "";
            DateOfChange = "";
            DateOfLastAppeal = "";
        }

        public override void Copy(AbstractFile file)
        {
            byte[] buffer = new byte[1024 * 1024]; //мегабайтный буфер
            using (Stream aStream = _entry.Open())
            {
                aStream.Read(buffer, 0, buffer.Length);
                file.Write(buffer);
            }
        }

        public override void Write(byte[] bytesArr)
        {
            using (Stream aStream = _entry.Open())
                aStream.Write(bytesArr, 0, bytesArr.Length);
        }

        public override void Replace(AbstractFolder inDirectory)
        {
            // нет упрощенного перемещения для архивных файлов, оставляем только обобщенный код
            AbstractFile abstractFile = inDirectory.CreateFile(Name);
            Copy(abstractFile);
            Remove();
        }

        public override void Remove()
        {
            _entry.Delete();
        }

        public override void Open()
        {
            _entry.ExtractToFile(System.IO.Path.Combine(Result, _entry.FullName));

            var psi = new ProcessStartInfo { FileName = System.IO.Path.Combine(Result + Name) };
            Process.Start(psi);
        }
    }
}
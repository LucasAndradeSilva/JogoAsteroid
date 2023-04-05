using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Helpers
{
    public static class FileHelper
    {
        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {                
                Directory.CreateDirectory(path);
            }
        }

        public static void WriterFile(string path, string fileName,  string content)
        {
            EnsureDirectoryExists(path);            
            File.WriteAllText(Path.Combine(path, fileName), content);
        }

        public static string ReadFile(string path)
        {            
            return File.ReadAllText(path);
        }

        public static void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public static void EnsureFileExist(string path, string filename)
        {
            var folderFile = Path.Combine(path, filename);
            if (!File.Exists(folderFile))            
                File.Create(folderFile);                                                                        
        }

        public static bool FileExist(string path)
        {
            return File.Exists(path);
        }
    }
}

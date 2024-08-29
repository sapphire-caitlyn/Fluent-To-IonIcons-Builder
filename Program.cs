using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;

public static class Program { 

    public static void Main(String[] args) {
        Console.WriteLine("Cloning from git...");
        #region[ Path Handling ]
        string RepoPath = Path.Combine(Directory.GetCurrentDirectory(), "Repo");
        if (Directory.Exists(RepoPath)) {
            DirectoryInfo RepoInfo = new DirectoryInfo(RepoPath) { Attributes = FileAttributes.Normal };
            DeleteFolder(RepoPath);
            RepoInfo.Create();
        } else {
            Directory.CreateDirectory(RepoPath);
        }
        #endregion[ Path Handling ]

        List<ZipArchiveEntry> lstIcons = GetFromGit();
        Console.WriteLine("Cloning assets from git done!");
        Console.WriteLine("Formatting assets to ionicons standards...");

        string OutputPath = Path.Combine(Directory.GetCurrentDirectory(), "Output");
        if (Directory.Exists(OutputPath)) {
            DeleteFolder(OutputPath);
            Directory.CreateDirectory(OutputPath);
        } else { 
            Directory.CreateDirectory(OutputPath);
        }

        foreach (ZipArchiveEntry iconArchive in lstIcons) {
            //Outlined
            if (iconArchive.Name.Contains("regular")) {
                MemoryStream MemStream = new();
                iconArchive.Open().CopyTo(MemStream);
                string File = ConvertMemoryStreamToString(MemStream)
                    .Replace("width=\"24\" height=\"24\"", "")
                    .Replace("fill=\"#212121\"", "fill=\"currentColor\"");

                string Nome = iconArchive.Name.Replace("ic_fluent", "fluent").Replace("_24_regular", "-outline").Replace("_", "-");
                string FilePath = Path.Combine(OutputPath, Nome);
                FileInfo Fi = new FileInfo(FilePath);

                if (Fi.Exists) {
                    Fi.Delete();
                }

                FileStream Fs = new FileStream(FilePath, FileMode.Create);
                Fs.Write(Encoding.UTF8.GetBytes(File));
                Fs.Close();
            } else {  
                MemoryStream MemStream = new();
                iconArchive.Open().CopyTo(MemStream);
                string File = ConvertMemoryStreamToString(MemStream)
                    .Replace("width=\"24\" height=\"24\"", string.Empty)
                    .Replace("fill=\"none\""             , string.Empty)
                    .Replace("fill=\"#212121\""          , string.Empty);
                string Nome = iconArchive.Name.Replace("ic_fluent", "fluent").Replace("_24_filled", string.Empty).Replace("_", "-");
                string FilePath = Path.Combine(OutputPath, Nome);
                FileInfo Fi = new FileInfo(FilePath);

                if (Fi.Exists) {
                    Fi.Delete();
                }

                FileStream   Fs = new FileStream(FilePath, FileMode.Create);
                Fs.Write(Encoding.UTF8.GetBytes(File));
                Fs.Close();
            }
        }

    }

    public static void DeleteFolder(string path) {
        var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

        foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories)) {
            info.Attributes = FileAttributes.Normal;
        }

        directory.Delete(true);
    }

    /// <summary>
    /// Gets assets from the microsof github repo
    /// </summary>
    /// <returns> Assets/Svg (Filled and Outlined) </returns>
    public static List<ZipArchiveEntry> GetFromGit() {
        string RepoUrl = "https://github.com/microsoft/fluentui-system-icons/archive/refs/heads/main.zip";
        HttpClient ReqClient = new HttpClient();
        Task<HttpResponseMessage> Req = ReqClient.GetAsync(new Uri(RepoUrl));
        Req.Wait();
        MemoryStream MemStream = new();
        Req.Result.Content.ReadAsStream().CopyTo(MemStream);
        ZipArchive zip = new ZipArchive(MemStream);
        List<ZipArchiveEntry> lstIcons = zip.Entries.Where(p => p.FullName.StartsWith("fluentui-system-icons-main/assets"))
                                                    .Where(p => p.FullName.EndsWith("24_regular.svg") || 
                                                                p.FullName.EndsWith("24_filled.svg")).ToList();
        return lstIcons;
    }

    static string ConvertMemoryStreamToString(MemoryStream memoryStream) {
        memoryStream.Position = 0;

        // Read the MemoryStream and convert to string
        using (StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8)) {
            return reader.ReadToEnd();
        }
    }
}
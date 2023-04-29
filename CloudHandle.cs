using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;

namespace FileRetrieval
{
    public class CloudHandle
    {
        string API_key = "AIzaSyC3Z_jc67nhEIR5nCtMykWvruLPodzLuKU";

        private static void DownloadFile(DriveService service, string fileId, string saveToPath)
        {
            var request = service.Files.Get(fileId);
            using (var fileStream = new FileStream(saveToPath, FileMode.Create, FileAccess.Write))
            {
                request.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
                {
                    switch (progress.Status)
                    {
                        case Google.Apis.Download.DownloadStatus.Downloading:
                            {
                                Console.WriteLine($"Downloading: {progress.BytesDownloaded}");
                                break;
                            }
                        case Google.Apis.Download.DownloadStatus.Completed:
                            {
                                Console.WriteLine("Download complete.");
                                break;
                            }
                        case Google.Apis.Download.DownloadStatus.Failed:
                            {
                                Console.WriteLine($"Download failed: {progress.Exception}");
                                break;
                            }
                    }
                };
                request.Download(fileStream);
            }
        }
    }
}

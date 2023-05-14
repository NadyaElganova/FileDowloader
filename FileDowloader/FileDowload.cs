using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDowloader
{
    internal class FileDowload: IDisposable
    {
        private readonly HttpClient _client;
        public FileDowload()
        {
            _client = new HttpClient();
        }
        public async Task DowloadFile(string uri, IProgress<double> progress, string path="file.zip")
        {
            ArgumentNullException.ThrowIfNull(uri);
            if (!Uri.TryCreate(uri, UriKind.Absolute, out var uriResult))
            {
                throw new ArgumentException("Invalid uri", nameof(uri));
            }

            using HttpResponseMessage response = await _client.GetAsync(uriResult, 
                HttpCompletionOption.ResponseHeadersRead);
            long? contentLength = response.Content.Headers.ContentLength;
            
            await using Stream contextStream = await response.Content.ReadAsStreamAsync(); 
            byte[] buffer = new byte[8192];
            long totalBytesRead = 0L;
            await using FileStream file = File.OpenWrite(path);

            while(true)
            {
                int countOfbytesRead = await contextStream.ReadAsync(buffer);
                if (countOfbytesRead == 0) 
                {
                    break;
                }
                if (countOfbytesRead == buffer.Length)
                {
                    //запись буфера в файл
                    await file.WriteAsync(buffer);
                }
                else //не все байты буффера были заполнены полезной нагрузкой 
                {
                    var bytesRead = buffer.Take(countOfbytesRead).ToArray();
                    //запись bytesRead в файл
                    await file.WriteAsync(bytesRead);
                }

                totalBytesRead += countOfbytesRead;
                if (contentLength != null && totalBytesRead % 10 == 0)
                {
                    progress.Report((double)totalBytesRead / contentLength.Value);   
                }                
             }

        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}

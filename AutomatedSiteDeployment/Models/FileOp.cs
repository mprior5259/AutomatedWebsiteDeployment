using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedSiteDeployment.Models
{
    internal class FileOp
    {
        public class Request
        {
            public string? Operation { get; set; }
            public string? SourcePath { get; set; }
            public string? DestinationPath { get; set; }
            public string? SearchPattern { get; set; }
            public SearchOption SearchOption { get; set; }
            public FileAttributes Attributes { get; set; }
            public Action<Result>? Callback { get; set; }
        }

        public class Result
        {
            public string? SourcePath { get; set; }
            public bool Exists { get; set; }
            public string[]? Directories { get; set; }
            public string[]? Files { get; set; }
            public long Length { get; set; }
            public DateTime LastWriteTimeUtc { get; set; }
            public FileAttributes Attributes { get; set; }
            public Stream? Stream { get; set; }
            public Exception? Exception { get; set; }
        }
    }
}

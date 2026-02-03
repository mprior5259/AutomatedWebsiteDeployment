using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedSiteDeployment.Models;
using AutomatedSiteDeployment.Helpers;

namespace AutomatedSiteDeployment.Agents
{
    internal class FileSystemAgent
    {
        private readonly string _username;
        private readonly string _password;
        private readonly string _server;
        private readonly Thread tWorker;
        private readonly BlockingCollection<FileOp.Request> OpQueue;

        public FileSystemAgent(string username, string password, string server)
        {
            _username = username;
            _password = password;
            _server = server;
            OpQueue = new BlockingCollection<FileOp.Request>();
            tWorker = new Thread(WorkerLoop);
            tWorker.IsBackground = true;
            tWorker.Start();
        }

        private void EnqueueOperation(FileOp.Request request)
        {
            OpQueue.Add(request);
        }

        private void WorkerLoop()
        {
            var impersonation =
                new ImpersonationHelper.MockImpersonationContext(_server, _username, _password);
            if (!impersonation._isImpersonating)
            {
                throw new System.Security.SecurityException($"Impersonation failed in {_server} path.");
            }
            foreach (var req in OpQueue.GetConsumingEnumerable())
            {
                switch (req.Operation)
                {
                    case "DirectoryExists":
                        {
                            var exists = Directory.Exists(req.SourcePath);
                            req.Callback?.Invoke(new FileOp.Result
                            {
                                Exists = exists,
                                SourcePath = req.SourcePath
                            });
                            break;
                        }
                    case "FileExists":
                        {
                            var exists = System.IO.File.Exists(req.SourcePath);
                            req.Callback?.Invoke(new FileOp.Result
                            {
                                Exists = exists,
                                SourcePath = req.SourcePath
                            });
                            break;
                        }
                    case "DeleteDirectory":
                        {
                            Exception? ex = null;
                            try
                            {
                                Directory.Delete(req.SourcePath, true);
                            }
                            catch (Exception exc)
                            {
                                ex = exc;
                            }
                            req.Callback?.Invoke(new FileOp.Result
                            {
                                SourcePath = req.SourcePath,
                                Exception = ex
                            });
                            break;
                        }
                    case "CreateDirectory":
                        {
                            Exception? ex = null;
                            try
                            {
                                Directory.CreateDirectory(req.SourcePath);
                            }
                            catch (Exception exc)
                            {
                                ex = exc;
                            }
                            req.Callback?.Invoke(new FileOp.Result
                            {
                                SourcePath = req.SourcePath,
                                Exception = ex
                            });
                            break;
                        }
                    case "GetDirectories":
                        {
                            string[] dirs;
                            Exception? ex = null;
                            try
                            {
                                dirs = Directory.GetDirectories(req.SourcePath, req.SearchPattern, req.SearchOption);
                            }
                            catch (Exception exc)
                            {
                                ex = exc;
                                dirs = new string[] { };
                            }
                            req.Callback?.Invoke(new FileOp.Result
                            {
                                Directories = dirs,
                                SourcePath = req.SourcePath,
                                Exception = ex
                            });
                            break;
                        }
                    case "GetDirectoryInfo":
                        {
                            DirectoryInfo? dirInfo = null;
                            Exception? ex = null;
                            try
                            {
                                dirInfo = new DirectoryInfo(req.SourcePath);
                            }
                            catch (Exception exc)
                            {
                                ex = exc;
                            }
                            req.Callback?.Invoke(new FileOp.Result
                            {
                                LastWriteTimeUtc = dirInfo.LastWriteTimeUtc,
                                Attributes = dirInfo.Attributes,
                                Exception = ex
                            });
                            break;
                        }
                    case "GetFiles":
                        {
                            string[] files;
                            Exception? ex = null;
                            try
                            {
                                files = Directory.GetFiles(req.SourcePath, req.SearchPattern, req.SearchOption);
                            }
                            catch (Exception exc)
                            {
                                ex = exc;
                                files = new string[] { };
                            }
                            req.Callback?.Invoke(new FileOp.Result
                            {
                                Files = files,
                                SourcePath = req.SourcePath,
                                Exception = ex
                            });
                            break;
                        }
                    case "GetFileInfo":
                        {
                            FileInfo? fileInfo = null;
                            Exception? ex = null;
                            try
                            {
                                fileInfo = new FileInfo(req.SourcePath);
                            }
                            catch (Exception exc)
                            {
                                ex = exc;
                            }
                            req.Callback?.Invoke(new FileOp.Result
                            {
                                Length = fileInfo.Length,
                                LastWriteTimeUtc = fileInfo.LastWriteTimeUtc,
                                Attributes = fileInfo.Attributes,
                                Exception = ex
                            });
                            break;
                        }
                    case "SetAttributes":
                        {
                            try
                            {
                                System.IO.File.SetAttributes(req.SourcePath, req.Attributes);
                                req.Callback?.Invoke(new FileOp.Result());
                            }
                            catch (Exception ex)
                            {
                                req.Callback?.Invoke(new FileOp.Result { Exception = ex });
                            }
                            break;
                        }
                    case "CopyFile":
                        {
                            try
                            {
                                System.IO.File.Copy(req.SourcePath, req.DestinationPath, true);
                                req.Callback?.Invoke(new FileOp.Result());
                            }
                            catch (Exception ex)
                            {
                                req.Callback?.Invoke(new FileOp.Result { Exception = ex });
                            }
                            break;
                        }
                    case "DeleteFile":
                        {
                            Exception? ex = null;
                            try
                            {
                                System.IO.File.Delete(req.SourcePath);
                            }
                            catch (Exception exc)
                            {
                                ex = exc;
                            }
                            req.Callback?.Invoke(new FileOp.Result
                            {
                                SourcePath = req.SourcePath,
                                Exception = ex
                            });
                            break;
                        }
                    case "OpenRead":
                        {
                            Exception? ex = null;
                            Stream? stream = null;
                            try
                            {
                                stream = new FileStream(req.SourcePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                            }
                            catch (Exception exc)
                            {
                                ex = exc;
                            }
                            req.Callback?.Invoke(new FileOp.Result
                            {
                                Stream = stream,
                                Exception = ex
                            });
                            break;
                        }
                    case "OpenWrite":
                        {
                            Exception? ex = null;
                            Stream? stream = null;
                            try
                            {
                                stream = new FileStream(req.SourcePath, FileMode.Create, FileAccess.Write, FileShare.None);
                            }
                            catch (Exception exc)
                            {
                                ex = exc;
                            }
                            req.Callback?.Invoke(new FileOp.Result
                            {
                                Stream = stream,
                                Exception = ex
                            });
                            break;
                        }
                    default:
                        req.Callback?.Invoke(new FileOp.Result
                        {
                            Exception = new NotImplementedException($"Operation '{req.Operation}' is not implemented.")
                        });
                        break;
                }
            }
            impersonation.Dispose();
        }

        #region Directory Operations

        public bool DirectoryExists(string path)
        {
            bool exists = false;
            Exception? opException = null;
            using (var completed = new ManualResetEvent(false))
            {
                EnqueueOperation(new FileOp.Request
                {
                    Operation = "DirectoryExists",
                    SourcePath = path,
                    Callback = result =>
                    {
                        exists = result.Exists;
                        opException = result.Exception;
                        completed.Set();
                    }
                });
                completed.WaitOne();
            }
            if (opException != null) throw opException;
            return exists;
        }

        public void CreateDirectory(string path)
        {
            Exception? opException = null;
            using (var completed = new ManualResetEvent(false))
            {
                EnqueueOperation(new FileOp.Request
                {
                    Operation = "CreateDirectory",
                    SourcePath = path,
                    Callback = result =>
                    {
                        opException = result.Exception;
                        completed.Set();
                    }
                });
                completed.WaitOne();
            }
            if (opException != null) throw opException;
        }

        public void DeleteDirectory(string path)
        {
            Exception? opException = null;
            using (var deleteCompleted = new ManualResetEvent(false))
            {
                EnqueueOperation(new FileOp.Request
                {
                    SourcePath = path,
                    Operation = "DeleteDirectory",
                    Callback = result =>
                    {
                        opException = result.Exception;
                        deleteCompleted.Set();
                    }
                });
                deleteCompleted.WaitOne();
            }
            if (opException != null) throw opException;
        }

        public string[]? GetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            string[]? directories = null;
            Exception? opException = null;
            using (var completed = new ManualResetEvent(false))
            {
                EnqueueOperation(new FileOp.Request
                {
                    Operation = "GetDirectories",
                    SourcePath = path,
                    SearchPattern = searchPattern,
                    SearchOption = searchOption,
                    Callback = result =>
                    {
                        directories = result.Directories;
                        opException = result.Exception;
                        completed.Set();
                    }
                });
                completed.WaitOne();
            }
            if (opException != null) throw opException;
            return directories;
        }

        public Dictionary<string, object> GetDirectoryInfo(string path)
        {
            var dirInfo = new Dictionary<string, object>();
            Exception? opException = null;
            using (var completed = new ManualResetEvent(false))
            {
                EnqueueOperation(new FileOp.Request
                {
                    Operation = "GetDirectoryInfo",
                    SourcePath = path,
                    Callback = result =>
                    {
                        dirInfo["LastWriteTimeUtc"] = result.LastWriteTimeUtc;
                        dirInfo["Attributes"] = result.Attributes;
                        opException = result.Exception;
                        completed.Set();
                    }
                });
                completed.WaitOne();
            }
            if (opException != null) throw opException;
            return dirInfo;
        }

        #endregion

        #region File Operations

        public string[]? GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            string[]? files = null;
            Exception? opException = null;
            using (var completed = new ManualResetEvent(false))
            {
                EnqueueOperation(new FileOp.Request
                {
                    Operation = "GetFiles",
                    SourcePath = path,
                    SearchPattern = searchPattern,
                    SearchOption = searchOption,
                    Callback = result =>
                    {
                        files = result.Files;
                        opException = result.Exception;
                        completed.Set();
                    }
                });
                completed.WaitOne();
            }
            if (opException != null) throw opException;
            return files;
        }

        public Dictionary<string, object> GetFileInfo(string path)
        {
            var fileInfo = new Dictionary<string, object>();
            Exception? opException = null;
            using (var completed = new ManualResetEvent(false))
            {
                EnqueueOperation(new FileOp.Request
                {
                    Operation = "GetFileInfo",
                    SourcePath = path,
                    Callback = result =>
                    {
                        fileInfo["Length"] = result.Length;
                        fileInfo["LastWriteTimeUtc"] = result.LastWriteTimeUtc;
                        fileInfo["Attributes"] = result.Attributes;
                        opException = result.Exception;
                        completed.Set();
                    }
                });
                completed.WaitOne();
            }
            if (opException != null) throw opException;
            return fileInfo;
        }

        public void SetAttributes(string path, FileAttributes attributes)
        {
            Exception? opException = null;
            using (var completed = new ManualResetEvent(false))
            {
                EnqueueOperation(new FileOp.Request
                {
                    Operation = "SetAttributes",
                    SourcePath = path,
                    Attributes = attributes,
                    Callback = result =>
                    {
                        opException = result.Exception;
                        completed.Set();
                    }
                });
                completed.WaitOne();
            }
            if (opException != null) throw opException;
        }

        public void CopyFile(string source, string dest, bool overwrite)
        {
            Exception? opException = null;
            using (var completed = new ManualResetEvent(false))
            {
                EnqueueOperation(new FileOp.Request
                {
                    Operation = "CopyFile",
                    SourcePath = source,
                    DestinationPath = dest,
                    Callback = result =>
                    {
                        opException = result.Exception;
                        completed.Set();
                    }
                });
                completed.WaitOne();
            }
            if (opException != null) throw opException;
        }

        public bool FileExists(string path)
        {
            bool exists = false;
            Exception? opException = null;
            using (var completed = new ManualResetEvent(false))
            {
                EnqueueOperation(new FileOp.Request
                {
                    Operation = "FileExists",
                    SourcePath = path,
                    Callback = result =>
                    {
                        exists = result.Exists;
                        opException = result.Exception;
                        completed.Set();
                    }
                });
                completed.WaitOne();
            }
            if (opException != null) throw opException;
            return exists;
        }

        public void DeleteFile(string path)
        {
            Exception? opException = null;
            using (var completed = new ManualResetEvent(false))
            {
                EnqueueOperation(new FileOp.Request
                {
                    Operation = "DeleteFile",
                    SourcePath = path,
                    Callback = result =>
                    {
                        opException = result.Exception;
                        completed.Set();
                    }
                });
                completed.WaitOne();
            }
            if (opException != null) throw opException;
        }

        public Stream OpenRead(string path)
        {
            Stream? resultStream = null;
            Exception? opException = null;
            using (var completed = new ManualResetEvent(false))
            {
                EnqueueOperation(new FileOp.Request
                {
                    Operation = "OpenRead",
                    SourcePath = path,
                    Callback = res =>
                    {
                        resultStream = res.Stream;
                        opException = res.Exception;
                        completed.Set();
                    }
                });
                completed.WaitOne();
            }
            if (opException != null) throw opException;
            return resultStream;
        }

        public Stream OpenWrite(string path)
        {
            Stream? resultStream = null;
            Exception? opException = null;
            using (var completed = new ManualResetEvent(false))
            {
                EnqueueOperation(new FileOp.Request
                {
                    Operation = "OpenWrite",
                    SourcePath = path,
                    Callback = res =>
                    {
                        resultStream = res.Stream;
                        opException = res.Exception;
                        completed.Set();
                    }
                });
                completed.WaitOne();
            }
            if (opException != null) throw opException;
            return resultStream;
        }

        #endregion

        public void Stop()
        {
            OpQueue.CompleteAdding();
            tWorker.Join();
        }

    }
}

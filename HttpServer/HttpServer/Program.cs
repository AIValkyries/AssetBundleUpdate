/****************************************************
	文件：Program.cs
	作者：Lonely
	github：https://blog.csdn.net/u014361280 
	日期：2020/07/16 20:53:27
	功能：Nothing
*****************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServer
{
    public enum DownLoadType
    {
        FileManifest = 0,
        ABFile = 1,
        ZipFile = 2
    }

    public enum ePlatformType
    {
        Android,
        IOS,
        Win
    }

    public class DownLoadInfo
    {
        public DownLoadType downLoadType;
        public ePlatformType platformType;
        public string fileName;
        public int version;
        public int DownLoadSize;
        public int BitsPerSecond;   // 每秒多少bit

        public HttpListenerContext context;
        public object LockObj;

        public DownLoadInfo()
        {
            LockObj = new object();
        }

        public void PrintInfo()
        {
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("DownLoadSize:" + DownLoadSize);
            Console.WriteLine("DownLoadSpeed:" + BitsPerSecond);
            Console.WriteLine("DownLoadType:" + downLoadType);
            Console.WriteLine("Version:" + version);
            Console.WriteLine("ePlatformType:" + platformType);
            Console.WriteLine("FileName:" + fileName);
            Console.WriteLine("-----------------------------------------");
        }

        public string GetFileName()
        {
            string[] context = fileName.Split('/');
            return context[context.Length - 1];
        }

        public string GetFullFileName()
        {
            int firstIndex = Environment.CurrentDirectory.IndexOf("HttpServer");
            StringBuilder builder = new StringBuilder();

            string currentPath = Environment.CurrentDirectory.Substring(0, firstIndex);
            builder.Append(currentPath);
            builder.Append("AssetBundleServer/");

            //switch (downLoadType)
            //{
            //    case DownLoadType.VersionFile:
            //        builder.Append("FileManifest/");
            //        break;
            //    case DownLoadType.ABFile:
            //        builder.Append("AllAsset/");
            //        break;
            //    case DownLoadType.ZipFile:
            //        builder.Append("FenBao/");
            //        break;
            //}

            //switch (platformType)
            //{
            //    case ePlatformType.Win:
            //        builder.Append("Win/");
            //        break;
            //    case ePlatformType.Android:
            //        builder.Append("Android/");
            //        break;
            //    case ePlatformType.IOS:
            //        builder.Append("IOS/");
            //        break;
            //}

            //if (downLoadType != DownLoadType.ABFile)
            //    builder.Append(string.Format("version_{0}/", version));

            builder.Append(fileName);
            return builder.ToString().Replace("\\", "/");
        }
      

    }

    class Program
    {
        public static string URL = "http://127.0.0.1:1500/AssetBundleServer/";
        static Dictionary<string, DownLoadInfo> downLoadInfos;

        public static DownLoadInfo GetDownLoadInfo(string fileName)
        {
            if (downLoadInfos == null)
                downLoadInfos = new Dictionary<string, DownLoadInfo>();

            DownLoadInfo info = null ;
            if (downLoadInfos.TryGetValue(fileName, out info)) 
            {
                return info;
            }

            info = new DownLoadInfo();
            downLoadInfos.Add(fileName, info);

            return info;
        }

        static void Main(string[] args)
        {
            HttpListener httpListener = new HttpListener();

            try
            {
                httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                httpListener.Prefixes.Add(URL);
                httpListener.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("服务启动失败...");
            }

            Console.WriteLine("服务器启动成功.......");

            int minThreadNum = 0;
            int maxThreadNum = 0;
            int portThreadNum = 0;

            ServicePointManager.UseNagleAlgorithm = true;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.CheckCertificateRevocationList = true;
            ServicePointManager.DefaultConnectionLimit = 100;

            //ServicePointManager.DefaultConnectionLimit = 100;

            ThreadPool.GetMaxThreads(out maxThreadNum, out portThreadNum);
            ThreadPool.GetMinThreads(out minThreadNum, out portThreadNum);

            Console.WriteLine("最大线程数:{0}", maxThreadNum);
            Console.WriteLine("最小空闲线程数：{0}", minThreadNum);
            Console.WriteLine("最大并发数={0}", ServicePointManager.DefaultConnectionLimit);

            Console.WriteLine("\n\n等待客户连接中。。。。");

            while (true)
            {
                //等待请求连接
                //没有请求则GetContext处于阻塞状态
                HttpListenerContext context = httpListener.GetContext();
                ThreadPool.QueueUserWorkItem(new WaitCallback(TaskProc), context);
            }
        }

        static void TaskProc(object o)
        {
            HttpListenerContext ctx = (HttpListenerContext)o;

            int length = URL.Length;
            string RequestURL = ctx.Request.Url.ToString();
            string fileName = RequestURL.Substring(length);

            string downloadSizeKey = "bytes";
            string downloadSpeedKey = "DownLoadSpeed";
            string downloadTypeKey = "DownLoadType";
            string versionKey = "Version";
            string osTypeKey = "OsType";

            DownLoadInfo downLoadInfo = GetDownLoadInfo(fileName);
            downLoadInfo.fileName = fileName;

            string[] allKeys = ctx.Request.Headers.AllKeys;
            for (int i = 0; i < allKeys.Length; i++)
            {
                string keyValues = ctx.Request.Headers[allKeys[i]];
                string[] context = keyValues.Split('=');
                if (context.Length <= 0)
                    continue;

                if (context[0] == downloadSizeKey)
                {
                    string[] byteCount = context[1].Split('-');
                    downLoadInfo.DownLoadSize = System.Convert.ToInt32(byteCount[0]);
                }
                else if (context[0] == downloadSpeedKey)
                {
                    downLoadInfo.BitsPerSecond = System.Convert.ToInt32(context[1]);
                }
                else if (context[0] == downloadTypeKey)
                {
                    downLoadInfo.downLoadType = (DownLoadType)Enum.Parse(typeof(DownLoadType), context[1]);
                }
                else if (context[0] == versionKey)
                {
                    downLoadInfo.version = System.Convert.ToInt32(context[1]);
                }
                else if (context[0] == osTypeKey)
                {
                    downLoadInfo.platformType = (ePlatformType)Enum.Parse(typeof(ePlatformType), context[1]);
                }
            }

            downLoadInfo.PrintInfo();

            string fullName = downLoadInfo.GetFullFileName();

            if (!File.Exists(fullName))
            {
                Console.WriteLine(fullName);
                Console.WriteLine("你要下载的文件不存在!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                ctx.Response.StatusCode = 101;
                ctx.Response.Close();
                return;
            }

            ctx.Response.StatusCode = 200; //设置返回给客服端http状态代码

            using (FileStream fs = File.Open(fullName, FileMode.Open))
            {
                if (downLoadInfo.downLoadType != DownLoadType.FileManifest)
                {
                    int number = 0;
                    int BitsPerSecond = downLoadInfo.BitsPerSecond;

                    Console.WriteLine("文件大小:" + fs.Length);

                    while (true)
                    {
                        int RemainingLength = (int)fs.Length - downLoadInfo.DownLoadSize;
                        if (RemainingLength <= 0)
                            break;

                        int downLength = RemainingLength < BitsPerSecond ? RemainingLength : BitsPerSecond;

                        byte[] downBytes = new byte[downLength];

                        fs.Seek(downLoadInfo.DownLoadSize, SeekOrigin.Begin);
                        fs.Read(downBytes, 0, downLength);
                        downLoadInfo.DownLoadSize += downLength;
                        number++;

                        ctx.Response.OutputStream.Write(downBytes, 0, downLength);
                        //Console.WriteLine(string.Format("总共输送了:{0} Bit", downLoadInfo.DownLoadSize));

                        Thread.Sleep(100);
                    }
                    ctx.Response.Close();
                    Console.WriteLine("输送完毕!");
                }
                else
                {
                    byte[] downBytes = new byte[fs.Length];
                    fs.Read(downBytes, 0, downBytes.Length);

                    string context = Encoding.Default.GetString(downBytes);
                    Console.WriteLine("传输数据:" + context);

                    ctx.Response.OutputStream.Write(downBytes, 0, downBytes.Length);
                    ctx.Response.Close();
                }
            }

        }

    }
}

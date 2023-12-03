using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MetaDataStringEditor;


/// <summary>
/// 兼容MetaDataFile文件的日志输出
/// </summary>
static class Logger
{
    public static void I(object o) => Console.WriteLine(o);
}

namespace ConsoleApp1
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ConsoleKey Command;    // 输入命令
            MetadataFile metadata;    // 操作MetaData的类
            StringJsonFile jsonFile;
            PathManager pathManager = new PathManager();    // 文件路径管理类

            //try
            {
                if (!pathManager.FindOpenPath(args)) return;

                // 读取文件
                Console.WriteLine("正在加载文件: " + pathManager.FilePath);
                Console.WriteLine();
                metadata = new MetadataFile(pathManager.FilePath);
                jsonFile = new StringJsonFile(metadata.strBytes);



                PrintOption();
                do
                {
                    Command = Console.ReadKey().Key;
                    switch (Command)
                    {
                        // 导出字符串
                        case ConsoleKey.D1:
                            Console.WriteLine("\b导出字符串\n");
                            if (pathManager.FindJsonSavePath())
                            {
                                Console.WriteLine("\n导出到" + pathManager.JsonPath + '\n');
                                jsonFile.WriteString(pathManager.JsonPath);
                            }
                            //Console.WriteLine(pathManager.FindJsonSavePath());
                            PrintOption();
                            break;

                        // 另存为metadata.dat
                        case ConsoleKey.D2:
                            Console.WriteLine("\b另存为global-metadata.dat\n");
                            if (pathManager.FindJsonOpenPath())
                            {
                                Console.WriteLine("\n读取" + pathManager.JsonPath + '\n');
                                jsonFile.ReadString(pathManager.JsonPath);
                            }
                            else Console.WriteLine("\n未选择更新字符串\n");
                            if (pathManager.FindDumpPath()) metadata.WriteToNewFile(pathManager.FilePath);
                            PrintOption();
                            break;

                        // 如果不是可选择
                        default: Console.Write('\b'); break;
                    }
                } while (Command != ConsoleKey.D0);

                Console.WriteLine("退出");
            }
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Source);
            //    Console.WriteLine(e.Message);
            //    Console.WriteLine("按任意键结束程序");
            //    Console.ReadKey();
            //}
        }

        static void PrintOption()
        {
            Console.WriteLine();
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("请选择操作");
            Console.WriteLine("1: 导出字符串");
            Console.WriteLine("2: 另存为global-metadata.dat");
            Console.WriteLine("0: 退出");
            Console.WriteLine();
        }
    }
}
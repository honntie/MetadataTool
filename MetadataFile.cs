using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace MetaDataStringEditor {
    class MetadataFile : IDisposable {
        public BinaryReader reader;    //读取字符串

        private uint stringLiteralOffset;    //字符串文本偏移
        private uint stringLiteralCount;    //字符串文本数量
        private long DataInfoPosition;    //数据位置
        private uint stringLiteralDataOffset;    //字符串文本数据偏移
        private uint stringLiteralDataCount;    //字符串文本数据数量
        private List<StringLiteral> stringLiterals = new List<StringLiteral>();    //存放字符串文本
        public List<byte[]> strBytes = new List<byte[]>();    //存放字符串字节

        public MetadataFile(string fullName) {
            reader = new BinaryReader(File.OpenRead(fullName));    //读取二进制

            // 读取文件
            ReadHeader();

            // 读取字符串
            ReadLiteral();
            ReadStrByte();

            Logger.I("基础读取完成");
        }

        public void ReadHeader() {
            Logger.I("读取头部");
            uint vansity = reader.ReadUInt32();     //ReadUInt32()读取四位16进制数据
            if (vansity != 0xFAB11BAF)  throw new Exception("标志检查不通过");

            int version = reader.ReadInt32();    //读取版本号
            stringLiteralOffset = reader.ReadUInt32();      // 列表区的位置，后面不会改了
            stringLiteralCount = reader.ReadUInt32();       // 列表区的大小，后面不会改了
            DataInfoPosition = reader.BaseStream.Position;  // 记一下当前位置，后面要用
            stringLiteralDataOffset = reader.ReadUInt32();  // 数据区的位置，可能要改
            stringLiteralDataCount = reader.ReadUInt32();   // 数据区的长度，可能要改
        }

        public void ReadLiteral() {
            Logger.I("读取Literal");
            //ProgressBar.SetMax((int)stringLiteralCount / 8);

            reader.BaseStream.Position = stringLiteralOffset;    //二进制流定位字符串位置

            /********** 保存字符串位置和长度 **********/
            for (int i = 0; i < stringLiteralCount / 8; i++) {
                stringLiterals.Add(new StringLiteral {
                    Length = reader.ReadUInt32(),
                    Offset = reader.ReadUInt32()
                });
                //ProgressBar.Report();
            }
        }

        public void ReadStrByte() {
            Logger.I("读取字符串的Bytes");
            //ProgressBar.SetMax(stringLiterals.Count);

            /********** 保存字符串字节 **********/
            for (int i = 0; i < stringLiterals.Count; i++) {
                reader.BaseStream.Position = stringLiteralDataOffset + stringLiterals[i].Offset;

                strBytes.Add(reader.ReadBytes((int)stringLiterals[i].Length));

                //ProgressBar.Report();
            }
        }

        public void WriteToNewFile(string fileName) {
            BinaryWriter writer = new BinaryWriter(File.Create(fileName));    //写入二进制文件

            // 先全部复制过去
            reader.BaseStream.Position = 0;
            reader.BaseStream.CopyTo(writer.BaseStream);

            // 更新Literal
            Logger.I("更新Literal");
            //ProgressBar.SetMax(stringLiterals.Count);
            writer.BaseStream.Position = stringLiteralOffset;
            uint count = 0;
            for (int i = 0; i < stringLiterals.Count; i++) {

                stringLiterals[i].Offset = count;
                stringLiterals[i].Length = (uint)strBytes[i].Length;

                writer.Write(stringLiterals[i].Length);
                writer.Write(stringLiterals[i].Offset);
                count += stringLiterals[i].Length;

                //ProgressBar.Report();
            }

            // 进行一次对齐，不确定是否一定需要，但是Unity是做了，所以还是补上为好
            var tmp = (stringLiteralDataOffset + count) % 4;
            if (tmp != 0) count += 4 - tmp;

            // 检查是否够空间放置
            if (count > stringLiteralDataCount) {
                // 检查数据区后面还有没有别的数据，没有就可以直接延长数据区
                if (stringLiteralDataOffset + stringLiteralDataCount < writer.BaseStream.Length) {
                    // 原有空间不够放，也不能直接延长，所以整体挪到文件尾
                    stringLiteralDataOffset = (uint)writer.BaseStream.Length;
                }
            }
            stringLiteralDataCount = count;

            // 写入string
            Logger.I("更新String");
            //ProgressBar.SetMax(strBytes.Count);
            writer.BaseStream.Position = stringLiteralDataOffset;
            for (int i = 0; i < strBytes.Count; i++) {
                writer.Write(strBytes[i]);
                //ProgressBar.Report();
            }

            // 更新头部
            Logger.I("更新头部");
            writer.BaseStream.Position = DataInfoPosition;
            writer.Write(stringLiteralDataOffset);
            writer.Write(stringLiteralDataCount);

            Logger.I("更新完成");
            writer.Close();
        }
        
        public void Dispose() {
            reader?.Dispose();
        }
        
        public class StringLiteral {
            public uint Length;
            public uint Offset;
        }
    }
}

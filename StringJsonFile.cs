using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace ConsoleApp1
{
    class StringJsonFile
    {
        List<byte[]> strBytes = new List<byte[]>();    //存放字符串字节
        JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        Dictionary<string, string> JsonData
        {
            get
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                string str;

                for (int i = 0; i < strBytes.Count; i++)
                {
                    str = Encoding.UTF8.GetString(strBytes[i]);
                    // 是否纯assic字符串
                    if (!str.Any(c => (uint)c >= 256)) continue;

                    result[i.ToString()] = str;
                }

                return result;
            }
        }

        public StringJsonFile(List<byte[]> origin) 
        { 
            strBytes = origin;
        }

        public void ReadString(string filePath)
        {
            Logger.I("更新字符串");
            foreach (var data in JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(filePath, Encoding.UTF8)))
            {
                strBytes[int.Parse(data.Key)] = Encoding.UTF8.GetBytes(data.Value);
            }

            //foreach (JsonArray array in JsonNode.Parse(File.ReadAllText(filePath, Encoding.UTF8)).AsArray())
            //{
            //    Logger.I(array[0]);
            //    Logger.I(array[1]);
            //    Logger.I(null);
            //    //strBytes[(int)array[0]] = Encoding.UTF8.GetBytes(array[1].ToString());
            //}
            Logger.I("更新完成\n");
        }

        public void WriteString(string filePath)
        {
            Logger.I("开导! ");

            File.WriteAllText(filePath, JsonSerializer.Serialize(JsonData, serializerOptions));

            //string str;
            //string text = "";
            //StreamWriter stream = new StreamWriter(filePath, false, Encoding.UTF8);
            //stream.WriteLine("[");
            //for (int i = 0; i < strBytes.Count; i++)
            //{
            //    str = Encoding.UTF8.GetString(strBytes[i]);
            //    // 是否纯assic字符串
            //    if (!str.Any(c => (uint)c >= 256)) continue;

            //    // 是否第一行字符串, 不是则尾部增加逗号
            //    if (!string.IsNullOrEmpty(text)) stream.WriteLine(",");

            //    // 换行符转义
            //    str = str.Replace("\n", @"\n");
            //    str = str.Replace("\r", @"\r");

            //    // 内容填充
            //    text = "    [";
            //    text += (i.ToString() + ", ").PadRight(12);
            //    text += '\"' + str + '\"';
            //    text += "]";
            //    stream.Write(text);
            //}
            //stream.Write("\n]");
            //stream.Close();

            //text = text.Remove(text.Length - 2) + "\n]";
            //File.WriteAllText(filePath, text, Encoding.UTF8);
            Logger.I("导出来了！");
        }
    }
}

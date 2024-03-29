﻿using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Common.Extensions
{
    /// <summary>
    /// 对象扩展
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 对象转xml格式数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="encoding">编码格式</param>
        /// <param name="omitXmlDeclaration">是否去除xml的声明</param>
        /// <param name="omitNameSpaces">是否忽略命名空间</param>
        /// <param name="indent">是否缩进元素</param>
        /// <param name="indentChars">缩进格式化字符</param>
        /// <returns></returns>
        public static string ToXml(this object obj, Encoding encoding = null, bool omitXmlDeclaration = false, bool omitNameSpaces = true, bool indent = true, string indentChars = null)
        {
            if (obj == null)
                return null;
            if (encoding == null)
                encoding = new UTF8Encoding(false);

            XmlSerializer ser = new XmlSerializer(obj.GetType());

            XmlWriterSettings settings = new XmlWriterSettings();
            //去除xml声明
            settings.OmitXmlDeclaration = omitXmlDeclaration;
            settings.Encoding = encoding;
            settings.Indent = indent;
            if (indent && indentChars != null)
                settings.IndentChars = indentChars;

            using MemoryStream ms = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(ms, settings))
            {
                if (omitNameSpaces)
                {
                    var ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    ser.Serialize(writer, obj, ns);
                }
                else
                {
                    ser.Serialize(writer, obj);
                }
            }
            return encoding.GetString(ms.ToArray());
        }

        static readonly JsonSerializerSettings DefaultJsonSetting = new JsonSerializerSettings();
        
        /// <summary>
        /// 对象转json格式数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, DefaultJsonSetting);
        }

        /// <summary>
        /// 对象转json格式数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, JsonSerializerSettings setting)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, setting);
        }

        /// <summary>
        /// 对象转json格式数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson<T>(this object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, typeof(T), DefaultJsonSetting);
        }

        /// <summary>
        /// 对象转json格式数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static string ToJson<T>(this object obj, JsonSerializerSettings setting)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, typeof(T), setting);
        }
    }
}

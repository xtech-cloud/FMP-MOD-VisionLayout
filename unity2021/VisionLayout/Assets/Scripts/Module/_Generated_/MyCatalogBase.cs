
//*************************************************************************************
//   !!! Generated by the fmp-cli 1.61.0.  DO NOT EDIT!
//*************************************************************************************

using System.Collections.Generic;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 目录类的基类
    /// </summary>
    public class MyCatalogBase
    {
        /// <summary>
        /// 段
        /// </summary>
        public class Section
        {
            /// <summary>
            /// 段的名称
            /// </summary>
            public string name { get; set; } = "";

            /// <summary>
            /// 段的路径，支持/字符分割
            /// </summary>
            public string path { get; set; } = "";

            /// <summary>
            /// 实例
            /// </summary>
            public string[] instanceS { get; set; } = new string[0];

            /// <summary>
            /// 内容列表
            /// </summary>
            /// <remarks>
            /// 支持正则表达式
            /// </remarks>
            public string[] contentS { get; set; } = new string[0];

            /// <summary>
            /// 键值对
            /// </summary>
            public Dictionary<string, string> kvS { get; set; } = new Dictionary<string, string>();
        }

        public Section[] sectionS { get; set; } = new Section[0];
    }
}


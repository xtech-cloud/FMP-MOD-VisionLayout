
using System.Xml.Serialization;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 配置类
    /// </summary>
    public class MyConfig : MyConfigBase
    {
        public class Action
        {
            [XmlAttribute("name")]
            public string name { get; set; }

            [XmlAttribute("disable")]
            public bool disable { get; set; }

            [XmlArray("Properties"), XmlArrayItem("Property")]
            public Parameter[] properties { get; set; }

            public Action()
            {
                name = "";
                disable = true;
                properties = new Parameter[0];
            }
        }


        public class LayerPattern
        {
            [XmlAttribute("name")]
            public string name { get; set; } = "";

            [XmlAttribute("interactable")]
            public bool interactable { get; set; } = false;

            [XmlArray("LayoutActions"), XmlArrayItem("Action")]
            public Action[] layoutActionS { get; set; }

            [XmlArray("InActions"), XmlArrayItem("Action")]
            public Action[] inActionS { get; set; }

            [XmlArray("OutActions"), XmlArrayItem("Action")]
            public Action[] outActionS { get; set; }
        }

        public class Style
        {
            [XmlAttribute("name")]
            public string name { get; set; } = "";

            [XmlArray("LayerPatterns"), XmlArrayItem("LayerPattern")]
            public LayerPattern[] layerPatternS { get; set; } = new LayerPattern[0];
        }


        [XmlArray("Styles"), XmlArrayItem("Style")]
        public Style[] styles { get; set; } = new Style[0];
    }
}


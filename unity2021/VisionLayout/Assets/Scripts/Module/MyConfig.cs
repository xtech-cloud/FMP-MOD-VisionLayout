
using System.Xml.Serialization;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 配置类
    /// </summary>
    public class MyConfig : MyConfigBase
    {
        public class Background
        {
            [XmlAttribute("color")]
            public string color { get; set; }
            [XmlAttribute("image")]
            public string image { get; set; }
            [XmlAttribute("video")]
            public string video { get; set; }
        }

        public class ToolBar
        {
            [XmlAttribute("clickTrigger")]
            public int clickTrigger { get; set; } = 20;
            [XmlAttribute("entryWidth")]
            public int entryWidth { get; set; } = 136;
            [XmlAttribute("logoImage")]
            public string logoImage { get; set; } = "";
            [XmlAttribute("paddingLeft")]
            public int paddingLeft { get; set; } = 37;
            [XmlAttribute("paddingRight")]
            public int paddingRight { get; set; } = 37;
            [XmlAttribute("paddingTop")]
            public int paddingTop { get; set; } = 48;
            [XmlAttribute("paddingBottom")]
            public int paddingBottom { get; set; } = 80;
            [XmlAttribute("spacing")]
            public int spacing { get; set; } = 14;
        }

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

        public class Profile : UiElement
        {
            [XmlAttribute("duration")]
            public int duration { get; set; } = 5;
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
            [XmlArray("Subjects"), XmlArrayItem("Subject")]
            public Subject[] subjects { get; set; } = new Subject[0];
        }

        public class Style
        {
            [XmlAttribute("name")]
            public string name { get; set; } = "";

            [XmlElement("Background")]
            public Background background { get; set; } = new Background();

            [XmlElement("ToolBar")]
            public ToolBar toolBar { get; set; } = new ToolBar();

            [XmlElement("Title")]
            public UiElement title { get; set; } = new UiElement();
            [XmlElement("Profile")]
            public Profile profile { get; set; } = new Profile();

            [XmlArray("LayerPatterns"), XmlArrayItem("LayerPattern")]
            public LayerPattern[] layerPatternS { get; set; } = new LayerPattern[0];
        }


        [XmlArray("Styles"), XmlArrayItem("Style")]
        public Style[] styles { get; set; } = new Style[0];
    }
}


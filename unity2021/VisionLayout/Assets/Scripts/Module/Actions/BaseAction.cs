using XTC.FMP.LIB.MVCS;
using XTC.oelFSM;
using System.Collections.Generic;
using static XTC.FMP.LIB.MVCS.View;
using UnityEngine;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 行为基类
    /// </summary>
    public abstract class BaseAction : Action
    {
        public MyInstance myInstance { get; set; }

        public RuntimeClone runtimeClone { get; set; }

        public ExtendFeatures extendFeatures { get; set; }

    }

    /// <summary>
    /// 动画行为
    /// </summary>
    public abstract class AnimationAction : BaseAction
    {
        /// <summary>
        /// 层的类别
        /// </summary>
        public enum LayerCategory
        {
            /// <summary>
            /// 显示
            /// </summary>
            Display,
            /// <summary>
            /// 消失 
            /// </summary>
            Disappear
        }

        /// <summary>
        /// 行为的标签
        /// </summary>
        public enum ActionCategory
        {
            /// <summary>
            /// 显示
            /// </summary>
            Display,
            /// <summary>
            /// 消失
            /// </summary>
            Disappear,
            /// <summary>
            /// 进入
            /// </summary>
            In,
            /// <summary>
            /// 退出
            /// </summary>
            Out,
        }

        /// <summary>
        /// 层的名称
        /// </summary>
        public string layer { get; set; }

        /// <summary>
        /// 行为的持续时间
        /// </summary>
        public float duration { get; set; }

        /// <summary>
        /// 布局模式
        /// </summary>
        public MyConfig.LayerPattern layerPattern { get; set; }

        /// <summary>
        /// 行为的配置
        /// </summary>
        public MyConfig.Action actionConfig { get; set; }

        /// <summary>
        /// 层是否活动
        /// </summary>
        protected bool active_ { get; set; }

        /// <summary>
        /// 计时器
        /// </summary>
        protected float timer_ { get; set; }

        /// <summary>
        /// 画布宽
        /// </summary>
        protected int canvasWidth_ { get; set; }

        /// <summary>
        /// 画布高
        /// </summary>
        protected int canvasHeight_ { get; set; }

        /// <summary>
        /// 布局的节点
        /// </summary>
        protected List<Cell> layoutCells_ { get; set; }

        /// <summary>
        /// 检测是否活动
        /// </summary>
        /// <param name="_actionName">行为名</param>
        /// <param name="_layerCategory">层的类别</param>
        /// <param name="_actionCategory">行为的类别</param>
        protected void checkActive(string _actionName, LayerCategory _layerCategory, ActionCategory _actionCategory)
        {
            //从状态机全局参数中取出选中的对应层类别的层的名称
            var layerName = getParameter("layer." + _layerCategory.ToString()).AsString;
            //从状态机全局参数中取出选中的对应行为类别的行为的名称
            var actionName = getParameter("action." + _actionCategory.ToString()).AsString;
            active_ = actionName.Equals(_actionName) && layerName.Equals(layer);

            if (!active_)
                finish();
        }



        /// <summary>
        /// 行为进入的基础方法
        /// </summary>
        /// <param name="_actionName">行为名</param>
        /// <param name="_layerCategory">层的类别</param>
        /// <param name="_actionCategory">行为的类别</param>
        protected virtual void baseEnter(string _actionName)
        {
            myInstance.getLogger().Trace(_actionName + ".onEnter");
            canvasWidth_ = getParameter(ParameterDefine.Virtual_Resolution_Width).AsInt;
            canvasHeight_ = getParameter(ParameterDefine.Virtual_Resolution_Height).AsInt;
            resetTimer();
        }

        /// <summary>
        /// 每帧更新的基础方法
        /// </summary>
        protected virtual void baseUpdate()
        {
            updateTimer();
            if (timer_ > duration)
            {
                finish();
            }
        }


        /// <summary>
        /// 行为退出的基础方法
        /// </summary>
        /// <param name="_actionName">行为的名称</param>
        protected virtual void baseExit(string _actionName)
        {
            myInstance.getLogger().Trace(_actionName + ".onExit");
        }

        /// <summary>
        /// 重置计时器
        /// </summary>
        protected void resetTimer()
        {
            timer_ = 0;
            duration = parseFloatFromProperty("duration");
        }

        /// <summary>
        /// 更新计时器
        /// </summary>
        protected void updateTimer()
        {
            timer_ += UnityEngine.Time.deltaTime;
        }

        /// <summary>
        /// 过滤布局层在Layout时构建的节点，结果存储到layoutCells中
        /// </summary>
        /// <remarks>
        /// 一个层可能有多个布局行为，每个布局行为下都有节点列表，此函数只获取指定层中指定布局行为下的节点
        /// </remarks>
        /// <param name="_layoutCatagory">布局行为的类别</param>
        /// <returns>是否成功</returns>
        protected bool filterLayoutCells(LayerCategory _layerCatagory)
        {
            // 从全局参数中获取选中的布局行为的名称
            string layoutAction = getParameter(string.Format("action.{0}", _layerCatagory.ToString())).AsString;
            if (string.IsNullOrEmpty(layoutAction))
            {
                return false;
            }
            // 获取布局行为下的节点
            layoutCells_ = getParameter(string.Format("layer.{0}.{1}.cells", layer, layoutAction)).Get<List<Cell>>();
            if (null == layoutCells_)
            {
                myInstance.getLogger().Error("layoutCells_ is null");
                finish();
                return false;
            }
            return true;
        }


        /// <summary>
        /// 从属性中解析浮点型值
        /// </summary>
        /// <param name="_name">属性名</param>
        /// <returns>浮点型值</returns>
        protected float parseFloatFromProperty(string _name)
        {
            float v = 0f;
            foreach (var property in actionConfig.properties)
            {
                if (property.key.Equals(_name))
                {
                    float.TryParse(property.value, out v);
                }
            }
            return v;
        }

        /// <summary>
        /// 从属性中解析整型值
        /// </summary>
        /// <param name="_name">属性名</param>
        /// <returns>整型值</returns>
        protected int parseIntFromProperty(string _name)
        {
            int v = 0;
            foreach (var property in actionConfig.properties)
            {
                if (property.key.Equals(_name))
                {
                    int.TryParse(property.value, out v);
                }
            }
            return v;
        }

        /// <summary>
        /// 从属性中解析布尔型值
        /// </summary>
        /// <param name="_name">属性名</param>
        /// <returns>布尔型值</returns>
        protected bool parseBoolFromProperty(string _name)
        {
            bool v = false;
            foreach (var property in actionConfig.properties)
            {
                if (property.key.Equals(_name))
                {
                    bool.TryParse(property.value, out v);
                }
            }
            return v;
        }

        /// <summary>
        /// 从属性中解析出字符型值
        /// </summary>
        /// <param name="_name">属性名</param>
        /// <returns>字符型值</returns>
        protected string parseStringFromProperty(string _name)
        {
            string v = "";
            foreach (var property in actionConfig.properties)
            {
                if (property.key.Equals(_name))
                {
                    v = property.value;
                }
            }
            return v;
        }

        /// <summary>
        /// 节点是否在画布矩形内
        /// </summary>
        /// <param name="_cell">节点</param>
        /// <param name="_canvasWidth">画布宽度</param>
        /// <param name="_canvasHeight">画布高度</param>
        /// <returns></returns>
        protected bool isCellInCanvasRect(Cell _cell, int _canvasWidth, int _canvasHeight)
        {
            return (_cell.dynamicX + _cell.width / 2 > -_canvasWidth / 2 &&
                _cell.dynamicX - _cell.width / 2 < _canvasWidth / 2 &&
                _cell.dynamicY + _cell.height / 2 > -_canvasHeight / 2 &&
                _cell.dynamicY - _cell.height / 2 < _canvasHeight / 2);
        }

        /// <summary>
        /// 过滤出画布矩形内的节点
        /// </summary>
        /// <returns>过滤出的节点列表</returns>
        protected List<Cell> filterInCanvasRectCells()
        {
            List<Cell> cells = new List<Cell>();
            foreach (var cell in layoutCells_)
            {
                if (isCellInCanvasRect(cell, canvasWidth_, canvasHeight_))
                    cells.Add(cell);
            }
            return cells;
        }


        /// <summary>
        /// 计算一个点绕开虚拟面板后的Y坐标
        /// </summary>
        /// <param name="_boards">虚拟面板的列表</param>
        /// <param name="_radius">虚拟面板的半径</param>
        /// <param name="_position">目标点位置</param>
        /// <returns>Y坐标</returns>
        protected float roundDummyBoardFitY(Dictionary<string, DummyBoard> _boards, int _radius, UnityEngine.Vector2 _position)
        {
            UnityEngine.Vector2 pos = _position;
            if (null == _boards)
                return pos.y;

            foreach (var board in _boards.Values)
            {
                // 节点中心位置是否在面板圆形区域内
                if (UnityEngine.Vector2.Distance(board.center, _position) <= _radius)
                {
                    //在XY坐标系中连接面板中心和节点中心,得到直角三角形，使用勾股定理求解Y轴投影长度
                    float x = _position.x - board.center.x;
                    float y = UnityEngine.Mathf.Sqrt(_radius * _radius - x * x);
                    // 工作台中心的Y坐标加上Y轴投影长度得到节点的Y坐标
                    if (_position.y > board.center.y)
                        pos.y = board.center.y + y;
                    else
                        pos.y = board.center.y - y;
                    break;
                }
            }
            return pos.y;
        }

        /// <summary>
        /// 计算一个点绕开虚拟面板后的X坐标
        /// </summary>
        /// <param name="_boards">虚拟面板的列表</param>
        /// <param name="_radius">虚拟面板的半径</param>
        /// <param name="_position">目标点</param>
        /// <returns>X坐标</returns>
        protected float roundDummyBoardFitX(Dictionary<string, DummyBoard> _boards, int _radius, UnityEngine.Vector2 _position)
        {
            UnityEngine.Vector2 pos = _position;
            if (null == _boards)
                return pos.x;

            foreach (var board in _boards.Values)
            {
                // 节点中心位置是否在面板圆形区域内
                if (UnityEngine.Vector2.Distance(board.center, _position) <= _radius)
                {
                    //在XY坐标系中连接面板中心和节点中心,得到直角三角形，使用勾股定理求解X轴投影长度
                    float y = _position.y - board.center.y;
                    float x = UnityEngine.Mathf.Sqrt(_radius * _radius - y * y);
                    // 工作台中心的Y坐标加上Y轴投影长度得到节点的Y坐标
                    if (_position.x > board.center.x)
                        pos.x = board.center.x + x;
                    else
                        pos.x = board.center.x - x;
                    break;
                }
            }
            return pos.x;
        }
    }

    /// <summary>
    /// 布局行为
    /// </summary>
    public abstract class LayoutAction : AnimationAction
    {

        /// <summary>
        /// 根据配置文件和资源进行布局
        /// </summary>
        /// <param name="_contentList">内容的列表</param>
        /// <param name="_kvS">catalog的kvS</param>
        public void Layout(List<string> _contentList, Dictionary<string, string> _kvS)
        {
            layout(_contentList, _kvS);
        }

        protected override void baseEnter(string _actionName)
        {
            base.baseEnter(_actionName);
            extendFeatures.toolbar.SwitchLayer(layer);
            extendFeatures.toolbar.SwitchInteractable(true);
            extendFeatures.imageTitle.Display(layer);
            extendFeatures.imageProfile.Display(layer);
        }

        /// <summary>
        /// 每帧更新的基础方法
        /// </summary>
        protected override void baseUpdate()
        {
            updateTimer();
            if (timer_ > duration)
            {
                // 计时器超过行为的持续时间，并且工具栏没有激活时，完成行为
                if (!extendFeatures.toolbar.IsActive)
                    finish();
            }
        }

        protected override void baseExit(string _actionName)
        {
            base.baseExit(_actionName);
            extendFeatures.toolbar.SwitchInteractable(false);
            extendFeatures.imageTitle.Disappear(layer);
            extendFeatures.imageProfile.Disappear(layer);
        }

        /// <summary>
        /// 根据配置文件和资源进行布局
        /// </summary>
        /// <param name="_contentList">内容的列表</param>
        /// <param name="_kvS">catalog的kvS</param>
        protected virtual void layout(List<string> _contentList, Dictionary<string, string> _kvS)
        {
            //在派生类中实现逻辑
        }

        /// <summary>
        /// 在层中创建一个新节点
        /// </summary>
        /// <param name="_layer">层的名称</param>
        /// <param name="_contentUri">内容的短路径</param>
        /// <returns>创建好的新节点</returns>
        protected UnityEngine.GameObject newCell(string _layer, string _contentUri)
        {
            UnityEngine.GameObject cellTemplate;
            if (!runtimeClone.cellTemplateMap.TryGetValue(_layer, out cellTemplate))
                return null;
            var clone = UnityEngine.GameObject.Instantiate(cellTemplate, cellTemplate.transform.parent);
            clone.name = _contentUri;
            var button = clone.GetComponent<UnityEngine.UI.Button>();
            string layer = _layer;
            button.onClick.AddListener(() =>
            {
                var pos = button.GetComponent<RectTransform>().anchoredPosition;
                Dictionary<string, object> variableS = new Dictionary<string, object>();
                variableS["{{uid}}"] = myInstance.uid;
                variableS["{{content_uri}}"] = clone.name;
                variableS["{{dummyboard_uid}}"] = System.Guid.NewGuid().ToString();
                variableS["{{dummyboard_position_x}}"] = pos.x;
                variableS["{{dummyboard_position_y}}"] = pos.y;
                variableS["{{dummyboard_uiSlot}}"] = getFullPathOfTransform(runtimeClone.rootUI.Find("[DummyBoard_Slot]"));
                //extendFeatures.toolbar.ClickHiddenArea(clone);
                foreach (var subject in layerPattern.subjects)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    foreach (var parameter in subject.parameters)
                    {
                        if (parameter.type == "string")
                        {
                            data[parameter.key] = parameter.value;
                        }
                        else if (parameter.type == "int")
                        {
                            data[parameter.key] = int.Parse(parameter.value);
                        }
                        else if (parameter.type == "float")
                        {
                            data[parameter.key] = float.Parse(parameter.value);
                        }
                        else if (parameter.type == "bool")
                        {
                            data[parameter.key] = bool.Parse(parameter.value);
                        }
                        else if (parameter.type == "_")
                        {
                            if (variableS.ContainsKey(parameter.value))
                            {
                                data[parameter.key] = variableS[parameter.value];
                            }
                        }
                    }
                    myInstance.getLogger().Trace("click the cell, position is {0} {1}", pos.x, pos.y);
                    myInstance.getMyEntry().getDummyModel().Publish(subject.message, data);
                }
            });
            button.interactable = false;
            return clone;
        }

        /// <summary>
        /// 在层中创建一个空对象
        /// </summary>
        /// <param name="_layer">层的名称</param>
        /// <returns>创建好的空对象</returns>
        protected UnityEngine.GameObject newBlankObject(string _layer)
        {
            UnityEngine.GameObject cellTemplate;
            if (!runtimeClone.cellTemplateMap.TryGetValue(_layer, out cellTemplate))
                return null;
            var clone = new UnityEngine.GameObject();
            clone.transform.SetParent(cellTemplate.transform.parent);
            var rtClone = clone.GetComponent<UnityEngine.RectTransform>();
            if (null == rtClone)
                rtClone = clone.AddComponent<UnityEngine.RectTransform>();
            rtClone.localPosition = UnityEngine.Vector3.zero;
            rtClone.localScale = UnityEngine.Vector3.one;
            rtClone.localRotation = UnityEngine.Quaternion.identity;
            rtClone.anchoredPosition = UnityEngine.Vector2.zero;
            return clone;
        }
        /// <summary>
        /// 禁用节点的交互
        /// </summary>
        protected void disableInteractable()
        {
            foreach (var cell in layoutCells_)
            {
                cell.target.GetComponent<UnityEngine.UI.Button>().interactable = false;
            }
        }

        /// <summary>
        /// 恢复节点的交互
        /// </summary>
        protected void restoreInteractable()
        {
            foreach (var cell in layoutCells_)
            {
                cell.target.GetComponent<UnityEngine.UI.Button>().interactable = layerPattern.interactable;
            }
        }

        /// <summary>
        /// 恢复节点的可见性
        /// </summary>
        protected void restoreVisible()
        {
            foreach (var cell in layoutCells_)
            {
                cell.target.gameObject.SetActive(cell.pinVisible);
            }
        }

        /// <summary>
        /// 加载内容封面图
        /// </summary>
        /// <param name="_contentUri">内容的短路径</param>
        /// <returns></returns>
        protected UnityEngine.Texture2D loadCellThumb(string _contentUri)
        {
            if (string.IsNullOrEmpty(_contentUri))
                return null;

            object img;
            UnityEngine.Texture2D imgTexture = null;
            if (myInstance.preloadsRepetition.TryGetValue(_contentUri + $"/{myInstance.getConfig().preloader.cell.picture}", out img))
            {
                imgTexture = img as UnityEngine.Texture2D;
            }

            if (null == imgTexture)
                imgTexture = new Texture2D(1,1);

            return imgTexture;
        }

        /// <summary>
        /// 加载内容元数据
        /// </summary>
        /// <param name="_contentUri">内容的短路径</param>
        /// <returns></returns>
        protected ContentMetaSchema loadContentMeta(string _contentUri)
        {
            if (string.IsNullOrEmpty(_contentUri))
                return null;

            object obj;
            ContentMetaSchema meta = null;
            if (myInstance.preloadsRepetition.TryGetValue(_contentUri + "/meta.json", out obj))
            {
                meta = obj as ContentMetaSchema;
            }
            return meta;
        }

        protected string getFullPathOfTransform(Transform _target)
        {
            string fullpath = "/" + _target.gameObject.name;
            Transform parent = _target.parent;
            while (null != parent)
            {
                fullpath = string.Format("/{0}{1}", parent.gameObject.name, fullpath);
                parent = parent.parent;
            }
            return fullpath;
        }
    }

    /// <summary>
    /// 变换行为
    /// </summary>
    public abstract class TransitionAction : AnimationAction
    {
        /// <summary>
        /// 行为过程中产生动画的节点的列表
        /// </summary>
        protected List<Cell> animCells { get; set; }

        protected void showLayer()
        {
            UnityEngine.Transform layerTransform = null;
            if (!runtimeClone.layerMap.TryGetValue(layer, out layerTransform))
            {
                myInstance.getLogger().Error("layer:{0} not found in RuntimeClone.layerMap", layer);
                return;
            }
            layerTransform.gameObject.SetActive(true);
        }

        protected void hideLayer()
        {
            UnityEngine.Transform layerTransform = null;
            if (!runtimeClone.layerMap.TryGetValue(layer, out layerTransform))
            {
                myInstance.getLogger().Error("layer:{0} not found in RuntimeClone.layerMap", layer);
                return;
            }
            layerTransform.gameObject.SetActive(false);
        }
    }
}

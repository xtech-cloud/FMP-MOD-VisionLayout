
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XTC.oelFSM;
using LibMVCS = XTC.FMP.LIB.MVCS;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 封装状态机逻辑的便捷类
    /// </summary>
    public class FastFSM
    {
        public MyInstance myInstance { get; set; }

        public UnityEngine.Vector2 virtualResolution { get; set; }
        public ExtendFeatures extendFeatures { get; private set; } = new ExtendFeatures();

        private Machine machine_ { get; set; }
        private State stateLayout_ { get; set; }
        private State stateTransition_ { get; set; }
        private SelectLayoutAction selectLayoutAction_ { get; set; }
        private SelectTransitionAction selectTransitionAction_ { get; set; }
        private RuntimeClone runtimeClone_ = new RuntimeClone();

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            runtimeClone_.rootUI = myInstance.rootUI.transform;
            // 初始化扩展功能
            initializeExtendFeatures();

            machine_ = new Machine();
            //创建初始状态
            var state_none = machine_.NewState("NONE");
            //设置初始状态为空状态
            machine_.onStartup.state = state_none;
            //状态机运转流程
            // 
            // Switch -> Transition -> Layout
            //   /|\                     |
            //    |______________________|
            // 设置状态机参数
            myInstance.getLogger().Trace("set parameters");
            machine_.SetParameter(ParameterDefine.Layer_Display, Parameter.FromString(""));
            machine_.SetParameter(ParameterDefine.Layer_Disappear, Parameter.FromString(""));
            machine_.SetParameter(ParameterDefine.LayoutAction_Display, Parameter.FromString(""));
            machine_.SetParameter(ParameterDefine.LayoutAction_Disappear, Parameter.FromString(""));
            machine_.SetParameter(ParameterDefine.TransitionAction_In, Parameter.FromString(""));
            machine_.SetParameter(ParameterDefine.TransitionAction_Out, Parameter.FromString(""));
            machine_.SetParameter(ParameterDefine.Virtual_Resolution_Width, Parameter.FromInt((int)virtualResolution.x));
            machine_.SetParameter(ParameterDefine.Virtual_Resolution_Height, Parameter.FromInt((int)virtualResolution.y));
            myInstance.getLogger().Trace("create switch state");
            // 创建阶段转换状态
            var stateSwitch = machine_.NewState("switch");
            // transition必须在layout之后创建
            selectLayoutAction_ = newAction(SelectLayoutAction.NAME, stateSwitch, null, null, null) as SelectLayoutAction;
            selectTransitionAction_ = newAction(SelectTransitionAction.NAME, stateSwitch, null, null, null) as SelectTransitionAction;
            // 创建对应的外部命令
            var cmd = machine_.NewCommand("GotoSwitch");
            cmd.state = stateSwitch;
            myInstance.getLogger().Trace("create transition state");
            // 创建切换状态
            stateTransition_ = machine_.NewState("transition");
            // 创建显示状态
            myInstance.getLogger().Trace("create layout state");
            stateLayout_ = machine_.NewState("layout");
            // 转换状态完成后进入到切换状态
            stateSwitch.onFinish.state = stateTransition_;
            // 切换状态完成后进入到显示状态
            stateTransition_.onFinish.state = stateLayout_;
            // 显示状态完成后进入到转换状态
            stateLayout_.onFinish.state = stateSwitch;

            // 装饰层的函数
            System.Action<string, MyConfig.LayerPattern, List<string>, Dictionary<string, string>> decorateLayer = (_layerName, _layerPattern, _contentList, _kvS) =>
            {
                selectLayoutAction_.layers.Add(_layerName);
                // 实例化布局的行为
                foreach (var action in _layerPattern.layoutActionS)
                {
                    if (action.disable)
                        continue;
                    if (!selectLayoutAction_.actions.ContainsKey(_layerName))
                        selectLayoutAction_.actions[_layerName] = new List<string>();
                    selectLayoutAction_.actions[_layerName].Add(action.name);
                    var theAction = newAction(action.name, stateLayout_, _layerName, _layerPattern, action);
                    (theAction as LayoutAction).Layout(_contentList, _kvS);
                }
                // 实例化入变换的行为
                foreach (var action in _layerPattern.inActionS)
                {
                    if (action.disable)
                        continue;
                    if (!selectTransitionAction_.inActions.ContainsKey(_layerName))
                        selectTransitionAction_.inActions[_layerName] = new List<string>();
                    selectTransitionAction_.inActions[_layerName].Add(action.name);
                    newAction(action.name, stateTransition_, _layerName, _layerPattern, action);
                }
                // 实例化出变换的行为
                foreach (var action in _layerPattern.outActionS)
                {
                    if (action.disable)
                        continue;
                    if (!selectTransitionAction_.outActions.ContainsKey(_layerName))
                        selectTransitionAction_.outActions[_layerName] = new List<string>();
                    selectTransitionAction_.outActions[_layerName].Add(action.name);
                    newAction(action.name, stateTransition_, _layerName, _layerPattern, action);
                }

            };

            myInstance.getLogger().Trace("create layers");
            List<string> preloadContentUriS = new List<string>();
            object objContentUriS;
            if (myInstance.preloadsRepetition.TryGetValue("ContentUriS", out objContentUriS))
            {
                preloadContentUriS = objContentUriS as List<string>;
            }
            // 遍历纲目，创建对应的层
            foreach (var section in myInstance.getCatalog().sectionS)
            {
                List<string> contentList = new List<string>();
                foreach (var contentPattern in section.contentS)
                {
                    foreach (var contentUri in preloadContentUriS)
                    {
                        if (BundleMetaSchema.IsMatch(contentUri, contentPattern))
                            contentList.Add(contentUri);
                    }
                }

                // 查找对应的LayerPattern
                string strLayerPattens = "";
                if (!section.kvS.TryGetValue("LayerPattern", out strLayerPattens))
                {
                    myInstance.getLogger().Error("LayerPattern not found in catalog.kvS");
                    continue;
                }
                MyConfig.LayerPattern layerPattern = null;
                foreach (var s in myInstance.getStyle().layerPatternS)
                {
                    if (strLayerPattens.Equals(s.name))
                        layerPattern = s;
                }
                if (null == layerPattern)
                {
                    myInstance.getLogger().Error("want to create the layer:{0} but none LayerPattern matched {1}", section.name, strLayerPattens);
                    continue;
                }
                // 以path作为层的名字，例如"A/1"，每个层中有多个布局行为，每个布局行为下都有自己的节点列表
                createLayer(section.path, layerPattern, section.name, section);
                decorateLayer(section.path, layerPattern, contentList, section.kvS);
            }

            myInstance.getLogger().Info(machine_.ToTreeString());

            myInstance.getLogger().Trace("initialize FSM finish");
            //运行状态机，执行初始的空状态
            machine_.Run();
            //执行运行命令，进入流程循环
            machine_.InvokeCommand("GotoSwitch");
        }

        public void Release()
        {
        }

        public IEnumerator updateFSM()
        {
            while (null != machine_)
            {
                machine_.Update();
                yield return new UnityEngine.WaitForEndOfFrame();
            }
        }


        private BaseAction newAction(string _name, State _state, string _layerName, MyConfig.LayerPattern _layerPattern, MyConfig.Action _actionConfig)
        {
            System.Action<BaseAction> decorateAnimationAction = (BaseAction _action) =>
            {
                AnimationAction theAction = _action as AnimationAction;
                theAction.layer = _layerName;
                theAction.layerPattern = _layerPattern;
                theAction.actionConfig = _actionConfig;
            };

            BaseAction action;
            switch (_name)
            {
                case SelectLayoutAction.NAME:
                    action = _state.NewAction<SelectLayoutAction>();
                    break;
                case SelectTransitionAction.NAME:
                    action = _state.NewAction<SelectTransitionAction>();
                    break;
                case HorizontalFlowLayoutAction.NAME:
                    action = _state.NewAction<HorizontalFlowLayoutAction>();
                    decorateAnimationAction(action);
                    break;
                case VerticalFlowLayoutAction.NAME:
                    action = _state.NewAction<VerticalFlowLayoutAction>();
                    decorateAnimationAction(action);
                    break;
                case StackedLayoutAction.NAME:
                    action = _state.NewAction<StackedLayoutAction>();
                    decorateAnimationAction(action);
                    break;
                case FenchLayoutAction.NAME:
                    action = _state.NewAction<FenchLayoutAction>();
                    decorateAnimationAction(action);
                    break;
                case FilmLayoutAction.NAME:
                    action = _state.NewAction<FilmLayoutAction>();
                    decorateAnimationAction(action);
                    break;
                case FrameLayoutAction.NAME:
                    action = _state.NewAction<FrameLayoutAction>();
                    decorateAnimationAction(action);
                    break;
                case ZipperLayoutAction.NAME:
                    action = _state.NewAction<ZipperLayoutAction>();
                    decorateAnimationAction(action);
                    break;
                case ScrollLayoutAction.NAME:
                    action = _state.NewAction<ScrollLayoutAction>();
                    decorateAnimationAction(action);
                    break;
                case DummyLayoutAction.NAME:
                    action = _state.NewAction<DummyLayoutAction>();
                    decorateAnimationAction(action);
                    break;
                /*
            case ExhibitLayoutAction.NAME:
                action = _state.NewAction<ExhibitLayoutAction>();
                decorateAnimationAction(action);
                break;
                */
                case EdgeFlyInTransitionAction.NAME:
                    action = _state.NewAction<EdgeFlyInTransitionAction>();
                    decorateAnimationAction(action);
                    break;
                case EdgeFlyOutTransitionAction.NAME:
                    action = _state.NewAction<EdgeFlyOutTransitionAction>();
                    decorateAnimationAction(action);
                    break;
                case FilmInTransitionAction.NAME:
                    action = _state.NewAction<FilmInTransitionAction>();
                    decorateAnimationAction(action);
                    break;
                case FilmOutTransitionAction.NAME:
                    action = _state.NewAction<FilmOutTransitionAction>();
                    decorateAnimationAction(action);
                    break;
                case DragTransitionAction.NAME:
                    action = _state.NewAction<DragTransitionAction>();
                    decorateAnimationAction(action);
                    break;
                case DropTransitionAction.NAME:
                    action = _state.NewAction<DropTransitionAction>();
                    decorateAnimationAction(action);
                    break;
                case BreatheInTransitionAction.NAME:
                    action = _state.NewAction<BreatheInTransitionAction>();
                    decorateAnimationAction(action);
                    break;
                case BreatheOutTransitionAction.NAME:
                    action = _state.NewAction<BreatheOutTransitionAction>();
                    decorateAnimationAction(action);
                    break;
                case FadeInTransitionAction.NAME:
                    action = _state.NewAction<FadeInTransitionAction>();
                    decorateAnimationAction(action);
                    break;
                case FadeOutTransitionAction.NAME:
                    action = _state.NewAction<FadeOutTransitionAction>();
                    decorateAnimationAction(action);
                    break;
                case FrameInTransitionAction.NAME:
                    action = _state.NewAction<FrameInTransitionAction>();
                    decorateAnimationAction(action);
                    break;
                case FrameOutTransitionAction.NAME:
                    action = _state.NewAction<FrameOutTransitionAction>();
                    decorateAnimationAction(action);
                    break;
                case ScrollInTransitionAction.NAME:
                    action = _state.NewAction<ScrollInTransitionAction>();
                    decorateAnimationAction(action);
                    break;
                case ScrollOutTransitionAction.NAME:
                    action = _state.NewAction<ScrollOutTransitionAction>();
                    decorateAnimationAction(action);
                    break;
                case DummyInTransitionAction.NAME:
                    action = _state.NewAction<DummyInTransitionAction>();
                    decorateAnimationAction(action);
                    break;
                case DummyOutTransitionAction.NAME:
                    action = _state.NewAction<DummyOutTransitionAction>();
                    decorateAnimationAction(action);
                    break;
                /*
            case DissolveInTransitionAction.NAME:
                action = _state.NewAction<DissolveInTransitionAction>();
                decorateAnimationAction(action);
                break;
            case DissolveOutTransitionAction.NAME:
                action = _state.NewAction<DissolveOutTransitionAction>();
                decorateAnimationAction(action);
                break;
            case MaskInTransitionAction.NAME:
                action = _state.NewAction<MaskInTransitionAction>();
                decorateAnimationAction(action);
                break;
            case MaskOutTransitionAction.NAME:
                action = _state.NewAction<MaskOutTransitionAction>();
                decorateAnimationAction(action);
                break;
                */
                default:
                    return null;
            }

            action.myInstance = myInstance;
            action.runtimeClone = runtimeClone_;
            action.extendFeatures = extendFeatures;
            return action;
        }

        /// <summary>
        /// 创建层
        /// </summary>
        /// <param name="_layer">层的名称</param>
        /// <param name="_layerPattern">层的模式</param>
        /// <param name="_alias">层的别名</param>
        private void createLayer(string _layer, MyConfig.LayerPattern _layerPattern, string _alias, MyCatalogBase.Section _catalogSection)
        {
            var layerTemplate = runtimeClone_.rootUI.Find("LayerContainer/LayerTemplate");
            // 实例化层
            var cloneLayer = GameObject.Instantiate(layerTemplate.gameObject, layerTemplate.transform.parent);
            cloneLayer.name = _layer;
            runtimeClone_.layerMap[_layer] = cloneLayer.transform;
            var goCellTemplate = cloneLayer.transform.Find("CellContainer/CellTemplate").gameObject;
            goCellTemplate.SetActive(false);
            runtimeClone_.cellTemplateMap[_layer] = goCellTemplate;

            extendFeatures.toolbar.HandleLayerCreated(_layer, _alias);
            extendFeatures.imageTitle.HandleLayerCreated(cloneLayer, _catalogSection);
            extendFeatures.imageProfile.HandleLayerCreated(cloneLayer, _catalogSection);
        }

        private void initializeExtendFeatures()
        {
            extendFeatures.toolbar.BindRootGameObject(myInstance.rootUI.transform.Find("ToolBar").gameObject);
            extendFeatures.toolbar.clickTrigger = myInstance.getStyle().toolBar.clickTrigger;
            extendFeatures.toolbar.OnSwitch = () =>
            {
                //强制切换到Switch状态
                machine_.InvokeCommand("GotoSwitch");
            };

            extendFeatures.imageTitle.myInstance = myInstance;
            extendFeatures.imageProfile.myInstance = myInstance;
            extendFeatures.imageProfile.mono = myInstance.getMono();
        }
    }
}

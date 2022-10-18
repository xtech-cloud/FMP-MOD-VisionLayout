
//*************************************************************************************
//   !!! Generated by the fmp-cli 1.59.0.  DO NOT EDIT!
//*************************************************************************************

using System.Collections.Generic;
using Grpc.Net.Client;
using XTC.FMP.LIB.MVCS;

namespace XTC.FMP.MOD.VisionLayout.LIB.MVCS
{

    /// <summary>
    /// 模块选项
    /// </summary>
    public class Options : UserData
    {
        /// 获取GRPC通道
        /// </summary>
        public GrpcChannel? getChannel()
        {
            return channel_;
        }

        /// <summary>
        /// 设置GRPC通道
        /// </summary>
        /// <param name="_channel">GRPC通道</param>
        public void setChannel(GrpcChannel? _channel)
        {
            channel_ = _channel;
        }

        /// <summary>
        /// 获取权限列表
        /// </summary>
        public Dictionary<string,string> getPermissionS()
        {
            return permissionS_;
        }

        /// <summary>
        /// 设置权限列表
        /// </summary>
        /// <param name="_permissionS">权限列表</param>
        public void setPermissionS(Dictionary<string,string> _permissionS)
        {
            permissionS_ = _permissionS;
        }

        /// <summary>
        /// GRPC通道
        /// </summary>
        private GrpcChannel? channel_;

        /// <summary>
        /// 权限列表
        /// </summary>
        private Dictionary<string, string> permissionS_ = new Dictionary<string, string>();
    }

    /// <summary>
    /// 模块入口基类
    /// </summary>
    public class EntryBase : UserData
    {
        /// <summary>
        /// 模块选项
        /// </summary>
        protected Options? options_;

        protected Dictionary<string, HealthyFacade?> facadeHealthyStaticMap_ = new Dictionary<string, HealthyFacade?>();
        protected Dictionary<string, HealthyModel?> modelHealthyStaticMap_ = new Dictionary<string, HealthyModel?>();
        protected Dictionary<string, HealthyView?> viewHealthyStaticMap_ = new Dictionary<string, HealthyView?>();
        protected Dictionary<string, HealthyController?> controllerHealthyStaticMap_ = new Dictionary<string, HealthyController?>();
        protected Dictionary<string, HealthyService?> serviceHealthyStaticMap_ = new Dictionary<string, HealthyService?>();

        protected Dictionary<string, HealthyFacade?> facadeHealthyDynamicMap_ = new Dictionary<string, HealthyFacade?>();
        protected Dictionary<string, HealthyModel?> modelHealthyDynamicMap_ = new Dictionary<string, HealthyModel?>();
        protected Dictionary<string, HealthyView?> viewHealthyDynamicMap_ = new Dictionary<string, HealthyView?>();
        protected Dictionary<string, HealthyController?> controllerHealthyDynamicMap_ = new Dictionary<string, HealthyController?>();
        protected Dictionary<string, HealthyService?> serviceHealthyDynamicMap_ = new Dictionary<string, HealthyService?>();

        /// <summary>
        /// 获取Healthy的UI装饰层
        /// </summary>
        /// <param name="_gid">直系的组的ID</param>
        /// <returns>UI装饰层</returns>
        public HealthyFacade? getStaticHealthyFacade(string _gid)
        {
            HealthyFacade? facade = null;
            if (!facadeHealthyStaticMap_.TryGetValue(HealthyFacade.NAME + "." + _gid, out facade))
                return null;
            return facade;
        }

        /// <summary>
        /// 获取Healthy的UI装饰层
        /// </summary>
        /// <param name="_gid">直系的组的ID</param>
        /// <returns>UI装饰层</returns>
        public HealthyFacade? getDynamicHealthyFacade(string _gid)
        {
            HealthyFacade? facade = null;
            if (!facadeHealthyDynamicMap_.TryGetValue(HealthyFacade.NAME + "." + _gid, out facade))
                return null;
            return facade;
        }


        /// <summary>
        /// 注入MVCS框架
        /// </summary>
        /// <param name="_framework">MVCS框架</param>
        /// <param name="_options">模块选项</param>
        public void Inject(Framework _framework, Options _options)
        {
            framework_ = _framework;
            options_ = _options;
        }

        /// <summary>
        /// 获取权限列表
        /// </summary>
        public Dictionary<string, string> getPermissionS()
        {
            return options_?.getPermissionS() ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// 静态注册
        /// </summary>
        /// <param name="_gid">直系的组的ID</param>
        /// <param name="_logger">日志</param>
        /// <returns>错误</returns>
        protected Error staticRegister(string _gid, Logger? _logger)
        {
            _logger?.Trace("StaticRegister");

            if (null == framework_)
            {
                return Error.NewNullErr("framework is null");
            }

            // 注册数据层
            var modelHealthy = new HealthyModel(HealthyModel.NAME + "." + _gid, _gid);
            modelHealthyStaticMap_[HealthyModel.NAME + "." + _gid] = modelHealthy;
            framework_.getStaticPipe().RegisterModel(modelHealthy);
            // 注册视图层
            var viewHealthy = new HealthyView(HealthyView.NAME + "." + _gid, _gid);
            viewHealthyStaticMap_[HealthyView.NAME + "." + _gid] = viewHealthy;
            framework_.getStaticPipe().RegisterView(viewHealthy);
            // 注册控制层
            var controllerHealthy = new HealthyController(HealthyController.NAME + "." + _gid, _gid);
            controllerHealthyStaticMap_[HealthyController.NAME + "." + _gid] = controllerHealthy;
            framework_.getStaticPipe().RegisterController(controllerHealthy);
            // 注册服务层
            var serviceHealthy = new HealthyService(HealthyService.NAME + "." + _gid, _gid);
            serviceHealthyStaticMap_[HealthyService.NAME + "." + _gid] = serviceHealthy;
            framework_.getStaticPipe().RegisterService(serviceHealthy);
            serviceHealthy.InjectGrpcChannel(options_?.getChannel());
            // 注册UI装饰层
            var facadeHealthy = new HealthyFacade(HealthyFacade.NAME + "." + _gid, _gid);
            facadeHealthyStaticMap_[HealthyFacade.NAME + "." + _gid] = facadeHealthy;
            var bridgeHealthy = new HealthyViewBridge();
            bridgeHealthy.service = serviceHealthy;
            facadeHealthy.setViewBridge(bridgeHealthy);
            framework_.getStaticPipe().RegisterFacade(facadeHealthy);

            return Error.OK;
        }

        /// <summary>
        /// 动态注册
        /// </summary>
        /// <param name="_gid">直系的组的ID</param>
        /// <param name="_logger">日志</param>
        /// <returns>错误</returns>
        protected Error dynamicRegister(string _gid, Logger _logger)
        {
            _logger.Trace("DynamicRegister");

            if (null == framework_)
            {
                return Error.NewNullErr("framework is null");
            }

            // 注册数据层
            var modelHealthy = new HealthyModel(HealthyModel.NAME + "." + _gid, _gid);
            modelHealthyDynamicMap_[HealthyModel.NAME + "." + _gid] = modelHealthy;
            framework_.getDynamicPipe().PushModel(modelHealthy);
            // 注册视图层
            var viewHealthy = new HealthyView(HealthyView.NAME + "." + _gid, _gid);
            viewHealthyDynamicMap_[HealthyView.NAME + "." + _gid] = viewHealthy;
            framework_.getDynamicPipe().PushView(viewHealthy);
            // 注册控制层
            var controllerHealthy = new HealthyController(HealthyController.NAME + "." + _gid, _gid);
            controllerHealthyDynamicMap_[HealthyController.NAME + "." + _gid] = controllerHealthy;
            framework_.getDynamicPipe().PushController(controllerHealthy);
            // 注册服务层
            var serviceHealthy = new HealthyService(HealthyService.NAME + "." + _gid, _gid);
            serviceHealthyDynamicMap_[HealthyService.NAME + "." + _gid] = serviceHealthy;
            framework_.getDynamicPipe().PushService(serviceHealthy);
            serviceHealthy.InjectGrpcChannel(options_?.getChannel());
            // 注册UI装饰层
            var facadeHealthy = new HealthyFacade(HealthyFacade.NAME + "." + _gid, _gid);
            facadeHealthyDynamicMap_[HealthyFacade.NAME + "." + _gid] = facadeHealthy;
            var bridgeHealthy = new HealthyViewBridge();
            bridgeHealthy.service = serviceHealthy;
            facadeHealthy.setViewBridge(bridgeHealthy);
            framework_.getDynamicPipe().PushFacade(facadeHealthy);

            return Error.OK;
        }

        /// <summary>
        /// 静态注销
        /// </summary>
        /// <param name="_gid">直系的组的ID</param>
        /// <param name="_logger">日志</param>
        /// <returns>错误</returns>
        protected Error staticCancel(string _gid, Logger _logger)
        {
            _logger?.Trace("StaticCancel");

            if (null == framework_)
            {
                return Error.NewNullErr("framework is null");
            }

            // 注销服务层
            HealthyService? serviceHealthy;
            if(serviceHealthyStaticMap_.TryGetValue(HealthyService.NAME + "." + _gid, out serviceHealthy))
            {
                framework_.getStaticPipe().CancelService(serviceHealthy);
                serviceHealthyStaticMap_.Remove(HealthyService.NAME + "." +_gid);
            }
            // 注销控制层
            HealthyController? controllerHealthy;
            if(controllerHealthyStaticMap_.TryGetValue(HealthyController.NAME + "." + _gid, out controllerHealthy))
            {
                framework_.getStaticPipe().CancelController(controllerHealthy);
                controllerHealthyStaticMap_.Remove(HealthyController.NAME + "." +_gid);
            }
            // 注销视图层
            HealthyView? viewHealthy;
            if(viewHealthyStaticMap_.TryGetValue(HealthyView.NAME + "." + _gid, out viewHealthy))
            {
                framework_.getStaticPipe().CancelView(viewHealthy);
                viewHealthyStaticMap_.Remove(HealthyView.NAME + "." +_gid);
            }
            // 注销UI装饰层
            HealthyFacade? facadeHealthy;
            if(facadeHealthyStaticMap_.TryGetValue(HealthyFacade.NAME + "." + _gid, out facadeHealthy))
            {
                framework_.getStaticPipe().CancelFacade(facadeHealthy);
                facadeHealthyStaticMap_.Remove(HealthyFacade.NAME + "." +_gid);
            }
            // 注销数据层
            HealthyModel? modelHealthy;
            if(modelHealthyStaticMap_.TryGetValue(HealthyModel.NAME + "." + _gid, out modelHealthy))
            {
                framework_.getStaticPipe().CancelModel(modelHealthy);
                modelHealthyStaticMap_.Remove(HealthyModel.NAME + "." +_gid);
            }

            return Error.OK;
        }

        /// <summary>
        /// 动态注销
        /// </summary>
        /// <param name="_gid">直系的组的ID</param>
        /// <param name="_logger">日志</param>
        /// <returns>错误</returns>
        protected Error dynamicCancel(string _gid, Logger _logger)
        {
            _logger?.Trace("DynamicCancel");

            if (null == framework_)
            {
                return Error.NewNullErr("framework is null");
            }

            // 注销服务层
            HealthyService? serviceHealthy;
            if(serviceHealthyDynamicMap_.TryGetValue(HealthyService.NAME + "." + _gid, out serviceHealthy))
            {
                framework_.getDynamicPipe().PopService(serviceHealthy);
                serviceHealthyDynamicMap_.Remove(HealthyService.NAME + "." +_gid);
            }
            // 注销控制层
            HealthyController? controllerHealthy;
            if(controllerHealthyDynamicMap_.TryGetValue(HealthyController.NAME + "." + _gid, out controllerHealthy))
            {
                framework_.getDynamicPipe().PopController(controllerHealthy);
                controllerHealthyDynamicMap_.Remove(HealthyController.NAME + "." +_gid);
            }
            // 注销视图层
            HealthyView? viewHealthy;
            if(viewHealthyDynamicMap_.TryGetValue(HealthyView.NAME + "." + _gid, out viewHealthy))
            {
                framework_.getDynamicPipe().PopView(viewHealthy);
                viewHealthyDynamicMap_.Remove(HealthyView.NAME + "." +_gid);
            }
            // 注销UI装饰层
            HealthyFacade? facadeHealthy;
            if(facadeHealthyDynamicMap_.TryGetValue(HealthyFacade.NAME + "." + _gid, out facadeHealthy))
            {
                framework_.getDynamicPipe().PopFacade(facadeHealthy);
                facadeHealthyDynamicMap_.Remove(HealthyFacade.NAME + "." +_gid);
            }
            // 注销数据层
            HealthyModel? modelHealthy;
            if(modelHealthyDynamicMap_.TryGetValue(HealthyModel.NAME + "." + _gid, out modelHealthy))
            {
                framework_.getDynamicPipe().PopModel(modelHealthy);
                modelHealthyDynamicMap_.Remove(HealthyModel.NAME + "." +_gid);
            }

            return Error.OK;
        }

        /// <summary>
        /// MVCS框架
        /// </summary>
        protected Framework? framework_;
    }
}


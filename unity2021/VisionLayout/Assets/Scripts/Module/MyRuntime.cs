
using System.Collections.Generic;
using UnityEngine;
using LibMVCS = XTC.FMP.LIB.MVCS;
using XTC.FMP.MOD.VisionLayout.LIB.MVCS;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace XTC.FMP.MOD.VisionLayout.LIB.Unity
{
    /// <summary>
    /// 运行时类
    /// </summary>
    ///<remarks>
    /// 存储模块运行时创建的对象
    ///</remarks>
    public class MyRuntime : MyRuntimeBase
    {
        private ObjectsPool preloadObjectsPool_;

        private List<string> preloadContentUriS_ = new List<string>();

        public MyRuntime(MonoBehaviour _mono, MyConfig _config, MyCatalog _catalog, Dictionary<string, LibMVCS.Any> _settings, FMP.LIB.MVCS.Logger _logger, MyEntryBase _entry)
            : base(_mono, _config, _catalog, _settings, _logger, _entry)
        {
        }

        public override void Preload(Action<int> _onProgress, Action _onFinish)
        {
            preloadObjectsPool_ = new ObjectsPool("preloads", logger_);
            preloadObjectsPool_.Prepare();

            CounterSequence sequenceBundleMeta = new CounterSequence(0);
            sequenceBundleMeta.OnFinish = () =>
            {
                preloads_["ContentUriS"] = preloadContentUriS_;
                CounterSequence sequenceContentFiles = new CounterSequence(0);
                sequenceContentFiles.OnFinish = _onFinish;
                preloadAllContentFiles(sequenceContentFiles);
            };
            preloadAllBundleMeta(sequenceBundleMeta);
        }

        private void preloadAllBundleMeta(CounterSequence _sequence)
        {
            //key: 包名，value: 内容的短路径列表
            Dictionary<string, List<string>> contentS = new Dictionary<string, List<string>>();

            // 先加载全部的包的meta
            foreach (var section in catalog_.sectionS)
            {
                foreach (var contentUri in section.contentS)
                {
                    string[] strs = contentUri.Split("/");
                    if (strs.Length != 2)
                    {
                        logger_.Error("content {0} is invalid", contentUri);
                        continue;
                    }
                    string bundle = strs[0];
                    if (!contentS.ContainsKey(bundle))
                    {
                        contentS[bundle] = new List<string>();
                        _sequence.Dial();
                    }
                    if (!contentS[bundle].Contains(contentUri))
                        contentS[bundle].Add(contentUri);
                }
            }

            logger_.Info("Ready to preload {0} bundle/meta.json", contentS.Count);
            if (contentS.Count == 0)
            {
                _sequence.OnFinish();
                return;
            }
            foreach (var bundle in contentS.Keys)
            {
                loadBundleMeta(bundle, (_bytes) =>
                {
                    BundleMetaSchema bundleMeta = null;
                    try
                    {
                        bundleMeta = JsonConvert.DeserializeObject<BundleMetaSchema>(Encoding.UTF8.GetString(_bytes));
                    }
                    catch (System.Exception ex)
                    {
                        logger_.Error("deserialize {0}/meta.json failed!", bundle);
                        logger_.Exception(ex);
                    }
                    if (null != bundleMeta)
                    {
                        foreach (var content in bundleMeta.foreign_content_uuidS)
                        {
                            string contentUri = string.Format("{0}/{1}", bundle, content);
                            foreach (var contentPattern in contentS[bundle])
                            {
                                if (BundleMetaSchema.IsMatch(contentUri, contentPattern))
                                {
                                    if (!preloadContentUriS_.Contains(contentUri))
                                        preloadContentUriS_.Add(contentUri);
                                }
                            }
                        }
                    }
                    _sequence.Tick();
                }, () =>
                {
                    _sequence.Tick();
                });
            }
        }

        private void preloadAllContentFiles(CounterSequence _sequence)
        {
            logger_.Info("Ready to preload {0} contents", preloadContentUriS_.Count);
            foreach (var contentUri in preloadContentUriS_)
            {
                //meta.json
                _sequence.Dial();
                //cover.png
                _sequence.Dial();
            }
            foreach (var contentUri in preloadContentUriS_)
            {
                preloadContentCover(contentUri, () =>
                {
                    _sequence.Tick();
                }, () =>
                {
                    _sequence.Tick();
                });

                preloadContentMeta(contentUri, () =>
                {
                    _sequence.Tick();
                }, () =>
                {
                    _sequence.Tick();
                });

            }
        }

        protected void loadBundleMeta(string _bundleName, System.Action<byte[]> _onFinish, Action _onError)
        {
            string assetRootPath = settings_["path.assets"].AsString();
            string dir = Path.Combine(assetRootPath, _bundleName);
            string filefullpath = Path.Combine(dir, "meta.json");
            preloadObjectsPool_.LoadText(filefullpath, null, _onFinish, _onError);
        }

        protected void preloadContentCover(string _contentUri, Action _onFinish, Action _onError)
        {
            string assetRootPath = settings_["path.assets"].AsString();
            string dir = Path.Combine(assetRootPath, _contentUri);
            string filefullpath = Path.Combine(dir, "cover.png");
            preloadObjectsPool_.LoadTexture(filefullpath, null, (_texture) =>
            {
                preloads_[_contentUri + "/cover.png"] = _texture;
                _onFinish();
            }, _onError);
        }

        protected void preloadContentMeta(string _contentUri, Action _onFinish, Action _onError)
        {
            string assetRootPath = settings_["path.assets"].AsString();
            string dir = Path.Combine(assetRootPath, _contentUri);
            string filefullpath = Path.Combine(dir, "meta.json");
            preloadObjectsPool_.LoadText(filefullpath, null, (_bytes) =>
            {
                try
                {
                    var contentMeta = JsonConvert.DeserializeObject<ContentMetaSchema>(Encoding.UTF8.GetString(_bytes));
                    preloads_[_contentUri + "/meta.json"] = contentMeta;
                }
                catch (System.Exception ex)
                {
                    logger_.Error("deserialize {0}/meta.json failed!", _contentUri);
                    logger_.Exception(ex);
                }

                _onFinish();
            }, _onError);
        }
    }
}


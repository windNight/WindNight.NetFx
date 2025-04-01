using System;

namespace WindNight.Core.Attributes.Abstractions
{
    public interface IAttribute
    {

    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ClearResultAttribute : Attribute, IAttribute
    {
        public ClearResultAttribute(bool isclear = true)
        {
            IsClear = isclear;
        }

        public bool IsClear { get; protected set; }
    }




    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class NonAuthAttribute : Attribute, IAttribute
    {
        public NonAuthAttribute(bool noAuth = true)
        {
            NoAuth = noAuth;
        }

        public bool NoAuth { get; protected set; }

    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class DebugApiAttribute : HiddenApiAttribute
    {
        public DebugApiAttribute() : base(true, false)
        {


        }

    }

    /// <summary>
    ///  sysApi 优先级高于 testApi
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class SysApiAttribute : HiddenApiAttribute
    {
        public SysApiAttribute() : base(false, true)
        {
            SysApiLevel = 0;
        }

        public SysApiAttribute(int level = 0) : base(false, true)
        {
            SysApiLevel = level;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"> <see cref="SysApiLevel"/> 默认 0  </param>
        /// <param name="debugApi"> 是否同时是debugApi </param>
        public SysApiAttribute(int level = 0, bool debugApi = false) : base(debugApi, true)
        {
            if (level < 0)
            {
                level = 0;
            }

            SysApiLevel = level;
        }

        /// <summary>  SysApi 等级 默认0 最低级别 不额外控制  </summary>
        public int SysApiLevel { get; protected set; }


    }


    /// <summary>
    ///     用户控制WebApi是否在SwaggerUI中显示的特性，如果打上标签的Controller或者方法 默认将不会显示
    ///     增加 区分 TestApi默认true   SysApi 默认false
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class HiddenApiAttribute : Attribute, IAttribute
    {
        public HiddenApiAttribute()
        {
            DebugApi = true;
            SysApi = false;
        }

        public HiddenApiAttribute(bool debugApi, bool sysApi = false)
        {
            DebugApi = debugApi;
            SysApi = sysApi;
        }

        public bool DebugApi { get; protected set; } = true;

        public bool SysApi { get; protected set; } = false;

    }
}

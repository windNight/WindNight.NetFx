namespace Swashbuckle.AspNetCore.Extensions.Abstractions
{
    public interface ISwaggerConfig
    {
        /// <summary>
        ///     自定义获取是否显示HiddenApi的配置
        /// </summary>
        /// <returns></returns>
        bool GetShowHiddenApiConfig();

        /// <summary>
        ///     自定义获取是否显示TestApi的配置
        /// </summary>
        /// <returns></returns>
        bool GetShowTestApiConfig();

        /// <summary>
        ///     自定义获取是否显示SysApi的配置
        /// </summary>
        /// <returns></returns>
        bool GetShowSysApiConfig();

        /// <summary>
        ///     自定义获取是否显示swagger的配置
        /// </summary>
        /// <returns></returns>
        bool GetHiddenSwaggerConfig();

        /// <summary>
        ///     自定义获取是否显示swagger的Schemas配置
        /// </summary>
        /// <returns></returns>
        bool GetHiddenSchemasConfig();


        bool IsOnline();
    }
}

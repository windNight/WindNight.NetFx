using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using WindNight.Core.Attributes.Abstractions;

namespace Swashbuckle.AspNetCore.Extensions.@internal
{
    internal static partial class AttributeExt
    {
        #region ApiDescription

        public static TAttr GetAttribute<TAttr>(this ApiDescription apiDesc)
            where TAttr : Attribute, IAttribute
        {
            try
            {
                return apiDesc.GetAttributeOnControllerAndAction<TAttr>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static IEnumerable<TAttr> GetAttributes<TAttr>(this ApiDescription apiDesc)
            where TAttr : Attribute, IAttribute
        {
            try
            {
                var attributes = apiDesc.GetAttributesOnControllerAndAction<TAttr>();
                return attributes;
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<TAttr>();
            }
        }

        public static bool HasAttribute<TAttr>(this ApiDescription apiDesc)
            where TAttr : Attribute, IAttribute
        {
            try
            {
                var attributes = apiDesc.GetAttributes<TAttr>();

                return !attributes.IsNullOrEmpty();
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion // end ApiDescription
    }


    internal static partial class AttributeExt
    {
        #region ApiDescription

        public static HiddenApiAttribute GetHiddenApiAttr(this ApiDescription apiDesc)
        {
            return apiDesc.GetAttribute<HiddenApiAttribute>();
        }

        public static DebugApiAttribute GetDebugApiAttr(this ApiDescription apiDesc)
        {
            return apiDesc.GetAttribute<DebugApiAttribute>();
        }

        public static SysApiAttribute GetSysApiAttr(this ApiDescription apiDesc)
        {
            return apiDesc.GetAttribute<SysApiAttribute>();
        }

        #endregion // end ApiDescription
    }

    internal static partial class AttributeExt
    {
        #region ApiDescription

        public static bool HasClearResultAttr(this ApiDescription apiDesc)
        {
            return apiDesc.HasAttribute<ClearResultAttribute>();
        }

        public static bool HasHiddenApiAttr(this ApiDescription apiDesc)
        {
            return apiDesc.HasAttribute<HiddenApiAttribute>();
        }

        public static bool HasDebugApiAttr(this ApiDescription apiDesc)
        {
            return apiDesc.HasAttribute<DebugApiAttribute>();
        }

        public static bool HasSysApiAttr(this ApiDescription apiDesc)
        {
            return apiDesc.HasAttribute<SysApiAttribute>();
        }

        public static bool HasNonAuthAttr(this ApiDescription apiDesc)
        {
            return apiDesc.HasAttribute<NonAuthAttribute>();
        }

        #endregion // end ApiDescription
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using WindNight.Core.Attributes.Abstractions;
using WindNight.Linq.Extensions.Expressions;

namespace WindNight.AspNetCore.Mvc.Extensions
{
    public static partial class AttributeExt
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
                return HardInfo.EmptyList<TAttr>();
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

        #region ActionDescriptor

        public static TAttr GetAttribute<TAttr>(this ActionDescriptor apiDesc)
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

        public static IEnumerable<TAttr> GetAttributes<TAttr>(this ActionDescriptor apiDesc)
            where TAttr : Attribute, IAttribute
        {
            try
            {
                var attributes = apiDesc.GetAttributesOnControllerAndAction<TAttr>();
                return attributes;
            }
            catch (Exception ex)
            {
                return HardInfo.EmptyList<TAttr>();
            }
        }

        public static bool HasAttribute<TAttr>(this ActionDescriptor apiDesc)
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

        #endregion // end ActionDescriptor
    }


    public static partial class AttributeExt
    {
        #region ApiDescription

        public static HiddenApiAttribute GetHiddenApiAttr(this ApiDescription apiDesc)
        {
            return apiDesc.GetAttribute<HiddenApiAttribute>();
        }

        public static ClearResultAttribute GetClearResultAttr(this ApiDescription apiDesc)
        {
            return apiDesc.GetAttribute<ClearResultAttribute>();
        }

        public static DebugApiAttribute GetDebugApiAttr(this ApiDescription apiDesc)
        {
            return apiDesc.GetAttribute<DebugApiAttribute>();
        }

        public static SysApiAttribute GetSysApiAttr(this ApiDescription apiDesc)
        {
            return apiDesc.GetAttribute<SysApiAttribute>();
        }

        public static NonAuthAttribute GetNonAuthAttr(this ApiDescription apiDesc)
        {
            return apiDesc.GetAttribute<NonAuthAttribute>();
        }


        public static IEnumerable<ClearResultAttribute> GetClearResultAttrs(this ApiDescription apiDesc)
        {
            return apiDesc.GetAttributes<ClearResultAttribute>();
        }

        public static IEnumerable<HiddenApiAttribute> GetHiddenApiAttrs(this ApiDescription apiDesc)
        {
            return apiDesc.GetAttributes<HiddenApiAttribute>();
        }

        public static IEnumerable<DebugApiAttribute> GetDebugApiAttrs(this ApiDescription apiDesc)
        {
            return apiDesc.GetAttributes<DebugApiAttribute>();
        }

        public static IEnumerable<SysApiAttribute> GetSysApiAttrs(this ApiDescription apiDesc)
        {
            return apiDesc.GetAttributes<SysApiAttribute>();
        }
        public static IEnumerable<NonAuthAttribute> GetNonAuthAttrs(this ApiDescription apiDesc)
        {
            return apiDesc.GetAttributes<NonAuthAttribute>();
        }

        public static ScheduleJobCanSkipAttribute GetScheduleJobCanSkipAttr(this ApiDescription apiDesc)
        {
            return apiDesc.GetAttribute<ScheduleJobCanSkipAttribute>();
        }

        public static IEnumerable<ScheduleJobCanSkipAttribute> GetScheduleJobCanSkipAttrs(this ApiDescription apiDesc)
        {
            return apiDesc.GetAttributes<ScheduleJobCanSkipAttribute>();
        }


        #endregion // end ApiDescription

        #region ActionDescriptor

        public static ClearResultAttribute GetClearResultAttr(this ActionDescriptor apiDesc)
        {
            return apiDesc.GetAttribute<ClearResultAttribute>();
        }

        public static HiddenApiAttribute GetHiddenApiAttr(this ActionDescriptor apiDesc)
        {
            return apiDesc.GetAttribute<HiddenApiAttribute>();
        }

        public static DebugApiAttribute GetDebugApiAttr(this ActionDescriptor apiDesc)
        {
            return apiDesc.GetAttribute<DebugApiAttribute>();
        }

        public static SysApiAttribute GetSysApiAttr(this ActionDescriptor apiDesc)
        {
            return apiDesc.GetAttribute<SysApiAttribute>();
        }

        public static NonAuthAttribute GetNonAuthAttr(this ActionDescriptor apiDesc)
        {
            return apiDesc.GetAttribute<NonAuthAttribute>();
        }

        public static ScheduleJobCanSkipAttribute GetScheduleJobCanSkipAttr(this ActionDescriptor apiDesc)
        {
            return apiDesc.GetAttribute<ScheduleJobCanSkipAttribute>();
        }


        public static IEnumerable<ClearResultAttribute> GetClearResultAttrs(this ActionDescriptor apiDesc)
        {
            return apiDesc.GetAttributes<ClearResultAttribute>();
        }

        public static IEnumerable<HiddenApiAttribute> GetHiddenApiAttrs(this ActionDescriptor apiDesc)
        {
            return apiDesc.GetAttributes<HiddenApiAttribute>();
        }

        public static IEnumerable<DebugApiAttribute> GetDebugApiAttrs(this ActionDescriptor apiDesc)
        {
            return apiDesc.GetAttributes<DebugApiAttribute>();
        }

        public static IEnumerable<SysApiAttribute> GetSysApiAttrs(this ActionDescriptor apiDesc)
        {
            return apiDesc.GetAttributes<SysApiAttribute>();
        }

        public static IEnumerable<NonAuthAttribute> GetNonAuthAttrs(this ActionDescriptor apiDesc)
        {
            return apiDesc.GetAttributes<NonAuthAttribute>();
        }

        public static IEnumerable<ScheduleJobCanSkipAttribute> GetScheduleJobCanSkipAttrs(this ActionDescriptor apiDesc)
        {
            return apiDesc.GetAttributes<ScheduleJobCanSkipAttribute>();
        }

        #endregion // end ActionDescriptor
    }

    public static partial class AttributeExt
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
        public static bool HasScheduleJobCanSkipAttr(this ApiDescription apiDesc)
        {
            return apiDesc.HasAttribute<ScheduleJobCanSkipAttribute>();
        }



        #endregion // end ApiDescription


        #region ActionDescriptor

        public static bool HasClearResultAttr(this ActionDescriptor apiDesc)
        {
            return apiDesc.HasAttribute<ClearResultAttribute>();
        }

        public static bool HasHiddenApiAttr(this ActionDescriptor apiDesc)
        {
            return apiDesc.HasAttribute<HiddenApiAttribute>();
        }

        public static bool HasDebugApiAttr(this ActionDescriptor apiDesc)
        {
            return apiDesc.HasAttribute<DebugApiAttribute>();
        }

        public static bool HasSysApiAttr(this ActionDescriptor apiDesc)
        {
            return apiDesc.HasAttribute<SysApiAttribute>();
        }

        public static bool HasNonAuthAttr(this ActionDescriptor apiDesc)
        {
            return apiDesc.HasAttribute<NonAuthAttribute>();
        }

        public static bool HasScheduleJobCanSkipAttr(this ActionDescriptor apiDesc)
        {
            return apiDesc.HasAttribute<ScheduleJobCanSkipAttribute>();
        }

        #endregion // end ActionDescriptor


        public static bool NeedNonAuth(this ActionDescriptor apiDesc, bool defaultValue = false)
        {
            return apiDesc.GetAttributes<NonAuthAttribute>().Any(m => m.NoAuth);
        }

        public static bool ScheduleJobCanSkip(this ActionDescriptor apiDesc, bool defaultValue = false)
        {
            return apiDesc.GetAttributes<ScheduleJobCanSkipAttribute>().Any(m => m.CanSkip);
        }

        public static bool NeedNonAuth(this ApiDescription apiDesc, bool defaultValue = false)
        {
            return apiDesc.GetAttributes<NonAuthAttribute>().Any(m => m.NoAuth);
        }

        public static bool ScheduleJobCanSkip(this ApiDescription apiDesc, bool defaultValue = false)
        {
            return apiDesc.GetAttributes<ScheduleJobCanSkipAttribute>().Any(m => m.CanSkip);
        }



    }


}

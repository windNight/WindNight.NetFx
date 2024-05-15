using System;
using System.Collections.Generic;
using System.Text;
#if NET45
using System.Collections;

#else
#endif

namespace WindNight.Core.Abstractions
{
    public interface ICurrentContext
    {

        string SerialNumber { get; }

#if NET45
        IDictionary
#else

        IDictionary<object, object>
#endif
        CurrentItems
        { get; }


        T GetItem<T>(string key);

        void AddItem(string key, object value);
        void AddItemIfNotExits(string key, object value);


#if !NET45
        T GetItemsFromAsyncLocal<T>(string key, T defaultValue = default);
       
        T SetItems2AsyncLocal<T>(string key, T setValue = default, bool isForce = false);


#endif



    }
}

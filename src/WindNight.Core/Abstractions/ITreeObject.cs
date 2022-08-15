using System;
using System.Collections.Generic;
using System.Text;

namespace WindNight.Core.Abstractions
{
    public interface ITreeObject
    {
        int Id { get; set; }
        int ParentId { get; set; }
    }

    public interface ITreeObject<T> : ITreeObject
    where T : ITreeObject, ITreeObject<T>, new()
    {
        List<T> Children { get; set; }

    }
}

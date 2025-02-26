namespace WindNight.Core.Abstractions
{
    public interface IKV<T>
    {
        T Key { get; set; }
        string Value { get; set; }
    }

    public interface IKV : IKV<int>
    {
    }

    public interface IKVs : IKV<string>
    {
    }


    public class KV<T> : IKV<T>
    {
        public T Key { get; set; }
        public string Value { get; set; }
    }

    public class KV : KV<int>, IKV
    {
    }

    public class KVs : KV<string>, IKVs
    {
    }
}

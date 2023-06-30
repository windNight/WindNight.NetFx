#if !NET45
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace WindNight.Core.NetCore.Internal
{
    /// <summary>
    ///Represents a JSON file as an <see cref="IConfigurationSource"/>.
    /// </summary>
    public class JsonObjectConfigurationSource : ObjectConfigurationSource
    {
        public JsonObjectConfigurationSource() { }

        /// <summary>
        /// Builds the <see cref="JsonObjectConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>An <see cref="JsonObjectConfigurationProvider"/></returns>
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
            => new JsonObjectConfigurationProvider(this);

    }

    /// <summary>
    /// Json Object based configuration provider
    /// </summary>
    public abstract class ObjectConfigurationProvider : ConfigurationProvider
    {
        /// <summary>
        /// The source settings for this provider.
        /// </summary>
        public ObjectConfigurationSource Source { get; }

        private bool _loaded;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="source">The source.</param>
        public ObjectConfigurationProvider(ObjectConfigurationSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        /// <summary>
        /// Load the configuration data from the json object.
        /// </summary>
        /// <param name="obj">The json data object.</param>
        public abstract void Load(object obj);

        /// <summary>
        /// Load the configuration data from the json object. Will throw after the first call.
        /// </summary>
        public override void Load()
        {
            if (_loaded)
            {
                throw new InvalidOperationException("ObjectConfigurationProvider cannot be loaded more than once.");
            }
            Load(Source.Object);
            _loaded = true;
        }
    }

    /// <summary>
    /// Loads configuration key/values from a json object into a provider.
    /// </summary>
    public class JsonObjectConfigurationProvider : ObjectConfigurationProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="source">The <see cref="JsonObjectConfigurationSource"/>.</param>
        public JsonObjectConfigurationProvider(JsonObjectConfigurationSource source) : base(source) { }

        /// <summary>
        /// Loads json configuration key/values from a json object into a provider.
        /// </summary>
        /// <param name="stream">The json <see cref="object"/> to load configuration data from.</param>
        public override void Load(object stream)
        {
            Data = JsonConfigurationFileParser.Parse(stream);
        }
    }

    /// <summary>
    /// Json Object based <see cref="IConfigurationSource" />.
    /// </summary>
    public abstract class ObjectConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// The json object containing the configuration data.
        /// </summary>
        public object Object { get; set; }

        /// <summary>
        ///  Json Object based<see cref= "IConfigurationSource" />.
        /// </summary> 
        public abstract IConfigurationProvider Build(IConfigurationBuilder builder);

    }



    internal class JsonConfigurationFileParser
    {
        private JsonConfigurationFileParser() { }

        private readonly IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<string> _context = new Stack<string>();
        private string _currentPath;

        public static IDictionary<string, string> Parse(Stream input)
            => new JsonConfigurationFileParser().ParseStream(input);

        public static IDictionary<string, string> Parse(object input)
            => new JsonConfigurationFileParser().ParseObject(input);

        private IDictionary<string, string> ParseObject(object input)
        {
            var streamConfigStream = new MemoryStream(input.ToJsonStr().ToBytes());
            return ParseStream(streamConfigStream);
        }


        private IDictionary<string, string> ParseStream(Stream input)
        {
            _data.Clear();

            var jsonDocumentOptions = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };

            using (var reader = new StreamReader(input))
            using (JsonDocument doc = JsonDocument.Parse(reader.ReadToEnd(), jsonDocumentOptions))
            {
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new FormatException($"{doc.RootElement.ValueKind} UnsupportedJSONToken");
                }
                VisitElement(doc.RootElement);
            }

            return _data;
        }

        private void VisitElement(JsonElement element)
        {
            foreach (var property in element.EnumerateObject())
            {
                EnterContext(property.Name);
                VisitValue(property.Value);
                ExitContext();
            }
        }

        private void VisitValue(JsonElement value)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.Object:
                    VisitElement(value);
                    break;

                case JsonValueKind.Array:
                    var index = 0;
                    foreach (var arrayElement in value.EnumerateArray())
                    {
                        EnterContext(index.ToString());
                        VisitValue(arrayElement);
                        ExitContext();
                        index++;
                    }
                    break;

                case JsonValueKind.Number:
                case JsonValueKind.String:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    var key = _currentPath;
                    if (_data.ContainsKey(key))
                    {
                        throw new FormatException($"key({key}) KeyIsDuplicated");
                    }
                    _data[key] = value.ToString();
                    break;

                default:
                    throw new FormatException($"{value.ValueKind} UnsupportedJSONToken");
            }
        }

        private void EnterContext(string context)
        {
            _context.Push(context);
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }

        private void ExitContext()
        {
            _context.Pop();
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }
    }



}
#endif
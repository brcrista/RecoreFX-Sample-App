using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using Recore;

namespace FileSync.Common
{
    [JsonConverter(typeof(FilepathJsonConverter))]
    public sealed class Filepath : Of<string>
    {
        public Filepath(string value) => Value = value;
    }

    internal class FilepathJsonConverter : JsonConverter<Filepath>
    {
        public override Filepath Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return new Filepath(value);
        }

        public override void Write(Utf8JsonWriter writer, Filepath value, JsonSerializerOptions options)
           => writer.WriteStringValue(value.Value);
    }
}

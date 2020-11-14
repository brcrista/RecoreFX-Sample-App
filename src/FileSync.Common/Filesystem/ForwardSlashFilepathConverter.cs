using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FileSync.Common.Filesystem
{
    internal sealed class ForwardSlashFilepathConverter : JsonConverter<ForwardSlashFilepath>
    {
        public override ForwardSlashFilepath Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string path = reader.GetString() ?? throw new JsonException();
            return new ForwardSlashFilepath(path);
        }

        public override void Write(Utf8JsonWriter writer, ForwardSlashFilepath value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}

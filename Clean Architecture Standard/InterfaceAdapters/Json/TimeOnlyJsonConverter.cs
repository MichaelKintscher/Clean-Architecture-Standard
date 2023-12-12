using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace CleanArchitectureStandard.InterfaceAdapters.Json
{
    public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
    {
        private readonly string format = "HH:mm:ss";
        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return TimeOnly.ParseExact(reader.GetString(), this.format, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(this.format, CultureInfo.InvariantCulture));
        }
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;
using GolemCore.Models.Triggers;

namespace GolemCore.Models.Effects;

public class EffectConverter : JsonConverter<Effect>
{
    // https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to?pivots=dotnet-6-0#support-polymorphic-deserialization
    // https://stackoverflow.com/questions/58074304/is-polymorphic-deserialization-possible-in-system-text-json/59744873#59744873

    private enum TypeDiscriminator
    {
        StatChangeEffect = 1,
    }
    
    public override bool CanConvert(Type typeToConvert) => typeof(Effect).IsAssignableFrom(typeToConvert);
    
    public override Effect? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

        if (!reader.Read())
        {
            throw new JsonException();
        }

        if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
        if (reader.GetString() != "TypeDiscriminator") throw new JsonException();

        if (!reader.Read() || reader.TokenType != JsonTokenType.Number) throw new JsonException();

        Effect? trigger;
        var typeDiscriminator = (TypeDiscriminator)reader.GetInt32();
        switch (typeDiscriminator)
        {
            case TypeDiscriminator.StatChangeEffect:
                if (!reader.Read() || reader.GetString() != "TypeValue") throw new JsonException();
                if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)throw new JsonException();
                trigger = (StatChangeEffect)JsonSerializer.Deserialize(ref reader, typeof(StatChangeEffect))!;
                break;
            default:
                throw new NotSupportedException();
        }

        if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
        {
            throw new JsonException();
        }

        return trigger ?? throw new InvalidOperationException("Effect shouldn't be null when deserializing");
    }

    public override void Write(Utf8JsonWriter writer, Effect effect, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (effect is StatChangeEffect statChangeEffect)
        {
            writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.StatChangeEffect);
            writer.WritePropertyName("TypeValue");
            JsonSerializer.Serialize(writer, statChangeEffect);
        }

        writer.WriteEndObject();
    }
}
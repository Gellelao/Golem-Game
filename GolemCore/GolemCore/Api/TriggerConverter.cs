using System.Text.Json;
using System.Text.Json.Serialization;
using GolemCore.Models.Triggers;

namespace GolemCore.Api;

public class TriggerConverter : JsonConverter<Trigger>
{
    // https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to?pivots=dotnet-6-0#support-polymorphic-deserialization
    // https://stackoverflow.com/questions/58074304/is-polymorphic-deserialization-possible-in-system-text-json/59744873#59744873

    private enum TypeDiscriminator
    {
        TurnTrigger = 1,
    }
    
    public override bool CanConvert(Type typeToConvert) => typeof(Trigger).IsAssignableFrom(typeToConvert);
    
    public override Trigger? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

        if (!reader.Read())
        {
            throw new JsonException();
        }

        if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
        if (reader.GetString() != "TypeDiscriminator") throw new JsonException();

        if (!reader.Read() || reader.TokenType != JsonTokenType.Number) throw new JsonException();

        Trigger? trigger;
        var typeDiscriminator = (TypeDiscriminator)reader.GetInt32();
        switch (typeDiscriminator)
        {
            case TypeDiscriminator.TurnTrigger:
                if (!reader.Read() || reader.GetString() != "TypeValue") throw new JsonException();
                if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)throw new JsonException();
                trigger = (TurnTrigger)JsonSerializer.Deserialize(ref reader, typeof(TurnTrigger))!;
                break;
            default:
                throw new NotSupportedException();
        }

        if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
        {
            throw new JsonException();
        }

        return trigger ?? throw new InvalidOperationException("Trigger shouldn't be null when deserializing");
    }

    public override void Write(Utf8JsonWriter writer, Trigger trigger, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (trigger is TurnTrigger turnTrigger)
        {
            writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.TurnTrigger);
            writer.WritePropertyName("TypeValue");
            JsonSerializer.Serialize(writer, turnTrigger);
        }

        writer.WriteEndObject();
    }
}
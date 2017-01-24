namespace Config.Serializers
{
    public interface IConfigDeserializer
    {
        SerializedConfig Deserialize (byte[] data);
    }
}

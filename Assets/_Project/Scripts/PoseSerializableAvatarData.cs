using System;
using System.IO;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class PoseSerializableAvatarData : INetworkSerializable
{
    public AvatarPacketData PacketData;
    public Vector3 position;
    public Quaternion rotation;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsWriter)
        {
            SerializeForNetwork(serializer);
        }
        else
        {
            DeserializeForNetwork(serializer);
        }
    }

    private void SerializeForNetwork<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        byte[] avatarBytes = SerializeData();
        serializer.SerializeValue(ref avatarBytes);
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref rotation);
    }

    private void DeserializeForNetwork<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        byte[] avatarBytes = null;
        serializer.SerializeValue(ref avatarBytes);
        DeserializeData(avatarBytes);

        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref rotation);
    }

    public static PoseSerializableAvatarData Deserialize(byte[] data)
    {
        PoseSerializableAvatarData result = new();
        result.DeserializeData(data);
        return result;
    }

    public byte[] SerializeData()
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                byte[] data = PacketData.Serialize();
                writer.Write(data);
            }
            return m.ToArray();
        }
    }

    public void DeserializeData(byte[] data)
    {
        using (MemoryStream m = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                PacketData = AvatarPacketData.Deserialize(data);
            }
        }
    }
}

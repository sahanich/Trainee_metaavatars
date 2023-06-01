using System;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using static Oculus.Avatar2.OvrAvatarEntity;

[Serializable]
public class PacketData
{
    public byte[] data;
    public StreamLOD lod;

    public byte[] Serialize()
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write(data.Length);
                writer.Write(data);
                writer.Write((byte)lod);
            }
            return m.ToArray();
        }
    }

    public static PacketData Deserialize(byte[] data)
    {
        PacketData result = new PacketData();

        using (MemoryStream m = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                int dataLength = reader.ReadInt32();
                result.data = reader.ReadBytes(dataLength);
                result.lod = (StreamLOD)reader.ReadByte();
            }
        }
        return result;
    }
};

[Serializable]
public class PoseSerializableAvatarData : INetworkSerializable
{
    public PacketData PacketData;

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
    }

    private void DeserializeForNetwork<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        byte[] avatarBytes = null;
        serializer.SerializeValue(ref avatarBytes);
        DeserializeData(avatarBytes);
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
                PacketData = PacketData.Deserialize(data);
            }
        }
    }
}

public class BaseSerializableAvatarData : INetworkSerializable
{
    public Vector3 position;
    public Quaternion rotation;
    public int assetId;
    public ulong userId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref rotation);
        serializer.SerializeValue(ref assetId);
        serializer.SerializeValue(ref userId);
    }
}
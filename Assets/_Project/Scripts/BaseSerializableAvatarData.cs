using Unity.Netcode;
using UnityEngine;

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
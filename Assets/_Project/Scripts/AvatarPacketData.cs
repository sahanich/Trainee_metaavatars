using System;
using System.IO;
using static Oculus.Avatar2.OvrAvatarEntity;

[Serializable]
public class AvatarPacketData
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

    public static AvatarPacketData Deserialize(byte[] data)
    {
        AvatarPacketData result = new AvatarPacketData();

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

//using System.Collections;
//using Unity.Netcode;

//public class NetworkString : INetworkSerialize
//{
//    private FixedString32Bytes info;

//    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
//    {
//        serializer.SerializeValue(ref info);
//    }

//    public override string ToString()
//    {
//        return info.ToString();
//    }

//    public static implicit operator string(NetoworkString s) => s.ToString();
//    public static implicit operator NetworkString(string s) => new NetworkString() { info = new FixedString32Bytes(s)};

//}

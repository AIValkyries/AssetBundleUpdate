//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: dataconfig_configfenbao.proto
namespace dataconfig
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ConfigFenBao")]
  public partial class ConfigFenBao : global::ProtoBuf.IExtensible
  {
    public ConfigFenBao() {}
    
    private string _Name;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"Name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string Name
    {
      get { return _Name; }
      set { _Name = value; }
    }
    private string _Pakage;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"Pakage", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string Pakage
    {
      get { return _Pakage; }
      set { _Pakage = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ConfigFenBaoArray")]
  public partial class ConfigFenBaoArray : global::ProtoBuf.IExtensible
  {
    public ConfigFenBaoArray() {}
    
    private readonly global::System.Collections.Generic.List<dataconfig.ConfigFenBao> _items = new global::System.Collections.Generic.List<dataconfig.ConfigFenBao>();
    [global::ProtoBuf.ProtoMember(1, Name=@"items", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<dataconfig.ConfigFenBao> items
    {
      get { return _items; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}
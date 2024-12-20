// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: RoomProtocol.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Network.Protocol {

  /// <summary>Holder for reflection information generated from RoomProtocol.proto</summary>
  public static partial class RoomProtocolReflection {

    #region Descriptor
    /// <summary>File descriptor for RoomProtocol.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static RoomProtocolReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChJSb29tUHJvdG9jb2wucHJvdG8SEE5ldHdvcmsuUHJvdG9jb2wiQwoIQ2hh",
            "dEluZm8SEAoIc2VuZGVySUQYASABKAUSEAoIc2VuZFRpbWUYAiABKAkSEwoL",
            "Y2hhdENvbnRlbnQYAyABKAliBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Network.Protocol.ChatInfo), global::Network.Protocol.ChatInfo.Parser, new[]{ "SenderID", "SendTime", "ChatContent" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class ChatInfo : pb::IMessage<ChatInfo> {
    private static readonly pb::MessageParser<ChatInfo> _parser = new pb::MessageParser<ChatInfo>(() => new ChatInfo());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ChatInfo> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Network.Protocol.RoomProtocolReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ChatInfo() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ChatInfo(ChatInfo other) : this() {
      senderID_ = other.senderID_;
      sendTime_ = other.sendTime_;
      chatContent_ = other.chatContent_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ChatInfo Clone() {
      return new ChatInfo(this);
    }

    /// <summary>Field number for the "senderID" field.</summary>
    public const int SenderIDFieldNumber = 1;
    private int senderID_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int SenderID {
      get { return senderID_; }
      set {
        senderID_ = value;
      }
    }

    /// <summary>Field number for the "sendTime" field.</summary>
    public const int SendTimeFieldNumber = 2;
    private string sendTime_ = "";
    /// <summary>
    /// YYYY-MM-DD HH:MM:SS
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string SendTime {
      get { return sendTime_; }
      set {
        sendTime_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "chatContent" field.</summary>
    public const int ChatContentFieldNumber = 3;
    private string chatContent_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string ChatContent {
      get { return chatContent_; }
      set {
        chatContent_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as ChatInfo);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(ChatInfo other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (SenderID != other.SenderID) return false;
      if (SendTime != other.SendTime) return false;
      if (ChatContent != other.ChatContent) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (SenderID != 0) hash ^= SenderID.GetHashCode();
      if (SendTime.Length != 0) hash ^= SendTime.GetHashCode();
      if (ChatContent.Length != 0) hash ^= ChatContent.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (SenderID != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(SenderID);
      }
      if (SendTime.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(SendTime);
      }
      if (ChatContent.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(ChatContent);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (SenderID != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(SenderID);
      }
      if (SendTime.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(SendTime);
      }
      if (ChatContent.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(ChatContent);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(ChatInfo other) {
      if (other == null) {
        return;
      }
      if (other.SenderID != 0) {
        SenderID = other.SenderID;
      }
      if (other.SendTime.Length != 0) {
        SendTime = other.SendTime;
      }
      if (other.ChatContent.Length != 0) {
        ChatContent = other.ChatContent;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            SenderID = input.ReadInt32();
            break;
          }
          case 18: {
            SendTime = input.ReadString();
            break;
          }
          case 26: {
            ChatContent = input.ReadString();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code

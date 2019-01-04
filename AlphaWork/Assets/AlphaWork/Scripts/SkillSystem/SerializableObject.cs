using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SkillSystem
{
  public class SerializableObject<T> : IXmlSerializable
  {
    public T Obj
    {
      set;
      get;
    }

    XmlSchema IXmlSerializable.GetSchema()
    {
      return null;
    }

    void IXmlSerializable.ReadXml(XmlReader reader)
    {
      reader.ReadStartElement();

      Obj = (T)new XmlSerializer(Type.GetType(reader.Name),
        new XmlRootAttribute { ElementName = reader.Name }).Deserialize(reader);

      reader.ReadEndElement();
    }

    void IXmlSerializable.WriteXml(XmlWriter writer)
    {
      var t = Obj.GetType();

      var ser = new XmlSerializer(t, new XmlRootAttribute { ElementName = t.FullName });

      ser.Serialize(writer, Obj);
    }

    public SerializableObject(T obj)
    {
      Obj = obj;
    }

    public SerializableObject()
    {

    }
  }
}

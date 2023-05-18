using System.Xml.Serialization;

namespace ConceptionQuiz_Api.utility
{
    [Serializable]
    public class QuestionType
    {
        public QuestionType()
        {

        }
        public string Category { get; set; }
        public List<string>? options { get; set; }


        public string SerializeToXml<T>()
        {
            string xmlString;
            
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, this);
                xmlString = writer.ToString();
            }
            return xmlString;
        }

        public T XmlDeserializer<T>(string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T instance;
            using( var stream = new StringReader(xmlString))
            {
                instance = (T)serializer.Deserialize(stream);
            }

            return instance;
        }


    }

  
}

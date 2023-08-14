using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PersistCompositeModel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "d:\\component.json";
            Console.Write("Please type (s) for save and (l) for load and (q) for quit:");
            var command = Console.ReadKey();
            string componentJson = string.Empty;
            Component component;
            while (command.Key != ConsoleKey.Q)
            {
                switch (command.Key)
                {
                    case ConsoleKey.S:
                        component = GenerateComponent();
                        componentJson = JsonConvert.SerializeObject(component);
                        File.WriteAllText(path, componentJson);
                        break;
                    case ConsoleKey.L:
                        component = LoadComponentFromFile(path);
                        break;
                }
                Console.Write("\nPlease type (s) for save and (l) for load and (q) for quit:");
                command = Console.ReadKey();
            }

            Console.ReadKey();
        }

        private static Component LoadComponentFromFile(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<Component>(json, new ComponentConverter());
                //return JsonConvert.DeserializeObject<Component>(json);
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        private static Component GenerateComponent()
        {
            Component component = new CompositComponent()
            {
                Components = new List<Component>()
                {
                    new SimpleComponent() { Title = "Parmida"},
                    new CompositComponent()
                    {
                        Components = new List<Component>()
                        {
                            new SimpleComponent(){Title = "Pariya"},
                            new SimpleComponent(){Title = "Kobi"}
                        }
                    }
                }
            };
            return component;
        }
    }
    public abstract class Component
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; }
        public Component()
        {
            Type = this.GetType().Name;
        }


    }
    public class SimpleComponent : Component
    {
        public string Title { get; set; }
    }
    public class CompositComponent : Component
    {
        public List<Component> Components { get; set; }
    }
    public class ComponentConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Component);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            JObject jsonObject = JObject.Load(reader);
            Component component;

            string type = jsonObject["Type"].ToObject<string>();
            switch (type)
            {
                case "SimpleComponent":
                    component = new SimpleComponent();
                    break;
                case "CompositComponent":
                    component = new CompositComponent();
                    break;
                default:
                    throw new Exception("Unknown component type");
            }

            serializer.Populate(jsonObject.CreateReader(), component);
            return component;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

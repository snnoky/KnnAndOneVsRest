using System;
using System.ComponentModel.Design;
using System.IO;
using Newtonsoft.Json;


namespace Zadanie2_1MIW
{
    class Program
    {
        static void Main(string[] args)
        {

            var decision = "";
            Console.WriteLine("Choose Set: a-Australian b-BCW c-Credit.");
            decision = Console.ReadLine();

            if (decision == "a")
            {
                //wczytaj australian
                StreamReader configFile = File.OpenText("../../../data/australian/ConfigAustralian.json");
                JsonSerializer serializer = new JsonSerializer();
                UserConfig userData = (UserConfig)serializer.Deserialize(configFile, typeof(UserConfig));

                var data = DataActions.GetValuesFromFile(userData.DataPath + userData.DataName + ".dat", userData.DataTypesPath,
                    userData.DataSeparator);

                DataActions.Menu(userData, data);
            }
            else if (decision == "b")
            {
                //wczytaj bcw
                StreamReader configFile = File.OpenText("../../../data/cancer/ConfigBCW.json");
                JsonSerializer serializer = new JsonSerializer();
                UserConfig userData = (UserConfig)serializer.Deserialize(configFile, typeof(UserConfig));

                var data = DataActions.GetValuesFromFile(userData.DataPath + userData.DataName + ".data", userData.DataTypesPath,
                    userData.DataSeparator);

                DataActions.Menu(userData, data);
            }
            else if (decision == "c")
            {
                //wczytaj crx
                StreamReader configFile = File.OpenText("../../../data/credit/ConfigCrx.json");
                JsonSerializer serializer = new JsonSerializer();
                UserConfig userData = (UserConfig)serializer.Deserialize(configFile, typeof(UserConfig));

                var data = DataActions.GetValuesFromFile(userData.DataPath + userData.DataName + ".data", userData.DataTypesPath,
                    userData.DataSeparator);

                DataActions.Menu(userData, data);
                
            }
            else
            {
                Console.WriteLine("Nic nie wczytano!");
            }

            
        }
    }
}

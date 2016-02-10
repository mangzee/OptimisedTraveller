using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OptimisedTravellerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = ConfigurationManager.AppSettings["GoogleMatrixURL"];
            string key = ConfigurationManager.AppSettings["GoogleMatrixKey"];
            Console.WriteLine("Please Enter your starting point");
            string startingPoint = Console.ReadLine();
            Console.WriteLine("Please Enter your ending point");
            string endingPoint = Console.ReadLine();
            Dictionary<string,Matrix> matrices=new Dictionary<string, Matrix>();
            //List<Matrix> matrices=new List<Matrix>();
            foreach (CommonHelpers.Modes mode in Enum.GetValues(typeof(CommonHelpers.Modes)))
            {
                string formattedurl = string.Format(url, startingPoint + ",Bangalore", endingPoint + ",Bangalore", mode.ToString(),key);
                string json;
                using (var webClient = new WebClient())
                {
                    webClient.Proxy = null;
                    json = webClient.DownloadString(formattedurl);
                }
                Matrix matrix=JsonConvert.DeserializeObject<Matrix>(json);
                if(matrix.status=="OK")
                {
                    matrices.Add(mode.ToString(),matrix);
                }
                    
            }
            Console.WriteLine("Your travel options are:");
            int i=1;
            string viableTravelMode="";
            int viableTime=0;
            foreach(var matri in matrices)
            {
                Console.WriteLine("Option " + i++ + ":");
                Console.WriteLine("Mode of travel : " + matri.Key);
                if (matri.Value.rows.FirstOrDefault().elements.FirstOrDefault().status!= "ZERO_RESULTS")
                {
                    if (viableTime == 0)
                        viableTime = Convert.ToInt32(matri.Value.rows.FirstOrDefault().elements.FirstOrDefault().duration.value);

                    if (Convert.ToInt32(matri.Value.rows.FirstOrDefault().elements.FirstOrDefault().duration.value) <= viableTime)
                    {
                        viableTime = Convert.ToInt32(matri.Value.rows.FirstOrDefault().elements.FirstOrDefault().duration.value);
                        viableTravelMode = matri.Key;
                    }
                    Console.WriteLine("Distance to be travelled :" + matri.Value.rows.FirstOrDefault().elements.FirstOrDefault().distance.text);
                    Console.WriteLine("Duration for the travel  :" + matri.Value.rows.FirstOrDefault().elements.FirstOrDefault().duration.text);
                }
                else
                {
                    Console.WriteLine("Distance to be travelled : N/A");
                    Console.WriteLine("Duration for the travel  : N/A");
                }
            }
            Console.WriteLine("\n Your viable options is to use the following mode of travel is "
                + viableTravelMode);
            Console.ReadLine();
        }
    }
}

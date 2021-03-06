using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ticketsbackend.SoapService
{
    public class CheckUserWithSoap
    {
        public static async Task<string> BAMCheckUser(string name, string password)
        {
            if (name.Contains(">") || name.Contains("<") || password.Contains(">") || password.Contains("<"))
            {
                Console.WriteLine(name);
                return "Kodeord eller Brugernavn indeholder ikke tilladte tegn";
            }

            Console.WriteLine(name);

            string soapString = @"<?xml version=""1.0"" encoding=""utf-8""?>
          <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
            xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
            xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" 
               xmlns:javabogservice=""http://soap.transport.brugerautorisation/"">
                <soap:Header/>
                 <soap:Body>
                   <javabogservice:hentBruger>
                        <!--Optional:-->
                        <arg0>name</arg0>
                          <!--Optional:-->
                          <arg1>password</arg1>
                        </javabogservice:hentBruger>
              </soap:Body>
          </soap:Envelope>";
            soapString = soapString.Replace("name", name).Replace("password", password);

            var response = await PostXmlRequest("http://javabog.dk:9901/brugeradmin", soapString);
            var responseXml = await response.Content.ReadAsStringAsync();


            var xml = XDocument.Parse(responseXml);


            // Parse Soap Response
            var soapResponse = xml.Descendants().Where(x => x.Name.LocalName == "return").Select(x => new SoapResponse
            {
                Firstname = (string) x.Element(x.Name.Namespace + "fornavn"),
                Lastname = (string) x.Element(x.Name.Namespace + "efternavn"),
                Username = (string) x.Element(x.Name.Namespace + "brugernavn"),
                PassWord = (string) x.Element(x.Name.Namespace + "adgangskode")
            }).FirstOrDefault();

            string temp = "";
            if (soapResponse != null)
            {
                if (soapResponse.Username.Equals(name) && soapResponse.PassWord.Equals(password))
                {
                    temp = soapResponse.Firstname + " " + soapResponse.Lastname;
                }
            }
            else
                temp = "Forkert brugernavn eller adgangskode!";


            return temp;
        }

        public static async Task<HttpResponseMessage> PostXmlRequest(string baseUrl, string xmlString)
        {
            using (var httpClient = new HttpClient())
            {
                var httpContent = new StringContent(xmlString, Encoding.UTF8, "text/xml");

                return await httpClient.PostAsync(baseUrl, httpContent);
            }
        }
    }

    public class SoapResponse
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string PassWord { get; set; }
        public string Username { get; set; }
    }
}
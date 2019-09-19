using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class  SPlayer
    {
        public void recieve(List<string> info) { Console.WriteLine("==="); foreach (string message in info) Console.WriteLine(message); }
        public SPlayerResponse response(SRequest request)
        {
            SPlayerResponse response = request.autoResponce;
            return response != null? response : solve(request);
        }
        SPlayerResponse solve(SRequest request)
        {
            Console.WriteLine("Request " + request.requestQuestion);
            int answer;
            while (!int.TryParse(Console.ReadLine(), out answer));
            return new SPlayerResponse(answer);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class  SPlayer
    {
        public SPlayerResponse response(SRequest request)
        {
            SPlayerResponse response = request.autoResponce;
            return response != null? response : solve(request);
        }
        SPlayerResponse solve(SRequest request)
        {
            Console.WriteLine("Request " + request.requestQuestion);
            Console.ReadKey();
            return new SPlayerResponse();
        }
    }
}

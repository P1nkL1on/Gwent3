using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class SPlayers
    {
        public SPlayers(params SPlayer[] players) { _players = new Dictionary<int, SPlayer>(); int index = 0; foreach (SPlayer player in players) _players.Add(index++, player); }
        public SPlayerResponse solve(SRequest request){ return _players[request.adresser].response(request); }

        Dictionary<int, SPlayer> _players;
    }
}

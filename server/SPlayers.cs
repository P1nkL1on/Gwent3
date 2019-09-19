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
        public SPlayerResponse solve(SRequest request, SLogger logger){
            // wherever one of players should make a descision
            // you flush to all players all new info/
            // and show them, that player is thinking
            foreach (int player in _players.Keys)
                _players[player].recieve(logger.flush(player));
                
            return _players[request.adresser].response(request); 
        }


        Dictionary<int, SPlayer> _players;
    }
}

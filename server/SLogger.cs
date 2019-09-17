using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class SCardView
    {
        public SCardView(SCard card) { }
    }
    class SLogger
    {
        public bool isVisible(SCard card, int player) { return _views[player].ContainsKey(card.id); }
        public void show(SCard card, int player = -1)
        {
            if (player < 0)
            {
                foreachPlayer((p) => { show(card, p); });
                return;
            }
            if (isVisible(card, player))
                return;
            _views[player].Add(card.id, new SCardView(card));
            log(String.Format("sh {0} <{1}>", card.id, card.name.ToString()), player);
        }
        public void hide(SCard card, int player = -1)
        {
            if (player < 0)
            {
                foreachPlayer((p) => { hide(card, p); });
                return;
            }
            if (!isVisible(card, player))
                return;
            _views[player].Remove(card.id);
            log(String.Format("unsh {0}", card.id));
        }
        // if card is shown before
        // then it has format X, where X - is its id
        // otherwise
        // card has format <cY>, where Y - index in its
        // current place
        string cardView(SCard card, int p)
        {
            return isVisible(card, p) || card.host == p ?
                String.Format("{0}", card.id.ToString())
              : String.Format("<c{0}>", card.game.cards.select(SFilter.located(card.location.place), SFilter.hostBy(card.host)).indexOf(card));
        }
        public void move(SCard card, SLocation moveTo, SLocation moveFrom) { foreachPlayer((p) => { log(String.Format("mv {0} {1}←{2} p{3}", cardView(card, p), moveTo, moveFrom, card.host), p); }); }
        public void damage(SCard card, SCard source, int dmg) { logAction(card, source, dmg, "dmg"); }
        void logAction(SCard card, SCard source, int dmg, string word) { foreachPlayer((p) => { log(String.Format("{3} {0}←{1} by {2}", cardView(card, p), cardView(source, p), dmg, word), p); }); }

        List<string> skippableCommands = new List<string>() { };
        List<string> stackableCommands = new List<string>() { "mv", "dmg", "bst", "wkn", "str" };

        string commandType(string message) { return message.Substring(0, message.IndexOf(' ')); }
        string uniteCommands(string cmd1, string cmd2)
        {
            int s1 = 0, s2 = 0, e1 = cmd1.Length - 1, e2 = cmd2.Length - 1;
            //while (cmd1[s1] == cmd2[s2] && cmd1[s1] != '<') { s1++; s2++; }
            //while (cmd1[e1] == cmd2[e2] && cmd1[e1] != '>') { e1--; e2--; }
            while (cmd1[s1] == cmd2[s2])
            {
                s1++; s2++;
                if (cmd1[s1] == '<' || cmd2[s2] == '<')
                    break;
            }
            while (cmd1[e1] == cmd2[e2])
            {
                e1--; e2--;
                if (cmd1[e1] == '>' || cmd2[e2] == '>')
                    break;
            }
            s1 = s2 = cmd1.Substring(0, s1).LastIndexOf(' ');

            int e1zeroInd = cmd1.Substring(e1).IndexOf(' '); if (e1zeroInd > 0) e1 += e1zeroInd;
            int e2zeroInd = cmd2.Substring(e2).IndexOf(' '); if (e2zeroInd > 0) e2 += e2zeroInd;

            if (s1 > e1 || s2 > e2)
                throw new Exception("Can not unite commands: " + cmd1 + " + " + cmd2);
            string start = cmd1.Substring(0, s1);
            string finish = cmd1.Substring(e1 + 1);
            string p1 = cmd1.Substring(s1 + 1, e1 - s1 - 1);
            string p2 = cmd2.Substring(s2 + 1, e2 - s2 - 1);

            return String.Format("{0} {1}, {2} {3}", start, p1, p2, finish);
        }

        void log(string message, int player = -1)
        {
            if (player >= 0)
                _messages[player].Add(message);
            else
                for (int i = 0; i < _messages.Count; ++i)
                    log(message, i);
        }
        public void flush(int player)
        {
            if (_messages[player].Count == 0)
                return;
            int ind = 0;
            do
            {
                string message = _messages[player][ind];
                string ct = commandType(message);
                if (stackableCommands.Contains(ct))
                    for (int i = ind + 1; i < _messages[player].Count; ++i)
                    {
                        string wasCt = commandType(_messages[player][i]);
                        if (skippableCommands.Contains(wasCt))
                            continue;
                        if (wasCt == ct)
                        {
                            message = uniteCommands(message, _messages[player][i]);
                            _messages[player].RemoveAt(i);
                            i--;
                        }
                        else
                            break;
                    }
                Console.WriteLine(message);
                _messages[player].RemoveAt(ind);
            } while (ind < _messages[player].Count);
        }
        public SLogger(int playersCount)
        {
            _views = new List<Dictionary<int, SCardView>>();
            _messages = new List<List<string>>();
            for (int i = 0; i < playersCount; ++i)
            {
                _views.Add(new Dictionary<int, SCardView>());
                _messages.Add(new List<string>());
            }
        }
        void foreachPlayer(PlayerAction action) { for (int i = 0; i < _messages.Count; ++i) action(i); }
        List<Dictionary<int, SCardView>> _views;
        List<List<string>> _messages;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    
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
            _views[player].Add(card.id, card.view());
            log(String.Format("sh {0} <{1}>", cardView(card, player), card.name.ToString()), player);
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
            //return isVisible(card, p) || card.host == p ?
            //    String.Format("[id={0}]", card.id.ToString())
            //  : String.Format("[c{0}]", card.game.cards.select(SFilter.located(card.location.place), SFilter.hostBy(card.host)).indexOf(card));
            string place = String.Format("{0}st card in p{1}'s {2}", card.game.cards.select(SFilter.located(card.location.place), SFilter.hostBy(card.host)).indexOf(card), card.host, card.location);
            string id = isVisible(card, p)? String.Format("id={0} ", card.id) : "";
            return String.Format("{0}{1}", id, place);
        }
        public void move(SCard card, SLocation moveTo) { foreachPlayer((p) => { log(String.Format("mv {0} {1}←{2} p{3}", cardView(card, p), moveTo, card.location, card.host), p); }); }
        public void damage(SCard card, SCard source, int dmg) { logAction(card, source, dmg, "dmg"); }
        void logAction(SCard card, SCard source, int dmg, string word) { foreachPlayer((p) => { log(String.Format("{3} {0}←{1} by {2}", cardView(card, p), cardView(source, p), dmg, word), p); }); }

        List<string> skippableCommands = new List<string>() {  };
        List<string> stackableCommands = new List<string>() { "mv", "dmg", "bst", "wkn", "str" };

        string commandType(string message) { return message.Substring(0, message.IndexOf(' ')); }
        bool uniteCommands(string cmd1, string cmd2, out string uniteCmd)
        {
            uniteCmd = "";
            // !!!
            return false;

            int s1 = 0, s2 = 0, e1 = cmd1.Length - 1, e2 = cmd2.Length - 1;
            do{
                s1++; s2++;
                if (cmd1[s1] != cmd2[s2])
                    return false;
            }while(cmd1[s1] != '[');
            do
            {
                e1--; e2--;
                if (cmd1[e1] != cmd2[e2])
                    return false;
            } while (cmd1[e1] != ']');
            string start = cmd1.Substring(0, s1 + 1);
            string end = cmd1.Substring(e1);
            string p1 = cmd1.Substring(s1 + 1, e1 - s1 - 1);
            string p2 = cmd2.Substring(s2 + 1, e2 - s2 - 1);

            uniteCmd = String.Format("{0}{2}, {1}{3}", start, p1, p2, end);
            return true;
        }

        void log(string message, int player = -1)
        {
            if (player >= 0)
                _messages[player].Add(message);
            else
                for (int i = 0; i < _messages.Count; ++i)
                    log(message, i);
        }
        public List<string> flush(int player)
        {
            List<string> flushMessages = new List<string>();
            if (_messages[player].Count == 0)
                return flushMessages;
            int ind = 0;
            do
            {
                string message = _messages[player][ind];
                string ct = commandType(message);
                if (stackableCommands.Contains(ct))
                    for (int i = ind + 1; i < _messages[player].Count; ++i)
                    {
                        string wasMessage = _messages[player][i];
                        string wasCt = commandType(wasMessage);

                        if (skippableCommands.Contains(wasCt))
                            continue;
                        if (wasCt == ct && wasMessage.Last() == message.Last())
                        {
                            string uniteMessage = "";
                            if (!uniteCommands(message, wasMessage, out uniteMessage))
                                break;
                            message = uniteMessage;
                            _messages[player].RemoveAt(i);
                            i--;
                        }
                        else
                            break;
                    }
                flushMessages.Add(message);
                _messages[player].RemoveAt(ind);
            } while (ind < _messages[player].Count);
            return flushMessages;
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

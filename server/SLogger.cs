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
        // points
        // logger should distribute messages to playuers
        // for player who cannot see a card in hand boosted
        // there should be a message like
        // enemy cards with pids 12 and 13 are boosted
        // and if allyes then same
        // but a cleint app do not knows, what does this pids means
        // but it has from server description of its own deck
        // not the enemies
        // and when enemy deck is played
        // name it and show
        //
        // public void onMove(SCards card, SLocation location)
        // {
        //     Console.WriteLine(card + " move to " + location);
        // }
        // public void onDamaged(SCards card, SCard source, int damage)
        // {
        //     Console.WriteLine(source + " damage " + card + " by " + damage);
        // }
        // public void onBoosted(SCards card, SCard source, int boost)
        // {
        //     Console.WriteLine(source + " boosts " + card + " by " + boost);
        // }

        public virtual bool isVisible(int player, SCard card) { return _views[player].ContainsKey(card.id); }
        public virtual void hide(SCards cards, int player = -1) { foreach (SCard card in cards.cards) hide(card, player); }
        public virtual void show(SCards cards, int player = -1) { foreach (SCard card in cards.cards) show(card, player); }
        public virtual void show(SCard card, int player = -1)
        {
            if (player < 0)
            {
                foreachPlayer((p) => { show(card, p); });
                return;
            }
            if (isVisible(player, card))
                return;
            _views[player].Add(card.id, new SCardView(card));
            log(String.Format("sh {0} {1}", card.id, card.name.ToString()), player);
        }

        public virtual void hide(SCard card, int player = -1)
        {
            if (player < 0)
            {
                foreachPlayer((p) => { hide(card, p); });
                return;
            }
            if (!isVisible(player, card))
                return;
            _views[player].Remove(card.id);
            log(String.Format("unsh {0}", card.id));
        }
        public virtual void move(SCards cards, SLocation moveTo)
        {
            Dictionary<SPlace, SCards> initPlaces = new Dictionary<SPlace, SCards>();
            foreach (SCard card in cards.cards)
                if (!initPlaces.ContainsKey(card.location.place))
                    initPlaces.Add(card.location.place, new SCards(card));
                else
                    initPlaces[card.location.place].addCard(card);
            foreach (SPlace place in initPlaces.Keys)
                move(initPlaces[place], moveTo, new SLocation(place));
        }
        public virtual void move(SCards cards, SLocation moveTo, SLocation moveFrom)
        {
            if (cards.isEmpty) return;
            foreachPlayer((p) =>
            {
                SCards vis = visible(p, cards);
                int invisCount = cards.count - vis.count;

                log(String.Format("mv {0}{1}{2} {3}←{4}",
                    vis.idsToString(),
                    vis.count > 0 && invisCount > 0 ? "+" : "",
                    invisCount > 0? (invisCount + "c") : "",
                    moveTo, moveFrom), p);
            });
        }

        void log(string message, int player = -1) { if (player >= 0) _messages[player].Add(message); else for (int i = 0; i < _messages.Count; ++i) _messages[i].Add(message); Console.WriteLine(String.Format("to {0}: {1}", player >= 0 ? player.ToString() : "all", message)); }
        SCards visible(int player, SCards cards) { SCards vis = new SCards(); foreach (SCard card in cards.cards) if (isVisible(player, card)) vis.addCard(card); return vis; }

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
    class SLoggerInvalid : SLogger
    {
        public SLoggerInvalid(int playerCount = 0) : base(playerCount) { }
        public override void show(SCard card, int player = -1) { }
        public override void show(SCards cards, int player = -1) { }
        public override void hide(SCard card, int player = -1) { }
        public override void hide(SCards cards, int player = -1) { }
        public override void move(SCards cards, SLocation moveTo) { }
    }
}

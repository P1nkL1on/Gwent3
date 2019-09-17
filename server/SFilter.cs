using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    delegate bool CardPredicat(SCard card);

    class SFilter
    {
        public static CardPredicat located(params SPlace[] places) { return (c) => { foreach (SPlace place in places) if (c.location.Equals(place)) return true; return false; }; }
        public static CardPredicat located(SPlace place) { return (c) => { return c.location.Equals(place); }; }
        public static CardPredicat located(SRow row) { return (c) => { return c.location.Equals(row); }; }
        public static CardPredicat otherThen(SCards scards) { return (c) => { foreach (SCard card in scards.cards) if (c.Equals(card)) return false; return true; }; }
        public static CardPredicat otherThen(SCard card) { return otherThen(new SCards(card)); }
        public static CardPredicat hostBy(int player) { return (c) => { return c.host == player; }; }
        public static CardPredicat hasId(int id) { return (c) => { return c.id == id; }; }
        public static CardPredicat withName(SCardName name) { return (c) => { return c.name == name; }; }
        public static CardPredicat mulliganName(SCardName muligannedCardName) { return (c) => { return c.name != muligannedCardName; }; }
        public static CardPredicat ally(SCard card) { return (c) => { return c.host == card.host; }; }
        public static CardPredicat enemy(SCard card) { return (c) => { return c.host != card.host; }; }

        static List<SPlace> gamePlaces = new List<SPlace>() { SPlace.hand, SPlace.deck, SPlace.board, SPlace.graveyard, SPlace.leader};
        public static CardPredicat inGame() { return (c) => { return gamePlaces.Contains(c.location.place); }; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class SRequest
    {
        public SRequest(string requestQuestion, int adressPlayer) { _adressPlayer = adressPlayer; _requestQuestion = requestQuestion; }

        public virtual string requestQuestion { get { return _requestQuestion; } }
        public virtual SPlayerResponse autoResponce { get { return null; } }
        public int adresser { get { return _adressPlayer; } }

        protected string _requestQuestion;
        protected int _adressPlayer;
    }
    class SRequestAbstractSelectCard : SRequest
    {
        public SRequestAbstractSelectCard(string requestQestion, List<int> cardPids, int adressPlayer) : base(requestQestion, adressPlayer) { _cardIds = cardPids; }
        public SRequestAbstractSelectCard(string requestQestion, SCards cards, int adressPlayer) : base(requestQestion, adressPlayer) { _cardIds = new List<int>(); foreach (SCard card in cards.cards)_cardIds.Add(card.id); }
        public int cardsCount { get { return _cardIds.Count; } }
        protected List<int> _cardIds;
        protected string cardIdsToString() { string res = ""; foreach (int id in _cardIds) res += (res.Length == 0 ? "" : ", ") + id; return res; }
        public override SPlayerResponse autoResponce { get { return cardsCount == 0 ? new SPlayerResponse() : null; } }
    }
    class SRequestSelectOneCard : SRequestAbstractSelectCard
    {
        public SRequestSelectOneCard(string requestQestion, SCards cards, SCard source) : base(requestQestion, cards, source.host) { sourceId = source.id; }
        public override SPlayerResponse autoResponce { get { return cardsCount == 1 ? new SPlayerResponse(_cardIds.First()) : base.autoResponce; } }
        public override string requestQuestion { get { return String.Format("{2}: {0}, select one card from following: {1}", base.requestQuestion, cardIdsToString(), sourceId); } }
        protected int sourceId;
    }
}

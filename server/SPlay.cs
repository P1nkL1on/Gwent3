using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    delegate void PlayerAction(int player);
    class SPlay
    {
        // started decks are added to cards!
        // SCards _startedDecks;
        SCards _cards;
        // players calss, which refers
        // to hidden responce logick
        // will farther will use a signals
        // to solve it on client machine
        SPlayers _players;
        SLogger _logger;

        int _playerCount;
        int _currentPlayer;
        Random _random;

        public SLogger logger { get { return _logger; } }
        public SPlayers players { get { return _players; } }
        public SCards cards { get { return _cards; } }

        public SPlay(SPlayers players, params SCards[] startedDecks)
        {
            _random = new Random();

            _players = players;
            _playerCount = startedDecks.Count();
            _logger = new SLogger(_playerCount);
            _cards = new SCards();
            // unite all started decks and host for it
            // and change its place to StartedDeck
            // then make default copy of it and then
            // place them to Deck
            int player = -1;
            foreach (SCards startedDeck in startedDecks)
            {
                ++player;
                startedDeck.setGame(this);
                startedDeck.setHost(player);
                startedDeck.setLocation(SPlace.startingdeck);
                _cards.addCards(startedDeck);

                SCards deck = startedDeck.defaultCopy();
                deck.setLocation(SPlace.deck);
                _cards.addCards(deck);
                shuffleDeck(player);
            }
        }

        public void start()
        {
            foreachPlayer((p) =>
            {
                drawCard(p, 3);
            });
            logger.flush(0);
            _cards.cards[17].maybe.move(new SLocation(SRow.ranged));
            logger.flush(0);
        }

        void foreachPlayer(PlayerAction action) { for (int i = 0; i < _playerCount; ++i) action(i); }
        SCards deck(SCard playersCard) { return deck(playersCard.host); }
        SCards deck(int player) { return _cards.select(SFilter.hostBy(player), SFilter.located(SPlace.deck)); }
        SCards drawCard(int player, int cardCount = 1) { return deck(player).first(cardCount).move(SPlace.hand); }

        // shuffle a card from somewhere to a deck
        // puts it to deck
        // then put it to random position in deck
        public SCard shuffleCard(SCard card)
        {
            card.maybe.move(SPlace.deck);
            _cards.putRandom(_random, card, SFilter.hostBy(card.host), SFilter.located(SPlace.deck));
            return card;
        }
        void mulliganCard(SCard card)
        {
            // currently solving for Mulligan-problem
            // if you swap a card you will always
            // draw from deck the first card
            // with a different name
            // (if all deck has only theese names)
            // then drop it
            bool allSameName = _cards
                .select(SFilter.hostBy(card.host), SFilter.located(SPlace.deck))
                .isAll(SFilter.withName(card.name));
            var deck = _cards.select(SFilter.hostBy(card.host), SFilter.located(SPlace.deck));
            // then select first or first
            // with different name
            // and move it yto your hand
            (allSameName ? deck.first() : deck.first(1, SFilter.mulliganName(card.name)))
                .move(SPlace.hand);
        }
        SCards shuffleDeck(int player) { return _cards.shuffle(_random, SFilter.hostBy(player), SFilter.located(SPlace.deck)); }

        //_cards.cards[7].maybe.move(new SLocation(SRow.meele));
        //_cards.cards[8].maybe.move(new SLocation(SRow.meele));

        //_cards.cards[17].maybe.move(new SLocation(SRow.ranged));
        //Console.WriteLine(_cards);
        //Console.WriteLine(shuffleDeck(0));
        //SCards drawn = drawCard(0);
        //Console.WriteLine(_cards);
        //if (drawn.count == 0)
        //    return;
        //shuffleCard(drawn.cards.First());
        //Console.WriteLine(_cards);
        // Console.WriteLine(_cards);
        //_cards.cards[8].maybe.move(new SLocation(SRow.meele));
        //_cards.cards[15].maybe.move(new SLocation(SRow.meele));
    }
}

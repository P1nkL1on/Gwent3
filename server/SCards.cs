using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    delegate void cardAction(SCard card);
    class SCards
    {
        public List<SCard> cards { get { return _cards; } }

        public SCards() { _cards = new List<SCard>(); }
        public SCards(params SCard[] cards) { _cards = cards.ToList(); }
        public SCards(List<SCard> cards) { _cards = cards; }

        public SCards addCard(SCard card) { _cards.Add(card); return this; }
        public SCards addCards(SCards cards) { _cards.AddRange(cards.cards); return this; }
        public SCards addCards(params SCard[] cards) { _cards.AddRange(cards.ToList()); return this; }
        public SCards addCards(List<SCard> cards) { _cards.AddRange(cards); return this; }
        public void setCards(SCards cards, List<int> indices) { for (int i = 0; i < indices.Count; ++i) _cards[indices[i]] = cards.cards[i]; }

        public bool isEmpty { get { return _cards.Count == 0; } }
        public int count { get { return _cards.Count; } }
        public SCards unite(SCards[] scards) { SCards res = new SCards(); foreachCard((c) => { res.addCard(c); }); return res; }
        public SCards defaultCopy() { SCards res = new SCards(); foreachCard((c) => { res.addCard(c.defaultCopy()); }); return res; }
        public SCards range(int start = 0, int range = -1) { return new SCards(range < 0 ? _cards.GetRange(start, _cards.Count - start) : _cards.GetRange(start, Math.Min(range, _cards.Count - start))); }
        public SCards range(List<int> indices) { SCards rangeCards = new SCards(); for (int i = 0; i < indices.Count; ++i) rangeCards.addCard(_cards[indices[i]]); return rangeCards; }

        public void setHost(int host) { foreachCard((c) => { c.host = host; }); }
        // do not execute any triggers
        // so, cant be used for group movement
        // or group killing / banishing
        public void setLocation(SLocation location) { foreachCard((c) => { c.location = location; }); }
        public void setLocation(SPlace place) { foreachCard((c) => { c.location = new SLocation(place); }); }
        public void setGame(SPlay game) { foreachCard((c) => { c.game = game; }); }

        public List<int> selectIndices(params CardPredicat[] filters)
        {
            List<int> res = new List<int>();
            for (int i = 0; i < count; ++i)
            {
                bool match = true;
                foreach (CardPredicat filter in filters)
                    if (!filter(_cards[i]))
                        match = false;
                if (match)
                    res.Add(i);
            }
            return res;
        }
        public SCards select(params CardPredicat[] filters)
        {
            List<SCard> cards = new List<SCard>();
            foreachCard((c) =>
            {
                bool match = true;
                foreach (CardPredicat filter in filters)
                    if (!filter(c))
                        match = false;
                if (match)
                    cards.Add(c);
            });
            return new SCards(cards);
        }
        public int indexOf(SCard card) { return _cards.IndexOf(card); }
        public bool contains(SCard card) { return _cards.Contains(card); }
        public bool isAll(params CardPredicat[] filters) { return select(filters).count == count; }
        public SCards first(int count = 1, params CardPredicat[] filters) { return select(filters).range(0, count); }
        public SCards shuffle(Random shuffler, params CardPredicat[] filters)
        {
            // get all indices, that should be shuffled
            // then get cards by theese indices
            // then push them back with random order
            List<int> indexes = selectIndices(filters);
            List<int> indexesUnshuffled = new List<int>(indexes);

            List<int> indexesShuffled = new List<int>();
            while (indexesUnshuffled.Count > 0)
            {
                int at = shuffler.Next(indexesUnshuffled.Count);
                indexesShuffled.Add(indexesUnshuffled[at]);
                indexesUnshuffled.RemoveAt(at);
            }
            setCards(range(indexes), indexesShuffled);
            return this;
        }
        SCards setCardIndex(SCard card, int index = 0, params CardPredicat[] filters)
        {
            // use for set a card first or last
            // in deck or maybe board
            // to do this required:
            // get all indices
            // then found in them a required card
            // make its index first
            // and reshuffle them
            //
            List<int> indexes = selectIndices(filters);
            List<int> newIndexes = new List<int>(indexes);
            int wasCardIndex = _cards.IndexOf(card);
            // now in range item has ind = wasRangeINdex
            // but required its place in param 'index'
            // change it if it exists
            int wasRangeIndex = newIndexes.IndexOf(wasCardIndex);
            // try to change a place of card in group
            // but grpup doesnt contain this card
            if (wasRangeIndex < 0)
                return this;
            newIndexes.RemoveAt(wasRangeIndex);
            newIndexes.Insert(index, wasCardIndex);

            setCards(range(newIndexes), indexes);
            return this;
        }
        public SCards putFirst(SCard card, params CardPredicat[] filters) { return setCardIndex(card, 0, filters); }
        public SCards putLast(SCard card, params CardPredicat[] filters) { int count = selectIndices(filters).Count; return setCardIndex(card, count - 1, filters); }
        public SCards putRandom(Random shuffler, SCard card, params CardPredicat[] filters) { int count = selectIndices(filters).Count; return setCardIndex(card, shuffler.Next(count), filters); }

        public SCards targetOneCard(SCard source, string requestString)
        {
            SRequestSelectOneCard request = new SRequestSelectOneCard(requestString, this, source);
            return select(SFilter.hasId(source.game.players.solve(request).value));
        }

        // move a group of units to somewhere
        public SCards move(SPlace place){return move(new SLocation(place));}
        public SCards move(SLocation location)
        {
            List<STType> triggers = new List<STType>();
            foreachCard((c) =>
            {
                STType trigger = STType.none;
                // see what to trigger on movement
                if (location.place == SPlace.board) trigger = c.location.place == SPlace.graveyard ? STType.onRessurect : STType.onDeploy;

                // if moven to graveyard
                // from hand = discard
                // from board = die -> then 
                // trigger deathwish 
                // if just moved from board to graveyard
                // then prevent onDie effect
                // from the higher level
                if (location.place == SPlace.graveyard)
                {
                    if (c.location.place == SPlace.hand) trigger = STType.onDiscard;
                    if (c.location.place == SPlace.board) trigger = STType.onDie;   //c.containsTag(STag.doomed)
                }
                if (location.place == SPlace.banish && c.location.place == SPlace.board) trigger = STType.onBanish;
                // when back to deck
                // shuffled when from board/graveyard
                // otherwise swapped
                if (location.place == SPlace.deck) trigger = c.location.place == SPlace.hand ? STType.onSwap : STType.onShuffle;

                // from deck to hand 
                if (location.place == SPlace.hand && c.location.place == SPlace.deck) trigger = STType.onDraw;

                // trigger moe when still on board,
                // but a row changed
                if (location.Equals(c.location) && location.place == SPlace.board && location.row != c.location.row) trigger = STType.onMove;

                // then move and remember trigger
                // SLocation prevLocation = c.location;
                c.game.logger.move(c, location, c.location);
                
                c.location = location;
                triggers.Add(trigger);
            });
            // after all cards moved
            // trigger in order their deploys, 
            // deathwishes or whatever
            for (int i = 0; i < triggers.Count; ++i)
                if (triggers[i] != STType.none)
                    _cards[i].trigger(triggers[i]);
            return this;
        }
        // damage all selected units by specified X
        // returns count of units died this way
        public SCards damage(int X, SCard source = null)
        {
            SCards banish = new SCards();
            SCards dead = new SCards();
            SCards armorLost = new SCards();
            List<int> recived = new List<int>();
            foreachCard((c) =>
            {
                SHitResult hitResult = c.power.damage(X);
                recived.Add(hitResult.healthLost);
                if (hitResult.shouldBeDead)
                    (c.containsTag(STag.doomed)? banish : dead).addCard(c);
                else if (hitResult.armorWasBroken)
                    armorLost.addCard(c);
            });
            // (using foreachCard of daed/armorLost)
            // move dead group to graveyard
            // trigger their deathwishes
            // if dead, but has doomed
            // move it to banish instead
            // and do not trigger deathwish
            // trigger all armorLost units 
            // on armorlost triggers
            // rest non-dead and non-banished
            // trigger for damaged
            // then return all killed cards
            // killed = banished + dead
            foreachCard((c) =>
            {
                int damagedFor = recived[_cards.IndexOf(c)];
                if (!dead.contains(c) && !banish.contains(c) && damagedFor > 0) c.trigger(STType.onDamaged, source, damagedFor);
                if (armorLost.contains(c)) c.trigger(STType.onArmorLost);
            });
            dead.move(SPlace.graveyard);
            banish.move(SPlace.banish);
            return dead.addCards(banish);
        }

        public SCards boost(int X, SCard source = null)
        {
            // boost all cards
            // then in order trigger
            // their onBoost
            foreachCard((c) => { c.power.boost(X); });
            foreachCard((c) => { c.trigger(STType.onBoost, source, X); });
            return this;
        }

        // gives you zero SCards to perform non-perform action
        // if flag is FALSE
        public SCards ifonly(bool flag) { return flag ? this : new SCards(); }

        // returns list of
        // ticked cards
        public SCards tick(int X = 1)
        {
            SCards finished = new SCards();
            foreachCard((c) => { if (c.timer.tick(X)) finished.addCard(c); });
            return finished;
        }

        public SCards setAmbush(bool isAmbush = true) { foreachCard((c) => { c.status.setAmbush(isAmbush); }); return this; }

        public string idsToString() { string res = ""; foreachCard((c) => { res += (res.Length == 0 ? "" : ", ") + c.id; }); return res.Length == 0? "" : String.Format("[{0}]", res); }
        public override string ToString() 
        {
            string res = "";
            int placeCount = 8;
            List<string> reses = new List<string>(placeCount);
            for (int i = 0; i < placeCount; ++i)
            {
                reses.Add("");
                select(SFilter.located((SPlace)i)).foreachCard((c) => { reses[i] += (reses[i].Length == 0 ? "" : ", ") + c.id; }); 
                if (reses[i].Length > 0)
                    res += String.Format("{1}=<{0}> ", reses[i], ((SPlace)i).ToString());
            }
            return res;
        }
        void foreachCard(cardAction action) { foreach (SCard _card in _cards) action(_card); }
        List<SCard> _cards;
    }
}

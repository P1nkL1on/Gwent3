using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    partial class SCard
    {
        static int _ids = 0;

        readonly int _id;
        int _player;
        SCardName _name;
        SPlay _game;
        SClan _clan;
        SRarity _rarity;
        SLocation _location;
        List<STag> _tags;
        SStatus _status;
        SPower _power;
        STimer _timer;
        Dictionary<STType, STrigger> _triggers;

        public SCards maybe { get { return new SCards(this); } }
        public SStatus status { get { return _status; } }
        public STimer timer { get { return _timer; }}
        public void setTimer(STimer timer) { _timer = timer; }
        public SCardName name { get { return _name; } }
        public SPower power { get { return _power; } }
        public int id { get { return _id; } }
        public SPlay game { get { return _game; } set { _game = value; } }
        public int host { get { return _player; } set { _player = value; } }
        public bool Equals(SCard card) { return _id == card._id; }
        public SLocation location { get { return _location; } set { _location = value; } }
        public bool containsTag(STag tag) { return _tags.Contains(tag); }

        public bool containsTrigger(STType triggerType) { return _triggers.ContainsKey(triggerType); }
        public void setTrigger(STType triggerType, STrigger trigger) { _triggers.Add(triggerType, trigger); }
        public void trigger(STType triggerType, SCard source = null, int param = 0)
        {
            if (containsTrigger(triggerType))
                _triggers[triggerType](this, source, param);
            
            // then show it to someone
            // or hide from someone
            viewAction(triggerType, source, param);
            // if a trigger from range 20..100
            // then its a trigger, what will trigger
            // other cards with %OtherTrigger% case
            int triggerNum = (int)triggerType;
            if (triggerNum < 20 || triggerNum > 100)
                return;
            foreach (SCard otherCard in _game.cards.select(SFilter.inGame(), SFilter.otherThen(this)).cards)
                otherCard.trigger((STType)(triggerNum + 100), this);
        }
        void viewAction(STType triggerType, SCard source, int param)
        {
            if (triggerType == STType.onDraw) game.logger.show(this, this.host);
            if (triggerType == STType.onDeploy) game.logger.show(this);
        }
        // default copy with clear status and default basepower
        // remains all triggers
        // remains a host index
        // remains pointer to game
        // remains timer, resets _now param
        // has none place and none row
        public SCard defaultCopy()
        {
            SCard copy = new SCard(_name, _clan, _rarity, _power.defaultPower, _tags.ToArray());
            copy._triggers = _triggers;
            copy._player = _player;
            copy._game = _game;
            copy._timer = _timer.defaultCopy();
            return copy;
        }

        SCard(SCardName name, SClan clan, SRarity rarity, int defaultPower, params STag[] tags)
        {
            _player = -1;
            _location = new SLocation();
            _rarity = rarity;
            _clan = clan;
            _id = _ids++;
            _name = name;
            _status = new SStatus();
            _power = new SPower(defaultPower);
            _tags = tags.ToList();
            _triggers = new Dictionary<STType, STrigger>();
            _timer = STimer.none();
        }

        public override string ToString() { return String.Format("(name={0}, id={1})", _name.ToString(), _id); }
    }

    enum STType
    {
        none = -1,

        onDeploy = 20,
        onRessurect,
        onSwap,
        onShuffle,
        onMove,
        onDraw,
        onDiscard,
        onDie,
        // rhe only reason to have onBanish trigger is to correctly understood
        // which card was detrouyed. Because doomed and weaken up cards are banished
        // (not actually dead) but destroyed anyway
        onBanish,

        onOtherDeploy = 120,
        onOtherRessurect,
        onOtherSwap,
        onOtherShuffle,
        onOtherMove,
        onOtherDraw,
        onOtherDiscard,
        onOtherDie,
        onOtherBanish,

        onTurnStart = 200,
        onTurnEnd = 201,

        onBoost = 300,
        onStrenthen = 301,
        onDamaged = 302,
        onWeaken = 303,
        onArmorLost = 304
    }

    delegate void STrigger(SCard self, SCard source = null, int param = 0);
}

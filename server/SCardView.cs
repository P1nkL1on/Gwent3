using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class SPowerView
    {
        public SPowerView(int current, int bas, int def, int armor) { this._current = current; _base = bas; _def = def; _armor = armor; }
        public override string ToString() { return _def > 0 ? (String.Format("power = {0}({1})", _current, _base) + (_armor > 0? string.Format(", armor = {0}", _armor) : "" )) : "power = no"; }
        int _def;
        int _base;
        int _current;
        int _armor;
    }
    class SStatusView
    {
        public SStatusView(bool isLocked, bool isSpy, bool isResilent, bool isImmune, bool isAmbush)
        {
            _isLocked = isLocked;
            _isSpy = isSpy;
            _isResilent = isResilent;
            _isImmune = isImmune;
            _isAmbush = isAmbush;
        }
        public override string ToString()
        {
            List<string> statuses = new List<string>() { "locked", "spying", "resilent", "immune", "ambushing" };
            List<bool> isStatusesActive = new List<bool>() { _isLocked, _isSpy, _isResilent, _isImmune, _isAmbush };
            string sts = "";
            for (int i = 0; i < isStatusesActive.Count; ++i)
                if (isStatusesActive[i])
                    sts += (sts.Length == 0 ? "" : ", ") + statuses[i];
            return sts.Length <= 0 ? "status = no" : String.Format("status = is {0}", sts);
        }
        bool _isLocked;
        bool _isSpy;
        bool _isResilent;
        bool _isImmune;
        bool _isAmbush;
    }
    class SLocationView
    {
        public SLocationView(SLocation location, int indexInPlace, int hostIndex) { _location = location; _index = indexInPlace; _host = hostIndex; }
        int _index;
        int _host;
        SLocation _location;
        public override string ToString() { return String.Format("place = {0}st card in player#{1}'s {2}", _index + 1, _host + 1, _location.ToString()); }
    }
    class SCardView
    {
        public SCardView(SCardName name, List<STag> tags, SClan clan, SRarity rar, SLocationView location, SPowerView power, SStatusView status)
        {
            _name = name;
            _tags = tags;
            _clan = clan;
            _rar = rar;
            _location = location;
            _power = power;
            _status = status;
        }
        SCardName _name;
        List<STag> _tags;
        SClan _clan;
        SRarity _rar;
        SLocationView _location;
        SPowerView _power;
        SStatusView _status;
        public override string ToString() { 
            string tags = "";
            foreach (STag tag in _tags)
                tags += (tags.Length == 0? "" : ", ") + tag.ToString();
            return String.Format("name = {0};\n{1} {2} {3}\n{4}\n{5}\n{6}", _name, _rar, _clan, tags, _location, _power, _status); 
        }
    }
}

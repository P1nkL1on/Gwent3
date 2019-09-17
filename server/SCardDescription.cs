using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class SCardDescription
    {
        public SCardDescription(int defaultPower, SClan clan, SRarity rarity, params STag[] tags)
        {
            _defaultPower = defaultPower;
            _clan = clan;
            _rarity = rarity;
            _tags = tags.ToList();
        }
        public SClan clan { get { return _clan; } }
        public SRarity rarity { get { return _rarity; } }
        public List<STag> tags { get { return _tags; } }
        public int defaultPower { get { return _defaultPower; } }

        SClan _clan;
        SRarity _rarity;
        List<STag> _tags;
        int _defaultPower;
    }
}

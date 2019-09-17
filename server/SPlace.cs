using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    enum SPlace
    {
        none = 0,
        startingdeck,
        deck,
        hand,
        board,
        graveyard,
        leader,
        banish
    }
    enum SRow
    {
        none = -1,
        meele = 0,
        ranged,
        support
    }
    class SLocation
    {
        public SLocation(SPlace place) { _place = place; _row = SRow.none; }
        public SLocation(SRow row) { _place = SPlace.board; _row = row; }
        public SLocation() { _place = SPlace.none; _row = SRow.none; }

        public bool Equals(SLocation location) { return _place == location._place; }
        public bool Equals(SPlace place) { return _place == place && (place != SPlace.board || _row != SRow.none); }
        public bool Equals(SRow row) { return _place == SPlace.board && _row == row; }

        public override string ToString() { return String.Format("{0}{1}", _place.ToString(), _row == SRow.none? "" : String.Format(", {0} row", _row.ToString())); }

        public SPlace place { get { return _place; } }
        public SRow row { get { return _row; } }

        SPlace _place;
        SRow _row;
    }
}

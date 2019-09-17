using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class STimer
    {
        public static STimer none() { STimer st = new STimer(-1); st._enable = false; return st; }
        public static STimer singleUse() { return new STimer(1, false); }
        public static STimer afterEvents(int eventCount) { return new STimer(eventCount, true); }
        public static STimer afterTurns(int turnCount) { return new STimer(turnCount); }
        public static STimer every(int turnCount = 1) { return new STimer(turnCount, true); }

        public STimer defaultCopy() {
            STimer copy = new STimer(_max, _repeat);
            copy._enable = _enable;
            return copy;
        }

        public bool tick(int X = 1)
        {
            if (!_enable)
                return false;
            _now += X;
            bool finished = (_now >= _max);
            if (finished && _repeat)
                _now -= _max;
            if (finished && !_repeat)
                _enable = false; ;
            return finished;
        }

        STimer(int max, bool repeat = false) { _enable = true; _repeat = repeat; _max = max; _now = 0;}

        bool _enable;
        bool _repeat;
        int _max;
        int _now;

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class SStatus
    {
        public SStatus() { _isLocked = false; _isSpy = false; _isResilent = false; _isImmune = false; _isAmbush = false; }

        public bool isLocked { get { return _isLocked; } }
        public bool isSpy { get { return _isSpy; } }
        public bool isResilent { get { return _isResilent; } }
        public bool isImmune { get { return _isImmune; } }
        public bool isAmbush { get { return _isAmbush; } }

        public bool toggleLocked() { return _isLocked = !_isLocked; }
        public bool toggleSpy() { return _isSpy = !_isSpy; }
        public bool toggleResilent() { return _isResilent = !_isResilent; }
        public void setAmbush(bool isAmbush = true) { _isAmbush = isAmbush; }
        public void setImmune(bool isImmune = true) { isImmune = _isImmune; }

        bool _isLocked;
        bool _isSpy;
        bool _isResilent;
        bool _isImmune;
        bool _isAmbush;
    }
}

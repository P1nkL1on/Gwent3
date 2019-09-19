using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class SHitResult
    {
        // public SHitResult() { _shouldBeBanished = _shouldBeDead = false; _healthLost = 0; }
        public SHitResult(bool shouldBeBanished, bool shouldBeDead, bool armorWasBroken, int healthLost, int armorLost) {
            _shouldBeDead = shouldBeDead;
            _shouldBeBanished = shouldBeBanished;
            _armorWasBroken = armorWasBroken;
            _healthLost = healthLost;
            _armorLost = armorLost;
        }

        public bool shouldBeDead { get { return _shouldBeDead; } }
        public bool shouldBeBanished { get { return _shouldBeBanished; } }
        public bool armorWasBroken { get { return _armorWasBroken; } }
        public int healthLost { get { return _healthLost; } }

        bool _shouldBeDead;
        bool _shouldBeBanished;
        bool _armorWasBroken;
        int _healthLost;
        int _armorLost;
    }
    class SPower
    {
        public SPower(int defaultPower) {
            _power = _basePower = _defaultPower = defaultPower;
            _armor = 0;
        }
        public int power { get { return _power; } }
        public int defaultPower { get { return _defaultPower; } }

        public bool isDamaged { get { return _power < _basePower; } }
        public bool isBoosted { get { return _power > _basePower; } }

        public SHitResult damage(int X) {
            bool wasArmorLost = false;
            if (X <= _armor)
            {
                wasArmorLost = X == _armor;
                _armor -= X;
                return new SHitResult(false, false, wasArmorLost, 0, X);
            }
            int armorLost = _armor;
            wasArmorLost = _armor > 0;
            X -= armorLost;
            _armor = 0;
            if (X >= _power)
                return new SHitResult(false, true, wasArmorLost, _power, armorLost);
            _power -= X;
            return new SHitResult(false, false, wasArmorLost, X, armorLost);
        }

        public SHitResult weaken(int X)
        {
            if (X >= _basePower)
                return new SHitResult(true, true, false, 0, 0);
            _power -= X;
            _basePower -= X;
            return new SHitResult(false, false, false, 0, 0);
        }

        public bool boost(int X)
        {
            _power += X;
            return X > 0;
        }

        public bool strengthen(int X)
        {
            _power += X;
            _basePower += X;
            return X > 0;
        }

        public SPowerView view() { return new SPowerView(power, _basePower, _defaultPower, _armor); }

        int _power;
        int _basePower;
        int _defaultPower;
        int _armor;
    }
}

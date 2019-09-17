using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    partial class SCard
    {
        SCards dealDamage(int X)
        {
            return this._game.cards
                .select(SFilter.located(SPlace.board), SFilter.otherThen(this))
                .targetOneCard(this, String.Format("Deal {0} damage to any other unit", X))
                .damage(X, this);
        }
        SCards dealDamageEnemy(int X)
        {
            return this._game.cards
                .select(SFilter.located(SPlace.board), SFilter.enemy(this))
                .targetOneCard(this, String.Format("Deal {0} damage to enemy unit", X))
                .damage(X, this);
        }
        
        public static SCard testArcher
        {
            get
            {
                // set it all basic characteristics
                // add it a trigger of deploying
                // screw source and param parameters
                // only me-SCard is valuable
                // then call CommonFunc dealDamage to
                // any unit 2 times
                // cause description says
                // "Deal 3 damage, then deal 1 damage."
                //
                SCard card = new SCard(SCardName.sArcher, SClan.scoetaels, SRarity.bronze, 7, STag.elf, STag.soldier);
                card.setTrigger(STType.onDeploy, (me, source, none) => { me.dealDamage(3); me.dealDamage(1); });
                return card;
            }
        }
        public static SCard testSkirmisher
        {
            get
            {
                SCard card = new SCard(SCardName.sSkirmisher, SClan.scoetaels, SRarity.bronze, 6, STag.dwarf, STag.soldier);
                card.setTrigger(STType.onDeploy, (me, source, none) => { me.maybe.ifonly(me.dealDamageEnemy(3).isEmpty).boost(3, me); });
                return card;
            }
        }
        public static SCard testSapper
        {
            get
            {
                // test ambush unit
                // when deployed ambush status 
                // seted to true, then added timer
                // single-activated for 2 turns
                // in turn start it ticks
                // and then if done set ambush false
                // tick() function of SCards
                // creates a new SCards(), which
                // contains only cards, who are
                // completed or activated their timers
                //
                SCard card = new SCard(SCardName.sSappers, SClan.scoetaels, SRarity.bronze, 11, STag.elf, STag.soldier);
                card.setTimer(STimer.afterTurns(2));
                card.setTrigger(STType.onDeploy, (me, source, none) => { me.status.setAmbush(); });
                card.setTrigger(STType.onTurnStart, (me, source, none) => { me.maybe.tick().setAmbush(false); });
                return card;
            }
        }
        public static SCard testAlzur
        {
            get
            {
                // logick of special card playing must be on otp level
                // after playing instantly remove it from the field
                // or even from game if it was ressurected
                SCard card = new SCard(SCardName.spAlzur, SClan.neutral, SRarity.bronze, 0, STag.special, STag.spell);
                card.setTrigger(STType.onDeploy, (me, source, none) => { me.dealDamage(9); });
                return card;
            }
        }
    }
}

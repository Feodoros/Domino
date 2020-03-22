using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domino
{
    class MSPlayer
    {
        static public string PlayerName = "Бывалый";
        static private List<MTable.SBone> lHand;
        
        // Отсортированная рука по сумме очков
        static public List<MTable.SBone> newHand = SortHand(lHand);

        #region NotTouchFuncs

        // инициализация игрока
        static public void Initialize()
        {
            lHand = new List<MTable.SBone>();
        }

        // Вывод на экран
        static public void PrintAll()
        { MTable.PrintAll(lHand); }

        // дать количество доминушек
        static public int GetCount()
        { return lHand.Count; }

        #endregion

        // сделать ход
        static public bool MakeStep(out MTable.SBone sb, out bool End)
        {
            sb = lHand.First();
            End = true;
                        
            //состояние стола
            List<MTable.SBone> tableCondition = MTable.GetGameCollection();

            //доминошки на концах цепочки
            MTable.SBone sLeft = tableCondition[0];
            MTable.SBone sRight = tableCondition[tableCondition.Count - 1];

            //доминошки, ушедшие из игры
            List<MTable.SBone> sBones;
            
            //значения, за которыми соперник ходит на базар
            List<int> nopeBones = new List<int>();

            //кол-во доминошек на базаре
            int countBonesOnShop = MTable.GetShopCount();
            
            //кол-во костей в руке оппонента
            int enemyBoneCount = 28 - tableCondition.Count - countBonesOnShop - lHand.Count;
            
            
            // Первый ход (если мы ходим первые)
            if (tableCondition.Count == 0)
            {
                sb = GetBoneOnZeroTurn(newHand);
            }
            
            
            return true;
        }

        #region Logic
        
        // Ставим доминошку с максимальной суммой и еще числа которой повторяются
        static public MTable.SBone GetBoneOnZeroTurn(List<MTable.SBone> newHand)
        {
            MTable.SBone s = newHand[0];
            foreach (var sBone in newHand)
            {
                if (freqNum[sBone.First] > 1 && freqNum[sBone.Second] > 1)
                    return sBone;
                
                if (freqNum[sBone.First] > 1 || freqNum[sBone.Second] > 1)
                    return sBone;
            }
            return s;
        }
        
        
        // добавить доминушку в свою руку
        static public void AddItem(MTable.SBone sb)
        {
            lHand.Add(sb);
        }

        // дать сумму очков на руке
        static public int GetScore()
        {
            int sum = 0;
            
            if (lHand.Count == 1)
            {
                if (lHand[0].First == 0 && lHand[0].Second == 0)
                    sum = 25;
            }

            else
            {
                foreach (var sBone in lHand)
                {
                    sum += sBone.First;
                    sum += sBone.Second;
                }
                
            }
            
            return sum;
        }

        
        #endregion
        
        #region Helpers
        
        // Создаем и заполняем список всех значений на руке
        static public List<int> numbers = FillListWithValues();

        // Создаем и заполняем словарь количества значений у нас на руке
        static public Dictionary<int, int> freqNum = FillDictWithFreqOfValues();
        
        static public List<int> FillListWithValues()
        {
            List<int> numbers = new List<int>(lHand.Count*2);
            foreach (var sBone in lHand)
            {
                numbers.Add(sBone.First);
                numbers.Add(sBone.Second);
            }

            return numbers;
        }

        static public Dictionary<int, int> FillDictWithFreqOfValues()
        {
            Dictionary<int, int> freqNum = new Dictionary<int, int>(lHand.Count);
            int inc = 0;
            
            foreach (var kvp in freqNum)
            {
                freqNum.Add(inc, numbers.Where(x => x == inc).ToList().Count);
                inc++;
            }

            return freqNum;
        }
        
        static public List<MTable.SBone> SortHand (List<MTable.SBone> hand)
        {
            for (int i = 0; i < hand.Count - 1; i++)
            {
                if (hand[i].First + hand[i].Second < hand[i + 1].First + hand[i + 1].Second)
                {
                    var tmp = hand[i + 1];
                    hand[i + 1] = hand[i];
                    hand[i] = tmp;
                    SortHand(hand);
                }
            }

            return hand;
        }
        
        #endregion
        
    }
    
}
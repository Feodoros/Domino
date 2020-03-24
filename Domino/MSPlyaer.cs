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
        
        // Значения, за которыми соперник ходит на базар
        static private List<int> nopeValues;
        
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

            //доминошки, ушедшие из игры
            List<MTable.SBone> sBones;
            
            // Значения, за которыми соперник ходит на базар
            nopeValues = RememberNopeValues(nopeValues);
            
            //кол-во доминошек на базаре
            int countBonesOnShop = MTable.GetShopCount();
            
            //кол-во костей в руке оппонента
            int enemyBoneCount = 28 - tableCondition.Count - countBonesOnShop - lHand.Count;
            
            
            // Первый ход (если мы ходим первые)
            if (tableCondition.Count == 0)
            {
                sb = GetBoneOnZeroTurn(newHand);
                lHand.Remove(sb);
                newHand.Remove(sb);
            }

            // Делаем ход, если уже есть доминошки на столе
            if (tableCondition.Count != 0)
            {
                // 1 Доминошка на столе
                if (tableCondition.Count == 1)
                {
                    // Доминошки на концах цепочки
                    MTable.SBone sLeft = tableCondition[0];
                    
                    // Значения на концах
                    leftValue = sLeft.First;
                    rightValue = sLeft.Second;
                }
                
                // Несколько доминошек на столе
                if (tableCondition.Count >= 2)
                {
                    // Доминошки на концах цепочки
                    MTable.SBone sLeft = tableCondition[0];
                    MTable.SBone sRight = tableCondition[tableCondition.Count - 1];

                    // Доминошки предыдущие за последними
                    MTable.SBone sLeftNext = tableCondition[1];
                    MTable.SBone sPreRight = tableCondition[tableCondition.Count - 2];

                    // Левые 2 последние доминошки соеденены First'ом
                    if (sLeftNext.First == sLeft.First)
                    {
                        leftValue = sLeft.Second;
                    }

                    // Левые 2 последние доминошки соеденены Second'ом
                    if (sLeftNext.Second == sLeft.Second)
                    {
                        leftValue = sLeft.First;
                    }


                    // Правые 2 последние доминошки соеденены First'ом
                    if (sLeftNext.First == sLeft.First)
                    {
                        rightValue = sLeft.Second;
                    }

                    // Правые 2 последние доминошки соеденены Second'ом
                    if (sLeftNext.Second == sLeft.Second)
                    {
                        rightValue = sLeft.First;
                    }

                }

                // Если мы не можем походить, то обращаемся к базару
                while (!numbers.Contains(rightValue) && !numbers.Contains(leftValue))
                {
                    // Проверяем есть ли доминошки в базаре,
                    // Если есть, то берем
                    MTable.SBone newSBone;
                    if (MTable.GetFromShop(out newSBone))
                    {
                        lHand.Add(newSBone);
                        numbers = FillListWithValues();
                        newHand = SortHand(lHand);
                        return true;
                    }
                    
                    // Если нет, то пропускаем ход
                    if (!MTable.GetFromShop(out newSBone))
                    {
                        return false;
                    }
                }
                
                // Можем походить 
                // Отсортированная рука по сумме и с подходящеми доминошками (чем можем походить)
                List<MTable.SBone> suitableHand = new List<MTable.SBone>();
                foreach (var sBone in newHand)
                {
                    if (sBone.First == rightValue || sBone.First == leftValue ||
                        sBone.Second == rightValue || sBone.Second == leftValue)
                        suitableHand.Add(sBone);
                }

                foreach (var sBone in suitableHand)
                {
                    // Если мы знаем что соперник ходил на базар
                    if (nopeValues.Count != 0)
                    {
                        // Первая часть доминошки соединяется со столом и вторая часть -- значение, которого нет у соперника
                        if (sBone.First == leftValue || sBone.First == rightValue)
                        {
                            if (nopeValues.Contains(sBone.Second))
                            {

                            }
                        }

                        // Вторая часть доминошки соединяется со столом и первая часть -- значение, которого нет у соперника
                        if (sBone.Second == leftValue || sBone.Second == rightValue)
                        {
                            if (nopeValues.Contains(sBone.First))
                            {

                            }
                        }
                    }

                    // Соперник не ходил на базар
                    if (nopeValues.Count == 0)
                    {
                        // Не в конце игры (когда базар еще не пустой)
                        // Есть ли на руках доминошка, которую можем положить на оба конца, то мы ее кладем
                        // Какой стороной кладем?
                        // 1) Постараться положить так, чтобы на концах были значения, которые у нас есть 
                        // 2) Логика с кол-м значений в игре
                        
                        // Конец игры
                        
                    }
                }
                
                
                
            }

            countBonesInShop = MTable.GetShopCount();
            return true;
        }

        #region Logic
        
        // Ставим доминошку с максимальной суммой и еще числа которой повторяются на нулевой ход
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
        
        // Запоминаем значения, за которыми проитивник ходит на базар
        static public List<int> RememberNopeValues(List<int> nopeValues)
        {
            if (countBonesInShop != MTable.GetShopCount())
            {
                nopeValues.Add(rightValue);
                nopeValues.Add(leftValue);
            }

            return nopeValues;
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
        
        // Значения на краях стола
        static public int rightValue;
        static public int leftValue;
        
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

        static public int countBonesInShop;

        #endregion

    }
    
}
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

        // Сделать ход
        public static bool MakeStep(out MTable.SBone sb, out bool End)
        {
            List<MTable.SBone> newHand = SortHand(lHand);
            List<int> numbersInHand = FillListWithValues(lHand);
            Dictionary<int, int> freqNumInHand = FillDictWithFreqOfValues(lHand);
            List<int> numbersInTable = FillListWithValues(MTable.GetGameCollection());
            
            // Создаем и заполняем словарь количества значений у нас на столе
            Dictionary<int, int> freqNumInTable = FillDictWithFreqOfValues(MTable.GetGameCollection());

            sb = lHand.First();
            //End = true;
                        
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
            
            // 1 Доминошка на столе
            if (tableCondition.Count == 1)
            {
                // Доминошки на концах цепочки
                MTable.SBone sLeft = tableCondition[0];
                
                // Значения на концах
                leftValue = sLeft.First;
                rightValue = sLeft.Second;
                
                // Если мы не можем походить, то обращаемся к базару
                bool checkHand = !numbersInHand.Contains(rightValue) && !numbersInHand.Contains(leftValue);
                if (checkHand)
                {
                    return GoToShop(out sb, out End);

                }
                
                // Можем походить 
                if (!checkHand)
                {
                    // Отсортированная рука по сумме и с подходящеми доминошками (чем можем походить)
                    List<MTable.SBone> suitableHand1 = new List<MTable.SBone>();
                    foreach (var sBone in newHand)
                    {
                        if (sBone.First == rightValue || sBone.First == leftValue ||
                            sBone.Second == rightValue || sBone.Second == leftValue)
                            suitableHand1.Add(sBone);
                    }

                    // 1 подходящая доминошка
                    if (suitableHand1.Count == 1)
                    {
                        sb = suitableHand1[0];
                    }

                    // Несколько подходящих доминошек
                    if (suitableHand1.Count > 1)
                    {
                        // Частота значений на руке
                        var sortedFreqDict = (from entry in freqNumInHand 
                            orderby entry.Value descending select entry);

                        List<int> sortedListOfFreq = new List<int>();
                        foreach (var kvp in sortedFreqDict)
                        {
                            sortedListOfFreq.Add(kvp.Key);
                        }
                        
                        // Частота значений на столе
                        var sortedFreqDictTable = (from entry in freqNumInTable 
                            orderby entry.Value descending select entry);
                        
                        List<int> sortedListOfFreqTable = new List<int>();
                        foreach (var kvp in sortedFreqDictTable)
                        {
                            sortedListOfFreqTable.Add(kvp.Key);
                        }
                        
                        
                        List<bool> checkFreqValues = new List<bool>();
                        foreach (var sBone in suitableHand1)
                        {
                            if (sBone.First == rightValue || sBone.First == leftValue)
                            {
                                if(freqNumInHand[sBone.Second] == 1)
                                    checkFreqValues.Add(true);
                                if(freqNumInHand[sBone.Second] > 1)
                                {
                                    checkFreqValues.Add(false);
                                }
                            }
                            
                            if (sBone.Second == rightValue || sBone.Second == leftValue)
                            {
                                if(freqNumInHand[sBone.First] == 1)
                                    checkFreqValues.Add(true);
                                if(freqNumInHand[sBone.First] > 1)
                                {
                                    checkFreqValues.Add(false);
                                }
                            }
                        }

                        // Все вторые половинки в одном экземпляре
                        bool oneRepeatValue = !checkFreqValues.Contains(false);
                        
                        
                        // Выкидываем доминошку с макс суммой
                        // И которая повторяется больше всех (у нас на руке)
                        // И так, чтобы у нас на руке остались выкидываемые значения
                        // Если все вторые половинки только в одном экземпляре,
                        // То выкидываем доминошку со второй половиной самой популярной на столе
                        if (!oneRepeatValue)
                        {
                            MTable.SBone zeroBone;
                            zeroBone.First = 0;
                            zeroBone.Second = 0;
                                
                            sortedListOfFreq.Reverse();
                            foreach (var value in sortedListOfFreq)
                            {
                                foreach (var sBone in suitableHand1)
                                {
                                    if (sBone.First == rightValue || sBone.First == leftValue)
                                    {
                                        if (sBone.Second == value)
                                        {
                                            sb = sBone;
                                            break;
                                        }
                                    }
                                    if (sBone.Second == rightValue || sBone.Second == leftValue)
                                    {
                                        if (sBone.First == value)
                                        {
                                            sb = sBone;
                                            break;
                                        }
                                    }
                                }
                            }
                                
                            if (suitableHand1.Contains(zeroBone))
                            {
                                sb = zeroBone;
                            }
                        }
                        
                        // Вторые половинки в одном экземпляре
                        if (oneRepeatValue)
                        {
                            MTable.SBone zeroBone;
                            zeroBone.First = 0;
                            zeroBone.Second = 0;
                                
                            sortedListOfFreqTable.Reverse();
                            foreach (var value in sortedListOfFreqTable)
                            {
                                foreach (var sBone in suitableHand1)
                                {
                                    if (sBone.First == rightValue || sBone.First == leftValue)
                                    {
                                        if (sBone.Second == value)
                                        {
                                            sb = sBone;
                                            break;
                                        }
                                    }
                                    if (sBone.Second == rightValue || sBone.Second == leftValue)
                                    {
                                        if (sBone.First == value)
                                        {
                                            sb = sBone;
                                            break;
                                        }
                                    }
                                }
                            }
                                
                            if (suitableHand1.Contains(zeroBone))
                            {
                                sb = zeroBone;
                            }
                        }
                        
                        
                    }
                    
                }

            }
            
            // Несколько доминошек на столе
            if (tableCondition.Count >= 2)
            {
                // Доминошки на концах цепочки
                MTable.SBone sLeft = tableCondition[0];
                MTable.SBone sRight = tableCondition[tableCondition.Count - 1];

                leftValue = sLeft.First;
                rightValue = sRight.Second;

                // Если мы не можем походить, то обращаемся к базару
                bool checkHand = !numbersInHand.Contains(rightValue) && !numbersInHand.Contains(leftValue);
                if (checkHand)
                {
                    return GoToShop(out sb, out End);

                }
                
                // Можем походить 
                else
                {
                    // Отсортированная рука по сумме и с подходящеми доминошками (чем можем походить)
                    List<MTable.SBone> suitableHand = new List<MTable.SBone>();
                    foreach (var sBone in newHand)
                    {
                        if (sBone.First == rightValue || sBone.First == leftValue ||
                            sBone.Second == rightValue || sBone.Second == leftValue)
                            suitableHand.Add(sBone);
                    }

                    // 1 подходящая доминошка
                    if (suitableHand.Count == 1)
                    {
                         sb = suitableHand[0];
                    }
                    
                    // Несколько подходящих доминошек
                    else
                    {
                        // Частота значений на руке
                        var sortedFreqDict = (from entry in freqNumInHand 
                            orderby entry.Value descending select entry);

                        List<int> sortedListOfFreq = new List<int>();
                        foreach (var kvp in sortedFreqDict)
                        {
                            sortedListOfFreq.Add(kvp.Key);
                        }
                        
                        // Частота значений на столе
                        var sortedFreqDictTable = (from entry in freqNumInTable 
                            orderby entry.Value descending select entry);
                        
                        List<int> sortedListOfFreqTable = new List<int>();
                        foreach (var kvp in sortedFreqDictTable)
                        {
                            sortedListOfFreqTable.Add(kvp.Key);
                        }
                        
                        List<bool> checkFreqValues = new List<bool>();
                        foreach (var sBone in suitableHand)
                        {
                            if (sBone.First == rightValue || sBone.First == leftValue)
                            {
                                if(freqNumInHand[sBone.Second] == 1)
                                    checkFreqValues.Add(true);
                                if(freqNumInHand[sBone.Second] > 1)
                                {
                                    checkFreqValues.Add(false);
                                }
                            }
                            
                            if (sBone.Second == rightValue || sBone.Second == leftValue)
                            {
                                if(freqNumInHand[sBone.First] == 1)
                                    checkFreqValues.Add(true);
                                if(freqNumInHand[sBone.First] > 1)
                                {
                                    checkFreqValues.Add(false);
                                }
                            }
                        }

                        // Все вторые половинки в одном экземпляре
                        bool oneRepeatValue = !checkFreqValues.Contains(false);
                        
                        // У меня и у соперника <= 3 доминошек на руке
                        bool ifEndGame = lHand.Count <= 3 && enemyBoneCount <= 3;

                        // Середины игры
                        if (!ifEndGame)
                        {
                            // Выкидываем доминошку с макс суммой
                            // И которая повторяется больше всех (у нас на руке)
                            // И так, чтобы у нас на руке остались выкидываемые значения
                            // Если все вторые половинки только в одном экземпляре, 
                            // То заставляем соперника пойти на базар
                            // Если не можем, 
                            // То выкидываем доминошку со второй половиной самой популярной на столе
                            if (!oneRepeatValue)
                            {
                                MTable.SBone zeroBone;
                                zeroBone.First = 0;
                                zeroBone.Second = 0;
                                
                                sortedListOfFreq.Reverse();
                                foreach (var value in sortedListOfFreq)
                                {
                                    foreach (var sBone in suitableHand)
                                    {
                                        if (sBone.First == rightValue || sBone.First == leftValue)
                                        {
                                            if (sBone.Second == value)
                                            {
                                                sb = sBone;
                                                break;
                                            }
                                        }
                                        if (sBone.Second == rightValue || sBone.Second == leftValue)
                                        {
                                            if (sBone.First == value)
                                            {
                                                sb = sBone;
                                                break;
                                            }
                                        }
                                    }
                                }
                                
                                if (suitableHand.Contains(zeroBone))
                                {
                                    sb = zeroBone;
                                }
                            }
                            
                            // Вторые половинки в одном экземпляре
                            if (oneRepeatValue)
                            {
                                // Можем заставить пойти на базар
                                if (nopeValues.Intersect(FillListWithValues(suitableHand)).ToList().Count != 0)
                                {
                                    foreach (var sBone in suitableHand)
                                    {
                                        if (leftValue == sBone.First && nopeValues.Contains(sBone.Second))
                                        {
                                            sb = sBone;
                                            break;
                                        }
                                        
                                        if (rightValue == sBone.First && nopeValues.Contains(sBone.Second))
                                        {
                                            sb = sBone;
                                            break;
                                        }
                                        
                                        if (leftValue == sBone.Second && nopeValues.Contains(sBone.First))
                                        {
                                            sb = sBone;
                                            break;
                                        }
                                        
                                        if (rightValue == sBone.Second && nopeValues.Contains(sBone.First))
                                        {
                                            sb = sBone;
                                            break;
                                        }
                                            
                                    }
                                }

                                // Не можем заставить пойти на базар, т.о.
                                // Смотрим на стол
                                if (nopeValues.Intersect(FillListWithValues(suitableHand)).ToList().Count == 0)
                                {
                                    
                                    MTable.SBone zeroBone;
                                    zeroBone.First = 0;
                                    zeroBone.Second = 0;
                                
                                    sortedListOfFreqTable.Reverse();
                                    foreach (var value in sortedListOfFreqTable)
                                    {
                                        foreach (var sBone in suitableHand)
                                        {
                                            if (sBone.First == rightValue || sBone.First == leftValue)
                                            {
                                                if (sBone.Second == value)
                                                {
                                                    sb = sBone;
                                                    break;
                                                }
                                            }
                                            if (sBone.Second == rightValue || sBone.Second == leftValue)
                                            {
                                                if (sBone.First == value)
                                                {
                                                    sb = sBone;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                
                                    if (suitableHand.Contains(zeroBone))
                                    {
                                        sb = zeroBone;
                                    }
                                }
                                
                            }
                        }
                        
                        // Конец игры
                        if (ifEndGame)
                        {
                            // Можем заставить пойти на базар
                            if (nopeValues.Intersect(FillListWithValues(suitableHand)).ToList().Count != 0)
                            {
                                foreach (var sBone in suitableHand)
                                {
                                    if (leftValue == sBone.First && nopeValues.Contains(sBone.Second))
                                    {
                                        sb = sBone;
                                        break;
                                    }
                                    
                                    if (rightValue == sBone.First && nopeValues.Contains(sBone.Second))
                                    {
                                        sb = sBone;
                                        break;
                                    }
                                    
                                    if (leftValue == sBone.Second && nopeValues.Contains(sBone.First))
                                    {
                                        sb = sBone;
                                        break;
                                    }
                                    
                                    if (rightValue == sBone.Second && nopeValues.Contains(sBone.First))
                                    {
                                        sb = sBone;
                                        break;
                                    }
                                        
                                }
                            }

                            // Не можем заставить пойти на базар, т.о.
                            // Смотрим на руку
                            if (nopeValues.Intersect(FillListWithValues(suitableHand)).ToList().Count == 0)
                            {
                                MTable.SBone zeroBone;
                                zeroBone.First = 0;
                                zeroBone.Second = 0;
                                
                                sortedListOfFreq.Reverse();
                                foreach (var value in sortedListOfFreq)
                                {
                                    foreach (var sBone in suitableHand)
                                    {
                                        if (sBone.First == rightValue || sBone.First == leftValue)
                                        {
                                            if (sBone.Second == value)
                                            {
                                                sb = sBone;
                                                break;
                                            }
                                        }
                                        if (sBone.Second == rightValue || sBone.Second == leftValue)
                                        {
                                            if (sBone.First == value)
                                            {
                                                sb = sBone;
                                                break;
                                            }
                                        }
                                    }
                                }
                                
                                if (suitableHand.Contains(zeroBone))
                                {
                                    sb = zeroBone;
                                }
                            }
                                
                        }

                        // Оборона (мы явно проигрываем)
                        if (enemyBoneCount*2 <= lHand.Count)
                        {
                            // Можем заставить пойти на базар
                            if (nopeValues.Intersect(FillListWithValues(suitableHand)).ToList().Count != 0)
                            {
                                foreach (var sBone in suitableHand)
                                {
                                    if (leftValue == sBone.First && nopeValues.Contains(sBone.Second))
                                    {
                                        sb = sBone;
                                        break;
                                    }
                                    
                                    if (rightValue == sBone.First && nopeValues.Contains(sBone.Second))
                                    {
                                        sb = sBone;
                                        break;
                                    }
                                    
                                    if (leftValue == sBone.Second && nopeValues.Contains(sBone.First))
                                    {
                                        sb = sBone;
                                        break;
                                    }
                                    
                                    if (rightValue == sBone.Second && nopeValues.Contains(sBone.First))
                                    {
                                        sb = sBone;
                                        break;
                                    }
                                        
                                }
                            }

                            // Не можем заставить пойти на базар, т.о.
                            // Смотрим на руку
                            if (nopeValues.Intersect(FillListWithValues(suitableHand)).ToList().Count == 0)
                            {
                                MTable.SBone zeroBone;
                                zeroBone.First = 0;
                                zeroBone.Second = 0;
                                
                                sortedListOfFreq.Reverse();
                                foreach (var value in sortedListOfFreq)
                                {
                                    foreach (var sBone in suitableHand)
                                    {
                                        if (sBone.First == rightValue || sBone.First == leftValue)
                                        {
                                            if (sBone.Second == value)
                                            {
                                                sb = sBone;
                                                break;
                                            }
                                        }
                                        if (sBone.Second == rightValue || sBone.Second == leftValue)
                                        {
                                            if (sBone.First == value)
                                            {
                                                sb = sBone;
                                                break;
                                            }
                                        }
                                    }
                                }
                                
                                if (suitableHand.Contains(zeroBone))
                                {
                                    sb = zeroBone;
                                }
                            }
                        }
                        
                        // Если кол-во очков на базаре и на руке у соперника 
                        // Больше, чем у нас, то вынудим соперника взять весь базар
                        /*if (193 - GetScore() + GetScoreFromTable() < GetScore() + GetScoreFromTable())
                        {
                            // Список всех значений на столе и на нашей руке
                            List<int> FullListOfValues = numbersInHand.Concat(numbersInTable).ToList();
                            
                            // Список всех доминошек на руке(которые подходят) и на столе
                            List<MTable.SBone> FullListOfSbones = MTable.GetGameCollection().Concat(suitableHand).ToList();
                            
                            // Словарь кол-ва значенийй на руке и на столе
                            Dictionary<int, int> FullFreqDict =
                                FillDictWithFreqOfValues(FullListOfSbones);

                            // Лист из значений, чья частота 5 или 6
                            List<int> mostFreqList = new List<int>();
                            foreach (var kvp in FullFreqDict)
                            {
                                if(kvp.Value >= 5)
                                    mostFreqList.Add(kvp.Key);
                            }

                            foreach (var sBone in suitableHand)
                            {
                                // Проверяем можем ли положить доминошку с такими значениями,
                                // Которых больше всего на столе и у нас на руке 
                                if ((sBone.First == rightValue && mostFreqList.Contains(sBone.Second) &&
                                     sBone.Second == leftValue) ||
                                    (sBone.Second == rightValue && mostFreqList.Contains(sBone.First) &&
                                     sBone.First == leftValue) ||
                                    (sBone.First == leftValue && mostFreqList.Contains(sBone.Second) &&
                                     sBone.Second == rightValue) ||
                                    (sBone.Second == leftValue && mostFreqList.Contains(sBone.First) &&
                                     sBone.First == rightValue))
                                {
                                    sb = sBone;
                                    break;
                                }
                            }
                            
                        
                    
                        } */
                        
                    }
                    
                }
            }

            countBonesInShop = MTable.GetShopCount();
            
            lHand.Remove(sb);
            if (rightValue == sb.First || rightValue == sb.Second)
                End = true;
                                
            else
                End = false;
            
            return true;
        }

        #region Logic
        
        // Подсчет кол-ва очков на столе и у себя в руке
        public static int GetScoreFromTable()
        {
            int sum = 0;
            foreach (var sBone in MTable.GetGameCollection())
            {
                sum += sBone.First;
                sum += sBone.Second;
            }

            return sum;
        }

        // Запоминаем значения, за которыми проитивник ходит на базар
        private static List<int> RememberNopeValues(List<int> nopeValues)
        {
            if (countBonesInShop != MTable.GetShopCount())
            {
                nopeValues.Add(rightValue);
                nopeValues.Add(leftValue);
            }

            return nopeValues;
        }
        
        // Добавить доминушку в свою руку
        public static void AddItem(MTable.SBone sb)
        {
            lHand.Add(sb);
        }

        // Дать сумму очков на руке
        public static int GetScore()
        {
            int sum = 0;
            
            if (lHand.Count == 1)
            {
                if (lHand[0].First == 0 && lHand[0].Second == 0)
                    sum = 25;
                else 
                    sum = lHand[0].First + lHand[0].Second;
            }

            else
            {
                foreach (var sBone in lHand)
                {
                    sum += sBone.First + sBone.Second;
                }
                
            }
            
            return sum;
        }

        // Идем на базар
        public static bool GoToShop(out MTable.SBone sb, out bool End)
        {
            sb = lHand.Last();
            End = true;

            List<int> numbersInHand = FillListWithValues(lHand);
            
            bool checkHand = !numbersInHand.Contains(rightValue) && !numbersInHand.Contains(leftValue);
            while (checkHand)
            {
                // Проверяем есть ли доминошки в базаре,
                // Если есть, то берем
                MTable.SBone newSBone;
                bool emptyShop = MTable.GetFromShop(out newSBone);
                if (emptyShop)
                {
                    lHand.Add(newSBone);
                    numbersInHand = FillListWithValues(lHand);
                    checkHand = !numbersInHand.Contains(rightValue) && !numbersInHand.Contains(leftValue);
                    if (!checkHand)
                    {
                                
                        sb = lHand.Last();
                        lHand.Remove(sb);
                        if (rightValue == sb.First || rightValue == sb.Second)
                            End = true;
                                
                        else
                            End = false;
                                
                        return true;
                    }

                    else
                    {
                        sb = lHand.Last();
                        End = true;
                    }
                                
                }
                    
                // Если нет, то пропускаем ход
                else
                {
                    sb = lHand.Last();
                    End = true;
                    return false;
                }
                            
            }

            return false;
        }
        
        #endregion
        
        #region Helpers
        
        // Значения, за которыми соперник ходит на базар
        static private List<int> nopeValues = new List<int>(){-1};
        
        // Значения на краях стола
        static public int rightValue;
        static public int leftValue;
        
        static public List<int> FillListWithValues(List<MTable.SBone> list)
        {
            List<int> numbers = new List<int>(list.Count*2);
            foreach (var sBone in list)
            {
                if (sBone.First == sBone.Second)
                {
                    numbers.Add(sBone.First);
                }
                else
                {
                    numbers.Add(sBone.First);
                    numbers.Add(sBone.Second);
                }
                
            }

            return numbers;
        }

        static public Dictionary<int, int> FillDictWithFreqOfValues(List<MTable.SBone> list)
        {
            List<int> numbers = FillListWithValues(list);
            Dictionary<int, int> freqNum = new Dictionary<int, int>();

            for (int i = 0; i < 7; i++)
            {
                if (numbers.Contains(i))
                {
                    freqNum.Add(i, numbers.Where(x => x == i).ToList().Count);
                }
                    
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

        static public int countBonesInShop = 13;

        #endregion
    }
    
}
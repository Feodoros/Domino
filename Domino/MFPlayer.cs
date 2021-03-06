﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domino
{
    class MFPlayer
    {
        static public string PlayerName = "Балбес";
        static private List<MTable.SBone> lHand;


        //=== Готовые функции =================
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

        //=== Функции для разработки =================
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

        // сделать ход
        static public bool MakeStep(out MTable.SBone sb, out bool End)
        {
            List<int> numbersInHand = FillListWithValues(lHand);
            
            sb = lHand.First();
            End = true;
            
            if (MTable.GetGameCollection().Count >= 1)
            {
                // 1 Доминошка на столе
                if (MTable.GetGameCollection().Count == 1)
                {
                    // Доминошки на концах цепочки
                    MTable.SBone sLeft = MTable.GetGameCollection()[0];

                    // Значения на концах
                    int leftValue = sLeft.First;
                    int rightValue = sLeft.Second;


                    // Если мы не можем походить, то обращаемся к базару
                    bool checkHand = !numbersInHand.Contains(rightValue) && !numbersInHand.Contains(leftValue);
                    if (checkHand)
                    {
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
                                
                            }
                    
                            // Если нет, то пропускаем ход
                            if (!emptyShop)
                            {
                                sb = lHand.Last();
                                End = true;
                                return false;
                            }
                            
                        }
                    }

                    // Можем походить 
                    if (!checkHand)
                    {
                        // Рука с подходящеми доминошками (чем можем походить)
                        List<MTable.SBone> suitableHand1 = new List<MTable.SBone>();
                        foreach (var sBone in lHand)
                        {
                            if (sBone.First == rightValue || sBone.First == leftValue ||
                                sBone.Second == rightValue || sBone.Second == leftValue)
                                suitableHand1.Add(sBone);
                        }

                        Random r = new Random();
                        sb = suitableHand1[r.Next(0, suitableHand1.Count)];
                        lHand.Remove(sb);
                        if (rightValue == sb.First || rightValue == sb.Second)
                            End = true;
                                
                        else
                            End = false;
                                
                        return true;
                    }
                }

                List<MTable.SBone> tableCondition = MTable.GetGameCollection();
                // Несколько доминошек на столе
                if (tableCondition.Count >= 2)
                {
                    // Доминошки на концах цепочки
                    MTable.SBone sLeft = tableCondition[0];
                    MTable.SBone sRight = tableCondition[tableCondition.Count - 1];
                    

                    int leftValue = sLeft.First;
                    int rightValue = sRight.Second;
                    
                    
                    // Если мы не можем походить, то обращаемся к базару
                    bool checkHand = !numbersInHand.Contains(rightValue) && !numbersInHand.Contains(leftValue);
                    if (checkHand)
                    {
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
                                
                            }
                    
                            // Если нет, то пропускаем ход
                            if (!emptyShop)
                            {
                                sb = lHand.Last();
                                End = true;
                                return false;
                            }
                            
                        }
                        
                    }
                    
                    // Можем походить 
                    if(!checkHand)
                    {
                        // Рука с подходящеми доминошками (чем можем походить)
                        List<MTable.SBone> suitableHand = new List<MTable.SBone>();
                        foreach (var sBone in lHand)
                        {
                            if (sBone.First == rightValue || sBone.First == leftValue ||
                                sBone.Second == rightValue || sBone.Second == leftValue)
                                suitableHand.Add(sBone);
                        }
                        
                        Random r = new Random();
                        sb = suitableHand[r.Next(0, suitableHand.Count)];
                        lHand.Remove(sb);
                        if (rightValue == sb.First || rightValue == sb.Second)
                            End = true;
                                
                        else
                            End = false;
                                
                        return true;
                    }
                }
            }
            
            return true;

        }
        
         
        static public List<int> FillListWithValues(List<MTable.SBone> list)
        {
            List<int> numbers = new List<int>(list.Count*2);
            foreach (var sBone in list)
            {
                numbers.Add(sBone.First);
                numbers.Add(sBone.Second);
            }

            return numbers;
        }

    }
}
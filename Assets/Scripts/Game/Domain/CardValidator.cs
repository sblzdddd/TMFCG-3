using System;
using System.Collections.Generic;
using System.Linq;
using Game.Shared.TmiCards.Card;

namespace Game.Domain
{
    public class CardValidator
    {
        // Internal representation of a hand type analysis
        private class HandAnalysis
        {
            public HandType Type;
            public int MainRank; // Comparable rank logic
            public int Length;
        }

        // Rules state that Blank cards can be any rank 3-A
        public CardValue WildRank { get; set; } = CardValue.Three; // Default, should be set by game logic

        public HandValidationResult Validate(HandCard prev, HandCard next)
        {
            if (next?.Cards == null || next.Cards.Count == 0)
                return new HandValidationResult(false, HandError.Incorrect, "No cards played");

            var nextOptions = Analyze(next.Cards);
            if (nextOptions.Count == 0)
                return new HandValidationResult(false, HandError.Incorrect, "Invalid hand combination");

            // If free play (first hand or previous player passed/cleared)
            if (prev?.Cards == null || prev.Cards.Count == 0)
            {
                // Just need to be a valid known type
                return new HandValidationResult(true, HandError.NoError, "");
            }

            var prevOptions = Analyze(prev.Cards);
            if (prevOptions.Count == 0)
            {
                // Assume prev was valid at the time, but if we can't parse it now, ignore it? 
                // Treat as free play.
                return new HandValidationResult(true, HandError.NoError, "");
            }

            // Check if ANY interpretation of 'next' can beat the best interpretation of 'prev'
            foreach (var nOpt in nextOptions)
            {
                switch (nOpt.Type)
                {
                    case HandType.Rocket:
                    {
                        // Rocket beats everything except possibly a bigger bomb?
                        // Rule: Rocket < 4-bomb.
                        // If prev is Rocket, cannot beat.
                        // If prev is Bomb >= 4 cards, cannot beat.
                        var beaten = false;
                        foreach(var pOpt in prevOptions)
                        {
                            switch (pOpt.Type)
                            {
                                case HandType.Rocket:
                                case HandType.NOfKind when pOpt.Length >= 4:
                                    beaten = true;
                                    break;
                                case HandType.Single:
                                case HandType.Pair:
                                case HandType.ConsecutivePairs:
                                case HandType.Straight:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        if (!beaten) return new HandValidationResult(true, HandError.NoError, "");
                        break;
                    }
                    case HandType.NOfKind:
                    {
                        // Next is Bomb
                        var wins = true;
                        // It must beat Prev.
                    
                        foreach(var pOpt in prevOptions)
                        {
                            switch (pOpt.Type)
                            {
                                case HandType.Rocket:
                                {
                                    if (nOpt.Length < 4) wins = false;
                                    break;
                                }
                                case HandType.NOfKind:
                                {
                                    if (CompareBomb(pOpt, nOpt) <= 0) wins = false;
                                    break;
                                }
                                case HandType.Single:
                                case HandType.Pair:
                                case HandType.ConsecutivePairs:
                                case HandType.Straight:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        if (wins) return new HandValidationResult(true, HandError.NoError, "");
                        break;
                    }
                    case HandType.Single:
                    case HandType.Pair:
                    case HandType.ConsecutivePairs:
                    case HandType.Straight:
                    default:
                    {
                        // Next is Normal
                        // Must match type of Prev
                        if (prevOptions.Where(pOpt => pOpt.Type == nOpt.Type && pOpt.Length == nOpt.Length).Any(pOpt => nOpt.MainRank > pOpt.MainRank))
                        {
                            return new HandValidationResult(true, HandError.NoError, "");
                        }

                        break;
                    }
                }
            }

            return new HandValidationResult(false, HandError.LowerValue, "Hand is not large enough or type mismatch");
        }

        private static int CompareBomb(HandAnalysis prev, HandAnalysis next)
        {
            // Count priority
            if (next.Length > prev.Length) return 1;
            if (next.Length < prev.Length) return -1;
            // Rank priority
            return next.MainRank.CompareTo(prev.MainRank);
        }

        private List<HandAnalysis> Analyze(List<HandCardItem> cards)
        {
            var results = new List<HandAnalysis>();
            var count = cards.Count;
            if (count == 0) return results;

            var wildCount = cards.Count(IsWild);
            var nonWilds = cards.Where(c => !IsWild(c)).ToList();
            var jokerSmallCount = cards.Count(c => c.Suit == CardSuit.JokerBlack); 
            var jokerBigCount = cards.Count(c => c.Suit == CardSuit.JokerRed);

            switch (count)
            {
                // 1. Rocket
                case 2 when jokerSmallCount == 1 && jokerBigCount == 1:
                    results.Add(new HandAnalysis { Type = HandType.Rocket, Length = 2, MainRank = 100 });
                    break;
                // 2. Single
                case 1:
                {
                    var c = cards[0];
                    var r = GetLogicRank(c); 
                    if (IsWild(c) && !IsJoker(c)) r = 13; // Wild Alone rank (Between 2 and Small Joker)
                
                    results.Add(new HandAnalysis { Type = HandType.Single, Length = 1, MainRank = r });
                    break;
                }
            }

            switch (count)
            {
                // 3. Pair
                case 2:
                {
                    if (CanBeSameRank(nonWilds))
                    {
                        var r = GetMaxRank(nonWilds);
                        if (nonWilds.Count == 0) r = 12; // Pair of 2s (Max)
                        results.Add(new HandAnalysis { Type = HandType.Pair, Length = 2, MainRank = r });
                    }

                    break;
                }
                // 4. NOfKind (Bomb) >= 3
                case >= 3:
                {
                    if (CanBeSameRank(nonWilds))
                    {
                        var r = GetMaxRank(nonWilds);
                        if (nonWilds.Count == 0) r = 12; // Bomb of 2s
                        results.Add(new HandAnalysis { Type = HandType.NOfKind, Length = count, MainRank = r });
                    }

                    break;
                }
            }

            // 5. Straight >= 2
            if (count >= 2 && !Has2OrJoker(nonWilds))
            {
                var sorted = nonWilds.Select(c => GetBaseRank(c.Value)).OrderBy(x => x).ToList();
                for (var start = 0; start <= 11 - count + 1; start++)
                {
                    if (!IsStraightMatch(sorted, wildCount, start, count)) continue;
                    var maxRank = start + count - 1;
                    results.Add(new HandAnalysis { Type = HandType.Straight, Length = count, MainRank = maxRank });
                }
            }

            // 6. ConsecutivePairs >= 4, even count
            if (count < 4 || count % 2 != 0 || Has2OrJoker(nonWilds)) return results;
            
            var numPairs = count / 2;
            var ranks = nonWilds.Select(c => GetBaseRank(c.Value)).ToList();
            
            for (var start = 0; start <= 11 - numPairs + 1; start++)
            {
                if (!IsConsecutivePairMatch(ranks, wildCount, start, numPairs)) continue;
                var maxRank = start + numPairs - 1;
                results.Add(new HandAnalysis { Type = HandType.ConsecutivePairs, Length = count, MainRank = maxRank });
            }

            return results;
        }

        private bool IsWild(HandCardItem c)
        {
            return c.Value == WildRank;
        }

        private static bool IsJoker(HandCardItem c)
        {
            return c.Suit is CardSuit.JokerBlack or CardSuit.JokerRed;
        }

        private bool Has2OrJoker(List<HandCardItem> cards)
        {
            foreach(var c in cards)
            {
                if (c.Value == CardValue.Two) return true;
                if (IsJoker(c)) return true;
            }
            return false;
        }

        private static bool CanBeSameRank(List<HandCardItem> nonWilds)
        {
            if (nonWilds.Count <= 1) return true;
            var first = nonWilds[0].Value;
            return nonWilds.All(c => c.Value == first);
        }

        private int GetMaxRank(List<HandCardItem> nonWilds)
        {
            if (nonWilds.Count == 0) return 0; 
            return GetLogicRank(nonWilds[0]);
        }
        
        private int GetBaseRank(CardValue v)
        {
            return v switch
            {
                CardValue.Three => 0,
                CardValue.Four => 1,
                CardValue.Five => 2,
                CardValue.Six => 3,
                CardValue.Seven => 4,
                CardValue.Eight => 5,
                CardValue.Nine => 6,
                CardValue.Ten => 7,
                CardValue.Jack => 8,
                CardValue.Queen => 9,
                CardValue.King => 10,
                CardValue.Ace => 11,
                CardValue.Two => 12,
                _ => -1
            };
        }
        
        private int GetLogicRank(HandCardItem c)
        {
             if (IsJoker(c))
             {
                 return c.Suit == CardSuit.JokerRed ? 15 : 14;
             }
             return GetBaseRank(c.Value);
        }

        private static bool IsStraightMatch(List<int> sortedRanks, int wildCount, int startRank, int length)
        {
            if (sortedRanks.Distinct().Count() != sortedRanks.Count) return false; 
            
            var needed = 0;
            for (var r = startRank; r < startRank + length; r++)
            {
                if (!sortedRanks.Contains(r)) needed++;
            }

            if (wildCount < needed) return false;
            return !sortedRanks.Any(x => x < startRank || x >= startRank + length);
        }

        private static bool IsConsecutivePairMatch(List<int> ranks, int wildCount, int startRank, int numPairs)
        {
            var req = new Dictionary<int, int>();
            for(var i=0; i<numPairs; i++)
            {
                req[startRank + i] = 2;
            }
            
            foreach(var r in ranks)
            {
                if (!req.ContainsKey(r)) return false;
                req[r]--;
            }
            
            var wildsNeeded = req.Values.Sum(x => Math.Max(0, x));
            return wildCount >= wildsNeeded;
        }
    }
}
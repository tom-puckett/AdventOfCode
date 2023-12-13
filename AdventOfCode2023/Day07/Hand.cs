namespace Day07;

internal class Hand : IComparable<Hand>
{
    internal enum HandKind
    {
        FiveOfAKind,
        FourOfAKind,
        FullHouse,
        ThreeOfAKind,
        TwoPair,
        OnePair,
        HighCard
    };

    static readonly Dictionary<string, HandKind> HandKindRanks = new Dictionary<string, HandKind>
    {
        { "5", HandKind.FiveOfAKind },
        { "41", HandKind.FourOfAKind },
        { "32", HandKind.FullHouse },
        { "311", HandKind.ThreeOfAKind },
        { "221", HandKind.TwoPair },
        { "2111", HandKind.OnePair },
        { "11111", HandKind.HighCard }
    };

    private Dictionary<char, int> CardCounts => new Dictionary<char, int>
    {
        { 'A', 0},
        { 'K', 0},
        { 'Q', 0},
        { 'J', 0},
        { 'T', 0},
        { '9', 0},
        { '8', 0},
        { '7', 0},
        { '6', 0},
        { '5', 0},
        { '4', 0},
        { '3', 0},
        { '2', 0},
    };

    internal Hand(Dictionary<char, int> denominations, Func<IEnumerable<KeyValuePair<char, int>>, IEnumerable<KeyValuePair<char, int>>>? countsTransformer = null)
    {
        Denominations = denominations;
        CountsTransformer = countsTransformer;
    }

    private Dictionary<char, int> Denominations { get; init; }

    public int bid { get; set; }

    public char[]? Cards { private get; set; }

    internal Func<IEnumerable<KeyValuePair<char, int>>, IEnumerable<KeyValuePair<char, int>>>? CountsTransformer { get; init; }

    internal HandKind Kind
    { 
        get
        {
            IEnumerable<KeyValuePair<char, int>> truncatedCounts = Cards!.Aggregate(CardCounts, (counts, card) => { counts[card]++; return counts; })
                                                     .OrderByDescending(v => v.Value);

            if (CountsTransformer is not null)
            {
                truncatedCounts = CountsTransformer(truncatedCounts);
            }
            truncatedCounts = truncatedCounts.Where(v => v.Value != 0);

            IEnumerable<int> truncatedValues = truncatedCounts.Select(v => v.Value);
            string kindString = truncatedValues.Aggregate("", (str, el) => str += el.ToString());
            
            return HandKindRanks[kindString];
        } 
    }

    /// <summary>
    /// Low to high means worse to better hand
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(Hand? other)
    {
        if (other is null) throw new ArgumentException(null, nameof(other));
        if (other.Cards is null) throw new ArgumentException(null, nameof(other.Cards));
        if (Cards is null) throw new ArgumentException(null, nameof(Cards));

        if (Kind < other.Kind)
        {
            return 1;
        }
        else if (Kind > other.Kind)
        {
            return -1;
        }
        else 
        { 
            for (int i = 0; i < Cards.Length; i++)
            {
                int thisDenomination = Denominations[Cards[i]];
                int otherDenomination = Denominations[other.Cards[i]];
                if (thisDenomination != otherDenomination)
                {
                    return thisDenomination < otherDenomination ? 1 : -1;
                }
            }
            return 0;
        }

    }
}

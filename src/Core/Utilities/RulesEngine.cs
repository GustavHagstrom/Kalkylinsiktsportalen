namespace Core.Utilities;
public class RulesEngine<T> where T : class
{
    private readonly List<IRule<T>> rules = new();

    public RulesEngine()
    {
        
    }
    public RulesEngine(IEnumerable<IRule<T>> rules)
    {
        this.rules.AddRange(rules);
    }
    public void AddRule(IRule<T> rule)
    {
        rules.Add(rule);
    }
    public void ApplyRules(T obj)
    {
        foreach (var rule in rules)
        {
            rule.Apply(obj);
        }
    }
}

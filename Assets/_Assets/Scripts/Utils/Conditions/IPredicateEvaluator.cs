namespace WatKhaoWong.Utils.Conditions
{
    public interface IPredicateEvaluator
    {
        public bool? Evaluate(EPredicateName methodName, string[] parameters);
    }
}
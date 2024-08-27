namespace WatKhaoWong.Utils.Conditions
{
    public interface IConditionEvaluator
    {
        public bool? Evaluate(EConditionType conditionType, EConditionValue[] conditionValues);
    }
}
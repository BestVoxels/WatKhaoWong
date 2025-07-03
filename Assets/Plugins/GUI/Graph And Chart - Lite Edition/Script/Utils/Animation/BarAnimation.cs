using UnityEngine;
using ChartAndGraph;

public class BarAnimation : MonoBehaviour
{
    public AnimationCurve Curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public bool AnimateOnEnable = true;
    public float AnimationTime = 3f;
    BarChart barChart;

	void Start()
    {
        barChart = GetComponent<BarChart>();
    }

    private void OnEnable()
    {
        if (AnimateOnEnable)
            Animate();
    }

    public void Animate()
    {
        if(barChart != null)
        {
            double max = barChart.DataSource.GetMaxValue();
            double min = barChart.DataSource.GetMinValue();
            barChart.DataSource.StartBatch();
            barChart.DataSource.AutomaticMaxValue = false;
            barChart.DataSource.AutomaticMinValue = false;
            barChart.DataSource.MaxValue = max;
            barChart.DataSource.MinValue = min;
            for (int i=0; i<barChart.DataSource.TotalCategories; i++)
                for(int j=0; j<barChart.DataSource.TotalGroups; j++)
                {
                    string category = barChart.DataSource.GetCategoryName(i);
                    string group = barChart.DataSource.GetGroupName(j);
                    double val = barChart.DataSource.GetValue(category, group);
                    barChart.DataSource.SetValue(category, group,0.0);
                    barChart.DataSource.SlideValue(category, group, val, AnimationTime, Curve);
                }
            barChart.DataSource.EndBatch();
        }
    }
}

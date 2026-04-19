// Custom control: TSlider
// Description: A custom slider UI element with a progress bar.
// No use of namespace here to avoid potential issues with UXML generation.

using UnityEngine.UIElements;

[UxmlElement]
public partial class TSlider : Slider
{
    public TSlider()
    {
        VisualElement root = this;
        VisualElement tracker = root.Q("unity-tracker");
        var progress = new VisualElement()
        {
            name = "t-slider-progress"
        };
        progress.AddToClassList("t-base-slider__progress");
        tracker.Add(progress);

        lowValue = 0;
        highValue = 100;
        progress.style.width = new StyleLength(Length.Percent(0));
        root.RegisterCallback<ChangeEvent<float>>((evt) =>
        {
            progress.style.width = new StyleLength(Length.Percent(evt.newValue));
        });
    }
}
using GamesTan.UI;
using UnityEngine;
using UnityEngine.UI;

public class UILogCell : MonoBehaviour, IScrollCell
{

    private LogCellData _data;

    public void BindData(LogCellData data)
    {
        _data = data;
        name = "Cell " + data.name;
    }
}

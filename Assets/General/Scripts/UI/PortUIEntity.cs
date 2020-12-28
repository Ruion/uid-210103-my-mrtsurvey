using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;
using Sirenix.OdinInspector;

/// <summary>
/// Use on port UI model popup to change lane, enable state of a port.
/// </summary>
public class PortUIEntity : MonoBehaviour
{
    #region Fields

    public bool is_enabled_ { get { return is_enabled; } set { is_enabled = value; ChangeState(); } }
    public bool is_enabled = false;

    public Color32[] colors;
    public GameObject model;
    public Image img_ { set { model.SetActive(true); img = value; SelectPort(value.transform.GetSiblingIndex() + 1); } }
    private Image img;
    public Transform portParent;

    private int motorId;
    public TextMeshProUGUI motorName;
    public TMP_InputField laneField;
    public TMP_InputField quantityField;
    public GameObject enabledBtn;
    public GameObject disabledBtn;
    public Button SaveButton;

    public bool tempPortIsEnabled;
    public int item_limit;

    public StockDBModelEntity stockDb;
    public TextMeshProUGUI totalText;

    #endregion Fields

    private void OnEnable()
    {
        if (stockDb == null) stockDb = FindObjectOfType<StockDBModelEntity>();
    }

    public void ChangeState()
    {
        img.color = colors[System.Convert.ToInt32(is_enabled)];
    }

    private void SelectPort(int id)
    {
        DataRowCollection drc = stockDb.ExecuteCustomSelectQuery("SELECT * FROM " + stockDb.dbSettings.tableName + " WHERE id = " + id);

        int.TryParse(drc[0]["id"].ToString(), out motorId);
        motorName.text = drc[0][1].ToString();
        quantityField.text = drc[0][2].ToString();
        laneField.text = drc[0][3].ToString();
        tempPortIsEnabled = is_enabled_ = !bool.Parse(drc[0][4].ToString());
        item_limit = System.Int32.Parse(drc[0]["item_limit"].ToString());

        if (drc[0][4].ToString() == "false") disabledBtn.SetActive(false);
        else disabledBtn.SetActive(true);
    }

    [Button(ButtonSizes.Medium)]
    public void SetPorts()
    {
        Image[] imgs = portParent.GetComponentsInChildren<Image>();

        DataRowCollection drc = stockDb.ExecuteCustomSelectQuery("SELECT * FROM " + stockDb.dbSettings.tableName);
        for (int i = 0; i < drc.Count; i++)
        {
            img_ = imgs[System.Int32.Parse(drc[i]["id"].ToString()) - 1];
            // name the box
            img.transform.Find("laneNumber").GetComponent<TextMeshProUGUI>().text = drc[i]["name"].ToString();

            // display the current amount of motor
            if (img.enabled == true)
                img.GetComponentsInChildren<TextMeshProUGUI>()[1].text = string.Format("{0}/{1}", new System.Object[] { drc[i]["quantity"].ToString(), drc[i]["item_limit"].ToString() });

            // display avaibility of motor
            imgs[System.Int32.Parse(drc[i]["id"].ToString()) - 1].color = colors[System.Convert.ToInt32(!bool.Parse(drc[i][4].ToString()))];
        }

        drc = stockDb.ExecuteCustomSelectQuery("SELECT SUM(quantity) FROM " + stockDb.dbSettings.tableName + " WHERE is_disabled = 'false'");
        totalText.text = drc[0][0].ToString();

        model.SetActive(false);
    }

    public void RevertPortInfo()
    {
        is_enabled_ = tempPortIsEnabled;
    }

    public void SavePort()
    {
        stockDb.ExecuteCustomNonQuery(
            "UPDATE " + stockDb.dbSettings.tableName +
            " SET quantity = '" + quantityField.text + "' ," +
            " lane = '" + laneField.text + "' ," +
            " is_disabled = '" + (!is_enabled).ToString().ToLower() + "'" +
            " WHERE id = " + motorId
            );
    }

    public void DropPort()
    {
        stockDb.DropTestSpecific(motorId);
    }

    public void Refill()
    {
        stockDb.ExecuteCustomNonQuery(
            "UPDATE " + stockDb.dbSettings.tableName +
            " SET quantity = '" + item_limit.ToString() + "' ," +
            " lane = '" + laneField.text + "' ," +
            " is_disabled = 'false'" +
            " WHERE id = " + motorId
            );
    }

    public void RefillAll()
    {
        /* stockDb.ExecuteCustomNonQuery(
             "UPDATE " + stockDb.dbSettings.tableName +
             " SET quantity = '5' , is_disabled = 'false'");*/

        // refill the lane to its item using sibling index
        for (int v = 0; v < portParent.childCount; v++)
        {
            //SelectPort(v + 1);
            //Refill();

            if (v + 1 == portParent.childCount) return;
            DataRowCollection drc = stockDb.ExecuteCustomSelectQuery("SELECT * FROM " + stockDb.dbSettings.tableName + " WHERE id = " + (v + 1));

            int selectedMotorId;
            int.TryParse(drc[0]["id"].ToString(), out selectedMotorId);
            int selectedItemLimit = System.Int32.Parse(drc[0]["item_limit"].ToString());

            // Update item_quantity to item_limit
            stockDb.ExecuteCustomNonQuery(
            "UPDATE " + stockDb.dbSettings.tableName +
            " SET quantity = '" + selectedItemLimit.ToString() + "' ," +
            " is_disabled = 'false'" +
            " WHERE id = " + selectedMotorId
            );
        }

        //await System.Threading.Tasks.Task.Delay(500);

        SetPorts();
    }

    public void ValidateInput()
    {
        if (laneField.text == "" || quantityField.text == "" || int.Parse(quantityField.text) < 1) SaveButton.interactable = false;
        else SaveButton.interactable = true;
    }
}
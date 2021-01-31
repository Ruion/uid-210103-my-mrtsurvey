using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;
using Sirenix.OdinInspector;
using System.Collections.Generic;

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
    public TMP_InputField maxQuantityField;
    public GameObject enabledBtn;
    public GameObject disabledBtn;
    public Button SaveButton;

    public bool tempPortIsEnabled;
    public int item_limit;

    public StockDBModelEntity stockDb;
    public DBModelEntity refillRecordDb;
    public TextMeshProUGUI totalText;

    [SerializeField] private List<Toggle> selectionToggles;

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
        maxQuantityField.text = drc[0]["item_limit"].ToString();
        tempPortIsEnabled = is_enabled_ = !bool.Parse(drc[0][4].ToString());
        item_limit = System.Int32.Parse(drc[0]["item_limit"].ToString());

        if (drc[0][4].ToString() == "false") disabledBtn.SetActive(false);
        else disabledBtn.SetActive(true);
    }

    [Button(ButtonSizes.Medium)]
    public void SetPorts()
    {
        Image[] imgs = portParent.GetComponentsInChildren<Image>();

        stockDb.LoadSetting();
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

    public void RevertSelectionToggles()
    {
        for (int i = 0; i < selectionToggles.Count; i++)
        {
            selectionToggles[i].isOn = false;
        }
    }

    public void SavePort()
    {
        if (string.IsNullOrEmpty(quantityField.text)) quantityField.text = "0";

        if (string.IsNullOrEmpty(maxQuantityField.text))
            maxQuantityField.text = 0.ToString();

        if (item_limit.ToString() != maxQuantityField.text)
            item_limit = System.Convert.ToInt32(maxQuantityField.text);

        if (System.Convert.ToInt32(quantityField.text) > item_limit)
            quantityField.text = item_limit.ToString();

        stockDb.ExecuteCustomNonQuery(
            "UPDATE " + stockDb.dbSettings.tableName +
            " SET quantity = '" + quantityField.text + "' ," +
            " lane = '" + laneField.text + "' ," +
            " item_limit = '" + maxQuantityField.text + "' ," +
            " is_disabled = '" + (!is_enabled).ToString().ToLower() + "'" +
            " WHERE id = " + motorId
            );

        // save refill record to db
        PlayerPrefs.SetString("motor_id", motorId.ToString());
        PlayerPrefs.SetString("quantity", quantityField.text);
        PlayerPrefs.SetString("refill_at", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        refillRecordDb.SaveToLocal();
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
            " WHERE id = " + motorId + " AND item_limit > 0"
            );

        // save refill record to db
        PlayerPrefs.SetString("motor_id", motorId.ToString());
        PlayerPrefs.SetString("quantity", item_limit.ToString());
        PlayerPrefs.SetString("refill_at", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        refillRecordDb.SaveToLocal();
    }

    public void RefillSelections()
    {
        // Get all selected Toggle
        Toggle[] selectedToggle = selectionToggles.FindAll(x => x.isOn == true).ToArray();
        List<int> selectionIds = new List<int>();
        // Get selected selectionToggles sibling index
        for (int t = 0; t < selectedToggle.Length; t++)
        {
            selectionIds.Add(selectedToggle[t].transform.GetSiblingIndex());
        }

        // refill the lane to its item using sibling index
        for (int v = 0; v < selectionIds.Count; v++)
        {
            DataRowCollection drc = stockDb.ExecuteCustomSelectQuery("SELECT * FROM " + stockDb.dbSettings.tableName + " WHERE id = " + (selectionIds[v] + 1));

            int selectedMotorId;
            int.TryParse(drc[0]["id"].ToString(), out selectedMotorId);
            int selectedItemLimit = System.Int32.Parse(drc[0]["item_limit"].ToString());

            if (selectedItemLimit < 1) continue;
            // Update item_quantity to item_limit
            stockDb.ExecuteCustomNonQuery(
            "UPDATE " + stockDb.dbSettings.tableName +
            " SET quantity = '" + selectedItemLimit.ToString() + "' ," +
            " is_disabled = 'false'" +
            " WHERE id = " + selectedMotorId + " AND item_limit > 0"
            );
        }

        // Clear selections
        RevertSelectionToggles();
    }

    public void RefillAll()
    {
        DataRowCollection motors = stockDb.ExecuteCustomSelectQuery("SELECT * FROM " + stockDb.dbSettings.tableName + " WHERE item_limit > 0");
        stockDb.ConnectDb();
        refillRecordDb.ConnectDb();

        using (var transaction = stockDb.db_connection.BeginTransaction())
        {
            var command = stockDb.db_connection.CreateCommand();

            using (var refillRecordTransaction = refillRecordDb.db_connection.BeginTransaction())
            {
                var refillRecordCommand = refillRecordDb.db_connection.CreateCommand();

                for (var i = 0; i < motors.Count; i++)
                {
                    command.CommandText =
                    "UPDATE " + stockDb.dbSettings.tableName +
                    " SET quantity = '" + motors[i]["item_limit"].ToString() + "' ," +
                    " is_disabled = 'false'" +
                    " WHERE id = " + motors[i][0].ToString() + " AND item_limit > 0";

                    command.ExecuteNonQuery();

                    string refill_at = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    refillRecordCommand.CommandText = $"INSERT INTO {refillRecordDb.dbSettings.tableName} (motor_id, quantity, refill_at) VALUES('{motors[i][0]}','{motors[i]["item_limit"]}','{refill_at}')";
                    refillRecordCommand.ExecuteNonQuery();
                }

                transaction.Commit();
                refillRecordTransaction.Commit();
            }
        }
        stockDb.Close();
        refillRecordDb.Close();

        SetPorts();
    }

    public void ValidateInput()
    {
        if (laneField.text == "" || quantityField.text == "" || int.Parse(quantityField.text) < 1) SaveButton.interactable = false;
        else SaveButton.interactable = true;
    }
}
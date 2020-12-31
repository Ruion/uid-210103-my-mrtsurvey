using UnityEngine;
using TMPro;

public class StockChecker : MonoBehaviour
{
    public StockDBModelEntity sDb;
    public GameObject outOfStockPanel;
    //public TextMeshProUGUI tier1StockText;

    public float checkStockSeconds = 60f;
    //public TextMeshProUGUI tier2StockText;
    //public TextMeshProUGUI tier3StockText;

    private void OnEnable()
    {
        CheckStock();
        InvokeRepeating("CheckStock", 2f, checkStockSeconds);
    }

    private void CheckStock()
    {
        int remainAmount = System.Int32.Parse(sDb.ExecuteCustomSelectObject("SELECT COUNT(quantity) FROM " + sDb.dbSettings.tableName + " WHERE quantity > 0 AND item_limit > 0 AND is_disabled = 'false'").ToString());
        //int tier1RemainAmount = System.Int32.Parse(sDb.ExecuteCustomSelectObject("SELECT COUNT(id) FROM " + sDb.dbSettings.tableName + " WHERE quantity > 0 AND is_disabled = 'false' AND voucher_code = 'TIER1'").ToString());
        //int tier2RemainAmount = System.Int32.Parse(sDb.ExecuteCustomSelectObject("SELECT COUNT(id) FROM " + sDb.dbSettings.tableName + " WHERE quantity > 0 AND is_disabled = 'false' AND voucher_code = 'TIER2'").ToString());
        //int tier3RemainAmount = System.Int32.Parse(sDb.ExecuteCustomSelectObject("SELECT COUNT(id) FROM " + sDb.dbSettings.tableName + " WHERE quantity > 0 AND is_disabled = 'false' AND voucher_code = 'TIER3'").ToString());

        bool isOutOfStock = false;
        // Debug.Log(name + " - CheckStock() : Vending Machine item remain " + stockRemainAmount);

        // if out of stock
        if (remainAmount < 1)
            isOutOfStock = true;
        else
        {
            isOutOfStock = false;
            outOfStockPanel.SetActive(false);
        }
        // if out of stock
        /* if (tier1RemainAmount < 1)
         { tier1StockText.gameObject.SetActive(true); isOutOfStock = true; }
         else
         { tier1StockText.gameObject.SetActive(false); }*/

        /* if (tier2RemainAmount < 1)
         { tier2StockText.gameObject.SetActive(true); isOutOfStock = true; }
         else
         { tier2StockText.gameObject.SetActive(false); }

         if (tier3RemainAmount < 1)
         { tier3StockText.gameObject.SetActive(true); isOutOfStock = true; }
         else
         { tier3StockText.gameObject.SetActive(false); }*/

        if (!isOutOfStock) outOfStockPanel.SetActive(false);
        else outOfStockPanel.SetActive(true);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
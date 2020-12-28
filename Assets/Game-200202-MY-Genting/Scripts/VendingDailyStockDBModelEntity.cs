using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class VendingDailyStockDBModelEntity : DBModelMaster
{
    public VoucherDBModelEntity vdb;
    public StockDBModelEntity sdb;

    public UnityEvent onRefillFinish;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [Button(ButtonSizes.Large)]
    public void DailyRefill()
    {
        // select all data from VendingDailyStockDatabaseModel.sqlite    
        DataRowCollection drcVoucherVending =
        ExecuteCustomSelectQuery(string.Format("SELECT * FROM {0}",
                                new System.Object[] { dbSettings.tableName }));

        foreach (DataRow v in drcVoucherVending)
        {
            // select all voucher_code from VoucherDBModelEntity
            DataRowCollection drcVoucher =
            vdb.ExecuteCustomSelectQuery(string.Format("SELECT * FROM {0} WHERE voucher_code = '{1}'",
                                     new System.Object[] { vdb.dbSettings.tableName, v["voucher_code"].ToString() }));

            DataRowCollection drcVending =
            sdb.ExecuteCustomSelectQuery(string.Format("SELECT * FROM {0} WHERE id = {1}",
                                     new System.Object[] { sdb.dbSettings.tableName, v["stock_id"].ToString() }));

            int item_limit = System.Int32.Parse(drcVending[0]["item_limit"].ToString());

            int voucher_quantity = System.Int32.Parse(drcVoucher[0]["quantity"].ToString());

            // skip iteration if no more voucher
            if(voucher_quantity < 0) continue;

            int addedAmount = 0;
            
            // Clear stock
            sdb.ExecuteCustomNonQuery(
                string.Format("UPDATE {0} SET quantity = '{1}' , is_disabled = '{2}' WHERE id = {3}", 
                new System.Object[] {   sdb.dbSettings.tableName, 
                                        "0", 
                                        "false" ,
                                        drcVending[0][0].ToString()}));

            //Distribute voucher quantity to linking lanes
            // add item into lane while still have voucher
            for (int i = 0; i < item_limit; i++)
            {
                if (voucher_quantity > 0)
                {
                    addedAmount++;
                    voucher_quantity--;
                }
            }

            if(addedAmount > 0)

            //update lanes
            sdb.ExecuteCustomNonQuery(
                string.Format("UPDATE {0} SET quantity = '{1}' , is_disabled = '{2}' WHERE id = {3}", 
                new System.Object[] {   sdb.dbSettings.tableName, 
                                        addedAmount.ToString(), 
                                        addedAmount > 0? "false" : "true" ,
                                        drcVending[0][0].ToString()}));

            // update voucher quantity
            vdb.ExecuteCustomNonQuery(
                string.Format("UPDATE {0} SET quantity = '{1}' WHERE id = {2}", 
                new System.Object[]{
                    vdb.dbSettings.tableName, voucher_quantity, drcVoucher[0][0].ToString()
                }));
        }

    }

    [Button(ButtonSizes.Large)]
    public void Refill()
    {
        // select all data from VendingDailyStockDatabaseModel.sqlite    
        DataRowCollection drcVoucherVending =
        ExecuteCustomSelectQuery(string.Format("SELECT * FROM {0}",
                                new System.Object[] { dbSettings.tableName }));

        foreach (DataRow v in drcVoucherVending)
        {
            // select all voucher_code from VoucherDBModelEntity
            DataRowCollection drcVoucher =
            vdb.ExecuteCustomSelectQuery(string.Format("SELECT * FROM {0} WHERE voucher_code = '{1}'",
                                     new System.Object[] { vdb.dbSettings.tableName, v["voucher_code"].ToString() }));

            DataRowCollection drcVending =
            sdb.ExecuteCustomSelectQuery(string.Format("SELECT * FROM {0} WHERE id = {1}",
                                     new System.Object[] { sdb.dbSettings.tableName, v["stock_id"].ToString() }));

            int item_limit = System.Int32.Parse(drcVending[0]["item_limit"].ToString());

           // Debug.Log(name + " - item_limit" + item_limit);

            int voucher_quantity = System.Int32.Parse(drcVoucher[0]["quantity"].ToString());

            // skip iteration if no more voucher
            if(voucher_quantity < 0) continue;

            int amountToAdd =
            System.Int32.Parse(drcVending[0]["item_limit"].ToString()) - System.Int32.Parse(drcVending[0]["quantity"].ToString());

           // Debug.Log(name + " - " + drcVending[0]["id"].ToString() + "| item limit - " + drcVending[0]["item_limit"].ToString() 
           // + "| quantity - " + drcVending[0]["quantity"].ToString()
           // + "| amountToAdd : " + amountToAdd);

            int currentLaneQuantity = System.Int32.Parse(drcVending[0]["quantity"].ToString());

            if(amountToAdd < 1) continue;

            // if need refill 
            int addedAmount = 0;
            
            // Clear stock
            sdb.ExecuteCustomNonQuery(
                string.Format("UPDATE {0} SET quantity = '{1}' , is_disabled = '{2}' WHERE id = {3}", 
                new System.Object[] {   sdb.dbSettings.tableName, 
                                        "0", 
                                        "false" ,
                                        drcVending[0][0].ToString()}));

            //Distribute voucher quantity to linking lanes
            // add item into lane while still have voucher
            for (int i = 0; i < amountToAdd; i++)
            {
                if (voucher_quantity > 0)
                {
                    addedAmount++;
                    voucher_quantity--;
                }
            }

            
                currentLaneQuantity += addedAmount;
                Debug.Log(name + " - " + drcVending[0][0].ToString() + "  " + drcVending[0][1].ToString() + " added amount" + addedAmount);
                //update lanes
                sdb.ExecuteCustomNonQuery(
                    string.Format("UPDATE {0} SET quantity = '{1}' , is_disabled = '{2}' WHERE id = {3}",
                    new System.Object[] {   sdb.dbSettings.tableName,
                                        currentLaneQuantity.ToString(),
                                        currentLaneQuantity > 0? "false" : "true" ,
                                        drcVending[0][0].ToString()}));

            if (addedAmount > 0)
            {
                // update voucher quantity
                vdb.ExecuteCustomNonQuery(
                    string.Format("UPDATE {0} SET quantity = '{1}' WHERE id = {2}",
                    new System.Object[]{
                    vdb.dbSettings.tableName, voucher_quantity, drcVoucher[0][0].ToString()
                    }));
            }
        }

        onRefillFinish.Invoke();

    }
}

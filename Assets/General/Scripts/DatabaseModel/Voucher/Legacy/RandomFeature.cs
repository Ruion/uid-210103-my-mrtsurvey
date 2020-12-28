using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.Linq;

public class RandomFeature : MonoBehaviour
{
    public VoucherDBModelEntity vdb;
    public int temp_total = 0;
    public List<ProbabilityCheck> voucher_probability = new List<ProbabilityCheck>();

    // Start is called before the first frame update
    void Start()
    {

    }

    public void MakeRandomProbability()
    {
        voucher_probability = new List<ProbabilityCheck>();
        temp_total = 0;

        DataRowCollection drc = vdb.ExecuteCustomSelectQuery("SELECT * FROM " + vdb.dbSettings.tableName + " WHERE quantity > 0");

        int count = 0;
        foreach (DataRow dr in drc)
        {
            ProbabilityCheck temp = new ProbabilityCheck();
            voucher_probability.Add(temp);
            voucher_probability[count].id = System.Int32.Parse(dr[0].ToString());
            voucher_probability[count].min_prob = temp_total;
            temp_total += System.Int32.Parse(dr["quantity"].ToString());
            voucher_probability[count].max_prob = temp_total;
            voucher_probability[count].name = dr["name"].ToString();
            voucher_probability[count].quantity = System.Int32.Parse(dr["quantity"].ToString());
            count += 1;
        }
    }

    [ContextMenu("CalculateProbability")]
    public ProbabilityCheck CalculateProbability()
    {
        MakeRandomProbability();

        if (voucher_probability.Count < 1) { return null; }

        int rand = UnityEngine.Random.Range(0, temp_total);

        ProbabilityCheck pc = voucher_probability.FirstOrDefault(x => rand > x.min_prob && rand < x.max_prob);

        if (pc != null) { Debug.Log("Choosen voucher name : " + pc.name); return pc; }
        else { Debug.Log("Choosen last voucher name : " + voucher_probability.LastOrDefault().name); return voucher_probability.LastOrDefault(); }
    }
}

public class ProbabilityCheck
{
    public int id;
    public int min_prob;
    public int max_prob;
    public string name;
    public int quantity;
}
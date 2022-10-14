using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GraphQlClient.Core;
public class GraphQLTest : MonoBehaviour
{
    // Start is called before the first frame update
    public GraphApi customReference;
    void Start()
    {
        Debug.Log("GraphQLTest");
        //GraphApi.Query graphTest = customReference.GetQueryByName("GetUsers", GraphApi.Query.Type.Query);
        //Debug.Log(graphTest.query);
        //string jsonData = JsonConvert.SerializeObject(new { query = graphTest.query });
        //byte[] postData = System.Text.Encoding.ASCII.GetBytes(jsonData);
        //
        //StartCoroutine(HttpManager.Instance.HttpPostHandler("https://hasura-unity.herokuapp.com/v1/graphql", postData, "application/json", null, CallBack));
        //GetQuery();
        GraphApi.Query graphTest = customReference.GetQueryByName("GetUsers", GraphApi.Query.Type.Query);
        string jsonData = JsonUtility.ToJson(new { query = graphTest.query });
        byte[] postData = System.Text.Encoding.ASCII.GetBytes(jsonData);
        StartCoroutine(HttpManager.Instance.HttpPostHandler("https://hasura-unity.herokuapp.com/v1/graphql", postData, "application/json", null, CallBack));
    }
    public async void GetQuery()
    {
        UnityWebRequest request = await customReference.Post("GetUsers", GraphApi.Query.Type.Query);
    }
    // Update is called once per frame
    public void CallBack(string _data_str)
    {
        Debug.Log(_data_str);
    }
}
